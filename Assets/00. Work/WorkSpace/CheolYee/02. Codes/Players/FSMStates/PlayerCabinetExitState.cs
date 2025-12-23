using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CabinetSystems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerCabinetExitState : PlayerState
    {
        private readonly PlayerCabinetController _cabinet;
        public PlayerCabinetExitState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            _cabinet = Player.GetCompo<PlayerCabinetController>();
        }
        
        public override void Enter()
        {
            base.Enter();
            _cabinet?.BeginExit();

            if (Player.AgentRenderer != null)
                Player.AgentRenderer.OnAnimationEnd += HandleAnimEnd;
        }
        private void HandleAnimEnd()
        {
            if (Player.AgentRenderer != null)
                Player.AgentRenderer.OnAnimationEnd -= HandleAnimEnd;

            _cabinet?.FinishExit();
            Player.ChangeState(PlayerStates.IDLE);
        }
    }
}