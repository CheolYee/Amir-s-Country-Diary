using _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class BossSpawner : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private GameObject bossPrefab;

        private Transform _player;
        private Boss _currentBoss;

        private void Awake()
        {
            TryCachePlayer();
        }

        private void OnEnable()
        {
            Bus<BossSpawnRequestEvent>.OnEvent += OnSpawnRequested;
        }

        private void OnDisable()
        {
            Bus<BossSpawnRequestEvent>.OnEvent -= OnSpawnRequested;
        }

        private void TryCachePlayer()
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) _player = go.transform;
        }

        private void OnSpawnRequested(BossSpawnRequestEvent evt)
        {
            if (_player == null) TryCachePlayer();
            if (_player == null) return;
            if (bossPrefab == null) return;

            // 이미 살아있으면(원하면 재소환 정책 변경 가능)
            if (_currentBoss != null && _currentBoss.Health != null && !_currentBoss.Health.IsDead)
                return;

            if (!BossSpawnRegistry.TryGet(evt.FloorId, out var points) || points == null)
                return;

            Transform spawn = ChooseFarther(points.A, points.B, evt.PlayerPos);
            if (spawn == null) return;

            var go = Object.Instantiate(bossPrefab, spawn.position, Quaternion.identity);
            _currentBoss = go.GetComponent<Boss>();

            if (_currentBoss != null)
                _currentBoss.SetTarget(_player);
        }

        private Transform ChooseFarther(Transform a, Transform b, Vector2 playerPos)
        {
            if (a == null) return b;
            if (b == null) return a;

            float da = ((Vector2)a.position - playerPos).sqrMagnitude;
            float db = ((Vector2)b.position - playerPos).sqrMagnitude;

            return (da >= db) ? a : b;
        }
    }
}