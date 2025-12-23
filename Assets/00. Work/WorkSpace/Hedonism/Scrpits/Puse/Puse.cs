using UnityEngine;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;
using PBG_01_PUSE;
using _00._Work.Resources._02._Codes.Utils;

public class Puse : MonoBehaviour
{
    [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [Header("Settings")]
        [SerializeField] private InputSo _inputSo; // 인스펙터에서 할당 필요 (모든 아이템 공통)

        // 스포너에 의해 주입될 데이터
        private ItemSo _itemData;
        private bool _canPickup = false;

        // 1. 스포너가 이 함수를 호출해 아이템의 정체성을 부여합니다.
        public void Initialize(ItemSo data)
        {
            _itemData = data;

            // 데이터에 맞는 이미지로 교체
            if (_spriteRenderer != null && data.itemSprite != null)
            {
                _spriteRenderer.sprite = data.itemSprite;
            }

            // 이름 변경 (에디터 디버깅용)
            gameObject.name = $"Item_{data.itemName}";
        }

        private void OnEnable()
        {
            if (_inputSo != null) _inputSo.OnInteractionKeyPressed += HandleInteraction;
        }

        private void OnDisable()
        {
            if (_inputSo != null) _inputSo.OnInteractionKeyPressed -= HandleInteraction;
        }

        // 2. Puse.cs에 있던 상호작용 로직 그대로 적용
        private void HandleInteraction()
        {
            // 플레이어가 범위 안에 있고, 데이터가 설정되어 있을 때만
            if (_canPickup && _itemData != null)
            {
                Debug.Log("체크 1 성공");
                // 인벤토리 싱글톤 접근 (사용하시는 인벤토리 구조에 맞춤)
                if (Inventory.Instance != null)
                {
                    if (Inventory.Instance.AddItem(_itemData))
                    {
                        Debug.Log($"[{_itemData.itemName}] 획득 성공!");
                        
                        // 획득 후 오브젝트 비활성화 또는 파괴
                        
                        Destroy(gameObject); // 필요시 파괴로 변경
                    }
                    else
                    {
                        Debug.Log("인벤토리가 가득 찼습니다.");
                    }
                }
                else
                {
                    Debug.LogError("Inventory Instance를 찾을 수 없습니다.");
                }
            }
        }

        // 3. 충돌 감지 로직
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player")) _canPickup = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player")) _canPickup = false;
        }
    }