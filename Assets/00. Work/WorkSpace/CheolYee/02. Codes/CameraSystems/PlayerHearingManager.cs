using System;
using System.Collections.Generic;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public class PlayerHearingManager : MonoBehaviour
    {
        [Header("Listener")]
        [SerializeField] private Transform listener; // 비워두면 자기 transform

        [Header("Noise Meter (UI Accumulation)")]
        [Range(0f, 1f)]
        [SerializeField] private float noiseMeter;

        [Tooltip("펄스 1회당 누적 기본 계수(heard 값에 곱해짐).")]
        [SerializeField] private float uiPulseGain = 0.22f;

        [Range(0f, 1f)]
        [SerializeField] private float alarmThreshold = 0.8f;

        [Range(0f, 1f)]
        [SerializeField] private float alarmClearThreshold = 0.75f; // 히스테리시스(깜빡임 방지)

        [Header("Optional: Direct UI Binding")]
        [SerializeField] private Image soundFillImage;
        [SerializeField] private float uiSmooth = 15f;

        [Header("Managed Volume Update")]
        [SerializeField] private float volumeSmooth = 18f; // 볼륨 Lerp 강도(클수록 빠르게 따라감)
        
        [Header("Noise Meter Decay Control")]
        [SerializeField] private float decayDelayAfterNoise = 1.2f; // 마지막 소리 이후 이 시간 동안은 감소 X
        [SerializeField] private float decayPerSecBase = 0.05f;     // 기본 감소(지금 0.12는 빠른 편)
        [SerializeField] private float decayPerSecWhileLoud = 0.02f; // (선택) 소리가 계속 들어올 때 더 천천히

        private float _lastNoiseTime;
        private bool _heardThisFrame;

        private bool _alarmOn;
        private float _uiTarget;

        private readonly Dictionary<int, float> _volumeCache = new(); // emitterId -> smoothed volume

        private void Awake()
        {
            if (listener == null)
                listener = transform;

            _uiTarget = noiseMeter;
            PublishMeter();
        }

        private void OnEnable()
        {
            Bus<NoiseEmittedEvent>.OnEvent += OnNoiseEmitted;
        }

        private void OnDisable()
        {
            Bus<NoiseEmittedEvent>.OnEvent -= OnNoiseEmitted;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // 프레임 시작
            _heardThisFrame = false;

            // (중요) 이벤트는 같은 프레임에 들어올 수 있으니,
            // 감쇠는 Update 후반에 처리하는 게 안전함.
            // => 그래서 아래 감쇠는 Update 맨 마지막으로 두는 걸 추천

            if (soundFillImage != null)
            {
                soundFillImage.fillAmount = Mathf.Lerp(
                    soundFillImage.fillAmount,
                    _uiTarget,
                    1f - Mathf.Exp(-uiSmooth * dt)
                );
            }

            UpdateAllEmitterVolumes(dt);
            ApplyNoiseDecay(dt);
        }
        
        private void ApplyNoiseDecay(float dt)
        {
            if (noiseMeter <= 0f) return;

            // 마지막 소리 이후 일정 시간은 감소 금지
            if (Time.time - _lastNoiseTime < decayDelayAfterNoise)
                return;

            float decay = _heardThisFrame ? decayPerSecWhileLoud : decayPerSecBase;

            float prev = noiseMeter;
            noiseMeter = Mathf.Max(0f, noiseMeter - decay * dt);

            if (!Mathf.Approximately(prev, noiseMeter))
            {
                _uiTarget = noiseMeter;
                PublishMeter();
                UpdateAlarmState();
            }
        }

        private void OnNoiseEmitted(NoiseEmittedEvent evt)
        {
            if (evt.Preset == null) return;

            _lastNoiseTime = Time.time;
            _heardThisFrame = true;

            Vector2 listenerPos = listener.position;
            float heard = ComputeHeard01(listenerPos, evt.Position, evt.Preset);

            float add = heard * uiPulseGain * evt.Preset.uiGainMultiplier;
            if (add > 0f)
            {
                float prev = noiseMeter;
                noiseMeter = Mathf.Clamp01(noiseMeter + add);

                if (!Mathf.Approximately(prev, noiseMeter))
                {
                    _uiTarget = noiseMeter;
                    PublishMeter();
                    UpdateAlarmState();
                }
            }

            if (NoiseEmitterRegistry.TryGet(evt.SourceId, out var emitter) && emitter != null && emitter.UseManagedVolume)
            {
                emitter.SetManagedVolume(heard);
                _volumeCache[evt.SourceId] = heard;
            }
        }


        private void UpdateAllEmitterVolumes(float dt)
        {
            Vector2 listenerPos = listener.position;

            foreach (var emitter in NoiseEmitterRegistry.All)
            {
                if (emitter == null || !emitter.UseManagedVolume) continue;

                var preset = emitter.Preset;
                var audioSource = emitter.Audio;
                if (preset == null || audioSource == null)
                    continue;

                // 재생 중이 아니면 볼륨 0으로
                if (!audioSource.isPlaying)
                {
                    emitter.SetManagedVolume(0f);
                    _volumeCache[emitter.SourceId] = 0f;
                    continue;
                }

                float target = ComputeHeard01(listenerPos, emitter.transform.position, preset);

                _volumeCache.TryGetValue(emitter.SourceId, out var current);

                float next = Mathf.Lerp(current, target, 1f - Mathf.Exp(-volumeSmooth * dt));
                _volumeCache[emitter.SourceId] = next;

                emitter.SetManagedVolume(next);
            }
        }

        /// <summary>
        /// heard = L0 * 1/(1+(d/H)^2), d>R면 0
        /// </summary>
        private float ComputeHeard01(Vector2 listenerPos, Vector2 sourcePos, NoisePresetSo preset)
        {
            if (preset == null) return 0f;

            float r = Mathf.Max(0f, preset.maxRange);
            float h = Mathf.Max(0.01f, preset.halfDistance);

            Vector2 diff = sourcePos - listenerPos;
            float d2 = diff.sqrMagnitude;

            if (r > 0f && d2 > r * r)
                return 0f;

            float d = Mathf.Sqrt(d2);
            float x = d / h;
            float atten = 1f / (1f + x * x);
            
            
            float heard = preset.loudness * atten;
            return Mathf.Clamp01(heard);
        }

        private void PublishMeter()
        {
            Bus<NoiseMeterChangedEvent>.Raise(new NoiseMeterChangedEvent(noiseMeter));
        }

        private void UpdateAlarmState()
        {
            if (!_alarmOn && noiseMeter >= alarmThreshold)
            {
                _alarmOn = true;
                Bus<NoiseAlarmRaisedEvent>.Raise(new NoiseAlarmRaisedEvent());
            }
            else if (_alarmOn && noiseMeter <= alarmClearThreshold)
            {
                _alarmOn = false;
                Bus<NoiseAlarmClearedEvent>.Raise(new NoiseAlarmClearedEvent());
            }
        }
        
        [ContextMenu("TEST/NoiseMeter +10%")]
        private void TEST_Add10()
        {
            SetNoiseMeter(Mathf.Clamp01(noiseMeter + 0.10f));
        }

        [ContextMenu("TEST/NoiseMeter +30%")]
        private void TEST_Add30()
        {
            SetNoiseMeter(Mathf.Clamp01(noiseMeter + 0.30f));
        }

        [ContextMenu("TEST/NoiseMeter Reset")]
        private void TEST_Reset()
        {
            SetNoiseMeter(0f);
        }

        private void SetNoiseMeter(float value01)
        {
            noiseMeter = value01;
            _uiTarget = noiseMeter;
            PublishMeter();
            UpdateAlarmState();
        }
        
    }
}