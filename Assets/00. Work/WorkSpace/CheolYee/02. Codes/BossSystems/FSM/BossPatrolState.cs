using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems.FSM
{
    public class BossPatrolState : BossState
    {
        private float _patrolEndTime;
        private float _nextFlipTime;

        private bool _paused;        // true면 정지구간
        private float _segmentEnd;   // 현재 구간(이동/정지) 종료 시각
        private int _dir;            // 이동 방향

        public BossPatrolState(Agent agent, AnimParamSo stateParam) : base(agent, stateParam) { }

        public override void Enter()
        {
            base.Enter();
            if (Boss == null || AgentMover == null) return;
            
            _nextFlipTime = 0f;

            AgentMover.SetMoveSpeedMultiplier(Boss.PatrolMul);

            _patrolEndTime = Time.time + Boss.RollPatrolDuration();
            _dir = Random.value < 0.5f ? -1 : 1;

            StartMoveSegment(); // 시작은 걷기
        }

        public override void Update()
        {
            if (Boss == null || AgentMover == null) return;
            
            if (!_paused && Time.time >= _nextFlipTime && Boss.IsWallAhead(_dir))
            {
                _dir *= -1;
                _nextFlipTime = Time.time + 0.2f; // 벽에 붙어서 연속 반전되는 것 방지
            }

            // 숨지 않았으면 다시 RUN
            if (Boss.Target != null && !Boss.IsHiddenTarget)
            {
                Boss.ChangeState(BossStates.RUN);
                return;
            }

            // 전체 패트롤 시간 끝나면 EXIT
            if (Time.time >= _patrolEndTime)
            {
                Boss.DecideExitDir();
                Boss.ChangeState(BossStates.EXIT);
                return;
            }

            // 구간 토글
            if (Time.time >= _segmentEnd)
            {
                if (_paused) StartMoveSegment();
                else StartPauseSegment();
            }

            // 실행
            AgentMover.SetMovementX(_paused ? 0f : _dir);
        }

        private void StartMoveSegment()
        {
            _paused = false;

            // 이동 구간마다 방향을 랜덤하게 바꾸고 싶으면 아래 한 줄 유지
            _dir = Random.value < 0.5f ? -1 : 1;

            _segmentEnd = Time.time + Boss.RollPatrolMoveSec();
        }

        private void StartPauseSegment()
        {
            _paused = true;
            _segmentEnd = Time.time + Boss.RollPatrolPauseSec();
        }

        public override void Exit()
        {
            AgentMover?.SetMoveSpeedMultiplier(1f);
            base.Exit();
        }
    }
}
