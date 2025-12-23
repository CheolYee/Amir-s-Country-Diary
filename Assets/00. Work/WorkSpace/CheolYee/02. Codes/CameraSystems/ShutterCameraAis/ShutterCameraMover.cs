using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.ShutterCameraAis
{
    public class ShutterCameraMover : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private bool useRigidbody;
        [SerializeField] private Rigidbody2D rb;

        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float patrolArriveDist = 0.15f;
        [SerializeField] private float patrolWaitSec = 0.6f;

        private int _patrolIndex;
        private float _patrolWaitUntil;
        
        private bool _hasMoveCmd;
        private Vector2 _moveTarget;
        private float _moveSpeed;

        public bool HasPatrolRoute => patrolPoints != null && patrolPoints.Length > 0;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
        }

        public void MoveTowards(Vector2 target, float speed)
        {
            if (useRigidbody && rb != null)
            {
                _hasMoveCmd = true;
                _moveTarget = target;
                _moveSpeed = speed;
                return;
            }

            // Transform 이동은 즉시 처리
            Vector2 cur = transform.position;
            Vector2 next = Vector2.MoveTowards(cur, target, speed * Time.deltaTime);
            transform.position = next;
        }
        
        private void FixedUpdate()
        {
            if (!useRigidbody || rb == null) return;
            if (!_hasMoveCmd) return;

            Vector2 cur = rb.position;
            Vector2 next = Vector2.MoveTowards(cur, _moveTarget, _moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(next);

            _hasMoveCmd = false; // 매 프레임 명령 갱신 방식
        }

        /// <summary>
        /// 순찰 루트를 따라 이동. 루트가 없으면 false.
        /// </summary>
        public bool TickPatrol()
        {
            if (!HasPatrolRoute) return false;
            if (Time.time < _patrolWaitUntil) return true;

            Vector2 target = patrolPoints[_patrolIndex].position;
            MoveTowards(target, patrolSpeed);

            if (Vector2.Distance(transform.position, target) <= patrolArriveDist)
            {
                _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
                _patrolWaitUntil = Time.time + patrolWaitSec;
            }

            return true;
        }
    }
}
