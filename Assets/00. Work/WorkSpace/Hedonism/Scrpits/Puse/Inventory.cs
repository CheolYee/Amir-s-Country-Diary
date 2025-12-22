using System;
using System.Collections.Generic;
using UnityEngine;
using _00._Work.Resources._02._Codes.Utils; // MonoSingleton, InputSo 경로
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item; // ItemSo 경로

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
        [SerializeField] private InputSo inputSo;
        [SerializeField] private int maxSlots = 3;

        [Header("Data")]
        [SerializeField] private List<InventorySlot> slots;

        public event Action OnInventoryUpdated;

        protected override void Awake()
        {
            base.Awake();
            // DontDestroyOnLoad(gameObject); // <-- 제거됨: 씬 이동 시 파괴됨

            // 슬롯 초기화
            if (slots == null || slots.Count != maxSlots)
            {
                slots = new List<InventorySlot>();
                for (int i = 0; i < maxSlots; i++) slots.Add(new InventorySlot());
            }
        }

        private void Start()
        {
            if (inputSo != null)
            {
                inputSo.OnInventory1KeyPressed += () => UseItem(0);
                inputSo.OnInventory2KeyPressed += () => UseItem(1);
                inputSo.OnInventory3KeyPressed += () => UseItem(2);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (inputSo != null)
            {
                inputSo.OnInventory1KeyPressed -= () => UseItem(0);
                inputSo.OnInventory2KeyPressed -= () => UseItem(1);
                inputSo.OnInventory3KeyPressed -= () => UseItem(2);
            }
        }

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
                Debug.Log($"사용: {slot.itemData.itemName}");
                slot.RemoveCount(1);
                if (slot.IsEmpty) slot.Clear();
                OnInventoryUpdated?.Invoke();
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