using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.ShutterCameraAis;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public class CameraDirector : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        private Transform _player;

        private void Awake()
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) _player = go.transform;
        }

        private void OnEnable()
        {
            Bus<NoiseAlarmRaisedEvent>.OnEvent += OnAlarmRaised;
        }

        private void OnDisable()
        {
            Bus<NoiseAlarmRaisedEvent>.OnEvent -= OnAlarmRaised;
        }

        private void OnAlarmRaised(NoiseAlarmRaisedEvent evt)
        {
            Bus<CameraGlobalSetPresenceEvent>.Raise(new CameraGlobalSetPresenceEvent(true, false));
            
            if (_player == null)
            {
                var go = GameObject.FindGameObjectWithTag(playerTag);
                if (go != null) _player = go.transform;
            }
            if (_player == null) return;

            ShutterCameraAI nearest = null;
            float best = float.PositiveInfinity;

            foreach (var cam in CameraRegistry.All)
            {
                if (cam == null) continue;
                float d = Vector2.Distance(cam.transform.position, _player.position);
                if (d < best)
                {
                    best = d;
                    nearest = cam;
                }
            }

            if (nearest != null)
                nearest.ForceHunt(_player);
        }
    }
}