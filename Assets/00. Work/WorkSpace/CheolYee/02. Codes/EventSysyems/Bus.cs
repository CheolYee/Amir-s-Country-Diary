namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public static class Bus<T> where T : IEvent
    {
        public delegate void Event(T evt);
        
        public static event Event OnEvent;
        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }
}