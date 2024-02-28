using UnityEngine;

namespace ThirdPersonShooter
{
    
    
    [RequireComponent(typeof(ThirdPersonShooterPlayerData))]
    public abstract class ThirdPersonShooterPlayerScript : MonoBehaviour1
    {
        protected ThirdPersonShooterPlayerData playerData;

        protected virtual void OnStateChanged(ThirdPersonShooterPlayerData.PlayerState state)
        {
            
        }
        
        
        protected virtual void OnEnable()
        {
            playerData = GetComponent<ThirdPersonShooterPlayerData>();
            playerData.OnStateChanged.AddListener(OnStateChanged);
        }

        protected virtual void OnDisable()
        {
            playerData.OnStateChanged.RemoveListener(OnStateChanged);
        }
    }
}
