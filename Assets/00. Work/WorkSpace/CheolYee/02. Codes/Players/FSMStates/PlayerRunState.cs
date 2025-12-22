using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerRunState : PlayerState
    {
        private const float RunMultiplier = 2f;
        private PlayerStamina _stamina; 
        
        public PlayerRunState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
            _stamina = Player.GetCompo<PlayerStamina>();
        }
        
        public override void Enter()
        {
            base.Enter();
            Bus<StaminaRunDrainToggleEvent>.Raise(new StaminaRunDrainToggleEvent(true));
            Mover.SetMoveSpeedMultiplier(RunMultiplier);
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

            if (_stamina != null && !_stamina.CanRun || Player.PlayerInput.RunKeyPressed == false)
            {
                Player.ChangeState(PlayerStates.WALK);
            }
        }

        public override void Exit()
        {
            Bus<StaminaRunDrainToggleEvent>.Raise(new StaminaRunDrainToggleEvent(false));
            Mover.SetMoveSpeedMultiplier(1f);
            base.Exit();
        }
    }
}