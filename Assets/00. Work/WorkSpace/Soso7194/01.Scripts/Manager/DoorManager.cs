using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Manager
{
    public class DoorManager : MonoSingleton<DoorManager>
    {
        [Header("Data Storage")]
        public Vector3 lastWorldPosition; // 되돌아올 위치
        public string lastWorldSceneName; // 되돌아올 씬 이름
        public int targetRoomIndex = -1; // 이동할 방 번호
        public bool isReturning = false; // 복귀 중인지 여부

        protected override void Awake()
        {
            base.Awake(); // 부모의 중복 체크 로직 실행
 
            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable()
        {
            if (Instance == this) 
                SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            if (Instance == this)
              SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬 로드가 완료되면 호출
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            // 1. 방으로 들어갈 때
            if (targetRoomIndex != -1 && !isReturning)
            {
                TeleportToRoomIndex(player);
            }
            // 2. 밖으로 나갈 때
            else if (isReturning)
            {
                TeleportToOriginalPosition(player);
            }
        }

        // 방 씬의 특정 인덱스로 이동
        void TeleportToRoomIndex(GameObject player)
        {
            // 방 씬에 배치된 RoomSpawnPoints 찾기
            RoomSpawnPoints points = FindFirstObjectByType<RoomSpawnPoints>();
        
            if (points != null && points.spawnPoints.Length > targetRoomIndex)
            {
                TeleportPlayer(player, points.spawnPoints[targetRoomIndex].position);
            }
        
            targetRoomIndex = -1; // 이동 후 초기화
        }

        // 원래 있던 월드 위치로 이동
        void TeleportToOriginalPosition(GameObject player)
        {
            TeleportPlayer(player, lastWorldPosition);
            isReturning = false;
        }

        // 2D 물리 간섭 방지 이동 로직
        void TeleportPlayer(GameObject player, Vector3 position)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            bool wasSimulated = false;

            if (rb != null)
            {
                wasSimulated = rb.simulated;
                rb.simulated = false; // 물리 연산 잠시 끄기
                rb.linearVelocity = Vector2.zero; // 이동 관성 제거 (Unity 6 이상, 구버전은 .velocity)
            }

            player.transform.position = position;

            if (rb != null)
            {
                rb.simulated = wasSimulated; // 복구
            }
        }
    }
}