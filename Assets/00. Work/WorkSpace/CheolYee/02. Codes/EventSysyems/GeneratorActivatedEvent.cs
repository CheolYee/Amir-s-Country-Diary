namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public readonly struct GeneratorActivatedEvent : IEvent
    {
        public readonly int Floor;
        public GeneratorActivatedEvent(int floor) => Floor = floor;
    }
}