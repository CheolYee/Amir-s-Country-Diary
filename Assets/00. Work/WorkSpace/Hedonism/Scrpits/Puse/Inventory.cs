using System;
using System.Collections.Generic;
using UnityEngine;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;

namespace PBG_01_PUSE
{
    [Serializable]
    public class InventorySlot
    {
        public ItemSo itemData;
        public int count;

        public void AddCount(int amount) => count += amount;
        public void RemoveCount(int amount) => count -= amount;
        public bool IsEmpty => count <= 0;
        public void Clear()
        {
            itemData = null;
            count = 0;
        }
    }

    public class Inventory : MonoSingleton<Inventory>
    {
        [Header("Settings")]
        private InputSo inputSo;
        [SerializeField] private int maxSlots = 3;

        [Header("Drop Settings")]
        [SerializeField] private GameObject dropItemPrefab; // 바닥에 떨어질 아이템 프리팹 (Puse 스크립트 부착 필수)
        private Transform playerTransform;

        [Header("Data")]
        [SerializeField] private List<InventorySlot> slots;

        public event Action OnInventoryUpdated;

        protected override void Awake()
        {
            base.Awake();

            if (slots == null || slots.Count != maxSlots)
            {
                slots = new List<InventorySlot>();
                for (int i = 0; i < maxSlots; i++) slots.Add(new InventorySlot());
            }
        }

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;

            if (inputSo != null)
            {
                // 람다식 대신 별도의 메서드를 연결해야 해제가 가능합니다.
                inputSo.OnInventory1KeyPressed += HandleUseSlot1;
                inputSo.OnInventory2KeyPressed += HandleUseSlot2;
                inputSo.OnInventory3KeyPressed += HandleUseSlot3;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (inputSo != null)
            {
                // 올바른 이벤트 해제 방식
                inputSo.OnInventory1KeyPressed -= HandleUseSlot1;
                inputSo.OnInventory2KeyPressed -= HandleUseSlot2;
                inputSo.OnInventory3KeyPressed -= HandleUseSlot3;
            }
        }

        public void SetInputSo(InputSo so)
        {
            // 런타임에 InputSo가 변경될 경우 기존 이벤트 해제 및 재등록 로직이 필요할 수 있음
            inputSo = so;
        }

        // 이벤트 연결용 래퍼 메서드
        private void HandleUseSlot1() => UseItem(0);
        private void HandleUseSlot2() => UseItem(1);
        private void HandleUseSlot3() => UseItem(2);

        public bool AddItem(ItemSo item)
        {
            foreach (var slot in slots)
            {
                if (slot.itemData == item)
                {
                    slot.AddCount(1);
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
            foreach (var slot in slots)
            {
                if (slot.itemData == null)
                {
                    slot.itemData = item;
                    slot.count = 1;
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
            return false;
        }

        public void UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            InventorySlot slot = slots[slotIndex];

            if (slot.itemData != null)
            {
                Debug.Log($"사용(드랍): {slot.itemData.itemName}");
                
                // 1. 아이템 드랍 (생성)
                SpawnDroppedItem(slot.itemData);

                // 2. 인벤토리에서 제거
                slot.RemoveCount(1);
                if (slot.IsEmpty) slot.Clear();
                
                OnInventoryUpdated?.Invoke();
            }
        }

        // 드랍 로직 구현
        private void SpawnDroppedItem(ItemSo itemData)
        {
            if (dropItemPrefab == null)
            {
                Debug.LogError("Inventory: Drop Item Prefab이 비어있습니다! 인스펙터에서 할당해주세요.");
                return;
            }

            Vector3 spawnPos = playerTransform != null ? playerTransform.position : Vector3.zero;
            // 플레이어 주변 랜덤 위치
            Vector3 randomOffset = (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 1.5f; 
            
            GameObject droppedObj = Instantiate(dropItemPrefab, spawnPos + randomOffset, Quaternion.identity);
            
            // 데이터 주입 (Puse 스크립트 이름이 Item 기능을 담당한다면 Puse 컴포넌트 가져오기)
            Puse itemScript = droppedObj.GetComponent<Puse>();
            if (itemScript != null)
            {
                itemScript.Initialize(itemData);
            }
        }

        public int GetItemCount(ItemType type)
        {
            int total = 0;
            foreach (var slot in slots)
            {
                if (slot.itemData != null && slot.itemData.itemType == type)
                    total += slot.count;
            }
            return total;
        }

        public bool TryConsumeItem(ItemType type, int amount = 1)
        {
            if (GetItemCount(type) < amount) return false;

            int remaining = amount;
            foreach (var slot in slots)
            {
                if (slot.itemData != null && slot.itemData.itemType == type)
                {
                    if (slot.count >= remaining)
                    {
                        slot.RemoveCount(remaining);
                        remaining = 0;
                    }
                    else
                    {
                        remaining -= slot.count;
                        slot.Clear();
                    }
                    if (slot.IsEmpty) slot.Clear();
                    if (remaining <= 0) break;
                }
            }
            OnInventoryUpdated?.Invoke();
            return true;
        }

        public List<InventorySlot> GetSlots() => slots;
    }
}