using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players
{
    public class Player : Agent
    {
        [field: SerializeField] public InputSo PlayerInput { get; private set; }

        [SerializeField] private StateListSo playerStateList;
        
        private IAgentMover _mover;

        private AgentStateMachine _stateMachine;
        
        public AgentState CurrentState => _stateMachine.CurrentState;

        protected override void AfterInitializeComponent()
        {
            base.AfterInitializeComponent();
            _mover = GetCompo<IAgentMover>();

            _stateMachine = new AgentStateMachine(this, playerStateList.states);
            
        }

        private void Start()
        {
            _stateMachine.ChangeState((int)PlayerStates.IDLE);
        }

        private void Update()
        {
            _stateMachine.UpdateMachine();
        }

        public void ChangeState(PlayerStates newState)
        {
            _stateMachine.ChangeState((int)newState);
        }
    }
}