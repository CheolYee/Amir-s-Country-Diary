using System.Collections.Generic;
using _00._Work.Resources._02._Codes.Utils;
using Unity.Cinemachine;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Manager
{
    public class CameraBoundManager : MonoSingleton<CameraBoundManager>
    {
        [Header("Component References")]
        [SerializeField] private CinemachineConfiner2D confiner; 

        [Header("Floor Settings")]
        [SerializeField] private List<FloorBoundData> floorDataList;

        private Dictionary<int, Collider2D> floorBoundDict;

        private void Awake()
        {
            floorBoundDict = new Dictionary<int, Collider2D>();
            foreach (var data in floorDataList)
            {
                if (!floorBoundDict.ContainsKey(data.floorIndex))
                {
                    floorBoundDict.Add(data.floorIndex, data.boundCollider);
                }
            }
        }

        // [기존] 층 번호로 변경
        public void ChangeCameraBound(int floorIndex)
        {
            if (floorBoundDict.TryGetValue(floorIndex, out Collider2D newBound))
            {
                ChangeConfinerInternal(newBound); // 아래 공통 함수 호출
                Debug.Log($"Camera Bound switched to Floor {floorIndex}");
            }
            else
            {
                Debug.LogWarning($"Floor {floorIndex}에 해당하는 바운더리가 없습니다!");
            }
        }

        // [추가] 직접 Collider를 넣어서 변경 (DoorManager 등에서 사용)
        public void ChangeCameraBound(Collider2D newBound)
        {
            if (newBound != null)
            {
                ChangeConfinerInternal(newBound);
            }
        }

        // [내부 로직 통합] 실제 시네머신에 적용하는 부분
        private void ChangeConfinerInternal(Collider2D newBound)
        {
            if (confiner == null) return;

            confiner.BoundingShape2D = newBound; 
            confiner.InvalidateBoundingShapeCache(); 
        }
    }

    [System.Serializable]
    public class FloorBoundData
    {
        public string floorName; 
        public int floorIndex;   
        public Collider2D boundCollider; 
    }
}