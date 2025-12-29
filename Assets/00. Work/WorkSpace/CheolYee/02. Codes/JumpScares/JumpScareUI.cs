using System.Collections;
using _00._Work.Resources._02._Codes;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.JumpScares
{
    public class JumpScareUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image black;
        [SerializeField] private Image scareImage;
        [SerializeField] private Sprite[] scareSprites;

        [Header("Timing (Realtime)")]
        [SerializeField] private float fadeInSec = 0.03f;
        [SerializeField] private float flickerOnSec = 0.08f;
        [SerializeField] private float flickerOffSec = 0.04f;
        [SerializeField] private int flickerCount = 2;
        [SerializeField] private float holdSec = 0.6f;

        [Header("Punch Zoom")]
        [SerializeField] private float punchStartScale = 0.65f;  // 시작 작게
        [SerializeField] private float punchPeakScale = 1.55f;   // 확 커짐(오버)
        [SerializeField] private float punchInSec = 0.06f;       // 팍! 커지는 시간
        [SerializeField] private float punchSettleSec = 0.10f;   // 살짝 내려앉는 시간
        [SerializeField] private float punchAfterHoldSec = 0.10f;// 크게 나온 채 잠깐 유지

        [Header("Violent Shake (during punch/hold)")]
        [SerializeField] private float shakeDurationSec = 0.28f;
        [SerializeField] private float shakePosPx = 55f;     // 엄청 흔들기
        [SerializeField] private float shakeRotDeg = 10f;
        [SerializeField] private float shakeScaleAmp = 0.10f;
        [SerializeField] private float shakeFreq = 35f;      // 흔들림 주파수

        [Header("Game Control")]
        [SerializeField] private bool pauseTimeScale = true;
        [SerializeField] private bool raiseGameOverEvent = true;

        private Coroutine _co;
        private bool _running;

        private Vector2 _basePos;
        private Quaternion _baseRot;
        private Vector3 _baseScale;

        private float _seedA, _seedB, _seedC;

        private void Awake()
        {
            if (group == null) group = GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 0f;
                group.blocksRaycasts = false;
                group.interactable = false;
            }

            if (scareImage != null)
            {
                var rt = scareImage.rectTransform;
                _basePos = rt.anchoredPosition;
                _baseRot = rt.localRotation;
                _baseScale = rt.localScale;

                scareImage.enabled = false;
                scareImage.preserveAspect = true;
            }

            if (black != null) black.enabled = true;

            _seedA = Random.Range(0f, 1000f);
            _seedB = Random.Range(0f, 1000f);
            _seedC = Random.Range(0f, 1000f);
        }

        private void OnEnable() => Bus<JumpScareEvent>.OnEvent += OnJumpScare;
        private void OnDisable() => Bus<JumpScareEvent>.OnEvent -= OnJumpScare;

        private void OnJumpScare(JumpScareEvent evt)
        {
            if (_running) return;
            if (_co != null) StopCoroutine(_co);
            
            SoundManager.Instance?.PlaySfx(SfxId.JumpScare1, 0.8f);
            SoundManager.Instance?.PlaySfx(SfxId.JumpScare2, 0.8f);
            
            _co = StartCoroutine(RunRoutine());
        }

        private IEnumerator RunRoutine()
        {
            _running = true;

            float prevTimeScale = Time.timeScale;
            if (pauseTimeScale) Time.timeScale = 0f;

            // 스프라이트 세팅
            if (scareSprites is { Length: > 0 } && scareImage != null)
                scareImage.sprite = scareSprites[Random.Range(0, scareSprites.Length)];

            if (group != null)
            {
                group.blocksRaycasts = true;
                group.alpha = 0f;
            }

            // 검은 화면 빠르게 등장
            yield return FadeGroupRealtime(0f, 1f, fadeInSec);

            // ✅ 핵심: 팍 커지면서 미친 흔들림
            ShowScare(true);
            ResetTransform();
            yield return PunchZoomAndViolentShake();
            // 살짝 더 보여주기
            yield return new WaitForSecondsRealtime(punchAfterHoldSec);

            // 플리커(짧게)
            for (int i = 0; i < flickerCount; i++)
            {
                ShowScare(false);
                ResetTransform();
                yield return new WaitForSecondsRealtime(flickerOffSec);

                ShowScare(true);
                yield return MicroShake(flickerOnSec, shakePosPx * 0.35f, shakeRotDeg * 0.35f);
            }

            // 마지막 홀드(약간의 잔떨림)
            ShowScare(true);
            yield return MicroShake(holdSec, shakePosPx * 0.20f, shakeRotDeg * 0.20f);

            // 종료
            ShowScare(false);
            ResetTransform();

            if (raiseGameOverEvent)
                Bus<GameOverEvent>.Raise(new GameOverEvent());
            
            Die.Instance.OnDie();
            SoundManager.Instance.PlayBgm(BgmId.Normal);

            // 필요하면 UI도 같이 사라지게
            yield return FadeGroupRealtime(1f, 0f, 0.1f);

            if (group != null)
                group.blocksRaycasts = false;

            if (pauseTimeScale) Time.timeScale = prevTimeScale;

            _running = false;
            _co = null;
        }

        private IEnumerator PunchZoomAndViolentShake()
        {
            if (scareImage == null) yield break;

            var rt = scareImage.rectTransform;

            // 1) punch in (start -> peak)
            float t = 0f;
            while (t < punchInSec)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / punchInSec);

                // EaseOutBack 느낌(오버슈트)
                float eased = EaseOutBack(u);

                float s = Mathf.Lerp(punchStartScale, punchPeakScale, eased);
                ApplyShake(rt, 1f); // 초반 최대

                rt.localScale = _baseScale * s;
                yield return null;
            }

            // 2) settle (peak -> 1.0 근처)
            t = 0f;
            float settleFrom = punchPeakScale;
            float settleTo = 1.08f; // 살짝 크게 남겨두면 더 압박감
            while (t < punchSettleSec)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / punchSettleSec);

                float eased = EaseOutQuad(u);
                float s = Mathf.Lerp(settleFrom, settleTo, eased);

                // 흔들림 감쇠
                float amp = 1f - u;
                ApplyShake(rt, amp);

                rt.localScale = _baseScale * s;
                yield return null;
            }

            // 3) 남은 shake duration 만큼 더 떨고 끝
            float remain = Mathf.Max(0f, shakeDurationSec - (punchInSec + punchSettleSec));
            if (remain > 0f)
                yield return MicroShake(remain, shakePosPx, shakeRotDeg);

            ResetTransform();
            rt.localScale = _baseScale * 1.08f;
        }

        private IEnumerator MicroShake(float sec, float posAmp, float rotAmp)
        {
            if (scareImage == null) yield break;

            var rt = scareImage.rectTransform;
            float t = 0f;
            while (t < sec)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / sec);

                // 감쇠(끝으로 갈수록 줄어듦)
                float amp = 1f - u;
                Vector2 p = Noise2D(Time.unscaledTime * shakeFreq) * (posAmp * amp);
                float r = Noise1D(Time.unscaledTime * shakeFreq + 31.7f) * rotAmp * amp;

                rt.anchoredPosition = _basePos + p;
                rt.localRotation = Quaternion.Euler(0f, 0f, r);

                // 살짝 스케일 떨림
                float s = 1f + Noise1D(Time.unscaledTime * shakeFreq + 77.1f) * shakeScaleAmp * amp;
                rt.localScale = _baseScale * s;

                yield return null;
            }

            ResetTransform();
        }

        private void ApplyShake(RectTransform rt, float amp01)
        {
            // punch 구간: 매우 거칠게(프레임마다 랜덤+노이즈 혼합)
            float amp = amp01;

            Vector2 rand = Random.insideUnitCircle * (shakePosPx * 0.35f * amp);
            Vector2 noise = Noise2D(Time.unscaledTime * shakeFreq) * (shakePosPx * amp);

            float rot = (Random.Range(-1f, 1f) * 0.4f + Noise1D(Time.unscaledTime * shakeFreq + 12.3f)) * shakeRotDeg * amp;

            rt.anchoredPosition = _basePos + rand + noise;
            rt.localRotation = Quaternion.Euler(0f, 0f, rot);

            float s = 1f + (Random.Range(-1f, 1f) * 0.25f + Noise1D(Time.unscaledTime * shakeFreq + 99.9f)) * shakeScaleAmp * amp;
            rt.localScale = _baseScale * s;
        }

        private void ResetTransform()
        {
            if (scareImage == null) return;
            var rt = scareImage.rectTransform;
            rt.anchoredPosition = _basePos;
            rt.localRotation = _baseRot;
            rt.localScale = _baseScale;
        }

        private IEnumerator FadeGroupRealtime(float from, float to, float sec)
        {
            if (group == null) yield break;

            if (sec <= 0f)
            {
                group.alpha = to;
                yield break;
            }

            float t = 0f;
            while (t < sec)
            {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, t / sec);
                yield return null;
            }
            group.alpha = to;
        }

        private void ShowScare(bool on)
        {
            if (scareImage != null) scareImage.enabled = on;
        }

        // ---- easing / noise ----
        private static float EaseOutQuad(float x) => 1f - (1f - x) * (1f - x);

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }

        private float Noise1D(float t)
        {
            // -1 ~ 1
            return (Mathf.PerlinNoise(_seedA, t) - 0.5f) * 2f;
        }

        private Vector2 Noise2D(float t)
        {
            float x = (Mathf.PerlinNoise(_seedB, t) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(_seedC, t) - 0.5f) * 2f;
            return new Vector2(x, y);
        }
    }

    public readonly struct GameOverEvent : IEvent { }
}
