using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerCabinetInsideState : PlayerState
    {
        public PlayerCabinetInsideState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            Mover.StopImmediately(true, true);
            Mover.CanManualMovement = false;
        }
    }
}