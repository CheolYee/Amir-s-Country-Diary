using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using _00._Work.WorkSpace.CheolYee._02._Codes.Players;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CabinetSystems
{
    [DisallowMultipleComponent]
    public class PlayerCabinetController : MonoBehaviour, IAgentComponent, IAfterInitialize
    {
        public CabinetInteractable Candidate { get; private set; }
        public CabinetInteractable Current { get; private set; }

        private Player _player;
        private InputSo _input;
        private AgentMover _mover;
        private PlayerHideStatus _hideStatus;
        private AgentRenderer _renderersToHide;
        public bool IsBusy { get; private set; }
        
        private bool _isInside;
        
        [SerializeField] private float interactDebounce = 0.15f;
        private float _lockUntil;

        private bool _initialized;
        private bool _subscribed;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            if (_player == null) return;

            _mover = _player.GetCompo<AgentMover>();
            _hideStatus = _player.GetCompo<PlayerHideStatus>();
            _input = _player.PlayerInput;

            if (_renderersToHide == null)
                _renderersToHide = _player.GetCompo<AgentRenderer>();

            _initialized = true;
        }

        public void Afterinitialize()
        {
            SubscribeInput();
        }

        private void OnEnable()
        {
            if (_initialized)
                SubscribeInput();
        }

        private void OnDisable()
        {
            UnsubscribeInput();
        }

        private void OnDestroy()
        {
            UnsubscribeInput();
        }

        private void SubscribeInput()
        {
            if (_subscribed) return;
            if (_input == null) return;

            _input.OnInteractionKeyPressed += OnInteractPressed;
            _subscribed = true;
        }

        private void UnsubscribeInput()
        {
            if (!_subscribed) return;
            if (_input != null)
                _input.OnInteractionKeyPressed -= OnInteractPressed;

            _subscribed = false;
        }

        // ========== Candidate (Cabinet -> Player) ==========

        public void SetCandidate(CabinetInteractable cab)
        {
            if (cab == null) return;
            if (_isInside || IsBusy) return;
            Candidate = cab;
        }

        public void ClearCandidate(CabinetInteractable cab)
        {
            if (Candidate == cab) Candidate = null;
        }

        // ========== Input ==========

        private void OnInteractPressed()
        {
            if (Time.time < _lockUntil) return;
            _lockUntil = Time.time + interactDebounce;
            if (_player == null || IsBusy) return;

            if (_isInside)
            {
                RequestExit();
                return;
            }

            // 밖이면 후보로 들어가기
            if (Candidate != null)
            {
                RequestEnter(Candidate);
            }
        }

        private void RequestEnter(CabinetInteractable cab)
        {
            if (cab == null) return;
            if (!cab.TryReserve(this)) return;

            Current = cab;
            Candidate = null;
            IsBusy = true;

            _player.ChangeState(PlayerStates.CABINETENTER);
        }

        private void RequestExit()
        {
            if (Current == null) return;
            IsBusy = true;
            _player.ChangeState(PlayerStates.CABINETEXIT);
        }

        public void BeginEnter()
        {
            if (Current == null || _player == null) return;
            
            _isInside = false;

            SetMovementBlocked(true);
            _mover?.StopImmediately(true, true);

            Current.PlayOpen();
            _player.AgentMover.SetGravityScale(0);
            _player.transform.position = Current.EnterPoint.position;
        }

        public void FinishEnter()
        {
            if (Current == null || _player == null) return;

            _player.transform.position = Current.InsidePoint.position;
            _hideStatus?.SetHidden(true);
            
            _isInside = true;
            IsBusy = false;
        }

        public void BeginExit()
        {
            if (Current == null || _player == null) return;
            
            Candidate = null;

            SetMovementBlocked(true);
            _mover?.StopImmediately(true, true);

            _isInside = false;
            _hideStatus?.SetHidden(false);

            Current.PlayClose();
        }

        public void FinishExit()
        {
            _player.transform.position = Current.ExitPoint.position;
            if (Current != null)
                Current.Release(this);

            Current = null;
            Candidate = null;
            IsBusy = false;

            SetMovementBlocked(false);
            _player.AgentMover.SetGravityScale(1);

            _lockUntil = Time.time + 0.25f;
        }

        private void SetMovementBlocked(bool blocked)
        {
            if (_mover == null) return;

            _mover.CanManualMovement = !blocked;
            _mover.SetMovementX(0f);
        }
    }
}