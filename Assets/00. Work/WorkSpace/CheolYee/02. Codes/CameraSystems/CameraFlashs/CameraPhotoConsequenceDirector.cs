using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class CameraPhotoConsequenceDirector : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private PlayerFloorTracker floorTracker; // 없으면 null 가능

        [Header("Flash")]
        [SerializeField] private float flashDuration = 0.12f;
        [Range(0f, 1f)]
        [SerializeField] private float flashPeakAlpha = 0.85f;

        [Header("Alert Text")]
        [SerializeField] private float alertDuration = 2.0f;
        [TextArea]
        [SerializeField] private string[] alertTexts =
        {
            "누군가가 쫓아오는 소리가 들립니다.",
            "불길한 기척이 느껴집니다.",
            "무언가가 이 층에 나타났습니다."
        };

        [Header("Boss")]
        [SerializeField] private bool spawnBoss = true;

        private Transform _player;

        private void Awake()
        {
            TryCachePlayer();
        }

        private void OnEnable()
        {
            Bus<PhotoTakenEvent>.OnEvent += OnPhotoTaken;
        }

        private void OnDisable()
        {
            Bus<PhotoTakenEvent>.OnEvent -= OnPhotoTaken;
        }

        private void TryCachePlayer()
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) _player = go.transform;
        }

        private void OnPhotoTaken(PhotoTakenEvent evt)
        {
            if (_player == null) TryCachePlayer();

            // 1) 플래시
            Bus<PhotoFlashEvent>.Raise(new PhotoFlashEvent(flashDuration, flashPeakAlpha));

            // 2) 빨간 경고문
            string msg = (alertTexts != null && alertTexts.Length > 0)
                ? alertTexts[Random.Range(0, alertTexts.Length)]
                : "위험한 기척이 느껴집니다.";

            Bus<AlertMessageEvent>.Raise(new AlertMessageEvent(msg, alertDuration));

            // 3) 보스 스폰 요청
            if (!spawnBoss) return;

            int floorId = floorTracker != null ? floorTracker.CurrentFloor : 0;
            Bus<BossSpawnRequestEvent>.Raise(new BossSpawnRequestEvent(floorId, evt.PlayerPos));
        }
    }
}