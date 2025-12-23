using System.Collections.Generic;
using _00._Work.Resources._02._Codes.Utils;
using PBG_01_PUSE;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    // 제공해주신 MonoSingleton 사용
    public class GeneratorUIPanel : MonoSingleton<GeneratorUIPanel>
    {
        [Header("References")]
        [SerializeField] private GameObject panelRoot;   // UI 전체 부모 오브젝트 (On/Off용)
        [SerializeField] private Transform slotParent;   // 슬롯이 생성될 Content Transform
        [SerializeField] private GameObject slotPrefab;  // GeneratorRecipeSlotUI 프리팹

        private List<GeneratorRecipe> currentRecipes;

        protected override void Awake()
        {
            base.Awake();
            Hide(); // 시작 시 숨김
        }

        private void Start()
        {
            // 인벤토리 아이템 변동 시 UI 실시간 갱신 (아이템 드랍/습득 시 수치 변경 반영)
            if (Inventory.Instance != null)
            {
                Inventory.Instance.OnInventoryUpdated += OnInventoryUpdated;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Inventory.Instance != null)
            {
                Inventory.Instance.OnInventoryUpdated -= OnInventoryUpdated;
            }
        }

        private void OnInventoryUpdated()
        {
            // 패널이 켜져 있을 때만 갱신
            if (panelRoot.activeSelf)
            {
                RefreshUI();
            }
        }

        // 발전기 범위 진입 시 호출
        public void Show(List<GeneratorRecipe> recipes)
        {
            currentRecipes = recipes;
            panelRoot.SetActive(true);
            RefreshUI();
        }

        // 발전기 범위 이탈 시 호출
        public void Hide()
        {
            currentRecipes = null;
            panelRoot.SetActive(false);
        }

        // UI 슬롯 다시 그리기
        public void RefreshUI()
        {
            if (currentRecipes == null) return;

            // 1. 기존 슬롯 삭제 (풀링 최적화 가능하지만 간단하게 구현)
            foreach (Transform child in slotParent)
            {
                Destroy(child.gameObject);
            }

            // 2. 레시피별 슬롯 생성
            foreach (var recipe in currentRecipes)
            {
                GameObject go = Instantiate(slotPrefab, slotParent);
                GeneratorRecipeSlotUI slotUI = go.GetComponent<GeneratorRecipeSlotUI>();
                if (slotUI != null)
                {
                    slotUI.SetData(recipe);
                }
            }
        }
    }
}