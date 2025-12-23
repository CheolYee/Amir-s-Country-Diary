using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems.FSM
{
    public class BossRunState : BossState
    {
        public BossRunState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam) { }

        public override void Enter()
        {
            base.Enter();
            AgentMover?.SetMoveSpeedMultiplier(Boss.RunMul);
        }

        public override void Update()
        {
            if (Boss == null || AgentMover == null) return;

            if (Boss.Target == null)
            {
                Boss.ChangeState(BossStates.PATROL);
                return;
            }

            if (Boss.IsHiddenTarget)
            {
                Boss.ChangeState(BossStates.PATROL);
                return;
            }

            float dx = Boss.Target.position.x - Agent.transform.position.x;
            float x = Mathf.Abs(dx) < 0.05f ? 0f : Mathf.Sign(dx);
            AgentMover.SetMovementX(x);

            Boss.TryKillPlayer();
        }

        public override void Exit()
        {
            AgentMover?.SetMoveSpeedMultiplier(1f);
            base.Exit();
        }
    }
}