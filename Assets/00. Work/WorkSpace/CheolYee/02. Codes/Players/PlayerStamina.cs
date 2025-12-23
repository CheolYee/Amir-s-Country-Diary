using System;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players
{
    public class PlayerStamina : MonoBehaviour, IAgentComponent, IAfterInitialize
    {
        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float regenPerSec = 12f;
        [SerializeField] private float regenDelayAfterUse = 0.6f;

        [Header("Run Drain")]
        [SerializeField] private float runDrainPerSec = 18f;
        
        [Header("Run Gate")]
        [SerializeField] private float minStaminaToStartRun = 25f;
        
        [Header("Depleted Penalty")]
        [SerializeField] private float regenLockSecAfterDepleted = 2.0f;
        
        public float Current => _current;
        public float Max => maxStamina;
        
        private float _regenBlockedUntil;
        public bool IsRegenBlocked => Time.time < _regenBlockedUntil;
        public bool CanRun => _current > 0f;
        public bool CanStartRun => _current >= minStaminaToStartRun;

        private float _current;
        private bool _runDraining;
        private float _lastUseTime;

        private bool _wasDepleted;
        public void Initialize(Agent agent) {}

        public void Afterinitialize()
        {
            _current = maxStamina;
            _lastUseTime = -999f;
            _wasDepleted = false;
            _regenBlockedUntil = 0f;
        }

        private void Start()
        {
            PublishChanged();
        }

        private void OnEnable()
        {
            Bus<StaminaConsumeEvent>.OnEvent += OnConsume;
            Bus<StaminaRunDrainToggleEvent>.OnEvent += OnRunDrainToggle;
        }

        private void OnDisable()
        {
            Bus<StaminaConsumeEvent>.OnEvent -= OnConsume;
            Bus<StaminaRunDrainToggleEvent>.OnEvent -= OnRunDrainToggle;
        }
        
        private void Update()
        {
            float dt = Time.deltaTime;

            bool usedThisFrame = false;

            if (_runDraining && _current > 0f)
            {
                usedThisFrame = true;
                Spend(runDrainPerSec * dt);
            }

            // 사용하지 않으면 딜레이 후 회복
            if (!usedThisFrame)
            {
                if (Time.time >= _regenBlockedUntil &&
                    Time.time - _lastUseTime >= regenDelayAfterUse &&
                    _current < maxStamina)
                {
                    float prev = _current;
                    _current = Mathf.Min(maxStamina, _current + regenPerSec * dt);
                    if (!Mathf.Approximately(prev, _current))
                        PublishChanged();
                }
            }

            //0 도달 이벤트
            if (_current <= 0f && !_wasDepleted)
            {
                _wasDepleted = true;

                _regenBlockedUntil = Time.time + regenLockSecAfterDepleted;
                Bus<StaminaDepletedEvent>.Raise(new StaminaDepletedEvent());
            }
            else if (_current > 0f)
            {
                _wasDepleted = false;
            }
        }

        private void OnConsume(StaminaConsumeEvent evt)
        {
            if (evt.Amount <= 0f) return;
            Spend(evt.Amount);
        }

        private void OnRunDrainToggle(StaminaRunDrainToggleEvent evt)
        {
            _runDraining = evt.IsOn && _current > 0f;
        }
        
        public void BlockRegenIndefinitely()
        {
            _regenBlockedUntil = float.PositiveInfinity;
        }
        
        public void UnblockRegen()
        {
            _regenBlockedUntil = 0f;
        }

        private void Spend(float amount)
        {
            if (amount <= 0f) return;

            _lastUseTime = Time.time;

            float prev = _current;
            _current = Mathf.Max(0f, _current - amount);

            if (!Mathf.Approximately(prev, _current))
                PublishChanged();

            //0 되면 달리기 드레인 자동 종료
            if (_current <= 0f)
                _runDraining = false;
        }

        private void PublishChanged()
        {
            Bus<StaminaChangedEvent>.Raise(new StaminaChangedEvent(_current, maxStamina));
        }
    }
}