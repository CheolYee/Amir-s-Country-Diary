using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public readonly struct PhotoTakenEvent : IEvent
    {
        public readonly int CameraId;
        public readonly Vector2 PlayerPos;

        public PhotoTakenEvent(int cameraId, Vector2 playerPos)
        {
            CameraId = cameraId;
            PlayerPos = playerPos;
        }
    }
}