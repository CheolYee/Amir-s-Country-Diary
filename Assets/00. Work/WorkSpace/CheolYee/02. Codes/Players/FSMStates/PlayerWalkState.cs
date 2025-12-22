using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerWalkState : PlayerState
    {
        private PlayerStamina _stamina;
        
        public PlayerWalkState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            _stamina = Player.GetCompo<PlayerStamina>();
        }
        
        public override void Update()
        {
            base.Update();
            
            float xInput = Player.PlayerInput.MoveInput.x;
            bool hasMove = !Mathf.Approximately(xInput, 0f);
            
            Mover.SetMovementX(xInput);
            if (!hasMove)
            {
                Player.ChangeState(PlayerStates.IDLE);
                return;
            }

            if (Player.PlayerInput.RunKeyPressed && (_stamina == null || _stamina.CanStartRun))
            {
                Player.ChangeState(PlayerStates.RUN);
            }
        }
    }
}