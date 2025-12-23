using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class PlayerFloorTracker : MonoBehaviour
    {
        public int CurrentFloor { get; private set; }

        private void OnEnable() => Bus<FloorChangedEvent>.OnEvent += OnFloorChanged;
        private void OnDisable() => Bus<FloorChangedEvent>.OnEvent -= OnFloorChanged;

        private void OnFloorChanged(FloorChangedEvent evt)
        {
            CurrentFloor = evt.FloorId;
        }
    }
}