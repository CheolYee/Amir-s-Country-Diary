using System.Collections.Generic;
using UnityEngine;
using _00._Work.Resources._02._Codes.Utils;
using System;

namespace PBG_01_PUSE
{
    public class GeneratorManager : MonoSingleton<GeneratorManager>
    {
        [Header("Door Settings")]
        [SerializeField] private GameObject mainDoor; 

        // 층별 전력 상태 (Key: 층수, Value: 전력ON/OFF)
        private Dictionary<int, bool> floorPowerState = new Dictionary<int, bool>();
        
        // UI 갱신을 위한 이벤트
        public event Action OnPowerStateChanged;

        protected override void Awake()
        {
            base.Awake();
            // 1, 2, 3층 초기화 (모두 꺼짐)
            floorPowerState[1] = false;
            floorPowerState[2] = false;
            floorPowerState[3] = false;
        }

        // 발전기가 호출하는 함수
        public void SetFloorPower(int floor, bool isPowered)
        {
            if (floorPowerState.ContainsKey(floor))
            {
                floorPowerState[floor] = isPowered;
                Debug.Log($"{floor}층 전력이 복구되었습니다.");
                
                OnPowerStateChanged?.Invoke(); // 상태 변경 알림
                CheckAllPowered();
            }
        }

        // 엘리베이터가 호출하는 함수 (해당 층 전력 확인)
        public bool IsFloorPowered(int floor)
        {
            if (floorPowerState.ContainsKey(floor))
                return floorPowerState[floor];
            return false;
        }

        // 모든 발전기가 켜졌는지 확인 (정문 개방용)
        private void CheckAllPowered()
        {
            foreach (var isPowered in floorPowerState.Values)
            {
                if (!isPowered) return; // 하나라도 꺼져있으면 리턴
            }

            Debug.Log("모든 층 전력 복구 완료! 정문 개방.");
            if (mainDoor != null) mainDoor.SetActive(false);
        }
    }
}