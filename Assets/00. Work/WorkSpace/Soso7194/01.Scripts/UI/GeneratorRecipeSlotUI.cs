using PBG_01_PUSE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// TextMeshPro 사용 (일반 Text라면 UnityEngine.UI.Text로 변경)

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class GeneratorRecipeSlotUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI statusText; // "1/3" (현재/필요)
        [SerializeField] private TextMeshProUGUI myCountText; // "(보유: 5)"

        [Header("Colors")]
        [SerializeField] private Color completeColor = Color.green;
        [SerializeField] private Color incompleteColor = Color.white;
        [SerializeField] private Color notEnoughColor = Color.red;

        public void SetData(GeneratorRecipe recipe)
        {
            if (recipe == null || recipe.requiredItem == null) return;

            // 1. 아이콘 및 이름
            if (recipe.requiredItem.itemSprite != null)
            {
                iconImage.sprite = recipe.requiredItem.itemSprite;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
            nameText.text = recipe.requiredItem.itemName;

            // 2. 진행도 표시
            statusText.text = $"{recipe.currentAmount} / {recipe.requiredAmount}";
            statusText.color = recipe.IsComplete ? completeColor : incompleteColor;

            // 3. 내 인벤토리 보유량 표시
            int myCount = 0;
            // 인벤토리 싱글톤 접근
            if (Inventory.Instance != null)
            {
                myCount = Inventory.Instance.GetItemCount(recipe.requiredItem.itemType);
            }

            myCountText.text = $"(보유: {myCount})";
            
            // 아직 완료 안됐는데 내 재료도 부족하면 빨간색 표시 등 시각적 피드백
            bool haveEnough = myCount > 0;
            if (!recipe.IsComplete && !haveEnough) 
                myCountText.color = notEnoughColor;
            else 
                myCountText.color = Color.white;
        }
    }
}