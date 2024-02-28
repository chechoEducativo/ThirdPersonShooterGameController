using System;
using UnityEngine;
using UnityEngine.Events;

namespace ThirdPersonShooter
{
    [Serializable]
    public class ThirdPersonShooterPlayerStateEvent : UnityEvent<ThirdPersonShooterPlayerData.PlayerState>
    {
        
    }
    public class ThirdPersonShooterPlayerData : MonoBehaviour1
    {
        [Serializable]
        public enum PlayerState
        {
            NormalMode,
            AimingMode,
        }

        public ThirdPersonShooterPlayerStateEvent OnStateChanged;
        private PlayerState state;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        public PlayerState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value != state)
                {
                    OnStateChanged?.Invoke(value);
                }

                state = value;
            }
        }
    }
}
