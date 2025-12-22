using UnityEngine;
using _00._Work.Resources._02._Codes.Utils; // MonoSingleton 경로
using _00._Work.Resources._04._Templates.FadeManager; // FadeManager 경로

namespace PBG_01_PUSE
{
    public class Elevator : MonoSingleton<Elevator>
    {
        [Header("UI Settings")]
        [SerializeField] private GameObject elevatorUIPanel;

        [Header("Elevator Settings")]
        [SerializeField] private Transform cabinVisual;
        [SerializeField] private int currentFloor = 1; // 현재 층을 기억하는 변수 (기본값 1층)

        [Header("Floor Positions")]
        [SerializeField] private Transform pos1F;
        [SerializeField] private Transform pos2F;
        [SerializeField] private Transform pos3F;

        private Transform currentPlayer;

        public void OpenUI(Transform playerTransform)
        {
            currentPlayer = playerTransform;
            if (elevatorUIPanel != null) elevatorUIPanel.SetActive(true);
        }

        public void CloseUI()
        {
            if (elevatorUIPanel != null) elevatorUIPanel.SetActive(false);
        }

        public void MoveToFloor(int targetFloor)
        {
            // [추가된 로직] 1. 현재 층과 목표 층이 같으면 아무것도 안 함
            if (currentFloor == targetFloor)
            {
                Debug.Log($"이미 {currentFloor}층에 있습니다.");
                return;
            }

            // 2. 전력 체크
            if (GeneratorManager.Instance != null && !GeneratorManager.Instance.IsFloorPowered(targetFloor))
            {
                Debug.Log($"{targetFloor}층 전력 부족!");
                return;
            }

            // 3. 목표 좌표 확인
            Transform targetPos = null;
            switch (targetFloor)
            {
                case 1: targetPos = pos1F; break;
                case 2: targetPos = pos2F; break;
                case 3: targetPos = pos3F; break;
            }

            if (targetPos == null) return;

            // 4. 이동 시작
            Debug.Log($"{currentFloor}층 -> {targetFloor}층 이동 시작");
            
            // 현재 층 정보를 갱신 (이동 확정)
            currentFloor = targetFloor; 

            CloseUI();

            // 5. FadeManager 연출
            if (FadeManager.Instance != null)
            {
                FadeManager.Instance.FadeIn(() => 
                {
                    PerformTeleport(targetPos);
                    FadeManager.Instance.FadeOut();
                });
            }
            else
            {
                PerformTeleport(targetPos);
            }
        }

        private void PerformTeleport(Transform targetPos)
        {
            if (cabinVisual != null) cabinVisual.position = targetPos.position;
            if (currentPlayer != null) currentPlayer.position = targetPos.position;
        }
    }
}