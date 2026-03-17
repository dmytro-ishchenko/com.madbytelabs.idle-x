using System;
using Exoa.Common;
using Exoa.Events;
using Exoa.Maths;
using Exoa.Touch;
using UnityEngine;

namespace Exoa.Cameras
{
    /// <summary>Base class for all touch camera controllers, providing common input handling, movement, rotation, focus, follow, and inertia logic</summary>
    public class CameraBase : MonoBehaviour, ITouchCamera
    {
        #region COMMON PARAMETERS

        /// <summary>Whether this camera mode is the default active mode</summary>
        public bool defaultMode;
        /// <summary>Whether this camera operates without a CameraModeSwitcher</summary>
        protected bool standalone;
        /// <summary>Screen-to-world depth converter based on ground height</summary>
        protected ScreenDepth HeightScreenDepth;

        /// <summary>Reference to the Unity Camera component</summary>
        protected Camera cam;
        /// <summary>Reference to the CameraBoundaries component for clamping</summary>
        protected CameraBoundaries camBounds;

        /// <summary>Initial offset position used for resetting the camera</summary>
        protected Vector3 initOffset;
        /// <summary>Initial rotation used for resetting the camera</summary>
        protected Quaternion initRotation;

        /// <summary>Target offset position of the camera center on the ground</summary>
        protected Vector3 finalOffset;
        /// <summary>Target world position of the camera transform</summary>
        protected Vector3 finalPosition;
        /// <summary>Target rotation of the camera transform</summary>
        protected Quaternion finalRotation;
        /// <summary>Target distance between the camera and its ground center point</summary>
        protected float finalDistance;
        /// <summary>Whether all user input moves are disabled</summary>
        protected bool disableMoves;
        /// <summary>Whether user translation inputs are disabled</summary>
        protected bool disableTranslations;
        /// <summary>Whether user rotation inputs are disabled</summary>
        protected bool disableRotations;

        /// <summary>Current pitch angle of the camera in degrees</summary>
        protected float currentPitch;
        /// <summary>Current yaw angle of the camera in degrees</summary>
        protected float currentYaw;
        /// <summary>Delta yaw rotation applied this frame</summary>
        protected float deltaYaw;
        /// <summary>Delta pitch rotation applied this frame</summary>
        protected float deltaPitch;

        /// <summary>Quaternion representing the twist rotation from pinch gestures</summary>
        protected Quaternion twistRot;

        /// <summary>World-space position of the camera center projected on the ground</summary>
        protected Vector3 worldPointCameraCenter;
        /// <summary>World-space position where the fingers center projects onto the ground</summary>
        protected Vector3 worldPointFingersCenter;
        /// <summary>World-space delta offset between finger center and camera center on the ground</summary>
        protected Vector3 worldPointFingersDelta;

        [Header("INPUTS")]
        /// <summary>Action mapped to right mouse button drag</summary>
        public InputMapFingerDrag rightClickDrag = InputMapFingerDrag.Translate;
        /// <summary>Action mapped to middle mouse button drag</summary>
        public InputMapFingerDrag middleClickDrag = InputMapFingerDrag.Translate;
        /// <summary>Action mapped to single finger or left mouse button drag</summary>
        public InputMapFingerDrag oneFingerDrag = InputMapFingerDrag.RotateAround;
        /// <summary>Action mapped to two-finger drag gesture</summary>
        public InputMapFingerDrag twoFingerDrag = InputMapFingerDrag.Translate;
        /// <summary>Action mapped to two-finger pinch gesture</summary>
        public InputMapFingerPinch twoFingerPinch = InputMapFingerPinch.ZoomAndRotate;
        /// <summary>Action mapped to scroll wheel input</summary>
        public InputMapScrollWheel scrollWheel = InputMapScrollWheel.ZoomUnderMouse;

        [Header("GROUND HEIGHT")]
        /// <summary>Y position of the ground plane used for finger-to-world projection</summary>
        public float groundHeight = 0f;
        /// <summary>Spring settings for animating ground height transitions</summary>
        public Springs groundHeightAnim;
        /// <summary>Internal spring value tracking the animated ground height</summary>
        private FloatSpring groundHeightValue;

        /// <summary>Available actions for finger or mouse drag input</summary>
        public enum InputMapFingerDrag { Translate, RotateAround, RotateHead, None };
        /// <summary>Available actions for two-finger pinch input</summary>
        public enum InputMapFingerPinch { ZoomAndRotate, RotateOnly, ZoomOnly, None };
        /// <summary>Available actions for scroll wheel input</summary>
        public enum InputMapScrollWheel { ZoomUnderMouse, ZoomInCenter, None };


        [Header("ROTATION")]
        /// <summary>Whether pitch (vertical) rotation is allowed</summary>
        public bool allowPitchRotation = true;
        /// <summary>Sensitivity multiplier for pitch rotation input</summary>
        public float PitchSensitivity = 0.25f;
        /// <summary>Whether to clamp the pitch angle within PitchMinMax bounds</summary>
        public bool PitchClamp = true;
        /// <summary>Minimum and maximum pitch angle in degrees</summary>
        public Vector2 PitchMinMax = new Vector2(5.0f, 90.0f);
        /// <summary>Initial pitch and yaw rotation values in degrees</summary>
        protected Vector2 initialRotation = new Vector2(35, 0);
        /// <summary>Whether yaw (horizontal) rotation is allowed</summary>
        public bool allowYawRotation = true;
        /// <summary>Sensitivity multiplier for yaw rotation input</summary>
        public float YawSensitivity = 0.25f;
        /// <summary>Whether to clamp the yaw angle within YawMinMax bounds</summary>
        public bool YawClamp;
        /// <summary>Minimum and maximum yaw angle in degrees</summary>
        public Vector2 YawMinMax = new Vector2(5.0f, 90.0f);

        [Header("MOVE")]
        /// <summary>Maximum speed for camera translation movement</summary>
        public float maxTranslationSpeed = 3f;


        #endregion


        #region GETTER & SETTERS

        /// <summary>
        /// The final quaternion rotation of the camera transform
        /// </summary>
        public Quaternion FinalRotation
        {
            get => finalRotation;
        }
        /// <summary>
        /// The final position of the camera transform
        /// </summary>
        public Vector3 FinalPosition
        {
            get => finalPosition;
        }
        /// <summary>
        /// The final offset of the camera's center point on ground
        /// This is not the camera position. The center of the camera is projected 
        /// on the ground, and this is it's position.
        /// </summary>
        public Vector3 FinalOffset
        {
            get => finalOffset;
            set => finalOffset = value;
        }
        /// <summary>
        /// This is the final distance between the camera's center point on the ground,
        /// and the camera transform's position
        /// </summary>
        public float FinalDistance
        {
            get => finalDistance;
        }

