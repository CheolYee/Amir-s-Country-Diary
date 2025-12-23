using _00._Work.Resources._02._Codes;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems
{
    public class Boss : Agent
    {
        [SerializeField] private StateListSo bossStateList;

        [Header("Speed Multipliers")]
        [SerializeField] private float patrolMul = 1.0f; // 걷기
        [SerializeField] private float runMul = 2.0f;    // 달리기
        public float PatrolMul => patrolMul;
        public float RunMul => runMul;

        [Header("Patrol Segments (Move / Pause)")]
        [SerializeField] private float patrolMinSec = 5f;
        [SerializeField] private float patrolMaxSec = 10f;
        [SerializeField] private float patrolMoveMinSec = 0.8f;
        [SerializeField] private float patrolMoveMaxSec = 2.0f;
        [SerializeField] private float patrolPauseMinSec = 0.3f;
        [SerializeField] private float patrolPauseMaxSec = 1.0f;
        
        [Header("Wall Check (Patrol Bounce)")]
        [SerializeField] private LayerMask wallMask;
        [SerializeField] private float wallCheckDistance = 0.25f;
        [SerializeField] private Vector2 wallCheckOffset = new(0.2f, 0f);

        public float RollPatrolDuration() => Random.Range(patrolMinSec, patrolMaxSec);
        public float RollPatrolMoveSec() => Random.Range(patrolMoveMinSec, patrolMoveMaxSec);
        public float RollPatrolPauseSec() => Random.Range(patrolPauseMinSec, patrolPauseMaxSec);

        [Header("Attack Box")]
        [SerializeField] private Vector2 attackBoxSize = new Vector2(1.2f, 1.6f);
        [SerializeField] private Vector2 attackBoxOffset = new Vector2(0.8f, 0f);
        [SerializeField] private LayerMask playerMask;

        [Header("Exit Direction")]
        [SerializeField] private bool randomExitSide = true; // true면 좌/우 랜덤으로 퇴장
        public int ExitDir { get; private set; } = 1; // -1 or +1

        public Transform Target { get; private set; }
        public PlayerHideStatus TargetHide { get; private set; }

        public bool IsHiddenTarget => TargetHide != null && TargetHide.IsHidden;

        public BossStates CurrentState { get; private set; }

        private AgentStateMachine _fsm;
        private bool _killedOnce;

        protected override void AfterInitializeComponent()
        {
            base.AfterInitializeComponent();
            if (bossStateList != null)
                _fsm = new AgentStateMachine(this, bossStateList.states);
        }

        private void Start()
        {
            ChangeState(BossStates.RUN); // 소환 시 바로 “찾으려 시도”
            SoundManager.Instance?.PlayBgm(BgmId.Hunt);
        }

        private void Update()
        {
            _fsm?.UpdateMachine();
        }

        public void ChangeState(BossStates s)
        {
            CurrentState = s;
            _fsm?.ChangeState((int)s);
        }

        public void SetTarget(Transform target)
        {
            Target = target;

            var agent = target != null ? target.GetComponentInParent<Agent>() : null;
            TargetHide = agent != null ? agent.GetCompo<PlayerHideStatus>() : target.GetComponent<PlayerHideStatus>();
        }

        public void DecideExitDir()
        {
            ExitDir = randomExitSide ? (Random.value < 0.5f ? -1 : 1) : ExitDir;
            if (ExitDir == 0) ExitDir = 1;
        }
        
        public bool IsWallAhead(int dir)
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * dir, wallCheckOffset.y);
            var hit = Physics2D.Raycast(origin, Vector2.right * dir, wallCheckDistance, wallMask);
            return hit.collider != null;
        }


        public bool TryKillPlayer()
        {
            if (_killedOnce) return false;
            if (Target == null) return false;
            if (IsHiddenTarget) return false;

            int facing = (Target.position.x >= transform.position.x) ? 1 : -1;
            Vector2 center = (Vector2)transform.position + new Vector2(attackBoxOffset.x * facing, attackBoxOffset.y);

            var hit = Physics2D.OverlapBox(center, attackBoxSize, 0f, playerMask);
            if (hit == null) return false;

            _killedOnce = true;
            Bus<JumpScareEvent>.Raise(new JumpScareEvent(GetInstanceID()));
            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // (원래 Gizmos 있으면 거기에 같이 넣어도 됨)
            Gizmos.color = Color.cyan;
            Vector2 oR = (Vector2)transform.position + new Vector2(wallCheckOffset.x * 1, wallCheckOffset.y);
            Vector2 oL = (Vector2)transform.position + new Vector2(wallCheckOffset.x * -1, wallCheckOffset.y);
            Gizmos.DrawLine(oR, oR + Vector2.right * wallCheckDistance);
            Gizmos.DrawLine(oL, oL + Vector2.left * wallCheckDistance);
        }
#endif
    }
}
