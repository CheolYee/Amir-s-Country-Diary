using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public class NoiseGaugeUi : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private float smooth = 15f;
        
        [SerializeField] private TextMeshProUGUI percentTextTMP;
        
        [Header("Color")]
        [SerializeField] private bool useGradientColor = true;
        [SerializeField] private Gradient fillGradient;

        private float _target;
        private float _display;

        private void OnEnable()
        {
            Bus<NoiseMeterChangedEvent>.OnEvent += OnChanged;
        }

        private void OnDisable()
        {
            Bus<NoiseMeterChangedEvent>.OnEvent -= OnChanged;
        }
        
        private void Reset()
        {
            // 기본 그라데이션(초록 -> 노랑 -> 빨강)
            fillGradient = new Gradient();
            fillGradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.2f, 1f, 0.2f), 0f),
                    new GradientColorKey(new Color(1f, 1f, 0.2f), 0.6f),
                    new GradientColorKey(new Color(1f, 0.2f, 0.2f), 1f),
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f),
                }
            );
        }

        private void Update()
        {
            if (fillImage == null) return;

            _display = Mathf.Lerp(_display, _target, 1f - Mathf.Exp(-smooth * Time.deltaTime));
            fillImage.fillAmount = _display;

            if (useGradientColor)
            {
                fillImage.color = fillGradient.Evaluate(_display);
            }

            int percent = Mathf.Clamp(Mathf.RoundToInt(_display * 100f), 0, 100);
            if (percentTextTMP != null) percentTextTMP.text = $"{percent}%";
        }

        private void OnChanged(NoiseMeterChangedEvent evt)
        {
            _target = Mathf.Clamp01(evt.Normalized);
        }
    }
}