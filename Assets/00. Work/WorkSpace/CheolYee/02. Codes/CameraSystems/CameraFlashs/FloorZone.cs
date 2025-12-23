using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class FloorZone : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private int floorId = 0;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            Bus<FloorChangedEvent>.Raise(new FloorChangedEvent(floorId));
        }
    }
}