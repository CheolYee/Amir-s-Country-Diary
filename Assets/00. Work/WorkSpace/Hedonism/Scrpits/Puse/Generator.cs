using UnityEngine;
using System.Collections.Generic;
using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item;

namespace PBG_01_PUSE
{
    [System.Serializable]
    public class GeneratorRecipe
    {
        public ItemSo requiredItem;
        public int requiredAmount = 1;
        [HideInInspector] public int currentAmount = 0;
        public bool IsComplete => currentAmount >= requiredAmount;
    }

    public class Generator : MonoBehaviour
    {
        [Header("Settings")]
        [Range(1, 3)] [SerializeField] private int floorNumber = 1; // 1~3층 설정
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
                    if (Inventory.Instance.TryConsumeItem(recipe.requiredItem.itemType, 1))
                    {
                        recipe.currentAmount++;
                        anyItemAdded = true;
                        Debug.Log($"{recipe.requiredItem.itemName} 주입! ({recipe.currentAmount}/{recipe.requiredAmount})");
                    }
                    else break;
                }
            }

            if (anyItemAdded) CheckCompletion();
            else if (!isActivated) Debug.Log("필요한 재료가 없습니다.");
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
            
            // 매니저에게 "몇 층"이 켜졌는지 보고
            if (GeneratorManager.Instance != null)
            {
                GeneratorManager.Instance.SetFloorPower(floorNumber, true);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) isPlayerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) isPlayerInRange = false;
        }
    }
}