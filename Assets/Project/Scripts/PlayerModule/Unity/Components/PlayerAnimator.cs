using UnityEngine;
using PlayerModule.Core.Data;
using PlayerModule.Data;

namespace PlayerModule.Unity.Components
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;
        private PlayerController playerController;

        // 动画参数ID缓存
        private readonly int SpeedHash = Animator.StringToHash("Speed");
        private readonly int IsStunnedHash = Animator.StringToHash("IsStunned");
        private readonly int ShootTriggerHash = Animator.StringToHash("Shoot");
        private readonly int DamageTriggerHash = Animator.StringToHash("Damage");
        private readonly int DeathTriggerHash = Animator.StringToHash("Death");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerController = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            if (playerController != null)
            {
                // 订阅事件
                SubscribeToEvents();
            }
        }

        private void OnDisable()
        {
            // 取消订阅事件
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            // 这里可以使用事件系统订阅事件
            // 例如：EventManager.Subscribe(PlayerEvents.PlayerMoved, OnPlayerMoved);
        }

        private void UnsubscribeFromEvents()
        {
            // 取消事件订阅
            // 例如：EventManager.Unsubscribe(PlayerEvents.PlayerMoved, OnPlayerMoved);
        }

        // 事件处理方法
        private void OnPlayerMoved(PlayerMovedEvent evt)
        {
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, evt.Velocity.magnitude);
            }
        }

        private void OnPlayerStunned(PlayerStunEvent evt)
        {
            if (animator != null)
            {
                animator.SetBool(IsStunnedHash, true);
                // 设置定时器来重置眩晕状态
                StartCoroutine(ResetStunState(evt.Duration));
            }
        }

        private void OnPlayerShoot(PlayerShootEvent evt)
        {
            if (animator != null)
            {
                animator.SetTrigger(ShootTriggerHash);
            }
        }

        private void OnHealthChanged(PlayerHealthChangedEvent evt)
        {
            if (animator != null && evt.ChangeAmount < 0)
            {
                animator.SetTrigger(DamageTriggerHash);
            }
        }

        private void OnPlayerDeath(PlayerDeathEvent evt)
        {
            if (animator != null)
            {
                animator.SetTrigger(DeathTriggerHash);
            }
        }

        private System.Collections.IEnumerator ResetStunState(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (animator != null)
            {
                animator.SetBool(IsStunnedHash, false);
            }
        }

        // 公共方法，用于直接控制动画
        public void PlayAnimation(string animationName)
        {
            if (animator != null)
            {
                animator.Play(animationName);
            }
        }

        public void SetAnimationSpeed(float speed)
        {
            if (animator != null)
            {
                animator.speed = speed;
            }
        }
    }
} 