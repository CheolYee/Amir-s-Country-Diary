using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Manager
{
    public class DoorManager : MonoSingleton<DoorManager>
    {
        [Header("Position Data")]
        public Vector3 lastWorldPosition; // 원래 있던 위치 저장

        // 1. 문으로 들어갈 때 호출
        public void EnterRoom(GameObject player, int roomIndex)
        {
            // 페이드 매니저에게 "어두워지면(콜백) 이동시켜줘"라고 요청
            FadeManager.Instance.FadeIn(() => 
            {
                // 1. 현재 위치(밖) 저장
                lastWorldPosition = player.transform.position;

                // 2. 방 위치 찾기
                RoomSpawnPoints points = FindFirstObjectByType<RoomSpawnPoints>();
                if (points != null && points.spawnPoints.Length > roomIndex)
                {
                    TeleportPlayer(player, points.spawnPoints[roomIndex].position);
                }
                else
                {
                    Debug.LogError($"방 인덱스 {roomIndex}를 찾을 수 없습니다.");
                }

                // 3. 이동 끝났으니 화면 밝히기
                FadeManager.Instance.FadeOut();
            });
        }

        // 2. 문에서 나올 때 호출
        public void ExitRoom(GameObject player)
        {
            FadeManager.Instance.FadeIn(() =>
            {
                // 1. 저장해둔 원래 위치로 이동
                TeleportPlayer(player, lastWorldPosition);

                // 2. 화면 밝히기
                FadeManager.Instance.FadeOut();
            });
        }

        // 물리 오류 방지 텔레포트 함수
        private void TeleportPlayer(GameObject player, Vector3 targetPos)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            bool wasSimulated = false;

            if (rb != null)
            {
                wasSimulated = rb.simulated;
                rb.simulated = false; // 물리 연산 끄기
                rb.linearVelocity = Vector2.zero; // 관성 제거
            }

            player.transform.position = targetPos;

            if (rb != null)
            {
                rb.simulated = wasSimulated; // 복구
            }
        }
    }
}