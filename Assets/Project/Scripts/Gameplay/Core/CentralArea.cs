using UnityEngine;
using System.Collections;
using Core;
using Core.FSM;

namespace Gameplay.Core
{
    /// <summary>
    /// Controls the central area mechanics including bullet collection and firing
    /// </summary>
    public class CentralArea : MonoBehaviour
    {
        [Header("Area Properties")]
        [SerializeField] private float radius = 1f; // For 2x2 circular area
        [SerializeField] private int requiredBullets = 21;
        [SerializeField] private Vector2 randomFireInterval = new Vector2(5f, 8f);
        [SerializeField] private float autoExplodeTime = 30f;

        private CircleCollider2D areaCollider;
        private int collectedBullets;
        private float explodeTimer;
        private bool isCharging;
        private StateMachine stateMachine;

        private void Awake()
        {
            // Set up collider
            areaCollider = gameObject.AddComponent<CircleCollider2D>();
            areaCollider.radius = radius;
            areaCollider.isTrigger = true;

            // Initialize state machine
            stateMachine = new StateMachine();
            stateMachine.AddState(new CollectingState(this));
            stateMachine.AddState(new ChargingState(this));
            stateMachine.AddState(new FiringState(this));

            Reset();
        }

        private void Start()
        {
            stateMachine.ChangeState<CollectingState>();
            StartCoroutine(AutoExplodeTimer());
        }

        private void Update()
        {
            stateMachine.Update();
        }

        public void CollectBullet()
        {
            collectedBullets++;
            if (collectedBullets >= requiredBullets)
            {
                isCharging = true;
                stateMachine.ChangeState<ChargingState>();
            }
        }

        public void FireBullets()
        {
            // Fire collected bullets in a pattern
            float angleStep = 360f / collectedBullets;
            for (int i = 0; i < collectedBullets; i++)
            {
                Vector2 direction = Quaternion.Euler(0, 0, angleStep * i) * Vector2.right;
                var bullet = ObjectPool.Instance.SpawnFromPool("Bullet", transform.position, Quaternion.identity);
                bullet.GetComponent<BulletController>().Initialize(direction, angleStep * i);
            }

            Reset();
        }

        private void Reset()
        {
            collectedBullets = 0;
            isCharging = false;
            explodeTimer = autoExplodeTime;
            stateMachine.ChangeState<CollectingState>();
        }

        private IEnumerator AutoExplodeTimer()
        {
            while (true)
            {
                if (!isCharging)
                {
                    explodeTimer -= Time.deltaTime;
                    if (explodeTimer <= 0)
                    {
                        FireBullets();
                        yield break;
                    }
                }
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                CollectBullet();
                ObjectPool.Instance.ReturnToPool("Bullet", other.gameObject);
            }
        }
    }

    // State implementations
    public class CollectingState : IState
    {
        private CentralArea area;
        public CollectingState(CentralArea area) => this.area = area;
        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }

    public class ChargingState : IState
    {
        private CentralArea area;
        private float chargeTimer;
        public ChargingState(CentralArea area) => this.area = area;
        
        public void Enter()
        {
            chargeTimer = UnityEngine.Random.Range(5f, 8f);
        }
        
        public void Update()
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
                area.FireBullets();
        }
        
        public void Exit() { }
    }

    public class FiringState : IState
    {
        private CentralArea area;
        public FiringState(CentralArea area) => this.area = area;
        public void Enter() => area.FireBullets();
        public void Update() { }
        public void Exit() { }
    }
}
