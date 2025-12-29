using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
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
        
        public NoiseEmitter noiseEmitter;

        private bool _isPlayerNearby = false;
        private GameObject _playerObject;
        
        private bool _interactionLocked;

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
            if (_interactionLocked) return;

            // 전환 중이면 무시
            if (FadeManager.Instance != null && FadeManager.Instance.IsTransitioning) return;

            if (_isPlayerNearby && _playerObject != null)
                ClimbStair();
        }

        private void ClimbStair()
        {
            if (targetSpawnPoint == null) return;
            

            _interactionLocked = true;

            noiseEmitter.EmitOnce();
            
            // 전환 끝나면 잠금 해제
            if (FadeManager.Instance != null)
                FadeManager.Instance.OnTransitionFinished += UnlockInteraction;

            FadeManager.Instance.FadeIn(() =>
            {
                TeleportPlayer(_playerObject, targetSpawnPoint.position);
                CameraBoundManager.Instance.ChangeCameraBound(targetFloorIndex);

                // (여기 TeleportPlayer 두 번 호출은 보통 불필요해 보여서 하나만 권장)
                // TeleportPlayer(_playerObject, targetSpawnPoint.position);

                FadeManager.Instance.FadeOut();
            });
        }
        
        private void UnlockInteraction()
        {
            _interactionLocked = false;

            if (FadeManager.Instance != null)
                FadeManager.Instance.OnTransitionFinished -= UnlockInteraction;
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