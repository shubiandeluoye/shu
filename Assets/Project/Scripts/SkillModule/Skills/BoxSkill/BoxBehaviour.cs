using UnityEngine;

namespace SkillModule.Skills
{
    public class BoxBehaviour : MonoBehaviour
    {
        private float currentDurability;
        private bool isPlaced;
        private bool canBePushed;
        private float pushForce;

        public void Initialize(float durability, bool canBePush, float force)
        {
            currentDurability = durability;
            canBePushed = canBePush;
            pushForce = force;
            isPlaced = true;
        }

        public void TakeDamage(float damage)
        {
            if (!isPlaced) return;
            
            currentDurability -= damage;
            if (currentDurability <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!canBePushed || !isPlaced) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                var direction = (transform.position - collision.transform.position).normalized;
                GetComponent<Rigidbody2D>()?.AddForce(direction * pushForce, ForceMode2D.Impulse);
            }
        }
    }
} 