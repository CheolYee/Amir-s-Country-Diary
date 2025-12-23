namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public readonly struct CameraGlobalSetPresenceEvent : IEvent
    {
        public readonly bool Present;   // true = 등장/활성, false = 잠복/비활성
        public readonly bool Hide;      // Present=false일 때 렌더/콜라이더 숨길지

        public CameraGlobalSetPresenceEvent(bool present, bool hide)
        {
            Present = present;
            Hide = hide;
        }
    }
}