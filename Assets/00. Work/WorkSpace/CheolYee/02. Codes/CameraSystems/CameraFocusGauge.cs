using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public class CameraFocusGauge : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Image fill;
        
        private void Awake()
        {
            if (root != null) root.SetActive(false);
            if (fill != null) fill.fillAmount = 0f;
        }

        public void SetVisible(bool visible)
        {
            if (root != null) root.SetActive(visible);
        }

        public void Set01(float value01)
        {
            if (fill == null) return;
            fill.fillAmount = Mathf.Clamp01(value01);
        }
    }
}