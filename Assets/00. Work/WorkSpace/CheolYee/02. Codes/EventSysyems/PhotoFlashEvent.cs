using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public readonly struct PhotoFlashEvent : IEvent
    {
        public readonly float Duration;
        public readonly float PeakAlpha;

        public PhotoFlashEvent(float duration, float peakAlpha)
        {
            Duration = duration;
            PeakAlpha = peakAlpha;
        }
    }
    
    public readonly struct AlertMessageEvent : IEvent
    {
        public readonly string Text;
        public readonly float Duration;

        public AlertMessageEvent(string text, float duration)
        {
            Text = text;
            Duration = duration;
        }
    }

    public readonly struct BossSpawnRequestEvent : IEvent
    {
        public readonly int FloorId;
        public readonly Vector2 PlayerPos;

        public BossSpawnRequestEvent(int floorId, Vector2 playerPos)
        {
            FloorId = floorId;
            PlayerPos = playerPos;
        }
    }
    
    public readonly struct FloorChangedEvent : IEvent
    {
        public readonly int FloorId;

        public FloorChangedEvent(int floorId)
        {
            FloorId = floorId;
        }
    }
}