
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems.FSM
{
    public class BossState : AgentState
    {
        protected Boss Boss;
        protected AgentMover AgentMover;
        
        public BossState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            Boss = agent as Boss;
            AgentMover = agent.AgentMover;
        }
    }
}