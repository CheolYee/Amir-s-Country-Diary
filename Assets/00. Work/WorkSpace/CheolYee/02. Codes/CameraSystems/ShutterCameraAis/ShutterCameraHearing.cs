using System.Collections.Generic;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.ShutterCameraAis
{
    public class ShutterCameraHearing : MonoBehaviour
    {
        [Header("Threshold / Timers")]
        [SerializeField] private float soundThreshold = 0.06f;
        [SerializeField] private float noSoundReturnSec = 5f;
        [SerializeField] private float switchDelaySec = 2f;
        [SerializeField] private float reacquireGapSec = 1f;
        [SerializeField] private float loopForgetSec = 1.2f;
        [SerializeField] private float impulseForgetSec = 6f;

        [Header("Investigate Speed")]
        [SerializeField] private float baseSpeed = 2.2f;
        [SerializeField] private float speedGain = 4.0f;

        private class Heard
        {
            public int ID;
            public Vector2 Pos;
            public float Score;
            public float FirstHeardTime;
            public float LastHeardTime;
            public NoisePresetSo Preset;
        }

        private readonly Dictionary<int, Heard> _map = new();
        private Heard _current;
        private Heard _candidate;
        private float _candidateSwitchTime;
        private float _lastMeaningfulTime = -999f;
        private bool _enabled = true;

        public bool HasTarget => _current != null;
        public Vector2 TargetPos => _current?.Pos ?? transform.position;
        public float TargetScore => _current?.Score ?? 0f;
        public float InvestigateSpeed => baseSpeed + TargetScore * speedGain;
        public bool ShouldReturnToPatrol => (Time.time - _lastMeaningfulTime) >= noSoundReturnSec && _current == null;

        private void OnEnable() => Bus<NoiseEmittedEvent>.OnEvent += OnNoise;
        private void OnDisable() => Bus<NoiseEmittedEvent>.OnEvent -= OnNoise;

        public void ClearTargets()
        {
            _current = null;
            _candidate = null;
        }
        
        public bool TryGetMove(out Vector2 targetPos, out float speed)
        {
            if (!HasTarget)
            {
                targetPos = default;
                speed = 0f;
                return false;
            }

            targetPos = TargetPos;
            speed = InvestigateSpeed;
            return true;
        }

        public void Tick()
        {
            if (!_enabled) { ClearTargets(); return; }
            if (_current != null && IsExpired(_current)) _current = null;
            if (_candidate != null && IsExpired(_candidate)) _candidate = null;

            if (_candidate != null && Time.time >= _candidateSwitchTime)
            {
                if (!IsExpired(_candidate) && _candidate.Score >= soundThreshold)
                    _current = _candidate;

                _candidate = null;
            }

            if (_current == null)
            {
                _current = GetBest();
            }
        }

        private void OnNoise(NoiseEmittedEvent evt)
        {
            if (evt.Preset == null) return;

            float score = ComputeScore(transform.position, evt.Position, evt.Preset);
            if (score < soundThreshold) return;

            _lastMeaningfulTime = Time.time;

            if (!_map.TryGetValue(evt.SourceId, out var h))
            {
                h = new Heard { ID = evt.SourceId, FirstHeardTime = Time.time };
                _map[evt.SourceId] = h;
            }
            else if (Time.time - h.LastHeardTime >= reacquireGapSec)
            {
                h.FirstHeardTime = Time.time;
            }

            h.Pos = evt.Position;
            h.Score = score;
            h.LastHeardTime = Time.time;
            h.Preset = evt.Preset;

            if (_current == null)
            {
                _current = h;
                return;
            }

            var best = GetBest();
            if (best != null && best.ID != _current.ID)
            {
                _candidate = best;
                _candidateSwitchTime = _candidate.FirstHeardTime + switchDelaySec;
            }
        }

        private Heard GetBest()
        {
            Heard best = null; float bestScore = 0f;

            foreach (var h in _map.Values)
            {
                if (IsExpired(h)) continue;
                if (h.Score > bestScore) { bestScore = h.Score; best = h; }
            }

            return bestScore >= soundThreshold ? best : null;
        }

        private bool IsExpired(Heard h)
        {
            if (h == null || h.Preset == null) return true;
            float forget = (h.Preset.type == NoiseType.Impulse) ? impulseForgetSec : loopForgetSec;
            return (Time.time - h.LastHeardTime) > forget;
        }

        private static float ComputeScore(Vector2 listenerPos, Vector2 sourcePos, NoisePresetSo p)
        {
            float r = Mathf.Max(0f, p.maxRange);
            float h = Mathf.Max(0.01f, p.halfDistance);

            Vector2 diff = sourcePos - listenerPos;
            float d2 = diff.sqrMagnitude;
            if (r > 0f && d2 > r * r) return 0f;

            float d = Mathf.Sqrt(d2);
            float x = d / h;
            float atten = 1f / (1f + x * x);
            return Mathf.Clamp01(p.loudness * atten * p.cameraGainMultiplier);
        }
        
        public void SetEnabled(bool camEnabled, bool clearTargets = false)
        {
            _enabled = camEnabled;
            if (clearTargets) ClearTargets();
        }

        public bool TryGetInvestigate(out Vector2 pos, out float speed)
        {
            pos = TargetPos;
            speed = InvestigateSpeed;
            return HasTarget;
        }
    }
}