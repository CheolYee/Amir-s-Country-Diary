using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.UISystems
{
    public class StaminaGaugeUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private float smooth = 15f; // 값이 부드럽게 따라오도록

        private float _target;

        private void OnEnable()
        {
            Bus<StaminaChangedEvent>.OnEvent += OnStaminaChanged;
        }

        private void OnDisable()
        {
            Bus<StaminaChangedEvent>.OnEvent -= OnStaminaChanged;
        }

        private void Update()
        {
            if (fillImage == null) return;
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, _target, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        }

        private void OnStaminaChanged(StaminaChangedEvent evt)
        {
            _target = Mathf.Clamp01(evt.Normalized);
        }
    }
}