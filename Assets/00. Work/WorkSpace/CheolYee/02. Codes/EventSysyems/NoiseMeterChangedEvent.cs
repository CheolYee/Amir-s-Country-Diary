namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public struct NoiseMeterChangedEvent : IEvent
    {
        public readonly float Normalized;
        public NoiseMeterChangedEvent(float normalized) => Normalized = normalized;
    }
    
    public readonly struct NoiseAlarmRaisedEvent : IEvent { }
    public readonly struct NoiseAlarmClearedEvent : IEvent { }
}