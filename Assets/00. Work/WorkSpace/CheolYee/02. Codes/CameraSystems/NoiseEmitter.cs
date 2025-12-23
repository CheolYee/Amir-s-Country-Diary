using System.Collections;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class NoiseEmitter : MonoBehaviour
    {
        [Header("Preset")]
        [SerializeField] private NoisePresetSo preset;

        [Header("Auto Play (Loop/Player용)")]
        [SerializeField] private bool playOnEnable;

        [Header("Audio Volume Control")]
        [Tooltip("매니저가 distance 계산해서 볼륨을 세팅한다면 키셈")]
        [SerializeField] private bool useManagedVolume = true;

        [Tooltip("프리셋 loudness와 별개로, 실제 AudioSource에 곱해질 기본 볼륨")]
        [Range(0f, 1f)]
        [SerializeField] private float baseAudioVolume = 1f;
        
        [SerializeField] private NoiseSourceKind noiseSourceKind = NoiseSourceKind.Player;

        public NoisePresetSo Preset => preset;
        public AudioSource Audio => _audio;
        public int SourceId => _sourceId;
        public bool IsEmitting => _isEmitting;
        public bool UseManagedVolume => useManagedVolume;

        private AudioSource _audio;
        private int _sourceId;
        private Coroutine _pulseRoutine;
        private bool _isEmitting;
        
        private int _clipCursor;
        private int _lastRandomIndex = -1;

        private void Awake()
        {
            _sourceId = GetInstanceID();
            _audio = GetComponent<AudioSource>();

            _audio.spatialBlend = 0f;
            _audio.playOnAwake = false;

            ApplyPresetToAudio();
            ApplyInitialVolume();
        }

        private void OnEnable()
        {
            NoiseEmitterRegistry.Register(this);
            
            if (playOnEnable)
                Begin(); //Loop/Player 타입이면 지속 펄스 시작
        }

        private void OnDisable()
        {
            NoiseEmitterRegistry.Unregister(this);
            
            End();
        }

        private void OnValidate()
        {
            if (_audio == null) _audio = GetComponent<AudioSource>();
            ApplyPresetToAudio();
            ApplyInitialVolume();
        }

        private void ApplyPresetToAudio()
        {
            if (_audio == null || preset == null) return;

            _audio.clip = PickClipForLoop();
            _audio.loop = preset.loop;
        }

        private void ApplyInitialVolume()
        {
            if (_audio == null) return;
            _audio.volume = useManagedVolume ? 0f : baseAudioVolume;
        }

        public void EmitOnce()
        {
            if (preset == null) return;

            var c = PickClipForOneShot();
            if (c != null)
            {
                // 매니저 볼륨 쓰더라도, 클립 재생은 해두고 볼륨은 매니저가 올려줄 수 있음
                _audio.loop = false;
                _audio.PlayOneShot(c, useManagedVolume ? 1f : baseAudioVolume);
            }

            RaiseNoiseEvent();
        }

        /// <summary>
        /// 지속(Loop/Player): 오디오 재생(옵션) + 펄스 이벤트를 주기적으로 발행
        /// (Alarm, Player Footstep Pulse 등)
        /// </summary>
        public void Begin()
        {
            if (preset == null) return;

            _isEmitting = true;

            var c = PickClipForLoop();
            if (c != null)
            {
                _audio.clip = c;
                _audio.loop = preset.loop;

                if (!_audio.isPlaying)
                    _audio.Play();
            }

            StartPulseRoutine();
        }

        public void End()
        {
            _isEmitting = false;

            if (_pulseRoutine != null)
            {
                StopCoroutine(_pulseRoutine);
                _pulseRoutine = null;
            }

            if (_audio != null && _audio.isPlaying)
                _audio.Stop();
        }

        /// <summary>
        /// (매니저용) 거리 기반 계산 결과를 AudioSource에 적용
        /// </summary>
        public void SetManagedVolume(float volume01)
        {
            if (_audio == null) return;

            // baseAudioVolume까지 곱해줌(연출/디버깅용)
            _audio.volume = Mathf.Clamp01(volume01) * baseAudioVolume;
        }

        private void StartPulseRoutine()
        {
            if (_pulseRoutine != null)
            {
                StopCoroutine(_pulseRoutine);
                _pulseRoutine = null;
            }

            if (preset.type == NoiseType.Impulse)
                return;

            float interval = Mathf.Max(0.05f, preset.pulseInterval);
            _pulseRoutine = StartCoroutine(PulseLoop(interval));
        }

        private IEnumerator PulseLoop(float interval)
        {
            RaiseNoiseEvent();

            var wait = new WaitForSeconds(interval);
            while (_isEmitting && isActiveAndEnabled)
            {
                yield return wait;
                RaiseNoiseEvent();
            }
        }

        private void RaiseNoiseEvent()
        {
            if (preset == null) return;

            var evt = new NoiseEmittedEvent(
                sourceId: _sourceId,
                position: transform.position,
                preset: preset,
                kind: noiseSourceKind
            );

            Bus<NoiseEmittedEvent>.Raise(evt);
        }
        
        private AudioClip PickClipForOneShot()
        {
            if (preset == null || preset.ClipCount == 0) return null;

            if (preset.clipSelectMode == ClipSelectMode.Sequential)
            {
                var c = preset.GetClip(_clipCursor);
                _clipCursor = (_clipCursor + 1) % preset.ClipCount;
                return c;
            }

            // Random
            int count = preset.ClipCount;
            int idx = Random.Range(0, count);

            if (preset.avoidImmediateRepeat && count >= 2 && idx == _lastRandomIndex)
                idx = (idx + 1) % count;

            _lastRandomIndex = idx;
            return preset.GetClip(idx);
        }
        
        private AudioClip PickClipForLoop()
        {
            if (preset == null) return null;
            if (preset.ClipCount > 0) return preset.GetClip(0);
            return null;
        }
    }
}