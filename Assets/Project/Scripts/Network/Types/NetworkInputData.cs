using UnityEngine;
using Fusion;

namespace Core.Network
{
    public struct NetworkInputData : INetworkInput
    {
        public const float MAX_INPUT_VALUE = 1.0f;

        // 移动输入
        public float HorizontalInput;
        public float VerticalInput;
        
        // 目标数据
        public NetworkBool HasTarget;
        public Vector3 TargetPosition;
        
        // 动作输入
        public NetworkButtons Actions;
        public NetworkBool IsJumping;
        public NetworkBool IsCrouching;
        public NetworkBool IsSprinting;
        
        // 技能输入
        public byte ActiveSkillIndex;
        public NetworkBool IsSkillTriggered;
        public Vector3 SkillDirection;
        
        // 交互输入
        public NetworkBool IsInteracting;
        public NetworkBool IsReloading;
        
        // 视角数据
        public float CameraRotation;
        public float CameraPitch;

        public Vector3 MovementInput;
        public Vector2 LookInput;
        public NetworkButtons Buttons;

        public void Reset()
        {
            HorizontalInput = 0;
            VerticalInput = 0;
            HasTarget = false;
            TargetPosition = Vector3.zero;
            Actions = default;
            IsJumping = false;
            IsCrouching = false;
            IsSprinting = false;
            ActiveSkillIndex = 0;
            IsSkillTriggered = false;
            SkillDirection = Vector3.zero;
            IsInteracting = false;
            IsReloading = false;
            CameraRotation = 0;
            CameraPitch = 0;
            MovementInput = Vector3.zero;
            LookInput = Vector2.zero;
            Buttons = default;
        }

        public bool IsMoving()
        {
            return Mathf.Abs(HorizontalInput) > 0.01f || Mathf.Abs(VerticalInput) > 0.01f;
        }

        public Vector3 GetMoveVector()
        {
            return new Vector3(HorizontalInput, 0, VerticalInput).normalized;
        }

        public static NetworkInputData Lerp(NetworkInputData from, NetworkInputData to, float t)
        {
            return new NetworkInputData
            {
                HorizontalInput = Mathf.Lerp(from.HorizontalInput, to.HorizontalInput, t),
                VerticalInput = Mathf.Lerp(from.VerticalInput, to.VerticalInput, t),
                HasTarget = t > 0.5f ? to.HasTarget : from.HasTarget,
                TargetPosition = Vector3.Lerp(from.TargetPosition, to.TargetPosition, t),
                Actions = to.Actions,
                IsJumping = t > 0.5f ? to.IsJumping : from.IsJumping,
                IsCrouching = t > 0.5f ? to.IsCrouching : from.IsCrouching,
                IsSprinting = t > 0.5f ? to.IsSprinting : from.IsSprinting,
                ActiveSkillIndex = to.ActiveSkillIndex,
                IsSkillTriggered = t > 0.5f ? to.IsSkillTriggered : from.IsSkillTriggered,
                SkillDirection = Vector3.Lerp(from.SkillDirection, to.SkillDirection, t),
                IsInteracting = t > 0.5f ? to.IsInteracting : from.IsInteracting,
                IsReloading = t > 0.5f ? to.IsReloading : from.IsReloading,
                CameraRotation = Mathf.LerpAngle(from.CameraRotation, to.CameraRotation, t),
                CameraPitch = Mathf.Lerp(from.CameraPitch, to.CameraPitch, t),
                MovementInput = Vector3.Lerp(from.MovementInput, to.MovementInput, t),
                LookInput = Vector2.Lerp(from.LookInput, to.LookInput, t),
                Buttons = to.Buttons
            };
        }
    }
} 