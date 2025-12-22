using System;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Manager;
using PBG_01_LockPick;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door
{
    public class MoveRoom : MonoBehaviour
    {
        [Header("Settings")]
        public InputSo inputSo;
        public enum DoorType { Enter, Exit }
        public DoorType type;

        [Header("If Enter Type")]
        public int roomIndexToGo = 0;

        private bool _isPlayerNearby = false;
        private GameObject _playerObject;

        private bool _isLocked = false; 
        private LockPick _lockPick;

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
            // [추가된 부분] Exit 타입이면 락픽 요청이 와도 강제로 끔
            if (type == DoorType.Exit)
            {
                needLock = false;
            }

            _isLocked = needLock;

            // 자식의 LockPick 컴포넌트 가져오기 (비활성화된 것도 포함)
            LockPick childLockPick = GetComponentInChildren<LockPick>(true);

            if (needLock)
            {
                if (childLockPick != null)
                {
                    _lockPick = childLockPick;
                    _lockPick.OnUnlocked += UnlockDoor; 
                    _lockPick.gameObject.SetActive(false); 
                }
                else
                {
                    // 락픽이 필요한데 컴포넌트가 없으면 잠금 해제 처리
                    _isLocked = false; 
                }
            }
            else
            {
                // 락픽이 필요 없는 경우 (Exit 포함), 자식 락픽 오브젝트가 있다면 확실히 꺼둠
                if (childLockPick != null)
                {
                    childLockPick.gameObject.SetActive(false);
                }
                _lockPick = null;
            }
        }

        private void HandleInteraction()
        {
            if (_isPlayerNearby && _playerObject != null)
            {
                // 1. 잠겨있으면 락픽 실행
                if (_isLocked && _lockPick != null)
                {
                    _lockPick.gameObject.SetActive(true);
                    _lockPick.ShowLockPick();
                    return;
                }

                // 2. 아니면 이동 (Exit이거나 잠금 해제된 Enter)
                if (type == DoorType.Enter)
                {
                    DoorManager.Instance.EnterRoom(_playerObject, roomIndexToGo);
                }
                else
                {
                    DoorManager.Instance.ExitRoom(_playerObject);
                }
            }
        }

        private void UnlockDoor()
        {
            _isLocked = false;
            if (_lockPick != null)
            {
                _lockPick.OnUnlocked -= UnlockDoor;
                _lockPick.gameObject.SetActive(false);
            }
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