using System.Collections.Generic;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems; // Bus 사용을 위해 추가
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door; // RoomSpawnPoints 접근을 위해 추가
using Unity.Cinemachine;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Manager
{
    public class CameraBoundManager : MonoSingleton<CameraBoundManager>
    {
        [Header("Component References")]
        [SerializeField] private CinemachineConfiner2D confiner; 
        
        // [추가] RoomSpawnPoints를 자동으로 찾거나 할당해서 기본값을 바꿔줘야 함
        [SerializeField] private RoomSpawnPoints roomSpawnPoints; 

        [Header("Floor Settings")]
        [SerializeField] private List<FloorBoundData> floorDataList;

        private Dictionary<int, Collider2D> floorBoundDict;

        protected override void Awake()
        {
            // Singleton 초기화는 부모에서 처리되지만, 필요하다면 여기서 추가 로직 수행
            base.Awake(); 

            floorBoundDict = new Dictionary<int, Collider2D>();
            foreach (var data in floorDataList)
            {
                if (!floorBoundDict.ContainsKey(data.floorIndex))
                {
                    floorBoundDict.Add(data.floorIndex, data.boundCollider);
                }
            }

            // RoomSpawnPoints가 연결 안 되어있으면 찾기
            if (roomSpawnPoints == null)
                roomSpawnPoints = FindFirstObjectByType<RoomSpawnPoints>();
        }

        // [추가] 이벤트 구독
        private void OnEnable()
        {
            Bus<FloorChangedEvent>.OnEvent += OnFloorChanged;
        }

        // [추가] 이벤트 해제
        private void OnDisable()
        {
            Bus<FloorChangedEvent>.OnEvent -= OnFloorChanged;
        }

        // [추가] FloorZone에 닿았을 때 호출되는 함수
        private void OnFloorChanged(FloorChangedEvent evt)
        {
            // 1. 카메라 바운드 변경
            ChangeCameraBound(evt.FloorId);

            // 2. RoomSpawnPoints의 Default 값도 같이 변경해줘야 함 (매우 중요)
            // 그래야 방에 들어갔다 나올 때 현재 층의 바운드가 유지됨
            if (floorBoundDict.TryGetValue(evt.FloorId, out Collider2D newBound))
            {
                if (roomSpawnPoints != null)
                {
                    roomSpawnPoints.defaultCameraBound = newBound;
                }
            }
        }

        // [기존] 층 번호로 변경
        public void ChangeCameraBound(int floorIndex)
        {
            if (floorBoundDict.TryGetValue(floorIndex, out Collider2D newBound))
            {
                ChangeConfinerInternal(newBound); 
                Debug.Log($"Camera Bound switched to Floor {floorIndex}");
            }
            else
            {
                Debug.LogWarning($"Floor {floorIndex}에 해당하는 바운더리가 없습니다!");
            }
        }

        // [기존] 직접 Collider를 넣어서 변경 (DoorManager 등에서 사용)
        public void ChangeCameraBound(Collider2D newBound)
        {
            if (newBound != null)
            {
                ChangeConfinerInternal(newBound);
            }
        }

        // [내부 로직] 실제 시네머신에 적용
        private void ChangeConfinerInternal(Collider2D newBound)
        {
            if (confiner == null) return;

            // 이미 같은 바운드면 굳이 갱신하지 않음 (성능 최적화)
            if (confiner.BoundingShape2D == newBound) return;

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