using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.ShutterCameraAis
{
    public class ShutterCameraAI : MonoBehaviour
    {
        public int CameraId => _cameraId;

        [SerializeField] private float huntSpeed = 5.5f;
        
        [Header("Hunt GiveUp When Hidden")]
        [SerializeField] private float hiddenGiveupSec = 3f; // 캐비닛 숨김 4초면 포기
        [SerializeField] private bool moveToLastKnownWhileHidden = true;

        [Header("Modules")]
        [SerializeField] private ShutterCameraMover mover;
        [SerializeField] private ShutterCameraHearing hearing;
        [SerializeField] private ShutterCameraFocus focus;

        private int _cameraId;

        private Transform _huntPlayer;
        private PlayerHideStatus _huntHide;
        private Vector2 _huntLastKnownPos;
        private float _huntHiddenSince = -1f;

        private void Awake()
        {
            _cameraId = GetInstanceID();
            if (mover == null) mover = GetComponent<ShutterCameraMover>();
        }

        private void OnEnable()
        {
            CameraRegistry.Register(this);

            if (focus != null)
            {
                focus.OnPhoto += HandlePhoto;
                focus.OnGiveUp += HandleGiveUp;
            }
        }

        private void OnDisable()
        {
            if (focus != null)
            {
                focus.OnPhoto -= HandlePhoto;
                focus.OnGiveUp -= HandleGiveUp;
            }

            CameraRegistry.Unregister(this);
        }

        public void ForceHunt(Transform player)
        {
            _huntPlayer = player;
            _huntLastKnownPos = player.position;
            _huntHiddenSince = -1f;

            var agent = player.GetComponentInParent<Agent>();
            _huntHide = agent != null ? agent.GetCompo<PlayerHideStatus>() : player.GetComponent<PlayerHideStatus>();

            focus?.SetExternalPlayer(player);
            hearing?.SetEnabled(false, clearTargets: true);
        }

        private void Update()
        {
            hearing?.Tick();

            // ✅ 헌팅 중 숨김 처리(락온 버그 핵심)
            if (_huntPlayer != null)
            {
                bool hidden = (_huntHide != null && _huntHide.IsHidden);

                if (!hidden)
                {
                    _huntHiddenSince = -1f;
                    _huntLastKnownPos = _huntPlayer.position; // 마지막으로 본 위치 갱신
                }
                else
                {
                    if (_huntHiddenSince < 0f)
                        _huntHiddenSince = Time.time;

                    // 숨김 지속이 길면 포기
                    if (Time.time - _huntHiddenSince >= hiddenGiveupSec)
                    {
                        StopHunt();
                    }
                    else
                    {
                        // 숨겼으면 “플레이어 트랜스폼”이 아니라 마지막 위치까지만 접근
                        if (moveToLastKnownWhileHidden)
                        {
                            mover?.MoveTowards(_huntLastKnownPos, huntSpeed);
                            return;
                        }
                    }
                }
            }

            // 포커스/포기 연출 중엔 이동 잠금
            if (focus != null && focus.IsMoveBlocked)
                return;

            // 포커스 추적(숨김이면 ShouldChasePlayer가 false가 됨)
            if (focus != null && focus.InRange && focus.Player != null && focus.ShouldChasePlayer)
            {
                if (focus.IsCenterReached)
                    return;

                mover?.MoveTowards(focus.Player.position, huntSpeed);
                return;
            }

            if (TryGetDesiredMove(out var pos, out var spd))
                mover?.MoveTowards(pos, spd);
            else
                mover?.TickPatrol();
        }

        private void HandlePhoto(Vector2 playerPos)
        {
            Bus<PhotoTakenEvent>.Raise(new PhotoTakenEvent(_cameraId, playerPos));
            StopHunt();
            hearing?.ClearTargets(); // 원하면 유지해도 됨
        }

        private void HandleGiveUp()
        {
            StopHunt();
        }

        private void StopHunt()
        {
            _huntPlayer = null;
            _huntHide = null;
            _huntHiddenSince = -1f;
            
            hearing?.SetEnabled(true);
        }
        
        private bool TryGetDesiredMove(out Vector2 targetPos, out float speed)
        {
            // 1) 헌팅 최우선
            if (_huntPlayer != null)
            {
                targetPos = _huntPlayer.position;
                speed = huntSpeed;
                return true;
            }

            // 2) 소리 조사
            if (hearing != null && hearing.TryGetMove(out targetPos, out speed))
                return true;

            // 3) 이동 없음(순찰로 처리)
            targetPos = default;
            speed = 0f;
            return false;
        }
    }
}
