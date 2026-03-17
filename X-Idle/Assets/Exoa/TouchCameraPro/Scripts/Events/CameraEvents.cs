using UnityEngine;

namespace Exoa.Events
{
    /// <summary>
    /// Central event hub for the TouchCameraPro camera system.
    /// All camera-related communication (focus, follow, perspective switch, etc.)
    /// flows through static delegates defined here.
    /// </summary>
    public class CameraEvents
    {
        /// <summary>
        /// Available button actions that can be requested through OnRequestButtonAction
        /// </summary>
        public enum Action
        {
            /// <summary>Toggle between orthographic and perspective modes</summary>
            SwitchPerspective,
            /// <summary>Enable or disable all camera movement inputs</summary>
            DisableCameraMoves,
            /// <summary>Show the help overlay</summary>
            Help,
            /// <summary>Reset the camera to its initial position and rotation</summary>
            ResetCameraPositionRotation,
            /// <summary>Force the camera into perspective mode if currently in orthographic</summary>
            ForcePerspectiveMode,
            /// <summary>Zoom the camera in by one step</summary>
            ZoomIn,
            /// <summary>Zoom the camera out by one step</summary>
            ZoomOut,
        };

        #region DELEGATES

        /// <summary>
        /// Delegate for requesting a ground height change with optional animation
        /// </summary>
        /// <param name="newHeight">The new ground height value</param>
        /// <param name="animate">Whether to animate the transition</param>
        /// <param name="duration">Duration of the animation in seconds</param>
        public delegate void OnRequestGroundHeightChangeHandler(float newHeight, bool animate, float duration);

        /// <summary>
        /// Delegate for button action events
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="active">Whether the action should be activated or deactivated</param>
        public delegate void OnCameraBoolEvent(Action action, bool active);

        /// <summary>
        /// Delegate for events that pass a single GameObject reference
        /// </summary>
        /// <param name="obj">The target GameObject</param>
        public delegate void OnCameraGameObjectEvent(GameObject obj);

        /// <summary>
        /// Delegate for requesting the camera to focus on a GameObject (animated zoom to fit)
        /// </summary>
        /// <param name="obj">The GameObject to focus on</param>
        /// <param name="allowYOffsetFromGround">Allow the camera to offset vertically to match the object's pivot height</param>
        public delegate void OnRequestObjectFocusHandler(GameObject obj, bool allowYOffsetFromGround);

        /// <summary>
        /// Delegate for requesting the camera to continuously follow a GameObject
        /// </summary>
        /// <param name="obj">The GameObject to follow (pass null to stop following)</param>
        /// <param name="focusOnFollow">Also animate the zoom distance to fit the object's bounds</param>
        /// <param name="allowYOffsetFromGround">Allow the camera to offset vertically to match the object's pivot height</param>
        /// <param name="allowUserInput">When true, the user can still zoom, rotate, and translate around the followed object</param>
        public delegate void OnRequestObjectFollowHandler(GameObject obj, bool focusOnFollow, bool allowYOffsetFromGround, bool allowUserInput = false);

        /// <summary>
        /// Delegate for perspective switch events
        /// </summary>
        /// <param name="orthoMode">True if the camera is in (or switching to) orthographic mode</param>
        public delegate void OnSwitchPerspectiveHandler(bool orthoMode);

        /// <summary>
        /// Delegate for parameterless camera events
        /// </summary>
        public delegate void OnCameraEventHandler();

        #endregion

        #region EVENTS

        /// <summary>Fired just before the perspective mode switch begins</summary>
        public static OnSwitchPerspectiveHandler OnBeforeSwitchPerspective;

        /// <summary>Fired just after the perspective mode switch completes</summary>
        public static OnSwitchPerspectiveHandler OnAfterSwitchPerspective;

        /// <summary>Fired when a UI button action is requested</summary>
        public static OnCameraBoolEvent OnRequestButtonAction;

        /// <summary>Fired to request the camera to focus on a GameObject</summary>
        public static OnRequestObjectFocusHandler OnRequestObjectFocus;

        /// <summary>Fired to request the camera to follow a GameObject</summary>
        public static OnRequestObjectFollowHandler OnRequestObjectFollow;

        /// <summary>Fired to request a ground height change</summary>
        public static OnRequestGroundHeightChangeHandler OnRequestGroundHeightChange;

        /// <summary>Fired when a focus or follow animation starts</summary>
        public static OnCameraEventHandler OnFocusStart;

        /// <summary>Fired when a focus animation completes, passing the focused GameObject</summary>
        public static OnCameraGameObjectEvent OnFocusComplete;

        /// <summary>Fired to request stopping any active focus or follow animation</summary>
        public static OnCameraEventHandler OnRequestStopFocusFollow;

        #endregion
    }
}
