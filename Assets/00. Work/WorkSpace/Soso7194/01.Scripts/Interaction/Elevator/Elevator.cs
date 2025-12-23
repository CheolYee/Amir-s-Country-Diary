using System;
using UnityEngine;
using _00._Work.Resources._02._Codes.Utils; 
using _00._Work.Resources._04._Templates.FadeManager;

namespace PBG_01_PUSE
{
    public class Elevator : MonoSingleton<Elevator>
    {
        [Header("UI Settings")]
        [SerializeField] private GameObject elevatorUIPanel;

        [Header("Elevator Settings")]
        [SerializeField] private Transform cabinVisual;
        [SerializeField] private int currentFloor = 1; 

        [Header("Floor Positions")]
        [SerializeField] private Transform pos1F;
        [SerializeField] private Transform pos2F;
        [SerializeField] private Transform pos3F;

        private Transform currentPlayer; // 현재 탑승한 플레이어
        
        public event Action OnFloorChanged;

        public int GetCurrentFloor() => currentFloor;

        public void OpenUI(Transform playerTransform)
        {
            currentPlayer = playerTransform; // UI를 열 때 플레이어를 등록
            if (elevatorUIPanel != null) elevatorUIPanel.SetActive(true);
        }

        public void CloseUI()
        {
            if (elevatorUIPanel != null) elevatorUIPanel.SetActive(false);
        }

        // 플레이어가 엘리베이터 구역을 벗어날 때 호출 (참조 해제)
        public void PlayerExited()
        {
            CloseUI();
            currentPlayer = null; 
        }

        // --- [UI 버튼 연결용 함수] ---
        public void OnClickFloor1() => RequestMove(1);
        public void OnClickFloor2() => RequestMove(2);
        public void OnClickFloor3() => RequestMove(3);

        private void RequestMove(int targetFloor)
        {
            if (currentFloor == targetFloor)
            {
                Debug.Log($"이미 {targetFloor}층입니다.");
                return;
            }
            MoveToFloor(targetFloor);
        }

        // --- [핵심 이동 로직] ---
        // onComplete: 이동이 다 끝나고(페이드 아웃 후) 실행할 행동
        private void MoveToFloor(int targetFloor, Action onComplete = null)
        {
            // 전력 체크
            if (GeneratorManager.Instance != null && !GeneratorManager.Instance.IsFloorPowered(targetFloor))
            {
                Debug.Log($"{targetFloor}층 전력 부족!");
                return;
            }

            Transform targetPos = GetFloorPosition(targetFloor);
            if (targetPos == null) return;

            Debug.Log($"{currentFloor}층 -> {targetFloor}층 이동 시작");
            OnFloorChanged?.Invoke();
            currentFloor = targetFloor; 
            
            CloseUI(); // 이동 중엔 UI 닫기

            if (FadeManager.Instance != null)
            {
                FadeManager.Instance.FadeIn(() => 
                {
                    PerformTeleport(targetPos);
                    FadeManager.Instance.FadeOut(); // 페이드 아웃이 끝나면...
                    onComplete?.Invoke();           // ...후속 작업(UI 열기 등) 실행
                });
            }
            else
            {
                PerformTeleport(targetPos);
                onComplete?.Invoke();
            }
        }

        private Transform GetFloorPosition(int floor)
        {
            switch (floor)
            {
                case 1: return pos1F;
                case 2: return pos2F;
                case 3: return pos3F;
                default: return null;
            }
        }

        private void PerformTeleport(Transform targetPos)
        {
            if (cabinVisual != null) cabinVisual.position = targetPos.position;
            
            // UI를 열고 들어온 플레이어(currentPlayer)가 있다면 같이 이동시킴
            if (currentPlayer != null) 
            {
                currentPlayer.position = targetPos.position;
            }
        }
        
        // --- [외부 호출용] 문 밖에서 엘리베이터 부를 때 ---
        public void CallElevator(int floor, Action onArrived)
        {
            // 밖에서 부를 때는 플레이어가 타지 않은 상태여야 함
            currentPlayer = null; 
            
            // 이미 그 층에 있다면 즉시 도착 처리
            if (currentFloor == floor)
            {
                onArrived?.Invoke();
                return;
            }

            MoveToFloor(floor, onArrived);
        }
    }
}