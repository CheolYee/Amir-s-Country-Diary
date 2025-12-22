using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem
{
    public abstract class AgentState
    {
        protected Agent Agent;
        protected AnimParamSo StateParam;
        protected bool IsTriggerCall;

        protected AgentRenderer Renderer;

        public AgentState(Agent agent, AnimParamSo stateParam)
        {
            Agent = agent;
            StateParam = stateParam;
            Renderer = agent.GetCompo<AgentRenderer>();
        }

        public virtual void Enter()
        {
            Renderer.SetParam(StateParam, true);
            IsTriggerCall = false;
        }
        
        public virtual void Update() { }

        public virtual void Exit()
        {
            if (Agent == null || Agent.GetCompo<AgentRenderer>() == null) return;
            Renderer.SetParam(StateParam, false);
        }
        
        public virtual void AnimationEndTrigger() => IsTriggerCall = true;
    }
}