using _00._Work.Resources._02._Codes;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems.FSM
{
    public class BossExitState : BossState
    {
        public BossExitState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam) { }

        public override void Enter()
        {
            base.Enter();
            AgentMover?.SetMoveSpeedMultiplier(Boss.PatrolMul); // 퇴장은 걷기
        }

        public override void Update()
        {
            if (Boss == null || AgentMover == null) return;

            AgentMover.SetMovementX(Boss.ExitDir);
        }

        public override void Exit()
        {
            AgentMover?.SetMoveSpeedMultiplier(1f);
            base.Exit();
        }
    }
}