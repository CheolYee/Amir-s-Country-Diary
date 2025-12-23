using System;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.ShutterCameraAis
{
    [RequireComponent(typeof(Collider2D))]
    public class ShutterCameraFocus : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private CameraFocusGauge gauge;
        
        [Header("Stop At Center")]
        [SerializeField] private float centerStopDistance = 0.1f;

        [Header("Rules")]
        [SerializeField] private float focusTimeSec = 2f;
        [SerializeField] private float focusDecayPerSec = 0.7f;
        [SerializeField] private float hiddenGiveupSec = 4f;
        [SerializeField] private float searchPauseSec = 1.2f;
        [SerializeField] private float cooldownSec = 1.0f;

        public bool InRange => _inRange;
        public Transform Player => _player;

        /// <summary>포커스 중/포기 연출 중에는 이동 멈추게 잠금</summary>
        public bool IsMoveBlocked => _mode == Mode.SearchPause && Time.time < _until;
        
        private bool IsPlayerHidden => _hide != null && _hide.IsHidden;

        public bool ShouldChasePlayer => _mode == Mode.Focusing && !_giveUpLock && !IsPlayerHidden;
        
        public bool IsCenterReached
        {
            get
            {
                if (_player == null) return false;
                return Vector2.Distance(transform.position, _player.position) <= centerStopDistance;
            }
        }

        public event Action<Vector2> OnPhoto; // 플레이어 위치 전달
        public event Action OnGiveUp;

        private enum Mode { Idle, Focusing, SearchPause, Cooldown }
        private Mode _mode = Mode.Idle;

        private bool _inRange;
        private Transform _player;
        private PlayerHideStatus _hide;

        private float _focusTimer;
        private float _hiddenTimer;
        private float _until; // pause/cooldown 종료 시간
        
        private bool _giveUpLock;

        private void Awake()
        {
            // 트리거 확인(실수 방지)
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;

            gauge?.SetVisible(false);
            gauge?.Set01(0f);
        }
        
        public void ForceReset()
        {
            ResetAll();
        }

        public void SetExternalPlayer(Transform player)
        {
            _player = player;
            var agent = player.GetComponentInParent<Agent>();
            _hide = agent != null ? agent.GetCompo<PlayerHideStatus>() : player.GetComponent<PlayerHideStatus>();
        }

        private void Update()
        {
            if (_player == null || !_inRange)
            {
                ResetAll();
                return;
            }

            if (_mode == Mode.SearchPause || _mode == Mode.Cooldown)
            {
                if (Time.time >= _until)
                    _mode = Mode.Idle;
            }

            if (_giveUpLock)
            {
                gauge?.SetVisible(false);

                if (_hide == null || !_hide.IsHidden)
                    _giveUpLock = false;

                return;
            }

            if (_mode == Mode.Idle)
                BeginFocusInternal();

            TickFocusInternal();
        }


        private void BeginFocusInternal()
        {
            _mode = Mode.Focusing;
            _focusTimer = 0f;
            _hiddenTimer = 0f;

            gauge?.SetVisible(true);
            gauge?.Set01(0f);
        }

        private void TickFocusInternal()
        {
            bool hidden = (_hide != null && _hide.IsHidden);

            if (hidden)
            {
                _hiddenTimer += Time.deltaTime;

                // 숨으면 게이지 감소(최소 0)
                _focusTimer = Mathf.Max(0f, _focusTimer - focusDecayPerSec * Time.deltaTime);
                gauge?.Set01(_focusTimer / focusTimeSec);

                if (_hiddenTimer >= hiddenGiveupSec)
                {
                    GiveUp();
                }
                return;
            }

            // 노출 상태: 게이지 증가
            _hiddenTimer = 0f;
            _focusTimer += Time.deltaTime;
            gauge?.Set01(_focusTimer / focusTimeSec);

            if (_focusTimer >= focusTimeSec)
            {
                // 사진 → 쿨다운
                Vector2 pos = _player.position;
                ResetFocusUI();
                _mode = Mode.Cooldown;
                _until = Time.time + cooldownSec;
                OnPhoto?.Invoke(pos);
            }
        }
        
        private void GiveUp()
        {
            ResetFocusUI();
            _mode = Mode.SearchPause;
            _until = Time.time + searchPauseSec;
            _giveUpLock = true;
            OnGiveUp?.Invoke();
        }

        private void ResetFocusUI()
        {
            _mode = Mode.Idle;
            _focusTimer = 0f;
            _hiddenTimer = 0f;

            if (gauge != null)
            {
                gauge.Set01(0f);
                gauge.SetVisible(false);
            }
        }
        private void ResetAll()
        {
            _inRange = false;
            _player = null;
            _hide = null;
            _giveUpLock = false;
            ResetFocusUI();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            _inRange = true;
            SetExternalPlayer(other.transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            _inRange = false;
        }
    }
}