using UnityEngine;
using Core.EventSystem;
using MapModule.Shapes;

namespace MapModule.Systems
{
    /// <summary>
    /// 形状系统
    /// 负责：
    /// 1. 管理形状的生命周期
    /// 2. 处理形状的切换
    /// 3. 维护形状状态
    /// </summary>
    public class ShapeSystem
    {
        private readonly MapConfig config;
        private readonly ShapeFactory shapeFactory;
        private readonly CentralAreaSystem centralAreaSystem;
        private BaseShape currentShape;
        private float shapeTimer;

        public ShapeSystem(MapConfig config, ShapeFactory factory, CentralAreaSystem centralArea)
        {
            this.config = config;
            this.shapeFactory = factory;
            this.centralAreaSystem = centralArea;
            shapeTimer = 0;
        }

        public void Update()
        {
            if (currentShape == null) return;

            shapeTimer += Time.deltaTime;
            if (shapeTimer >= config.ShapeChangeInterval)
            {
                TriggerShapeChange();
            }
        }

        public void ChangeShape(ShapeType type, Vector3 position)
        {
            // 回收当前形状
            if (currentShape != null)
            {
                shapeFactory.RecycleShape(currentShape);
            }

            // 创建新形状
            currentShape = shapeFactory.CreateShape(type);
            if (currentShape != null)
            {
                // 使用中心区域系统获取随机位置
                Vector3 randomPosition = centralAreaSystem.GetRandomPosition();
                currentShape.SetPosition(randomPosition);
            }
            shapeTimer = 0;

            // 触发事件
            EventManager.Instance.TriggerEvent(new ShapeActionEvent
            {
                Type = type,
                ActionType = ShapeActionType.None,
                Position = currentShape?.transform.position ?? Vector3.zero,
                ActionData = currentShape?.GetConfig()
            });
        }

        public void HandleSkillHit(int skillId, Vector3 hitPoint)
        {
            currentShape?.HandleSkillHit(skillId, hitPoint);
        }

        public ShapeState GetCurrentShapeState()
        {
            return currentShape?.GetState();
        }

        private void TriggerShapeChange()
        {
            ShapeType newType = GetRandomEnabledShapeType();
            Vector3 newPosition = Vector3.zero; // 由CentralAreaSystem决定具体位置
            ChangeShape(newType, newPosition);
        }

        private ShapeType GetRandomEnabledShapeType()
        {
            float totalChance = 0;
            foreach (var shapeConfig in config.ShapeConfigs)
            {
                if (shapeConfig.Enabled)
                {
                    totalChance += shapeConfig.SpawnChance;
                }
            }

            float random = Random.Range(0, totalChance);
            float currentChance = 0;

            foreach (var shapeConfig in config.ShapeConfigs)
            {
                if (shapeConfig.Enabled)
                {
                    currentChance += shapeConfig.SpawnChance;
                    if (random <= currentChance)
                    {
                        return shapeConfig.Type;
                    }
                }
            }

            return ShapeType.Circle; // 默认返回圆形
        }

        public void Dispose()
        {
            if (currentShape != null)
            {
                shapeFactory.RecycleShape(currentShape);
                currentShape = null;
            }
        }
    }
} 