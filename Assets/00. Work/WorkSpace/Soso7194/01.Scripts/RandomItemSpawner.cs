using System.Collections.Generic;
using System.Linq; // [중요] 리스트 중복 제거 기능을 위해 추가
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts
{
    [System.Serializable]
    public struct ItemSpawnData
    {
        public string label;
        public ItemSo itemSo;
        [Range(1, 10)] public int count;
    }

    public class RandomItemSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject _itemPrefab; 

        [Header("Spawn Config")]
        [SerializeField] private List<ItemSpawnData> _spawnList; 

        [Header("Positions")]
        [SerializeField] private List<Transform> _spawnPoints; 

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

            // [수정된 부분] 시작하자마자 위치 리스트에서 'null'과 '중복된 위치'를 싹 제거합니다.
            // 이렇게 하면 실수로 같은 곳을 2번 넣었어도 1곳으로 처리됩니다.
            _spawnPoints = _spawnPoints
                            .Where(p => p != null) // 빈칸 제거
                            .Distinct()            // 중복 제거
                            .ToList();

            // 1. 아이템 덱 생성
            List<ItemSo> itemsToCreate = new List<ItemSo>();
            foreach (var data in _spawnList)
            {
                if (data.itemSo == null) continue;
                for (int i = 0; i < data.count; i++) itemsToCreate.Add(data.itemSo);
            }

            // 2. 개수 체크
            if (itemsToCreate.Count > _spawnPoints.Count)
            {
                Debug.LogWarning($"[Spawner] 위치는 {_spawnPoints.Count}곳인데, 생성할 아이템은 {itemsToCreate.Count}개입니다.");
            }

            // 3. 셔플 (위치 섞기)
            List<Transform> shuffledPoints = new List<Transform>(_spawnPoints);
            ShuffleList(shuffledPoints);

            // 4. 생성
            int spawnCount = Mathf.Min(itemsToCreate.Count, shuffledPoints.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                Transform targetPos = shuffledPoints[i];
                ItemSo targetData = itemsToCreate[i];

                GameObject newObj = Instantiate(_itemPrefab, targetPos.position, Quaternion.identity);
                
                // 데이터 주입 (Puse 스크립트 사용)
                var itemComp = newObj.GetComponent<Puse>();
                if (itemComp != null)
                {
                    itemComp.Initialize(targetData);
                }
            }
        }

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