        /// <summary>
        /// This blocks the user input moves (rotation and translation), in order to move/animate the camera by script
        /// </summary>
        public bool DisableMoves
        {
            get => disableMoves;
            set => disableMoves = value;
        }

        /// <summary>
        /// This blocks the user translation inputs  
        /// </summary>
        public bool DisableTranslations
        {
            get => disableTranslations;
            set => disableTranslations = value;
        }


        /// <summary>
        /// This blocks the user rotation inputs  
        /// </summary>
        public bool DisableRotations
        {
            get => disableRotations;
            set => disableRotations = value;
        }

        /// <summary>
        /// This returns the current Pitch and Yaw rotations of the camera
        /// </summary>
        public Vector2 PitchAndYaw
        {
            get => new Vector2(currentPitch, currentYaw);
        }

        /// <summary>
        /// Returns true if the camera is currently rotating
        /// </summary>
        /// <returns></returns>
        public bool IsRotating()
        {
            return !disableRotations && !disableMoves && (IsInputMatching(InputMapFingerDrag.RotateAround) ||
                IsInputMatching(InputMapFingerDrag.RotateHead) ||
                IsInputMatching(InputMapFingerPinch.RotateOnly));
        }

        /// <summary>
        /// Returns true if the camera is currently focusing or following an object
        /// </summary>
        public bool IsFocusingOrFollowing
        {
            get => isFocusingOrFollowing;
        }
        #endregion


        #region INIT / DESTROY / UPDATE

        /// <summary>
        /// Destroys the component
        /// </summary>
        virtual protected void OnDestroy()
        {
            CameraEvents.OnBeforeSwitchPerspective -= OnBeforeSwitchPerspective;
            CameraEvents.OnAfterSwitchPerspective -= OnAfterSwitchPerspective;
            CameraEvents.OnRequestButtonAction -= OnRequestButtonAction;
            CameraEvents.OnRequestObjectFocus -= FocusCameraOnGameObject;
            CameraEvents.OnRequestObjectFollow -= FollowGameObject;
            CameraEvents.OnRequestGroundHeightChange -= SetGroundHeightAnimated;
            CameraEvents.OnRequestStopFocusFollow -= StopFollow;
        }

        /// <summary>
        /// Starts the camera script
        /// </summary>
        virtual protected void Start()
        {
            cam = GetCamera();
            // Fix for jittering issues when camera is really high
            cam.nearClipPlane = 1f;

            camBounds = GetComponent<CameraBoundaries>();
            standalone = GetComponent<CameraModeSwitcher>() == null;
            Init();
            CameraEvents.OnBeforeSwitchPerspective += OnBeforeSwitchPerspective;
            CameraEvents.OnAfterSwitchPerspective += OnAfterSwitchPerspective;

            if (standalone)
            {
                CameraEvents.OnRequestButtonAction += OnRequestButtonAction;
                CameraEvents.OnRequestObjectFocus += FocusCameraOnGameObject;
                CameraEvents.OnRequestObjectFollow += FollowGameObject;
                CameraEvents.OnRequestGroundHeightChange += SetGroundHeightAnimated;
                CameraEvents.OnRequestStopFocusFollow += StopFollow;
            }
        }

        /// <summary>Returns the Camera component, finding it if not yet cached</summary>
        /// <returns>The Camera component attached to this GameObject or the main camera</returns>
        public Camera GetCamera()
        {
            if (cam != null)
                return cam;
            cam = GetComponent<Camera>();
            if (cam == null) cam = Camera.main;
            return cam;
        }

        /// <summary>
        /// Init some camera parameters
        /// </summary>
        virtual protected void Init()
        {
            if (PitchClamp && PitchMinMax.x <= 0f)
            {
                PitchMinMax.x = Mathf.Max(PitchMinMax.x, 0.1f);
                Debug.LogWarning("CameraBase: PitchMinMax.x must be greater than 0. Clamped to " + PitchMinMax.x, this);
            }
            CreateConverter();
        }
        /// <summary>
        /// Create the depth converter between fingers on screen and the 3D World
        /// By default we use the scene y plan, to convert our finger's position
        /// </summary>
        virtual protected void CreateConverter()
        {
            HeightScreenDepth = new ScreenDepth(ScreenDepth.ConversionType.HeightIntercept, -5, groundHeight);
        }


        /// <summary>Tracks whether the first LateUpdate frame has executed</summary>
        private bool firstUpdateDone;
        /// <summary>
        /// Some elements require a first Update call, so we need to know if it has been done
        /// </summary>
        virtual protected void LateUpdate()
        {
            if (!firstUpdateDone)
            {
                enabled = defaultMode || standalone;
                firstUpdateDone = true;
            }
#if DEBUG_TCP
            DebugInputs();
#endif
        }

        /// <summary>
        /// In case the camera is standalone (no CameraModeSwitcher) then this is apply 
        /// the position and rotation to the camera
        /// </summary>
        virtual protected void ApplyToCamera()
        {
            if (standalone)
            {
                try
                {
                    if (!float.IsNaN(FinalPosition.x) && !float.IsNaN(FinalPosition.y) && !float.IsNaN(FinalPosition.z))
                    {
                        transform.position = FinalPosition;
                    }
                }
                catch (System.Exception) { };

                if (!float.IsNaN(FinalRotation.x) && !float.IsNaN(FinalRotation.y) && !float.IsNaN(FinalRotation.z))
                {
                    transform.rotation = FinalRotation;
                }
            }
        }

        /// <summary>
        /// Return the matrix of the camera transform, in order to blend it when switching modes
        /// </summary>
        /// <returns></returns>
        virtual public Matrix4x4 GetMatrix()
        {
            return new Matrix4x4();
        }

        #endregion


        #region GROUND HEIGHT
        /// <summary>
        /// This let you change the ground height at any moment in order to
        /// change at which y position the fingers will be intercepted.
        /// This version lets you animate it
        /// </summary>
        /// <param name="newHeight"></param>
        /// <param name="animate"></param>
        /// <param name="duration"></param>
        public void SetGroundHeightAnimated(float newHeight, bool animate, float duration)
        {
            if (animate)
            {
                groundHeightValue.Reset(groundHeight);
                //groundHeight = groundHeightAnim.Update(ref groundHeightValue, newHeight);
                //SetGroundHeight(groundHeight);
            }
            else
            {
                SetGroundHeight(newHeight);
            }
        }

