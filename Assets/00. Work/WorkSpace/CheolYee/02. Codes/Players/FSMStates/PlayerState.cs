using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public abstract class PlayerState : AgentState
    {
        protected readonly Player Player;
        protected readonly AgentMover Mover;
        
        protected PlayerState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            Player = agent as Player;
            Mover = agent.GetCompo<AgentMover>();
        }
    }
}