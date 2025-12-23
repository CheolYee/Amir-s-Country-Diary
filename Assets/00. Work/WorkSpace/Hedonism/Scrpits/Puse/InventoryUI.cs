using UnityEngine;
using PBG_01_PUSE;
using _00._Work.Resources._02._Codes.Utils;

public class InventoryUI : MonoBehaviour
{
    [Header("Slots (Size = 3)")]
    [SerializeField] private InventorySlotUI[] slotUIs;

    [SerializeField] private InputSo inputSo;
    [SerializeField] GameObject item;
    private void Awake()
    {
        if (inputSo == null)
        {
            Debug.LogError("InventoryUI: InputSo 안 들어옴");
            return;
        }

        // 여기서 Inventory의 inputSo를 정의해준다
        Inventory.Instance.SetInputSo(inputSo);
    }

    private void Start()
    {
        // 인벤토리 변경 이벤트 구독
        Inventory.Instance.OnInventoryUpdated += Refresh;

        // 초기 상태 반영
        Refresh();
    }

    private void OnDestroy()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryUpdated -= Refresh;
    }

    /// <summary>
    /// Inventory 슬롯 리스트 순서 그대로 UI에 반영
    /// </summary>
    private void Refresh()
    {
        var slots = Inventory.Instance.GetSlots();

        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].Refresh(slots[i]);
        }
    }
}
