using UnityEngine;
using UnityEngine.InputSystem;


namespace ThirdPersonShooter
{
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonShooterMotionController : ThirdPersonShooterPlayerScript
    {
        private readonly Vector2Dampener inputDampener = new Vector2Dampener(0.5f, 3f);
        private ThirdPersonShooterCameraManager cameraManager;
        private Animator animator;
        
        public void Move(InputAction.CallbackContext context)
        {
            Vector2 inputValue = context.ReadValue<Vector2>();
            inputDampener.TargetValue = inputValue;
        }

        private Vector3 GetDirectionVector()
        {
            Vector3 ret;
            Transform cameraTransform = cameraManager.CameraTransform;
            Vector3 cameraForward = Vector3.Lerp(cameraTransform.forward, cameraTransform.up,
                Mathf.Abs(Vector3.Dot(cameraTransform.forward, transform.up)));
            ret = Vector3.ProjectOnPlane(cameraForward, transform.up) * inputDampener.CurrentValue.y +
                  cameraTransform.right * inputDampener.CurrentValue.x;
            return ret.normalized;
        }

        private void Awake()
        {
            cameraManager = GetComponent<ThirdPersonShooterCameraManager>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            inputDampener.Update();
            if (playerData.State == ThirdPersonShooterPlayerData.PlayerState.NormalMode)
            {
                Vector3 movementDirection = GetDirectionVector();

                //Rotate by camera and set animation data
                transform.rotation = Quaternion.LookRotation(movementDirection, transform.up);
                animator.SetFloat("MotionY", inputDampener.CurrentValue.magnitude);
                animator.SetFloat("MotionX", 0);
            }
            else
            {
                animator.SetFloat("MotionY", inputDampener.CurrentValue.y);
                animator.SetFloat("MotionX", inputDampener.CurrentValue.x);
            }
        }
    }
}
