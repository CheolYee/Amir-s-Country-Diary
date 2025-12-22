using PBG_01_PUSE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private GameObject contentParent;

        public void UpdateSlot(InventorySlot slot)
        {
            if (slot != null && slot.itemData != null && slot.count > 0)
            {
                contentParent.SetActive(true);
                iconImage.sprite = slot.itemData.itemSprite;
                countText.text = slot.count > 1 ? slot.count.ToString() : "";
            }
            else
            {
                contentParent.SetActive(false);
            }
        }
    }
}