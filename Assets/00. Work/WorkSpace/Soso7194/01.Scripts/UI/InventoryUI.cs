using System.Collections.Generic;
using PBG_01_PUSE;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform slotsParent;
        [SerializeField] private InventorySlotUI slotPrefab;
        private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();

        private void Start()
        {
            Inventory.Instance.OnInventoryUpdated += RefreshUI;
            InitializeUI();
        }

        private void InitializeUI()
        {
            foreach (Transform child in slotsParent) Destroy(child.gameObject);
            uiSlots.Clear();

            var dataSlots = Inventory.Instance.GetSlots();
            for (int i = 0; i < dataSlots.Count; i++)
            {
                uiSlots.Add(Instantiate(slotPrefab, slotsParent));
            }
            RefreshUI();
        }

        private void RefreshUI()
        {
            var dataSlots = Inventory.Instance.GetSlots();
            for (int i = 0; i < uiSlots.Count; i++)
            {
                uiSlots[i].UpdateSlot(dataSlots[i]);
            }
        }
    }
}