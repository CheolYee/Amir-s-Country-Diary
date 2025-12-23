namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public struct JumpScareEvent : IEvent
    {
        public readonly int BossId;

        public JumpScareEvent(int bossId)
        {
            BossId = bossId;
        }
    }
}