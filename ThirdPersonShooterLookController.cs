using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonShooter
{
    public class ThirdPersonShooterLookController : ThirdPersonShooterPlayerScript
    {
        private readonly Vector2Dampener inputDampener = new Vector2Dampener(0.1f, 100f);
        
        [SerializeField] private Transform cameraPivot;

        public void Look(InputAction.CallbackContext context)
        {
            if (playerData.State != ThirdPersonShooterPlayerData.PlayerState.NormalMode) return;
            Vector2 inputValue = context.ReadValue<Vector2>();
            inputDampener.TargetValue = inputValue;
        }

        protected override void OnStateChanged(ThirdPersonShooterPlayerData.PlayerState state)
        {
            if (state == ThirdPersonShooterPlayerData.PlayerState.NormalMode)
            {
                cameraPivot.forward = Vector3.ProjectOnPlane(GetComponent<ThirdPersonShooterCameraManager>().CameraTransform.forward, transform.up);
            }
        }

        private void Update()
        {
            inputDampener.Update();
            Vector2 currentInput = inputDampener.CurrentValue;
            cameraPivot.RotateAround(transform.position, transform.up, currentInput.x);
            cameraPivot.RotateAround(transform.position, cameraPivot.right, -currentInput.y);
        }
    }
}
