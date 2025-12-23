using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems
{
    public class Boss : Agent
    {
        [SerializeField] private StateListSo bossStateList;

        public Transform Target { get; private set; }
        public PlayerHideStatus TargetHide { get; private set; }

        private AgentStateMachine _fsm;

        protected override void AfterInitializeComponent()
        {
            base.AfterInitializeComponent();
            if (bossStateList != null)
                _fsm = new AgentStateMachine(this, bossStateList.states);
        }

        private void Start()
        {
            //_fsm?.ChangeState((int)BossStates.CHASE);
        }

        private void Update()
        {
            _fsm?.UpdateMachine();
        }

        public void SetTarget(Transform target)
        {
            Target = target;

            var agent = target != null ? target.GetComponentInParent<Agent>() : null;
            TargetHide = agent != null ? agent.GetCompo<PlayerHideStatus>() : target.GetComponent<PlayerHideStatus>();
        }

        /*public void ChangeState(BossStates s)
        {
            _fsm?.ChangeState((int)s);
        }*/
    }
}