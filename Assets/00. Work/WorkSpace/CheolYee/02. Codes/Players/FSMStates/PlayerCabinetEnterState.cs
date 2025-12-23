using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CabinetSystems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerCabinetEnterState : PlayerState
    {
        private readonly PlayerCabinetController _cabinet;
        public PlayerCabinetEnterState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            _cabinet = Player.GetCompo<PlayerCabinetController>();
        }
        
        public override void Enter()
        {
            base.Enter();
            _cabinet?.BeginEnter();

            if (Player.AgentRenderer != null)
                Player.AgentRenderer.OnAnimationEnd += HandleAnimEnd;
        }

        private void HandleAnimEnd()
        {
            if (Player.AgentRenderer != null)
                Player.AgentRenderer.OnAnimationEnd -= HandleAnimEnd;

            _cabinet?.FinishEnter();
            Player.ChangeState(PlayerStates.CABINETINSIDE);
        }
    }
}