using System;
using System.Collections;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
using _00._Work.WorkSpace.CheolYee._02._Codes.Players;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Manager;
using PBG_01_LockPick;
using Unity.Cinemachine; 
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door
{
    public class MoveRoom : MonoBehaviour
    {
        [Header("Settings")]
        public InputSo inputSo;
        public enum DoorType { Enter, Exit, Move}
        public DoorType type;
        public NoiseEmitter noiseEmitter;

        [Header("If Enter Type")]
        public int roomIndexToGo = 0;
        
        [Header("If Move Type")]
        [Tooltip("only 1st floor, 2st floor, 3st floor")]
        public int floorIndexToGo = 0;

        private bool _isPlayerNearby = false;
        private GameObject _playerObject;

        private bool _isLocked = false; 
        private LockPick _lockPick;
        [SerializeField] private GameObject _parent;
        
        private bool _isUsingLockPick = false; 

        // 물리 제어용 변수
        private AgentMover _mover;

        private void OnEnable()
        {
            if (inputSo != null)
                inputSo.OnInteractionKeyPressed += HandleInteraction;
        }

        private void OnDisable()
        {
            if (inputSo != null)
                inputSo.OnInteractionKeyPressed -= HandleInteraction;
            
            if (_lockPick != null)
                _lockPick.OnUnlocked -= UnlockDoor;
        }

        public void InitializeLock(bool needLock)
        {
            if (type == DoorType.Exit) needLock = false;

            _isLocked = needLock;
            // 고장 상태 초기화 로직 제거 (재시도 가능하므로 필요 없음)

            LockPick childLockPick = GetComponentInChildren<LockPick>(true);

            if (needLock)
            {
                if (childLockPick != null)
                {
                    _lockPick = childLockPick;
                    _lockPick.OnUnlocked += UnlockDoor; 
                    _parent.SetActive(false);
                }
                else
                {
                    _isLocked = false; 
                }
            }
            else
            {
                if (childLockPick != null)
                {
                    childLockPick.gameObject.SetActive(false);
                    _parent.SetActive(false);
                }
                _lockPick = null;
            }
        }

        private void HandleInteraction()
        {
            // 상호작용 중이거나 플레이어가 없으면 무시
            if (_isUsingLockPick || !_isPlayerNearby || _playerObject == null) return;

            // 잠겨있고 락픽 스크립트가 있다면 락픽 실행
            if (_isLocked && _lockPick != null)
            {
                // [수정] 고장 여부(_isBroken)를 체크하지 않고 바로 실행합니다.
                StartLockPicking();
                _lockPick.ShowLockPick();
                StartCoroutine(CheckLockPickState());
                return;
            }

            noiseEmitter.EmitOnce();
            switch (type)
            {
                case DoorType.Enter:
                    DoorManager.Instance.EnterRoom(_playerObject, roomIndexToGo);
                    break;

                case DoorType.Move:
                    CameraBoundManager.Instance.ChangeCameraBound(floorIndexToGo);
                    break;

                case DoorType.Exit:
                    DoorManager.Instance.ExitRoom(_playerObject);
                    break;
            }
        }

        private void StartLockPicking()
        {
            _isUsingLockPick = true;
            _mover = _playerObject.GetComponent<Player>().GetCompo<AgentMover>();

            if (_mover != null)
            {
                _mover.StopImmediately(true, true);
                _mover.CanManualMovement = false;
                
            }
        }

        private IEnumerator CheckLockPickState()
        {
            // activeSelf는 부모가 꺼져도 true일 수 있습니다.
            // activeInHierarchy는 부모가 꺼지면 false가 되므로 화면에 보이는지 정확히 알 수 있습니다.
            while (_lockPick != null && _lockPick.gameObject.activeInHierarchy)
            {
                yield return null;
            }

            // UI가 사라지면 여기로 넘어옴
            if (_isLocked)
            {
                Debug.Log("락픽 실패. 다시 시도하세요.");
            }

            // 플레이어 움직임 복구
            EndLockPicking();
        }

        private void EndLockPicking()
        {
            _isUsingLockPick = false; // 다시 상호작용 가능하도록 플래그 해제

            if (_mover != null)
            {
                _mover.StopImmediately(false, false);
                _mover.CanManualMovement = true;
                
            }
        }

        private void UnlockDoor()
        {
            _isLocked = false;
            
            if (_lockPick != null)
            {
                _lockPick.OnUnlocked -= UnlockDoor;
            }
            // UnlockDoor가 호출되어도 코루틴(CheckLockPickState)이 UI 꺼짐을 감지하여 EndLockPicking을 수행합니다.
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = true;
                _playerObject = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = false;
                _playerObject = null;
            }
        }
    }
}