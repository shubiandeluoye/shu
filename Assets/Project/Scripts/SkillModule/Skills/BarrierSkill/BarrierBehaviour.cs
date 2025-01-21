using UnityEngine;

namespace SkillModule.Skills
{
    public class BarrierBehaviour : MonoBehaviour
    {
        private float currentHealth;
        private float remainingDuration;
        private bool isActive;

        private bool blocksProjectiles;
        private bool blocksPlayers;
        private float damageReduction;

        public void Initialize(float health, bool blockProjectiles, bool blockPlayers, float reduction, float duration)
        {
            currentHealth = health;
            blocksProjectiles = blockProjectiles;
            blocksPlayers = blockPlayers;
            damageReduction = reduction;
            remainingDuration = duration;
            isActive = true;
        }

        private void Update()
        {
            if (!isActive) return;

            remainingDuration -= Time.deltaTime;
            if (remainingDuration <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!isActive) return;
            
            float reducedDamage = damage * (1 - damageReduction);
            currentHealth -= reducedDamage;

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!isActive) return;

            if (blocksProjectiles && other.gameObject.CompareTag("Projectile"))
            {
                TakeDamage(10f);
            }
            else if (blocksPlayers && other.gameObject.CompareTag("Player"))
            {
                var playerRb = other.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 pushDirection = (other.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDirection * 10f, ForceMode2D.Impulse);
                }
            }
        }
    }
} 