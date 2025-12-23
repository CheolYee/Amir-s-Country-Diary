using System;
using _00._Work.Resources._02._Codes.Utils;
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

        [Header("If Enter Type")]
        public int roomIndexToGo = 0;
        
        [Header("If Move Type")]
        [Tooltip("only 1st floor, 2st floor, 3st floor")]
        public int floorIndexToGo = 0;

        private bool _isPlayerNearby = false;
        private GameObject _playerObject;

        private bool _isLocked = false; 
        private LockPick _lockPick;
        
        // [추가] 락픽 사용 중인지 확인하는 플래그
        private bool _isUsingLockPick = false; 
        
        // [추가] 플레이어 물리 제어를 위한 변수
        private Rigidbody2D _playerRb;
        private RigidbodyType2D _originalBodyType;

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
                    _isLocked = false; 
                }
            }
            else
            {
                if (childLockPick != null) childLockPick.gameObject.SetActive(false);
                _lockPick = null;
            }
        }

        private void HandleInteraction()
        {
            if (_isUsingLockPick || !_isPlayerNearby || _playerObject == null) return;

            if (_isLocked && _lockPick != null)
            {
                _lockPick.ShowLockPick();
                return;
            }

            // [수정된 로직]
            switch (type)
            {
                case DoorType.Enter:
                    // 방으로 들어갈 때는 DoorManager가 카메라 변경까지 담당함
                    // CameraBoundManager.Instance.ChangeCameraBound 호출 삭제!
                    DoorManager.Instance.EnterRoom(_playerObject, roomIndexToGo);
                    break;

                case DoorType.Move:
                    // 단순히 층만 이동하는 문 (1층 복도 -> 2층 복도)
                    CameraBoundManager.Instance.ChangeCameraBound(floorIndexToGo);
                    // 플레이어 위치 이동 로직이 필요하다면 여기에 추가 (예: 텔레포트)
                    break;

                case DoorType.Exit:
                    DoorManager.Instance.ExitRoom(_playerObject);
                    break;
            }
        }

        // 락픽 성공(해제) 시 호출
        private void UnlockDoor()
        {
            // 잠금 해제 처리
            _isLocked = false;
            
            // 락픽 UI 끄기
            if (_lockPick != null)
            {
                _lockPick.OnUnlocked -= UnlockDoor;
                _lockPick.gameObject.SetActive(false);
            }

            // [수정] 플레이어 움직임 복구
            EndLockPicking();
        }

        // 락픽 종료(성공 혹은 취소 등) 시 플레이어 풀어주기
        private void EndLockPicking()
        {
            _isUsingLockPick = false;

            if (_playerRb != null)
            {
                _playerRb.bodyType = _originalBodyType; // 원래 물리 상태(Dynamic 등)로 복구
                _playerRb = null;
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