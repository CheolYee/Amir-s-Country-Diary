using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public readonly struct StaminaChangedEvent : IEvent
    {
        public readonly float Current;
        public readonly float Max;

        public float Normalized => Max <= 0f ? 0f : Current / Max;

        public StaminaChangedEvent(float current, float max)
        {
            Current = current;
            Max = max;
        }
    }

    //스테미나 소모
    public readonly struct StaminaConsumeEvent : IEvent
    {
        public readonly float Amount;
        public StaminaConsumeEvent(float amount) => Amount = amount;
    }

    //달리기 드레인
    public readonly struct StaminaRunDrainToggleEvent : IEvent
    {
        public readonly bool IsOn;
        public StaminaRunDrainToggleEvent(bool isOn) => IsOn = isOn;
    }

    //0 도달 알림
    public readonly struct StaminaDepletedEvent : IEvent { }
}
