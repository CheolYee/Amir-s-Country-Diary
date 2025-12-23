using System.Collections;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class PhotoFlashUI : MonoBehaviour
    {
        [SerializeField] private Image flashImage;

        private Coroutine _co;

        private void Awake()
        {
            SetAlpha(0f);
        }

        private void OnEnable() => Bus<PhotoFlashEvent>.OnEvent += OnFlash;
        private void OnDisable() => Bus<PhotoFlashEvent>.OnEvent -= OnFlash;

        private void OnFlash(PhotoFlashEvent evt)
        {
            if (flashImage == null) return;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(FlashRoutine(evt.Duration, evt.PeakAlpha));
        }

        private IEnumerator FlashRoutine(float duration, float peakAlpha)
        {
            // 즉시 번쩍
            SetAlpha(peakAlpha);

            // 빠르게 감쇠
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(peakAlpha, 0f, t / duration);
                SetAlpha(a);
                yield return null;
            }

            SetAlpha(0f);
            _co = null;
        }

        private void SetAlpha(float a)
        {
            var c = flashImage.color;
            c.a = Mathf.Clamp01(a);
            flashImage.color = c;
        }
    }
}