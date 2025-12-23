using PBG_01_PUSE;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class ElevatorUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button btn1F;
        [SerializeField] private Button btn2F;
        [SerializeField] private Button btn3F;
        [SerializeField] private Button btnClose;

        [Header("Colors")]
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color inactiveColor = Color.red;

        private void Start()
        {
            // [수정] 각 버튼이 전용 함수를 호출하도록 변경
            btn1F.onClick.AddListener(() => Elevator.Instance.OnClickFloor1());
            btn2F.onClick.AddListener(() => Elevator.Instance.OnClickFloor2());
            btn3F.onClick.AddListener(() => Elevator.Instance.OnClickFloor3());
            
            btnClose.onClick.AddListener(() => Elevator.Instance.CloseUI());

            if (GeneratorManager.Instance != null)
                GeneratorManager.Instance.OnPowerStateChanged += UpdateButtonStates;
        }

        private void OnEnable()
        {
            UpdateButtonStates();
        }

        private void OnDestroy()
        {
            if (GeneratorManager.Instance != null)
                GeneratorManager.Instance.OnPowerStateChanged -= UpdateButtonStates;
        }

        private void UpdateButtonStates()
        {
            if (GeneratorManager.Instance == null) return;
            UpdateSingleButton(btn1F, 1);
            UpdateSingleButton(btn2F, 2);
            UpdateSingleButton(btn3F, 3);
        }

        private void UpdateSingleButton(Button btn, int floor)
        {
            bool isPowered = GeneratorManager.Instance.IsFloorPowered(floor);
            btn.interactable = isPowered;

            var colors = btn.colors;
            colors.normalColor = isPowered ? activeColor : inactiveColor;
            colors.selectedColor = isPowered ? activeColor : inactiveColor;
            colors.disabledColor = inactiveColor;
            btn.colors = colors;
        }
    }
}