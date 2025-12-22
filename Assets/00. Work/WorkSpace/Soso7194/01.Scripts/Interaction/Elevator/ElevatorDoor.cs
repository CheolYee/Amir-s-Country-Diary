using UnityEngine;
using _00._Work.Resources._02._Codes.Utils; // InputSo 경로

namespace PBG_01_PUSE
{
    public class ElevatorDoor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private InputSo inputSo;
        [Tooltip("이 문이 몇 층 문인지 설정하세요 (1, 2, 3)")]
        [Range(1, 3)] [SerializeField] private int floorNumber = 1; // 층수 설정 추가
        
        private bool isPlayerInRange = false;
        private Transform playerTransform;

        private void OnEnable()
        {
            if (inputSo != null) inputSo.OnInteractionKeyPressed += HandleInteraction;
        }

        private void OnDisable()
        {
            if (inputSo != null) inputSo.OnInteractionKeyPressed -= HandleInteraction;
        }

        private void HandleInteraction()
        {
            // 플레이어가 범위 안에 있고, 엘리베이터 매니저가 존재할 때
            if (isPlayerInRange && Elevator.Instance != null)
            {
                // [추가된 로직] 현재 층(이 문이 있는 층)에 전력이 들어왔는지 확인
                if (GeneratorManager.Instance != null)
                {
                    if (!GeneratorManager.Instance.IsFloorPowered(floorNumber))
                    {
                        Debug.Log($"[ElevatorDoor] {floorNumber}층 전력이 복구되지 않아 문이 열리지 않습니다.");
                        // 전력이 없으면 여기서 함수 종료 (UI 안 뜸)
                        return; 
                    }
                }

                // 전력이 있을 때만 UI 열기 요청
                Elevator.Instance.OpenUI(playerTransform);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerInRange = true;
                playerTransform = collision.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerInRange = false;
                playerTransform = null;
                
                // 범위 벗어나면 UI 닫기
                if (Elevator.Instance != null) Elevator.Instance.CloseUI();
            }
        }
    }
}