        /// <summary>
        /// This let you change the ground height at any moment in order to
        /// change at which y position the fingers will be intercepted
        /// </summary>
        /// <param name="y"></param>
        public void SetGroundHeight(float y)
        {
            groundHeight = y;
            HeightScreenDepth.Distance = groundHeight;
            finalOffset.y = groundHeight;
        }
        #endregion


        #region INPUTS
        /// <summary>
        /// Converts user inputs to actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected bool IsInputMatching(InputMapScrollWheel action)
        {
            if (scrollWheel == action && CameraInputs.GetScroll() != 1)
                return true;
            return false;
        }

        /// <summary>
        /// Converts user inputs to actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected bool IsInputMatching(InputMapFingerPinch action)
        {
            if (twoFingerPinch == action && CameraInputs.GetFingerCount() == 2)
                return true;
            return false;
        }

        /// <summary>
        /// Converts user inputs to actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected bool IsInputMatching(InputMapFingerDrag action)
        {
            if (action == InputMapFingerDrag.Translate && DisableTranslations)
                return false;
            if (action == InputMapFingerDrag.RotateAround && DisableRotations)
                return false;
            if (action == InputMapFingerDrag.RotateHead && DisableRotations)
                return false;

            if (middleClickDrag == action && BaseTouchInput.GetMouseIsHeld(2) && !Application.isMobilePlatform)
                return true;
            if (rightClickDrag == action && BaseTouchInput.GetMouseIsHeld(1) && !Application.isMobilePlatform)
                return true;
            if (twoFingerDrag == action && CameraInputs.GetFingerCount() == 2)
                return true;
            if (oneFingerDrag == action &&
                (BaseTouchInput.GetTouchCount() == 1 ||
                BaseTouchInput.GetMouseIsHeld(0)) &&
                !BaseTouchInput.GetMouseIsHeld(2) &&
                !BaseTouchInput.GetMouseWentUp(1) &&
                !BaseTouchInput.GetMouseWentUp(2))
            {
                return true;
            }

            return false;
        }
        #endregion


        #region BOUNDARIES
        /// <summary>
        /// Is the camera using camera edge boundaries instead of center mode
        /// </summary>
        /// <returns></returns>
        protected bool IsUsingCameraEdgeBoundaries()
        {
            return camBounds != null && camBounds.IsUsingCameraEdgeBoundaries();
        }

        /// <summary>
        /// Method used to clamp the camera inside the boundaries collider
        /// </summary>
        /// <param name="finalPosition"></param>
        /// <param name="clampApplied"></param>
        /// <param name="bottomEdges"></param>
        /// <param name="topEdges"></param>
        /// <returns></returns>
        protected Vector3 ClampCameraCorners(Vector3 targetPosition, out bool clampApplied, float currentPitch)
        {

            bool anyFingerDown = CameraInputs.GetFingerCount() > 0;
            clampApplied = false;
            if (camBounds != null)
                return camBounds.ClampCameraCorners(targetPosition, out clampApplied,
                    currentPitch, HeightScreenDepth, cam, groundHeight, anyFingerDown);
            return targetPosition;
        }

        /// <summary>
        /// Clamp any given point inside the boundaries collider on XZ plan. Y will be unchanged
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        virtual protected Vector3 ClampInCameraBoundaries(Vector3 targetPosition)
        {
            return ClampInCameraBoundaries(targetPosition, out bool isInBoundaries);
        }

        /// <summary>
        /// Clamp any given point inside the boundaries collider on XZ plan. Y will be unchanged
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        virtual protected Vector3 ClampInCameraBoundaries(Vector3 targetPosition, out bool isInBoundaries)
        {
            bool anyFingerDown = CameraInputs.GetFingerCount() > 0;
            if (anyFingerDown) isApplyingMoveInertia = false;

            isInBoundaries = true;
            if (camBounds != null)
                return camBounds.ClampInBoundsXZ(targetPosition, out isInBoundaries, groundHeight, anyFingerDown || isApplyingMoveInertia);
            return targetPosition;
        }
        #endregion


        #region EVENTS
        /// <summary>
        /// Called just before the perspective switch happens
        /// </summary>
        /// <param name="orthoMode"></param>
        virtual protected void OnBeforeSwitchPerspective(bool orthoMode)
        {

        }

        /// <summary>
        /// Called just after the perspective switch happened
        /// </summary>
        /// <param name="orthoMode"></param>

        virtual protected void OnAfterSwitchPerspective(bool orthoMode)
        {

        }

        /// <summary>
        /// Catch event actions and interpret them
        /// </summary>
        /// <param name="action"></param>
        /// <param name="active"></param>
        protected void OnRequestButtonAction(CameraEvents.Action action, bool active)
        {
            if (action == CameraEvents.Action.ResetCameraPositionRotation)
                ResetCamera();
            else if (action == CameraEvents.Action.DisableCameraMoves)
                DisableMoves = (active);
        }
        #endregion


        #region POSITION
        /// <summary>
        /// Calculate the offset position on the ground, given the camera's position and rotation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        virtual protected Vector3 CalculateOffset(Vector3 pos, Quaternion rot)
        {
            float tan = Mathf.Tan(Mathf.Deg2Rad * rot.eulerAngles.x);
            tan = tan == 0f ? 0.000001f : tan;

            float adj = (pos.y - groundHeight) / tan;

            Vector3 camForward = Quaternion.Euler(0, rot.eulerAngles.y, 0) * Vector3.forward;
            Vector3 camOffset = pos.SetY(groundHeight) + camForward.normalized * adj;
            return camOffset;
        }

        /// <summary>
        /// Calculate the offset position on the ground, given the camera's position, rotation, distance from ground and ground height
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="distance"></param>
        /// <param name="groundHeight"></param>
        /// <returns></returns>
        virtual protected Vector3 CalculateOffset(Vector3 pos, Quaternion rot, float distance, float groundHeight)
        {
            Vector3 offset = pos - rot * (Vector3.back * distance);
            return offset.SetY(groundHeight);
        }

        /// <summary>
        /// Calculates the camera transform's position giving the offset, rotation and distance
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rot"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        virtual protected Vector3 CalculatePosition(Vector3 center, Quaternion rot, float distance)
        {
            Vector3 p = rot * (Vector3.back * distance) + center;
            p = p.Round(1000f);
            return p;
        }


        #endregion


        #region DISTANCE
        /// <summary>
        ///  Calculates the camera's distance from the ground, giving his transform position and rotation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        protected float CalculateDistance(Vector3 pos, Quaternion rot)
        {
            float cos = Mathf.Cos(Mathf.Deg2Rad * (90 - rot.eulerAngles.x));
            cos = cos == 0f ? 0.000001f : cos;

            float distance = Mathf.Abs((pos.y - groundHeight) / cos);
            return distance;
        }

