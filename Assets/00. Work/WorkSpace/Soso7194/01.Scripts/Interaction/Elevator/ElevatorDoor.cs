using UnityEngine;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems; 

namespace PBG_01_PUSE
{
    public class ElevatorDoor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private InputSo inputSo;
        [Tooltip("이 문이 몇 층 문인지 설정 (1, 2, 3)")]
        [Range(1, 3)] [SerializeField] private int floorNumber = 1; 
        
        [SerializeField] private NoiseEmitter noiseEmitter;
        
        private bool isPlayerInRange = false;
        private Transform playerTransform;

        private void OnEnable()
        {
            if (inputSo != null) inputSo.OnInteractionKeyPressed += HandleInteraction;
            if (Elevator.Instance != null)
                Elevator.Instance.OnFloorChanged += OnElevatorMoved;
        }

        private void OnDisable()
        {
            if (inputSo != null) inputSo.OnInteractionKeyPressed -= HandleInteraction;
            if (Elevator.Instance != null)
                Elevator.Instance.OnFloorChanged -= OnElevatorMoved;
        }

        private void OnElevatorMoved()
        {
            if (noiseEmitter != null) noiseEmitter.EmitOnce();
        }

        private void HandleInteraction()
        {
            if (isPlayerInRange && Elevator.Instance != null)
            {
                // 1. 전력 체크
                if (GeneratorManager.Instance != null && !GeneratorManager.Instance.IsFloorPowered(floorNumber))
                {
                    Debug.Log($"[ElevatorDoor] {floorNumber}층 전력 부족.");
                    return; 
                }

                // 2. 엘리베이터 호출 로직
                // "엘리베이터야, 이 층(floorNumber)으로 와라. 그리고 도착하면 {} 안의 내용을 실행해라"
                Elevator.Instance.CallElevator(floorNumber, () => 
                {
                    // 도착 후 실행될 코드 (콜백)
                    // 플레이어가 여전히 문 앞에 기다리고 있다면 UI를 열어줌
                    if (isPlayerInRange)
                    {
                        Elevator.Instance.OpenUI(playerTransform);
                    }
                });
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
                
                // 범위 벗어나면 UI 닫고, 플레이어 탑승 상태 해제
                if (Elevator.Instance != null) 
                {
                    Elevator.Instance.PlayerExited();
                }
            }
        }
    }
}