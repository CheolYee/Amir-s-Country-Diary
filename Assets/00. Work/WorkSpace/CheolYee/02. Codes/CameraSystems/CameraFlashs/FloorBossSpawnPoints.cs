using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class FloorBossSpawnPoints : MonoBehaviour
    {
        public int FloorId => floorId;
        public Transform A => spawnA;
        public Transform B => spawnB;

        [SerializeField] private int floorId = 0;
        [SerializeField] private Transform spawnA;
        [SerializeField] private Transform spawnB;

        private void OnEnable() => BossSpawnRegistry.Register(this);
        private void OnDisable() => BossSpawnRegistry.Unregister(this);
    }
}