        /// <summary>
        /// Calculates a clamped distance after a zoom
        /// </summary>
        /// <param name="camPos"></param>
        /// <param name="worldPoint"></param>
        /// <param name="minMaxDistance"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        protected float CalculateClampedDistance(Vector3 camPos, Vector3 worldPoint, Vector2 minMaxDistance, float multiplier = 1)
        {
            Vector3 distance = (camPos - worldPoint);
            return Mathf.Clamp(distance.magnitude * multiplier, minMaxDistance.x, minMaxDistance.y);
        }

        /// <summary>
        /// Calculates a clamped distance after a zoom
        /// </summary>
        /// <param name="camPos"></param>
        /// <param name="worldPoint"></param>
        /// <param name="minMaxDistance"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        protected float CalculateClampedDistance(Vector3 camPos, Vector3 worldPoint, float minMaxDistance, float multiplier = 1)
        {
            Vector3 distance = (camPos - worldPoint);
            return Mathf.Clamp(distance.magnitude * multiplier, minMaxDistance, minMaxDistance);
        }

        /// <summary>
        /// Calculates a clamped distance
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="minMaxDistance"></param>
        /// <returns></returns>
        protected float CalculateClampedDistance(float distance, Vector2 minMaxDistance)
        {
            return Mathf.Clamp(distance, minMaxDistance.x, minMaxDistance.y);
        }

        /// <summary>
        /// Calculates a clamped distance after a zoom
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="minMaxDistance"></param>
        /// <returns></returns>
        protected float CalculateClampedDistance(float distance, float minMaxDistance)
        {
            return Mathf.Clamp(distance, minMaxDistance, minMaxDistance);
        }

        /// <summary>
        /// Calculates a clamped distance, meant to be overriden
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="minMaxDistance"></param>
        /// <returns></returns>
        virtual protected float CalculateClampedDistance(float distance)
        {
            return distance;
        }
        #endregion


        #region ROTATION
        /// <summary>
        /// Gives the initial rotation of the camera to be able to reset it later
        /// </summary>
        /// <returns></returns>
        virtual protected Quaternion GetInitialRotation()
        {
            return initRotation;
        }

        /// <summary>
        /// Gives the initial rotation as pitch/yaw vector of the camera to be able to reset it later
        /// </summary>
        /// <returns></returns>
        virtual protected Vector2 GetInitialRotationVec()
        {
            return initRotation.eulerAngles;
        }

        /// <summary>
        /// Calculate the sensivity of the rotation from user's input
        /// </summary>
        /// <returns></returns>
        protected float GetRotationSensitivity()
        {

            // Adjust sensitivity by FOV?
            if (cam.orthographic == false)
            {
                return cam.fieldOfView / 90.0f;
            }

            return 1.0f;
        }


        /// <summary>Clamps twist rotation so yaw stays within bounds relative to the current rotation</summary>
        /// <param name="twistRot">The twist rotation to clamp</param>
        /// <param name="currentRotation">The current camera rotation</param>
        /// <returns>The clamped twist rotation</returns>
        protected Quaternion ClampTwist(Quaternion twistRot, Quaternion currentRotation)
        {
            Quaternion r1 = twistRot * currentRotation;
            Quaternion r2 = ClampYawRotation(r1);
            Quaternion r3 = r2 * Quaternion.Inverse(currentRotation);
            return r3;
        }
        /// <summary>Clamps a rotation quaternion so both pitch and yaw stay within configured bounds</summary>
        /// <param name="rot">The rotation to clamp</param>
        /// <returns>The clamped rotation</returns>
        virtual protected Quaternion ClampRotation(Quaternion rot)
        {
            Vector2 pitchYaw = GetRotationToPitchYaw(rot);
            Vector2 clampedPitchYaw = ClampPitchYaw(pitchYaw);
            return GetRotationFromPitchYaw(clampedPitchYaw);
        }

        /// <summary>Clamps a rotation quaternion so yaw stays within configured bounds</summary>
        /// <param name="rot">The rotation to clamp</param>
        /// <returns>The yaw-clamped rotation</returns>
        virtual protected Quaternion ClampYawRotation(Quaternion rot)
        {
            Vector2 pitchYaw = GetRotationToPitchYaw(rot);
            Vector2 clampedYaw = ClampYaw(pitchYaw);
            return GetRotationFromPitchYaw(clampedYaw);
        }
        /// <summary>
        /// Clamp the rotation if needed
        /// </summary>
        virtual protected void ClampPitchYaw()
        {
            //Log("ClampPitchYaw2");
            if (PitchClamp)
                currentPitch = Mathf.Clamp(NormalizeAngle(currentPitch), Mathf.Max(PitchMinMax.x, 0.1f), PitchMinMax.y);
            if (YawClamp)
                currentYaw = Mathf.Clamp(NormalizeAngle(currentYaw), YawMinMax.x, YawMinMax.y);
        }

        /// <summary>
        /// Clamp the rotation if needed
        /// </summary>
        virtual protected Vector2 ClampPitchYaw(Vector2 pitchYaw)
        {
            if (PitchClamp)
                pitchYaw.x = Mathf.Clamp(NormalizeAngle(pitchYaw.x), Mathf.Max(PitchMinMax.x, 0.1f), PitchMinMax.y);
            if (YawClamp)
                pitchYaw.y = Mathf.Clamp(NormalizeAngle(pitchYaw.y), YawMinMax.x, YawMinMax.y);
            return pitchYaw;
        }

        /// <summary>
        /// Clamps only the yaw component of the pitch/yaw vector within configured bounds
        /// </summary>
        /// <param name="pitchYaw">The pitch/yaw vector to clamp</param>
        /// <returns>The vector with yaw clamped</returns>
        virtual protected Vector2 ClampYaw(Vector2 pitchYaw)
        {
            if (YawClamp)
                pitchYaw.y = Mathf.Clamp(NormalizeAngle(pitchYaw.y), YawMinMax.x, YawMinMax.y);
            return pitchYaw;
        }



        /// <summary>
        /// Rotate the camera manually
        /// </summary>
        /// <param name="delta">the increment values (pitch, yaw)</param>
        public void RotateFromVector(Vector2 delta)
        {
            var sensitivity = GetRotationSensitivity();
            if (allowYawRotation)
            {
                deltaYaw = delta.x * YawSensitivity * sensitivity;
                currentYaw += deltaYaw;
            }
            if (allowPitchRotation)
            {
                deltaPitch = -delta.y * PitchSensitivity * sensitivity;
                currentPitch += deltaPitch;
            }
            ClampPitchYaw();
        }

