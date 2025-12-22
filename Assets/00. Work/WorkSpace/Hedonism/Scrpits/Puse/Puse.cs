using UnityEngine;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;
using PBG_01_PUSE;
using _00._Work.Resources._02._Codes.Utils;

public class Puse : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InputSo inputSo;
    [SerializeField] private ItemSo itemData;

    private bool canPickup = false;

    private void OnEnable()
    {
        if (inputSo != null) inputSo.OnInteractionKeyPressed += HandleInteraction;
    }

    private void OnDisable()
    {
        if (inputSo != null) inputSo.OnInteractionKeyPressed -= HandleInteraction;
    }

    private void HandleInteraction()
    {
        if (canPickup && Inventory.Instance != null)
        {
            if (Inventory.Instance.AddItem(itemData))
            {
                Debug.Log($"{itemData.itemName} 획득!");
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) canPickup = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) canPickup = false;
    }
}