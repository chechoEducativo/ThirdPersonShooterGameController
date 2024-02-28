using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ThirdPersonShooter
{
    public class ThirdPersonShooterAimController : ThirdPersonShooterPlayerScript
    {

        [System.Serializable]
        public class BoolUnityEvent : UnityEvent<bool>
        {
            
        }


        [SerializeField] private Transform aimTarget;
        [SerializeField] private LayerMask detectionMask;

        [Header("Camera rotation")]
        [SerializeField] [Range(0,90)] private float aimMaxAngle;
        [SerializeField] [Range(0,-90)] private float aimMinAngle;
        [SerializeField] private RotationConstraint cameraParentRotator;
        
        [Header("Character animation")]
        [SerializeField] private Vector2 horizontalLimits;
        [SerializeField] private Vector2 verticalLimits;
        [SerializeField] private AimConstraint torsoAimConstraint;

        public BoolUnityEvent OnToggleAim;


        private readonly Vector2Dampener lookInputDampener = new Vector2Dampener(0.1f, 100f);
        private ThirdPersonShooterCameraManager cameraManager;
        private Animator animator;

        private int ikLayerIndex;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            ikLayerIndex = animator.GetLayerIndex("AimingMode");
            cameraManager = GetComponent<ThirdPersonShooterCameraManager>();
        }

        public void Aim(InputAction.CallbackContext context)
        {
            Vector2 inputValue = context.ReadValue<Vector2>();
            lookInputDampener.TargetValue = inputValue;
        }

        public void ToggleAim(InputAction.CallbackContext context)
        {
            float inputValue = context.ReadValue<float>();
            if (inputValue == 0)
            {
                playerData.State = ThirdPersonShooterPlayerData.PlayerState.NormalMode;
            }
            else
            {
                playerData.State = ThirdPersonShooterPlayerData.PlayerState.AimingMode;
                Rect pixelRect = cameraManager.Camera.pixelRect;
                lookInputDampener.TargetValue = new Vector2(pixelRect.width * 0.5f, pixelRect.height * 0.5f);
            }
        }

        protected override void OnStateChanged(ThirdPersonShooterPlayerData.PlayerState state)
        {
            if (state == ThirdPersonShooterPlayerData.PlayerState.AimingMode)
            {
                animator.SetLayerWeight(ikLayerIndex, 1);
                torsoAimConstraint.weight = 1;
                cameraParentRotator.weight = 1;
                OnToggleAim?.Invoke(true);
            }
            else
            {
                torsoAimConstraint.weight = 0;
                cameraParentRotator.weight = 0;
                animator.SetLayerWeight(ikLayerIndex, 0);
                OnToggleAim?.Invoke(false);
            }
        }

        private bool EvaluateScreenLimits(out Vector2 motionDirection)
        {
            Rect pixelRect = cameraManager.Camera.pixelRect;
            Vector2 rectSize = new Vector2(pixelRect.width, pixelRect.height);
            Vector2 currentInput =(lookInputDampener.CurrentValue / rectSize) -new Vector2(0.5f, 0.5f);
            motionDirection = currentInput;
            return currentInput.x < horizontalLimits.x || currentInput.x > horizontalLimits.y ||
                   currentInput.y < verticalLimits.x || currentInput.y > verticalLimits.y;
        }

        private void Update()
        {
            lookInputDampener.Update();
            if (playerData.State != ThirdPersonShooterPlayerData.PlayerState.AimingMode) return;
            Vector2 dampInput = lookInputDampener.CurrentValue;
            Transform cameraTransform = cameraManager.CameraTransform;
            Ray ray = cameraManager.Camera.ScreenPointToRay(dampInput);
            Vector3 targetPosition = ray.origin + ray.direction * 10;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, detectionMask))
            {
                targetPosition = hit.point;
            }
            
            aimTarget.position = targetPosition;

            if (EvaluateScreenLimits(out Vector2 motionDirection))
            {
                motionDirection = motionDirection.normalized;
                transform.RotateAround(transform.position, transform.up, motionDirection.x * 80 * Time.deltaTime);
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (playerData.State != ThirdPersonShooterPlayerData.PlayerState.AimingMode) return;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, aimTarget.position);
            //animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, aimTarget.position);
            //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
        }
    }
}
