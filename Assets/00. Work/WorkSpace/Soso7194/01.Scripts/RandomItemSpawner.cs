using System.Collections.Generic;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts
{
    // 인스펙터에서 "아이템 종류 + 개수"를 설정하기 위한 데이터 셋
    [System.Serializable]
    public struct ItemSpawnData
    {
        public string label; // 인스펙터 구분을 위한 단순 이름 (옵션)
        public ItemSo itemSo; // 생성할 아이템 데이터
        [Range(1, 10)] public int count; // 생성할 개수
    }

    public class RandomItemSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject _itemPrefab; // 빈 아이템 프리팹 (Item 스크립트 부착됨)

        [Header("Spawn Config")]
        // 여기에 "Fuse 2개", "Bolt 1개" 식으로 목록을 추가하세요.
        [SerializeField] private List<ItemSpawnData> _spawnList; 

        [Header("Positions")]
        [SerializeField] private List<Transform> _spawnPoints; // 배치 가능한 위치들

        private void Start()
        {
            SpawnItems();
        }

        public void SpawnItems()
        {
            if (_itemPrefab == null)
            {
                Debug.LogError("아이템 프리팹이 연결되지 않았습니다.");
                return;
            }

            // 1. 생성해야 할 모든 아이템을 하나의 리스트(Queue)로 풀어서 나열
            // 예: [Fuse, Fuse, Bolt, Board, Board, Board]
            List<ItemSo> itemsToCreate = new List<ItemSo>();

            foreach (var data in _spawnList)
            {
                if (data.itemSo == null) continue;

                for (int i = 0; i < data.count; i++)
                {
                    itemsToCreate.Add(data.itemSo);
                }
            }

            // 2. 예외 처리: 위치보다 아이템이 많으면 경고
            if (itemsToCreate.Count > _spawnPoints.Count)
            {
                Debug.LogWarning($"[Spawner] 위치는 {_spawnPoints.Count}곳인데, 아이템은 {itemsToCreate.Count}개입니다. 넘치는 아이템은 생성되지 않습니다.");
            }

            // 3. 위치 리스트 섞기 (랜덤성 부여)
            // 원본 리스트 보호를 위해 복사본 사용
            List<Transform> shuffledPoints = new List<Transform>(_spawnPoints);
            ShuffleList(shuffledPoints);

            // 4. 실제 생성 (아이템 개수와 위치 개수 중 적은 쪽만큼만 반복)
            int spawnCount = Mathf.Min(itemsToCreate.Count, shuffledPoints.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                Transform targetPos = shuffledPoints[i];
                ItemSo targetData = itemsToCreate[i];

                // 프리팹 생성
                GameObject newObj = Instantiate(_itemPrefab, targetPos.position, Quaternion.identity);
                
                // 데이터 주입
                var itemComp = newObj.GetComponent<Puse>();
                if (itemComp != null)
                {
                    itemComp.Initialize(targetData);
                }
            }
        }

        // 리스트 셔플 함수 (피셔-예이츠 알고리즘)
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}