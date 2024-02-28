using System;
using Cinemachine;
using UnityEngine;


namespace ThirdPersonShooter
{
    /// <summary>
    /// Handles Camera instantiation and camera modes
    /// </summary>
    public class ThirdPersonShooterCameraManager : ThirdPersonShooterPlayerScript
    {
        [SerializeField] private CinemachineBrain camera;
        [SerializeField] private CinemachineVirtualCamera normalMotionCamera;
        [SerializeField] private CinemachineVirtualCamera aimingMotionCamera;

        protected override void OnStateChanged(ThirdPersonShooterPlayerData.PlayerState state)
        {
            switch (state)
            {
                case ThirdPersonShooterPlayerData.PlayerState.NormalMode:
                    normalMotionCamera.gameObject.SetActive(true);
                    aimingMotionCamera.gameObject.SetActive(false);
                    break;
                case ThirdPersonShooterPlayerData.PlayerState.AimingMode:
                    normalMotionCamera.gameObject.SetActive(false);
                    aimingMotionCamera.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public Camera Camera => camera.OutputCamera;
        public Transform CameraTransform => camera.transform;
    }
}
