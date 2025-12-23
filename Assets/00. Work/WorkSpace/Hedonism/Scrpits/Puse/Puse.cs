using System.Collections;
using UnityEngine;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;
using PBG_01_PUSE;
using _00._Work.Resources._02._Codes.Utils;

public class Puse : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
        
    [Header("Settings")]
    [SerializeField] private InputSo _inputSo;

    [SerializeField] private GameObject _text;
    
    private ItemSo _itemData;
    private bool _canPickup = false;
    private bool _isDropCooldown = false; // 드랍 쿨타임

    public void Initialize(ItemSo data)
    {
        _itemData = data;

        if (_spriteRenderer != null && data.itemSprite != null)
        {
            _spriteRenderer.sprite = data.itemSprite;
        }

        gameObject.name = $"Item_{data.itemName}";
        
        // 생성 직후 1초간 획득 불가 (바로 다시 먹는 것 방지)
        StartCoroutine(DropCooldownRoutine());
    }
    
    private IEnumerator DropCooldownRoutine()
    {
        _isDropCooldown = true;
        yield return new WaitForSeconds(1.0f);
        _isDropCooldown = false;
    }

    private void OnEnable()
    {
        if (_inputSo != null) _inputSo.OnInteractionKeyPressed += HandleInteraction;
        _text.SetActive(false);
    }

    private void OnDisable()
    {
        if (_inputSo != null) _inputSo.OnInteractionKeyPressed -= HandleInteraction;
    }

    private void HandleInteraction()
    {
        if (_isDropCooldown) return; // 쿨타임 중이면 무시

        if (_canPickup && _itemData != null)
        {
            if (Inventory.Instance != null)
            {
                if (Inventory.Instance.AddItem(_itemData))
                {
                    Debug.Log($"[{_itemData.itemName}] 획득 성공!");
                    
                    // 인벤토리에 넣었으므로 월드 오브젝트는 파괴
                    Destroy(gameObject); 
                }
                else
                {
                    Debug.Log("인벤토리가 가득 찼습니다.");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canPickup = true;
            _text.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canPickup = false;
            _text.SetActive(false);
        }
    }
}