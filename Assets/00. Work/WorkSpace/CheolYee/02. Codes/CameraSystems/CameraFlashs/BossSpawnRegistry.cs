using System.Collections.Generic;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public static class BossSpawnRegistry
    {
        private static readonly Dictionary<int, FloorBossSpawnPoints> Map = new();

        public static void Register(FloorBossSpawnPoints points)
        {
            if (points == null) return;
            Map[points.FloorId] = points;
        }

        public static void Unregister(FloorBossSpawnPoints points)
        {
            if (points == null) return;
            if (Map.TryGetValue(points.FloorId, out var cur) && cur == points)
                Map.Remove(points.FloorId);
        }

        public static bool TryGet(int floorId, out FloorBossSpawnPoints points)
            => Map.TryGetValue(floorId, out points);
    }
}