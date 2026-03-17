using Exoa.Common;
using Exoa.Designer;
using Exoa.Events;
using Exoa.Touch;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Cameras
{
    /// <summary>
    /// Base class for orthographic camera controllers, providing zoom, pan, rotation, and focus/follow functionality.
    /// </summary>
    public class CameraOrthoBase : CameraBase, ITouchOrthoCamera
    {
        /// <summary>
        /// Min and max allowed orthographic size values for zoom clamping.
        /// </summary>
        [Header("DISTANCE")]
        public Vector2 sizeMinMax = new Vector2(1, 12);

        /// <summary>
        /// The current target orthographic size of the camera.
        /// </summary>
        protected float finalSize = 5.0f;

        /// <summary>
        /// Conversion factor from world distance to orthographic size.
        /// </summary>
        protected const float distanceToSize = .4f;

        /// <summary>
        /// Initial orthographic size used for reset.
        /// </summary>
        protected float initSize = 6f;

        /// <summary>
        /// Initial distance from ground used for reset.
        /// </summary>
        protected float initDistance = 10f;

        /// <summary>
        /// Fixed Z distance of the camera from the ground plane.
        /// </summary>
        [Header("Z DISTANCE")]
        public float fixedDistance = 30f;

        /// <summary>
        /// returns the camera orthographic size
        /// </summary>
        public float FinalSize
        {
            set => finalSize = value;
            get => finalSize;
        }

        /// <summary>
        /// Init some camera parameters
        /// </summary>
        override protected void Init()
        {
            base.Init();
            finalSize = initSize;
        }


        /// <summary>
        /// Main update loop handling touch input for zoom, rotation, translation, and inertia.
        /// </summary>
        virtual protected void Update()
        {
            if (disableMoves)
                return;

            List<TouchFinger> twoFingers = CameraInputs.TwoFingerFilter.UpdateAndGetFingers();
            List<TouchFinger> oneFinger = CameraInputs.OneFingerFilter.UpdateAndGetFingers();
            float zoomRatio = 1;
            bool anyInteraction = false;
            bool IsInBoundaries = true;
            Vector2 screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

            worldPointCameraCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(screenCenter));
            worldPointFingersDelta = Vector3.zero;
            worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(CameraInputs.screenPointAnyFingerCountCenter));
            twistRot = Quaternion.identity;

            if (IsInputMatching(InputMapFingerDrag.RotateAround))
            {
                RotateFromVector(CameraInputs.GetAnyPixelScaledDelta());
                finalRotation = GetRotationFromPitchYaw();
                finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapFingerDrag.RotateHead))
            {
                RotateFromVector(CameraInputs.GetAnyPixelScaledDelta());
                finalRotation = GetRotationFromPitchYaw();
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapFingerPinch.ZoomAndRotate) || IsInputMatching(InputMapFingerPinch.ZoomOnly))
            {
                zoomRatio = CameraInputs.pinchRatio;
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapScrollWheel.ZoomInCenter))
            {
                zoomRatio = CameraInputs.GetScroll();
                worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(screenCenter));
                anyInteraction = true;
            }

            if (IsInputMatching(InputMapScrollWheel.ZoomUnderMouse))
            {
                zoomRatio = CameraInputs.GetScroll();
                worldPointFingersCenter = ClampInCameraBoundaries(HeightScreenDepth.Convert(BaseTouchInput.GetMousePosition()));
                anyInteraction = true;
            }

            finalSize = Mathf.Clamp(finalSize * zoomRatio, sizeMinMax.x, sizeMinMax.y);

            if ((sizeMinMax.y == finalSize && zoomRatio > 1) || (sizeMinMax.x == finalSize && zoomRatio < 1))
            {
                zoomRatio = 1;
            }

            if (IsInputMatching(InputMapFingerPinch.ZoomAndRotate) || IsInputMatching(InputMapFingerPinch.RotateOnly))
            {
                twistRot = Quaternion.AngleAxis(allowYawRotation ? CameraInputs.twistDegrees : 0, GetRotateAroundVector());
                twistRot = ClampTwist(twistRot, finalRotation);
                anyInteraction = true;
            }
            if (IsInputMatching(InputMapFingerDrag.Translate))
            {
                if (!isFocusingOrFollowing)
                {
                    worldPointFingersDelta = Vector3.ClampMagnitude(HeightScreenDepth.ConvertDelta(CameraInputs.lastScreenPointAnyFingerCountCenter,
                    CameraInputs.screenPointAnyFingerCountCenter, gameObject), maxTranslationSpeed);
                    anyInteraction = true;
                }
                else if (allowUserInputDuringFollow)
                {
                    Vector3 dragDelta = Vector3.ClampMagnitude(HeightScreenDepth.ConvertDelta(CameraInputs.lastScreenPointAnyFingerCountCenter,
                    CameraInputs.screenPointAnyFingerCountCenter, gameObject), maxTranslationSpeed);
                    userFollowOffset -= dragDelta;
                    anyInteraction = true;
                }
            }
            Vector3 vecFingersCenterToCamera = (finalPosition - worldPointFingersCenter);
            float vecFingersCenterToCameraDistance = vecFingersCenterToCamera.magnitude * zoomRatio;
            vecFingersCenterToCamera = vecFingersCenterToCamera.normalized * vecFingersCenterToCameraDistance;

            Vector3 targetPosition = worldPointFingersCenter + vecFingersCenterToCamera;

            Vector3 offsetFromFingerCenter = worldPointFingersCenter - worldPointFingersDelta;

            finalRotation = twistRot * finalRotation;
            finalRotation = ClampRotation(finalRotation);
            finalPosition = twistRot * (targetPosition - worldPointFingersCenter) + offsetFromFingerCenter;

            Vector2 pitchYaw = GetRotationToPitchYaw(finalRotation);
            currentPitch = pitchYaw.x;
            currentYaw = pitchYaw.y;

            Vector3 newWorldPointCameraCenter = CalculateOffset(finalPosition, finalRotation);
            Vector3 newWorldPointCameraCenterClamped = ClampInCameraBoundaries(newWorldPointCameraCenter, out IsInBoundaries);

            finalOffset = newWorldPointCameraCenterClamped;

            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);


            // Apply Edge Boundaries
            if (IsUsingCameraEdgeBoundaries())
            {
                finalPosition = ClampCameraCorners(finalPosition, out IsInBoundaries, pitchYaw.x);
                finalOffset = CalculateOffset(finalPosition, finalRotation, finalDistance, groundHeight);
            }

            if (isFocusingOrFollowing)
            {
                HandleFocusAndFollow();
                ApplyToCamera();
                return;
            }
            if (!IsInBoundaries)
            {
                ClearInertia();
            }
            else if (anyInteraction)
            {
                CalculateInertia();
            }
            else
            {
                ApplyInertia();

                finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            }

            ApplyToCamera();

        }



        /// <summary>
        /// Converts a distance from ground to a camera orthographic size
        /// </summary>
        /// <param name="d"></param>
        public void SetSizeByDistance(float d)
        {
            finalDistance = d;
            finalSize = Mathf.Clamp(d * distanceToSize, sizeMinMax.x, sizeMinMax.y);
        }

        /// <summary>
        /// Clamps the distance and updates the orthographic size accordingly.
        /// </summary>
        /// <param name="distance">The raw distance value to clamp.</param>
        /// <returns>The original distance value.</returns>
        override protected float CalculateClampedDistance(float distance)
        {
            finalSize = Mathf.Clamp(distance * distanceToSize, sizeMinMax.x, sizeMinMax.y);
            return distance;
        }

        /// <summary>
        /// Clamps the orthographic size within the allowed min/max range.
        /// </summary>
        /// <param name="size">The raw size value to clamp.</param>
        /// <returns>The original size value.</returns>
        protected float CalculateClampedSize(float size)
        {
            finalSize = Mathf.Clamp(size, sizeMinMax.x, sizeMinMax.y);
            return size;
        }

        /// <summary>
        /// Converts the orthographic size to a distance from ground
        /// </summary>
        /// <returns></returns>
        public float GetDistanceFromSize()
        {
            return finalSize / distanceToSize;
        }

        /// <summary>
        /// In case the camera is standalone (no CameraModeSwitcher) then this is apply 
        /// the position and rotation to the camera
        /// </summary>
        override protected void ApplyToCamera()
        {
            if (!standalone || !enabled)
                return;

            base.ApplyToCamera();

            cam.orthographicSize = finalSize;
        }

        /// <summary>
        /// Return the matrix of the camera transform, in order to blend it when switching modes
        /// </summary>
        /// <returns></returns>
        override public Matrix4x4 GetMatrix()
        {
            float aspect = cam.aspect;
            float near = cam.nearClipPlane, far = cam.farClipPlane;
            finalSize = Mathf.Clamp(finalSize, sizeMinMax.x, sizeMinMax.y);
            return Matrix4x4.Ortho(-finalSize * aspect, finalSize * aspect, -finalSize, finalSize, near, far);
        }

        /// <summary>
        /// Converts a camera offset on ground to a camera position
        /// </summary>
        virtual public void SetPositionByOffset()
        {
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
        }

        /// <summary>
        /// Set the initial values for the reset function
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rotation"></param>
        /// <param name="distanceOrSize"></param>
        override public void SetResetValues(Vector3 offset, Quaternion rotation, float size)
        {
            initOffset = offset;
            initRotation = rotation;
            initSize = size;
        }




        #region FOLLOW & FOCUS

        /// <summary>
        /// Moves the Camera offset position on ground and distance from it, in 1 frame
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        override public void MoveCameraToInstant(float targetDistanceOrSize)
        {
            //FocusCamera(targetPosition, targetDistanceOrSize, true);
            finalSize = CalculateClampedSize(targetDistanceOrSize);
            finalDistance = GetDistanceFromSize();
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
            Log("MoveCameraToInstant finalSize:" + finalSize + " finalDistance:" + finalDistance);
            ApplyToCamera();
        }


        /// <summary>
        /// Moves the Camera offset position on ground and distance from it, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetDistanceOrSize"></param>
        override public void MoveCameraToInstant(Vector3 targetOffset, float targetDistanceOrSize)
        {
            //FocusCamera(targetPosition, targetDistanceOrSize, true);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            finalSize = CalculateClampedSize(targetDistanceOrSize);
            //finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it and rotation, in 1 frame
        /// </summary>
        /// <param name="targetOffset"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        override public void MoveCameraToInstant(Vector3 targetOffset, float targetDistanceOrSize, Vector2 targetRotation)
        {
            //FocusCamera(targetPosition, targetDistanceOrSize, targetRotation, true);
            finalRotation = GetRotationFromPitchYaw(targetRotation);
            finalRotation = ClampRotation(finalRotation);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            //finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalSize = CalculateClampedSize(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
        }

        /// <summary>
        /// Moves the Camera offset position on ground, distance from it and rotation, in 1 frame
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetDistanceOrSize"></param>
        /// <param name="targetRotation"></param>
        override public void MoveCameraToInstant(Vector3 targetOffset, float targetDistanceOrSize, Quaternion targetRotation)
        {
            //FocusCamera(targetOffset, targetDistanceOrSize, targetRotation, true);
            finalRotation = ClampRotation(targetRotation);
            finalOffset = ClampInCameraBoundaries(targetOffset);
            //finalDistance = CalculateClampedDistance(targetDistanceOrSize);
            finalSize = CalculateClampedSize(targetDistanceOrSize);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            ApplyToCamera();
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
        override protected void FocusCamera(
            bool changeOffsetPosition, Vector3 targetOffsetPosition,
           bool changeDistance, float targetDistanceOrSize,
           bool changeRotation, Quaternion targetRotation,
           bool allowYOffsetFromGround = false,
           bool instant = false)
        {

            if (!instant)
            {
                followMoveOffset.Reset(finalOffset);
                followMoveDistanceOrSize.Reset(finalSize);
                followMoveRotation.Reset(finalRotation);
            }
            else
            {
                followMoveOffset.Reset(targetOffsetPosition);
                followMoveDistanceOrSize.Reset(targetDistanceOrSize);
                followMoveRotation.Reset(targetRotation);
            }

            base.FocusCamera(changeOffsetPosition, targetOffsetPosition, changeDistance, targetDistanceOrSize,
                changeRotation, targetRotation, allowYOffsetFromGround, instant);
        }


        /// <summary>
        /// Focus the camera on a GameObject (distance animation)
        /// </summary>
        /// <param name="go">The gameObject to get closer to</param>
        /// <param name="allowYOffsetFromGround">Allow offseting the camera from the ground to match the object's pivot y position and height</param>
        override public void FocusCameraOnGameObject(GameObject go, bool allowYOffsetFromGround = false)
        {
            followMoveOffset.Reset(finalOffset);
            followMoveDistanceOrSize.Reset(finalSize);

            base.FocusCameraOnGameObject(go, allowYOffsetFromGround);
        }

        /// <summary>
        /// Follow a game object
        /// </summary>
        /// <param name="go">The game object to follow</param>
        /// <param name="doFocus">Also focus on it (distance animation)</param>
        /// <param name="allowYOffsetFromGround">Allow offseting the camera from the ground to match the object's pivot y position and height</param>
        /// <param name="allowUserInput">Whether user input is permitted during the follow operation.</param>
        override public void FollowGameObject(GameObject go, bool doFocus, bool allowYOffsetFromGround = false, bool allowUserInput = false)
        {
            followMoveOffset.Reset(finalOffset);
            followMoveDistanceOrSize.Reset(finalSize);

            base.FollowGameObject(go, doFocus, allowYOffsetFromGround, allowUserInput);
        }


        /// <summary>
        /// Handle the camera focus/follow/moveto
        /// </summary>
        virtual protected void HandleFocusAndFollow()
        {
            if (!isFocusingOrFollowing)
                return;

            if (focusTargetGo == null || !focusTargetGo)
            {
                StopFollow();
                return;
            }

            {
                Bounds b = focusTargetGo.GetBoundsRecursive();

                if (b.size != Vector3.zero || b.center != Vector3.zero)
                {
                    // offseting the bounding box
                    if (allowYOffsetFromGround)
                    {
                        float yOffset = b.center.y;
                        b.extents = b.extents.SetY(b.extents.y + yOffset);

                    }
                    b.center = b.center.SetY(groundHeight);

                    Vector3 max = b.size;
                    // Get the radius of a sphere circumscribing the bounds
                    float radius = max.magnitude * focusRadiusMultiplier;

                    focusTargetPosition = b.center;
                    focusTargetPosition = ClampInCameraBoundaries(focusTargetPosition);
                    focusTargetDistanceOrSize = Mathf.Clamp(radius, sizeMinMax.x, sizeMinMax.y);
                }
            }

            if (enableDistanceChange && !followMoveDistanceOrSize.Completed)
            {
                finalSize = followMove.Update(ref followMoveDistanceOrSize,
                    focusTargetDistanceOrSize, OnFollowFocusCompleted);
            }
            if (enableRotationChange && !followMoveRotation.Completed)
            {
                finalRotation = followMove.Update(ref followMoveRotation,
                    focusTargetRotation, OnFollowFocusCompleted);
                currentPitch = finalRotation.eulerAngles.x;
                currentYaw = finalRotation.eulerAngles.y;
            }
            if (enablePositionChange)
            {
                if (enableFollowGameObject && followMoveOffset.Completed)
                    followMoveOffset.Completed = false;

                Vector3 followTarget = focusTargetPosition + userFollowOffset;

                if (!followMoveOffset.Completed)
                {
                    finalOffset = worldPointCameraCenter = followMove.Update(ref followMoveOffset,
                        followTarget, OnFollowFocusCompleted);
                }
            }

            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

        }

        #endregion

    }
}
