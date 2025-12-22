using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam)
        {
        }
        
        public override void Update()
        {
            base.Update();
            float xInput = Player.PlayerInput.MoveInput.x;
            bool hasMove = !Mathf.Approximately(xInput, 0f);
            
            if (!hasMove) return;

            if (Player.PlayerInput.RunKeyPressed)
                Player.ChangeState(PlayerStates.RUN);
            else
                Player.ChangeState(PlayerStates.WALK);
        }
    }
}