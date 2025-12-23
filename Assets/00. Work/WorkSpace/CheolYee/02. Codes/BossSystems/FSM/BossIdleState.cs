using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems.FSM
{
    public class BossIdleState : BossState
    {
        public BossIdleState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
        }
        
        public override void Update()
        {
            base.Update();
            AgentMover.SetMovementX(0f);
        }
    }
}