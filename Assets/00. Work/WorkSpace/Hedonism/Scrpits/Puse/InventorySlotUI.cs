using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PBG_01_PUSE;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    /// <summary>
    /// InventorySlot 데이터를 받아 UI에 반영
    /// </summary>
    public void Refresh(InventorySlot slot)
    {
        // 슬롯이 비어있을 때
        if (slot == null || slot.itemData == null || slot.count <= 0)
        {
            iconImage.enabled = false;
            countText.text = "";
            return;
        }

        // 아이템 있음
        iconImage.enabled = true;
        iconImage.sprite = slot.itemData.itemSprite;

        // 중첩 아이템만 개수 표시
        countText.text = slot.count > 1 ? slot.count.ToString() : "";
    }
}
