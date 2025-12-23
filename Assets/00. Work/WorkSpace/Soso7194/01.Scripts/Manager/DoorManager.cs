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

        // [추가] 현재 페이드(장면 전환) 중인지 확인하는 변수
        private bool isTransitioning = false;

        public void EnterRoom(GameObject player, int roomIndex)
        {
            // [추가] 이미 전환 중이라면 함수 실행을 막음 (상호작용 무시)
            if (isTransitioning) return;

            // 전환 시작 -> 잠금
            isTransitioning = true;

            FadeManager.Instance.FadeIn(() => 
            {
                lastWorldPosition = player.transform.position;
                RoomSpawnPoints points = FindFirstObjectByType<RoomSpawnPoints>();
                
                if (points != null && points.rooms != null && roomIndex < points.rooms.Length)
                {
                    var roomData = points.rooms[roomIndex];

                    if (roomData.spawnPoint != null)
                        TeleportPlayer(player, roomData.spawnPoint.position);

                    if (roomData.cameraBound != null)
                    {
                        CameraBoundManager.Instance.ChangeCameraBound(roomData.cameraBound);
                    }
                }
                
                // [수정] FadeOut이 "완료된 후"에 잠금을 해제해야 안전합니다.
                // 람다 식을 사용하여 완료 콜백을 등록합니다.
                FadeManager.Instance.FadeOut(() => 
                {
                    isTransitioning = false; // 상호작용 잠금 해제
                });
            });
        }

        public void ExitRoom(GameObject player)
        {
            // [추가] 이미 전환 중이라면 함수 실행 막음
            if (isTransitioning) return;

            // 전환 시작 -> 잠금
            isTransitioning = true;

            FadeManager.Instance.FadeIn(() =>
            {
                TeleportPlayer(player, lastWorldPosition);

                RoomSpawnPoints points = FindFirstObjectByType<RoomSpawnPoints>();
                
                if (points != null && points.defaultCameraBound != null)
                {
                    CameraBoundManager.Instance.ChangeCameraBound(points.defaultCameraBound);
                }

                // [수정] FadeOut 완료 시 잠금 해제
                FadeManager.Instance.FadeOut(() => 
                {
                    isTransitioning = false; // 상호작용 잠금 해제
                });
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

            Vector3 deltaPos = targetPos - player.transform.position;
            player.transform.position = targetPos;
            CinemachineCore.OnTargetObjectWarped(player.transform, deltaPos);

            if (rb != null) rb.simulated = wasSimulated; 
        }
    }
}