        /// <summary>
        /// Useful to clamp a rotation between two angles
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="rangemin"></param>
        /// <param name="rangemax"></param>
        /// <returns></returns>
        protected float ModularClamp(float val, float min, float max, float rangemin = -180f, float rangemax = 180f)
        {
            var modulus = Mathf.Abs(rangemax - rangemin);
            if ((val %= modulus) < 0f) val += modulus;
            return Mathf.Clamp(val + Mathf.Min(rangemin, rangemax), min, max);
        }


        /// <summary>
        /// Normalize an angle between -180 and 180
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        protected float NormalizeAngle(float a)
        {
            if (a > 180) a -= 360;
            if (a < -180) a += 360;
            return a;
        }

        /// <summary>
        /// Converts pitch/yaw rotations to a quaternion
        /// </summary>
        /// <returns></returns>
#if DAMPING_FEATURE_PREVIEW
        /// <summary>Smoothed current pitch value used for rotation damping</summary>
        protected float dampedCurrentPitch;
        /// <summary>Smoothed current yaw value used for rotation damping</summary>
        protected float dampedCurrentYaw;
        /// <summary>Previous frame yaw value for damping delta calculation</summary>
        protected float previousYaw;
        /// <summary>Lerp factor controlling how quickly rotation catches up to the target</summary>
        public float rotationDamping = .02f;
#endif
        virtual protected Quaternion GetRotationFromPitchYaw()
        {
            //Log("GetRotationFromPitchYaw");
#if DAMPING_FEATURE_PREVIEW
            // DAMPING TESTS
            if (dampedCurrentPitch - currentPitch > 180) dampedCurrentPitch -= 360;
            else if (dampedCurrentPitch - currentPitch < -180) dampedCurrentPitch += 360;
            if (dampedCurrentYaw - currentYaw > 180) dampedCurrentYaw -= 360;
            else if (dampedCurrentYaw - currentYaw < -180) dampedCurrentYaw += 360;

            if (dampedCurrentPitch == 0) dampedCurrentPitch = currentPitch;
            if (dampedCurrentYaw == 0) dampedCurrentYaw = currentYaw;

            dampedCurrentPitch = (Mathf.Lerp(dampedCurrentPitch, (currentPitch), rotationDamping));
            dampedCurrentYaw = (Mathf.Lerp(dampedCurrentYaw, (currentYaw), rotationDamping));
            return Quaternion.Euler(dampedCurrentPitch, dampedCurrentYaw, 0);
#else
            currentPitch = NormalizeAngle(currentPitch);
            currentYaw = NormalizeAngle(currentYaw);
            return Quaternion.Euler(currentPitch, currentYaw, 0);

#endif


        }

        /// <summary>
        /// Converts pitch/yaw rotations to a quaternion
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        virtual protected Quaternion GetRotationFromPitchYaw(float pitch, float yaw)
        {
            //Log("GetRotationFromPitchYaw2");

            pitch = NormalizeAngle(pitch);
            yaw = NormalizeAngle(yaw);

            return Quaternion.Euler(pitch, yaw, 0);
        }

        /// <summary>
        /// Converts pitch/yaw rotations to a quaternion
        /// </summary>
        /// <param name="pitchYawVec"></param>
        /// <returns></returns>
        virtual protected Quaternion GetRotationFromPitchYaw(Vector2 pitchYawVec)
        {
            //Log("GetRotationFromPitchYaw3");

            pitchYawVec.x = NormalizeAngle(pitchYawVec.x);
            pitchYawVec.y = NormalizeAngle(pitchYawVec.y);

            return Quaternion.Euler(pitchYawVec.x, pitchYawVec.y, 0);
        }

        /// <summary>
        /// Converts the quaternion rotation to pitch/yaw values
        /// </summary>
        /// <returns></returns>
        virtual protected Vector2 GetRotationToPitchYaw()
        {
            //Log("GetRotationToPitchYaw");
            return new Vector2(finalRotation.eulerAngles.x, finalRotation.eulerAngles.y);
        }

        /// <summary>
        /// Converts the quaternion rotation to pitch/yaw values
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        virtual protected Vector2 GetRotationToPitchYaw(Quaternion rot)
        {
            //Log("GetRotationToPitchYaw2");
            return new Vector2(NormalizeAngle(rot.eulerAngles.x), rot.eulerAngles.y);
        }
        #endregion


        #region RESET

        /// <summary>
        /// Reset the camera to initial values
        /// </summary>
        virtual public void ResetCamera()
        {
            throw new Exception("ResetCamera needs to be overridden!");
        }

        /// <summary>
        /// Set the initial values for the reset function
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rotation"></param>
        /// <param name="distanceOrSize"></param>
        virtual public void SetResetValues(Vector3 offset, Quaternion rotation, float distanceOrSize)
        {
            throw new Exception("SetResetValues needs to be overridden!");
        }

        /// <summary>
        /// indicates the vector for AngleAxis, to rotate around
        /// </summary>
        /// <returns></returns>
        virtual protected Vector3 GetRotateAroundVector()
        {
            return Vector3.up;
        }
        #endregion


        #region MOVE

        /// <summary>
        /// Moves the Camera transform position to a new position, in 1 frame
        /// </summary>
        /// <param name="targetCamPos"></param>
        public void RelocateCameraInstant(Vector3 targetCamPos)
        {
            finalPosition = targetCamPos;
            finalOffset = CalculateOffset(finalPosition, finalRotation);
            finalDistance = CalculateDistance(finalPosition, finalRotation);

            ApplyToCamera();
        }

        /// <summary>
        /// Rotates the camera around a viewport point by a given angle
        /// </summary>
        /// <param name="viewportPoint">The viewport point to rotate around</param>
        /// <param name="angle">The angle in degrees to rotate</param>
        public void RotateAround(Vector2 viewportPoint, float angle)
        {
            Quaternion twistRot = Quaternion.AngleAxis(angle, GetRotateAroundVector());
            Vector2 screenPoint = cam.ViewportToScreenPoint(viewportPoint);
            Vector3 worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(screenPoint));
            Vector3 vecFingersCenterToCamera = (finalPosition - worldPointFingersCenter);
            float vecFingersCenterToCameraDistance = vecFingersCenterToCamera.magnitude * 1f;
            vecFingersCenterToCamera = vecFingersCenterToCamera.normalized * vecFingersCenterToCameraDistance;

            Vector3 targetPosition = worldPointFingersCenter + vecFingersCenterToCamera;
            Vector3 offsetFromFingerCenter = worldPointFingersCenter - worldPointFingersDelta;

            finalPosition = twistRot * (targetPosition - worldPointFingersCenter) + offsetFromFingerCenter;
            finalRotation = twistRot * finalRotation;

