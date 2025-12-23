using _00._Work.Resources._02._Codes.Utils;
using UnityEngine;
using PBG_01_PUSE;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySlotUI[] slotUIs;
    [SerializeField] private InputSo inputSo;

    private Inventory _inv;

    private void Start()
    {
        if (inputSo == null)
        {
            Debug.LogError("InventoryUI: InputSo 안 들어옴");
            return;
        }

        _inv = Inventory.Instance;
        if (_inv == null)
        {
            Debug.LogError("InventoryUI: Inventory 인스턴스를 찾거나 생성할 수 없음");
            return;
        }

        _inv.SetInputSo(inputSo);
        _inv.OnInventoryUpdated += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_inv != null)
            _inv.OnInventoryUpdated -= Refresh;
    }

    private void Refresh()
    {
        if (_inv == null) return;

        var slots = _inv.GetSlots();
        int n = Mathf.Min(slotUIs.Length, slots.Count);

        for (int i = 0; i < n; i++)
        {
            if (slotUIs[i] != null)
                slotUIs[i].Refresh(slots[i]);
        }
    }
}