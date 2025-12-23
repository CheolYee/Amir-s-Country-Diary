using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Manager;
using Unity.Cinemachine;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Stair
{
    public class StairController : MonoBehaviour
    {
        [Header("Input Settings")]
        public InputSo inputSo; // 인스펙터에서 InputSo를 연결해주세요

        [Header("Stair Settings")]
        [Tooltip("이 계단을 통해 이동할 층 번호 (예: 1, 2)")]
        public int targetFloorIndex; 
        
        [Tooltip("이동 후 플레이어가 서 있을 위치")]
        public Transform targetSpawnPoint;

        private bool _isPlayerNearby = false;
        private GameObject _playerObject;

        // 1. 이벤트 구독 (켜질 때)
        private void OnEnable()
        {
            if (inputSo != null)
            {
                inputSo.OnInteractionKeyPressed += HandleInteraction;
            }
        }

        // 2. 이벤트 구독 해제 (꺼질 때 - 메모리 누수 방지)
        private void OnDisable()
        {
            if (inputSo != null)
            {
                inputSo.OnInteractionKeyPressed -= HandleInteraction;
            }
        }

        // 3. 상호작용 키가 눌렸을 때 실행될 함수
        private void HandleInteraction()
        {
            // 플레이어가 근처에 있고, 이동 중이 아닐 때만 실행
            if (_isPlayerNearby && _playerObject != null)
            {
                ClimbStair();
            }
        }

        private void ClimbStair()
        {
            if (targetSpawnPoint == null) return;

            // 페이드 아웃 -> 이동 -> 페이드 인
            FadeManager.Instance.FadeIn(() =>
            {
                // 플레이어 이동 (물리 버그 방지 + 카메라 컷)
                TeleportPlayer(_playerObject, targetSpawnPoint.position);
                
                // 바운더리 변경 (CameraBoundManager 사용)
                CameraBoundManager.Instance.ChangeCameraBound(targetFloorIndex);
                
                TeleportPlayer( _playerObject, targetSpawnPoint.position);
                
                FadeManager.Instance.FadeOut();
            });
        }

        // 안전한 이동 함수 (카메라 울렁거림 방지 포함)
        private void TeleportPlayer(GameObject player, Vector3 targetPos)
        {
            // 1. 물리 연산 잠시 끄기 (충돌 방지)
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            bool wasSimulated = false;
            if (rb != null)
            {
                wasSimulated = rb.simulated;
                rb.simulated = false;
                rb.linearVelocity = Vector2.zero;
            }

            // 2. 이동할 거리 계산 (중요!)
            Vector3 deltaPos = targetPos - player.transform.position;

            // 3. 플레이어 실제 이동
            player.transform.position = targetPos;

            // 방법 1: 시네머신에게 "타겟이 이만큼 순간이동했다"고 알림 (필수)
            CinemachineCore.OnTargetObjectWarped(player.transform, deltaPos);

            // 방법 2: 현재 활성화된 카메라를 찾아서 "이전 위치 기억 삭제" 시킴 (가장 확실함)
            // (Cinemachine 3.x 버전 기준)
            var vCam = FindFirstObjectByType<CinemachineCamera>(); 
            if (vCam != null)
            {
                // "이전 프레임의 위치 정보를 무효화해라" -> 즉시 새 위치에서 다시 계산 시작
                vCam.PreviousStateIsValid = false; 
            }
            // ---------------------------------------------------------

            // 4. 물리 연산 복구
            if (rb != null) rb.simulated = wasSimulated;
        }

        // --- 플레이어 감지 ---
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