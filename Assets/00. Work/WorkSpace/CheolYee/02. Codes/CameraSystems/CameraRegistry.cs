using System.Collections.Generic;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.ShutterCameraAis;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public static class CameraRegistry
    {
        private static readonly Dictionary<int, ShutterCameraAI> Cameras = new();

        public static void Register(ShutterCameraAI cam)
        {
            if (cam == null) return;
            Cameras[cam.CameraId] = cam;
        }

        public static void Unregister(ShutterCameraAI cam)
        {
            if (cam == null) return;
            if (Cameras.TryGetValue(cam.CameraId, out var cur) && cur == cam)
                Cameras.Remove(cam.CameraId);
        }

        public static IEnumerable<ShutterCameraAI> All => Cameras.Values;
    }
}