            currentPitch = NormalizeAngle(finalRotation.eulerAngles.x);
            currentYaw = (finalRotation.eulerAngles.y);

            Vector3 newWorldPointCameraCenter = CalculateOffset(finalPosition, finalRotation);
            Vector3 newWorldPointCameraCenterClamped = ClampInCameraBoundaries(newWorldPointCameraCenter);

            finalOffset = newWorldPointCameraCenterClamped;

            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground to a new position, in 1 frame
        /// </summary>
        /// <param name="targetPosition"></param>
        public void MoveCameraToInstant(Vector3 targetOffset)
        {
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }


        /// <summary>
        /// Moves the Camera offset position on ground and distance from it, in 1 frame
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        virtual public void MoveCameraToInstant(float targetDistanceOrSize)
        {
            finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground and distance from it, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetDistanceOrSize"></param>
        virtual public void MoveCameraToInstant(Vector3 targetOffset, float targetDistanceOrSize)
        {
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }



        /// <summary>
        /// Moves the Camera offset position on ground and rotation, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetRotation"></param>
        virtual public void MoveCameraToInstant(Vector3 targetOffset, Vector2 targetRotation)
        {
            finalRotation = GetRotationFromPitchYaw(targetRotation);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
            finalDistance = CalculateDistance(finalPosition, finalRotation);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground and rotation, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetRotation"></param>
        virtual public void MoveCameraToInstant(Vector3 targetOffset, Quaternion targetRotation)
        {
            finalRotation = ClampRotation(targetRotation);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
            finalDistance = CalculateDistance(finalPosition, finalRotation);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground and rotation, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetRotation"></param>
        virtual public void MoveCameraToInstant(Quaternion targetRotation)
        {
            finalRotation = ClampRotation(targetRotation);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it and rotation, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        virtual public void MoveCameraToInstant(Vector3 targetOffset, float targetDistanceOrSize, Vector2 targetRotation)
        {
            finalRotation = GetRotationFromPitchYaw(targetRotation);
            finalRotation = ClampRotation(finalRotation);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it and rotation, in 1 frame
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        virtual public void MoveCameraToInstant(Vector3 targetOffset, float targetDistanceOrSize, Quaternion targetRotation)
        {
            finalRotation = ClampRotation(targetRotation);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground, animated
        /// </summary>
        /// <param name="targetPosition"></param>
        public void MoveCameraTo(Vector3 targetPosition)
        {
            FocusCamera(targetPosition);
        }

        /// <summary>
        /// Moves the Camera rotation, animated
        /// </summary>
        /// <param name="targetRotation"></param>
        public void MoveCameraTo(Quaternion targetRotation)
        {
            FocusCamera(false, Vector3.zero, false, 0, true, targetRotation);
        }

        /// <summary>
        /// Moves the Camera distance form the ground, animated
        /// </summary>
        /// <param name="targetDistanceOrSize"></param>
        public void MoveCameraTo(float targetDistanceOrSize)
        {
            FocusCamera(false, Vector3.zero, true, targetDistanceOrSize, false, Quaternion.identity);
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it, animated
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        public void MoveCameraTo(Vector3 targetPosition, float targetDistanceOrSize)
        {
            FocusCamera(targetPosition, targetDistanceOrSize);
        }

        /// <summary>
        /// Moves the Camera offset position on ground, and rotation, animated
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetRotation"></param>
        public void MoveCameraTo(Vector3 targetPosition, Vector2 targetRotation)
        {
            FocusCamera(targetPosition, targetRotation);
        }

        /// <summary>
        /// Moves the Camera offset position on ground, and rotation, animated
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetRotation"></param>
        public void MoveCameraTo(Vector3 targetPosition, Quaternion targetRotation)
        {
            FocusCamera(targetPosition, targetRotation);
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it and rotation, animated
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        public void MoveCameraTo(Vector3 targetPosition, float targetDistanceOrSize, Vector2 targetRotation)
        {
            FocusCamera(targetPosition, targetDistanceOrSize, targetRotation);
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it and rotation, animated
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        public void MoveCameraTo(Vector3 targetPosition, float targetDistanceOrSize, Quaternion targetRotation)
        {
            FocusCamera(targetPosition, targetDistanceOrSize, targetRotation);
        }

        #endregion

        #region FOCUS & FOLLOW
        [Header("FOCUS")]
        /// <summary>Multiplier applied to the focus radius when focusing on a target</summary>
        public float focusRadiusMultiplier = 1f;
        /// <summary>
        /// Alias of MoveCameraTo
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="instant"></param>
        public void FocusCamera(Vector3 targetPosition, bool instant = false)
        {
            FocusCamera(true, targetPosition, false, -1, false, Quaternion.identity, true, instant);
        }

        /// <summary>
        /// Alias of MoveCameraTo
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="instant"></param>
        public void FocusCamera(Vector3 targetPosition, float targetDistanceOrSize, bool instant = false)
        {
            FocusCamera(true, targetPosition, true, targetDistanceOrSize, false, Quaternion.identity, true, instant);
        }

        /// <summary>
        /// Alias of MoveCameraTo
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetRotation"></param>
        /// <param name="instant"></param>
        public void FocusCamera(Vector3 targetPosition, Vector2 targetRotation, bool instant = false)
        {
            FocusCamera(true, targetPosition, false, -1, true, GetRotationFromPitchYaw(targetRotation), true, instant);
        }

        /// <summary>
        /// Alias of MoveCameraTo
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetRotation"></param>
        /// <param name="instant"></param>
        public void FocusCamera(Vector3 targetPosition, Quaternion targetRotation, bool instant = false)
        {
            FocusCamera(true, targetPosition, false, -1, true, targetRotation, true, instant);
        }

        /// <summary>
        /// Alias of MoveCameraTo
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        /// <param name="instant"></param>
        public void FocusCamera(Vector3 targetPosition, float targetDistanceOrSize, Vector2 targetRotation, bool instant = false)
        {
            FocusCamera(true, targetPosition, true, targetDistanceOrSize, true, GetRotationFromPitchYaw(targetRotation), true, instant);
        }

        /// <summary>
        /// Alias of MoveCameraTo
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        /// <param name="instant"></param>
        public void FocusCamera(Vector3 targetPosition, float targetDistanceOrSize, Quaternion targetRotation, bool instant = false)
        {
            FocusCamera(true, targetPosition, true, targetDistanceOrSize, true, targetRotation, true, instant);
        }

        /// <summary>
        /// Setup the camera move animation
        /// </summary>
        /// <param name="targetOffsetPosition"></param>
        /// <param name="changeDistance"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="changeRotation"></param>
        /// <param name="targetRotation"></param>
        /// <param name="allowYOffsetFromGround"></param>
        /// <param name="instant"></param>
        virtual protected void FocusCamera(
           bool changeOffsetPostion, Vector3 targetOffsetPosition,
           bool changeDistance, float targetDistanceOrSize,
           bool changeRotation, Quaternion targetRotation,
           bool allowYOffsetFromGround = false,
           bool instant = false)
        {
            Log("FocusCamera(offset:" + targetOffsetPosition +
                " changeDistance:" + changeDistance + " dist:" + targetDistanceOrSize +
                " changeRotation:" + changeRotation + " rot:" + targetRotation.eulerAngles +
           " allowYOffset:" + allowYOffsetFromGround +
           " instant:" + instant, this);

            this.focusTargetPosition = ClampInCameraBoundaries(targetOffsetPosition);
            this.focusTargetDistanceOrSize = targetDistanceOrSize;
            this.focusTargetRotation = ClampRotation(targetRotation);

            this.isFocusingOrFollowing = true;
            this.enableFollowGameObject = false;
            this.enablePositionChange = changeOffsetPostion;
            this.enableDistanceChange = changeDistance;
            this.enableRotationChange = changeRotation;
            this.allowYOffsetFromGround = allowYOffsetFromGround;


            if (!enablePositionChange) followMoveOffset.Completed = true;
            if (!enableDistanceChange) followMoveDistanceOrSize.Completed = true;
            if (!enableRotationChange) followMoveRotation.Completed = true;


            CameraEvents.OnFocusStart?.Invoke();
        }

        /// <summary>
        /// Focus the camera on a GameObject (distance animation)
        /// </summary>
        /// <param name="go">The gameObject to get closer to</param>
        /// <param name="allowYOffsetFromGround">Allow offseting the camera from the ground to match the object's pivot y position and height</param>
        virtual public void FocusCameraOnGameObject(GameObject go, bool allowYOffsetFromGround = false)
        {
            Log("FocusCameraOnGameObject(go:" + go +
               " allowYOffsetFromGround:" + allowYOffsetFromGround, this);

            this.focusTargetGo = go;
            this.isFocusingOrFollowing = this.focusTargetGo != null;
            this.enableFollowGameObject = false;
            this.enablePositionChange = true;
            this.enableDistanceChange = true;
            this.enableRotationChange = false;
            this.allowYOffsetFromGround = allowYOffsetFromGround;


            if (!enablePositionChange) followMoveOffset.Completed = true;
            if (!enableDistanceChange) followMoveDistanceOrSize.Completed = true;
            if (!enableRotationChange) followMoveRotation.Completed = true;

            if (go != null) CameraEvents.OnFocusStart?.Invoke();
        }

        [Header("FOLLOW")]
        /// <summary>Spring settings used for follow movement animations</summary>
        public Springs followMove;
        /// <summary>Spring controlling the animated distance or orthographic size during follow</summary>
        public FloatSpring followMoveDistanceOrSize;
        /// <summary>Spring controlling the animated offset position during follow</summary>
        public Vector3Spring followMoveOffset;
        /// <summary>Spring controlling the animated rotation during follow</summary>
        public QuaternionSpring followMoveRotation;

        /// <summary>The GameObject the camera is currently focusing on or following</summary>
        protected GameObject focusTargetGo;
        /// <summary>The target offset position the camera is animating towards</summary>
        protected Vector3 focusTargetPosition;
        /// <summary>The target distance or orthographic size the camera is animating towards</summary>
        protected float focusTargetDistanceOrSize;
        /// <summary>The target rotation the camera is animating towards</summary>
        protected Quaternion focusTargetRotation;

        /// <summary>Whether distance change is enabled during the current focus or follow animation</summary>
        protected bool enableDistanceChange;
        /// <summary>Whether position change is enabled during the current focus or follow animation</summary>
        protected bool enablePositionChange;
        /// <summary>Whether rotation change is enabled during the current focus or follow animation</summary>
        protected bool enableRotationChange;
        /// <summary>Whether the camera is currently following a GameObject continuously</summary>
        protected bool enableFollowGameObject;
        /// <summary>Whether the camera is currently in a focus or follow animation</summary>
        protected bool isFocusingOrFollowing;
        /// <summary>Whether the camera is allowed to offset vertically from the ground plane during follow</summary>
        protected bool allowYOffsetFromGround;
        /// <summary>Whether user input is permitted while the camera is following a target</summary>
        protected bool allowUserInputDuringFollow;
        /// <summary>Accumulated user input offset applied on top of the follow position</summary>
        protected Vector3 userFollowOffset;

        /// <summary>
        /// Follow a game object
        /// </summary>
        /// <param name="go">The game object to follow</param>
        /// <param name="doFocus">Also focus on it (distance animation)</param>
        /// <param name="allowYOffsetFromGround">Allow offseting the camera from the ground to match the object's pivot y position and height</param>
        virtual public void FollowGameObject(GameObject go, bool doFocus, bool allowYOffsetFromGround = false, bool allowUserInput = false)
        {
            Log("FollowGameObject(go:" + go + " doFocus:" + doFocus +
               " allowYOffsetFromGround:" + allowYOffsetFromGround +
               " allowUserInput:" + allowUserInput, this);

            this.focusTargetGo = go;
            this.isFocusingOrFollowing = this.focusTargetGo != null;
            this.enableFollowGameObject = this.focusTargetGo != null;
            this.enablePositionChange = this.focusTargetGo != null;
            this.enableDistanceChange = doFocus && !allowUserInput;
            this.enableRotationChange = false;
            this.allowYOffsetFromGround = allowYOffsetFromGround;
            this.allowUserInputDuringFollow = allowUserInput && this.focusTargetGo != null;
            this.userFollowOffset = Vector3.zero;


            if (!enablePositionChange) followMoveOffset.Completed = true;
            if (!enableDistanceChange) followMoveDistanceOrSize.Completed = true;
            if (!enableRotationChange) followMoveRotation.Completed = true;

            if (go != null) CameraEvents.OnFocusStart?.Invoke();
        }

        /// <summary>
        /// Stop follow/focus/moveto animations
        /// </summary>
        public void StopFollow()
        {
            FollowGameObject(null, false);
        }


        /// <summary>
        /// Called each frame to check if all follow/focus animations have completed, and stops follow if so
        /// </summary>
        virtual protected void OnFollowFocusCompleted()
        {
            bool distanceCompleted = !enableDistanceChange || followMoveDistanceOrSize.Completed;
            bool rotationCompleted = !enableRotationChange || followMoveRotation.Completed;
            bool moveCompleted = !enablePositionChange || followMoveOffset.Completed;

            //Debug.Log("enableFollowGameObject:" + enableFollowGameObject);
            if (enableFollowGameObject)
                moveCompleted = false;

            Log("OnFollowFocusCompleted move:" + moveCompleted +
                " rotation:" + rotationCompleted +
                " distance:" + distanceCompleted);


            if (distanceCompleted && rotationCompleted && moveCompleted)
            {
                StopFollow();
                CameraEvents.OnFocusComplete?.Invoke(focusTargetGo);
            }
        }

        #endregion


        #region INERTIA
        [Header("INERTIA")]
        /// <summary>Whether translation inertia is enabled after the user releases input</summary>
        public bool enableTranslationInertia;
        /// <summary>Tracks whether the previous frame had user interaction for inertia calculation</summary>
        protected bool prevFrameInterction;
        /// <summary>Handles the position inertia movement animation</summary>
        public Move positionInertiaMove;
        /// <summary>Tracks position acceleration for computing translation inertia</summary>
        public Acceleration positionAcceleration;
        /// <summary>Multiplier for the position inertia force</summary>
        [Range(0.01f, 50)]
        public float positionInertiaForce = 1f;
        /// <summary>Whether move inertia is currently being applied</summary>
        protected bool isApplyingMoveInertia;

        /// <summary>Whether rotation inertia is enabled after the user releases input</summary>
        public bool enableRotationInertia;
        /// <summary>Handles the rotation inertia movement animation</summary>
        public Move rotationInertiaMove;
        /// <summary>Tracks rotation acceleration for computing rotation inertia</summary>
        public Acceleration rotationAcceleration;
        /// <summary>Multiplier for the rotation inertia force</summary>
        [Range(0.01f, 100)]
        public float rotationInertiaForce = 1f;


        /// <summary>
        /// Calculate the inertia of the camera base on previous frames
        /// </summary>
        virtual protected void CalculateInertia()
        {
            //print("CalculateInertia prevFrameInterction:" + prevFrameInterction);
            if (prevFrameInterction == false)
            {
                positionAcceleration.Clear();
                rotationAcceleration.Clear();
                prevFrameInterction = true;
            }
            positionAcceleration.AddPosition(finalOffset);
            rotationAcceleration.AddPosition(new Vector2(currentPitch, currentYaw), false, false, true);
            isApplyingMoveInertia = false;
        }


        /// <summary>
        /// Clear the inertia of the camera
        /// </summary>
        virtual protected void ClearInertia()
        {
            positionAcceleration.Clear();
            rotationAcceleration.Clear();
            prevFrameInterction = true;
            isApplyingMoveInertia = false;
        }



        /// <summary>
        /// Apply the inertia animation
        /// </summary>
        virtual protected void ApplyInertia()
        {
            //print("ApplyInertia prevFrameInterction:" + prevFrameInterction);
            if (prevFrameInterction == true)
            {
                positionAcceleration.AddPosition(finalOffset, false, true);
                rotationAcceleration.AddPosition(new Vector2(currentPitch, currentYaw), false, true);
                //print("positionAcceleration.OutputAcceleration:" + positionAcceleration.OutputAcceleration +
                //" magnitude:" + positionAcceleration.OutputAcceleration.magnitude);

                //print("rotationAcceleration.OutputAcceleration:" + rotationAcceleration.OutputAcceleration +
                //" magnitude:" + rotationAcceleration.OutputAcceleration.magnitude);

                positionInertiaMove.Init(positionAcceleration.OutputAcceleration.magnitude * positionInertiaForce,
                    positionAcceleration.OutputAcceleration.normalized);

                rotationInertiaMove.Init(rotationAcceleration.OutputAcceleration.magnitude * rotationInertiaForce,
                    rotationAcceleration.OutputAcceleration.normalized);

                prevFrameInterction = false;
            }

            if (enableTranslationInertia)
            {
                Vector3 posInertia = positionInertiaMove.Update();
                if (posInertia != Vector3.zero)
                {
                    isApplyingMoveInertia = true;
                    finalOffset += posInertia;
                    finalOffset = ClampInCameraBoundaries(finalOffset, out bool IsInBoundaries);

                }
                else
                {
                    isApplyingMoveInertia = false;
                }
            }
            else
            {
                isApplyingMoveInertia = false;
            }

            if (enableRotationInertia)
            {

                Vector3 rotInertia = rotationInertiaMove.Update();
                currentPitch += rotInertia.x;
                currentYaw += rotInertia.y;
                ClampPitchYaw();
                finalRotation = GetRotationFromPitchYaw();
                //print("rotInertia:" + rotInertia);

            }


        }
        #endregion


        #region DEBUG
#if DEBUG_TCP
        [Header("DEBUG")]
        protected string dbg;
#endif
        /// <summary>Timestamp of the last debug print to throttle debug output</summary>
        protected float lastPrint;
        /// <summary>
        /// Debug feature if DEBUG_TCP compilation variable is added
        /// </summary>
        protected void DebugInputs()
        {
#if DEBUG_TCP
            if (lastPrint > Time.time - 1)
                return;

            lastPrint = Time.time;

            dbg = ("[TCP] INPUT DETAILS ");
            dbg += "\n" + ("GetFingerCount " + CameraInputs.GetFingerCount());
            dbg += "\n" + ("GetMouseButton(0) " + BaseTouchInput.GetMouseIsHeld(0));
            dbg += "\n" + ("GetMouseButton(1) " + BaseTouchInput.GetMouseIsHeld(1));
            dbg += "\n" + ("GetMouseButton(2) " + BaseTouchInput.GetMouseIsHeld(2));
            dbg += "\n" + ("IsInputMatching Rotate " + IsInputMatching(InputMapFingerDrag.RotateAround));
            dbg += "\n" + ("IsInputMatching Translate " + IsInputMatching(InputMapFingerDrag.Translate));
            dbg += "\n" + ("oneFingerDrag " + oneFingerDrag);
            dbg += "\n" + ("twoFingerDrag " + twoFingerDrag);
            dbg += "\n" + ("isMobilePlatform " + Application.isMobilePlatform);
            dbg += "\n" + ("UseMouse " + InputTouch.Instance.UseMouse);
            dbg += "\n" + ("UseSimulator " + InputTouch.Instance.UseSimulator);

            if (!string.IsNullOrEmpty(dbg)) Debug.Log(dbg);
#endif
        }

        /// <summary>
        /// Logs a debug message prefixed with [TCP] when DEBUG_TCP is defined
        /// </summary>
        protected void Log(string msg, UnityEngine.Object context = null)
        {
#if DEBUG_TCP
            Debug.Log("[TCP] " + msg, context);
#endif
        }
        #endregion
    }
}

