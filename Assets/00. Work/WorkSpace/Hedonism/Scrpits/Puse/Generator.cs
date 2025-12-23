using UnityEngine;
using System.Collections.Generic;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;
using _00._Work.WorkSpace.Soso7194._01.Scripts.UI;

namespace PBG_01_PUSE
{
    [System.Serializable]
    public class GeneratorRecipe
    {
        public ItemSo requiredItem;
        public int requiredAmount = 1;
        public int currentAmount = 0; // [HideInInspector] 제거 (테스트용)
        public bool IsComplete => currentAmount >= requiredAmount;
    }

    public class Generator : MonoBehaviour
    {
        [Header("Settings")]
        [Range(1, 3)] [SerializeField] private int floorNumber = 1;
        [SerializeField] private InputSo inputSo;
        [SerializeField] private List<GeneratorRecipe> recipes;
        [SerializeField] private GameObject activeVisual;
        
        private bool isActivated = false;
        private bool isPlayerInRange = false;

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
            if (!isPlayerInRange || isActivated) return;

            bool anyItemAdded = false;

            foreach (var recipe in recipes)
            {
                if (recipe.IsComplete) continue;

                while (!recipe.IsComplete)
                {
                    // Inventory 싱글톤 안전 참조
                    if (Inventory.Instance != null && Inventory.Instance.TryConsumeItem(recipe.requiredItem.itemType, 1))
                    {
                        recipe.currentAmount++;
                        anyItemAdded = true;
                        Debug.Log($"{recipe.requiredItem.itemName} 주입! ({recipe.currentAmount}/{recipe.requiredAmount})");
                    }
                    else break;
                }
            }

            if (anyItemAdded)
            {
                // [수정] 아이템 주입 후 UI 수치 즉시 갱신
                if (GeneratorUIPanel.Instance != null)
                    GeneratorUIPanel.Instance.RefreshUI();
                
                CheckCompletion();
            }
            else if (!isActivated)
            {
                Debug.Log("필요한 재료가 없습니다.");
            }
        }

        private void CheckCompletion()
        {
            foreach (var recipe in recipes) if (!recipe.IsComplete) return;
            ActivateGenerator();
        }

        private void ActivateGenerator()
        {
            isActivated = true;
            Debug.Log($"{floorNumber}층 발전기 가동!");
            
            if (activeVisual != null) activeVisual.SetActive(true);

            // [수정] 가동 완료 시 UI 갱신 (완료 상태 표시) 또는 끄기
            if (GeneratorUIPanel.Instance != null && isPlayerInRange)
                GeneratorUIPanel.Instance.RefreshUI(); 
            
            if (GeneratorManager.Instance != null)
            {
                GeneratorManager.Instance.SetFloorPower(floorNumber, true);
            }
            
            Bus<GeneratorActivatedEvent>.Raise(new GeneratorActivatedEvent(floorNumber));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerInRange = true;
                
                // [수정] 플레이어 진입 시 UI 싱글톤 호출하여 패널 켜기
                // 이미 활성화된 발전기도 정보를 보고 싶다면 !isActivated 조건 제거
                if (!isActivated && GeneratorUIPanel.Instance != null)
                {
                    GeneratorUIPanel.Instance.Show(recipes);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerInRange = false;

                // [수정] 플레이어 이탈 시 UI 싱글톤 호출하여 패널 끄기
                if (GeneratorUIPanel.Instance != null)
                {
                    GeneratorUIPanel.Instance.Hide();
                }
            }
        }
    }
}