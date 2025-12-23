using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems.FSM
{
    public class BossChaseState : BossState
    {
        private const float RunMul = 2.0f;
        private const float WalkMul = 1.0f;
        
        public BossChaseState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            AgentMover?.SetMoveSpeedMultiplier(WalkMul);
        }

        public override void Update()
        {
            if (Boss == null || Boss.Target == null || AgentMover == null) return;

            bool hidden = (Boss.TargetHide != null && Boss.TargetHide.IsHidden);

            AgentMover.SetMoveSpeedMultiplier(hidden ? WalkMul : RunMul);

            float dx = Boss.Target.position.x - Agent.transform.position.x;
            float x = Mathf.Abs(dx) < 0.05f ? 0f : Mathf.Sign(dx);

            AgentMover.SetMovementX(x);
        }

        public override void Exit()
        {
            AgentMover?.SetMoveSpeedMultiplier(1f);
            base.Exit();
        }
    }
}