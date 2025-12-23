using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door;
using Unity.Cinemachine;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Manager
{
    public class DoorManager : MonoSingleton<DoorManager>
    {
        [Header("Position Data")]
        public Vector3 lastWorldPosition; 

        public void EnterRoom(GameObject player, int roomIndex)
        {
            FadeManager.Instance.FadeIn(() => 
            {
                lastWorldPosition = player.transform.position;
                RoomSpawnPoints points = FindFirstObjectByType<RoomSpawnPoints>();
                
                if (points != null && points.rooms != null && roomIndex < points.rooms.Length)
                {
                    var roomData = points.rooms[roomIndex];

                    if (roomData.spawnPoint != null)
                        TeleportPlayer(player, roomData.spawnPoint.position);

                    // [수정] CameraBoundManager에게 요청
                    if (roomData.cameraBound != null)
                    {
                        CameraBoundManager.Instance.ChangeCameraBound(roomData.cameraBound);
                    }
                }
                // ... 에러 로그 생략 ...
                
                FadeManager.Instance.FadeOut();
            });
        }

        public void ExitRoom(GameObject player)
        {
            FadeManager.Instance.FadeIn(() =>
            {
                TeleportPlayer(player, lastWorldPosition);

                RoomSpawnPoints points = FindFirstObjectByType<RoomSpawnPoints>();
                // [수정] 나올 때도 CameraBoundManager에게 요청
                if (points != null && points.defaultCameraBound != null)
                {
                    CameraBoundManager.Instance.ChangeCameraBound(points.defaultCameraBound);
                }

                FadeManager.Instance.FadeOut();
            });
        }

        private void TeleportPlayer(GameObject player, Vector3 targetPos)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            bool wasSimulated = false;

            if (rb != null)
            {
                wasSimulated = rb.simulated;
                rb.simulated = false; 
                rb.linearVelocity = Vector2.zero; 
            }

            // 1. 이동하기 전 위치와 이동할 위치의 차이(Delta)를 구합니다.
            Vector3 deltaPos = targetPos - player.transform.position;

            // 2. 플레이어 실제 이동
            player.transform.position = targetPos;

            // 3. [핵심] 시네머신에게 "이 물체가 순간이동 했다"고 알림
            // 이렇게 하면 카메라가 중간 과정을 생략하고 즉시 목표 지점으로 '컷(Cut)' 됩니다.
            CinemachineCore.OnTargetObjectWarped(player.transform, deltaPos);

            if (rb != null) rb.simulated = wasSimulated; 
        }
    }
}