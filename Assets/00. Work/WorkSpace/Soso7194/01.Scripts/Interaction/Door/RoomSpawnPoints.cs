using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door
{
    public class RoomSpawnPoints : MonoBehaviour
    {
        // 인스펙터에서 데이터를 묶어서 보기 위해 구조체(Struct) 정의
        [System.Serializable] 
        public struct RoomData
        {
            public string roomName;         // (선택) 에디터에서 방 이름 적어두기 용도
            public Transform spawnPoint;    // 플레이어가 이동할 위치
            public Collider2D cameraBound;  // 이 방에서 사용할 카메라 경계
        }

        [Header("Default (Hallway/LivingRoom)")]
        public Collider2D defaultCameraBound; // 방에서 나왔을 때(거실 등) 적용될 기본 경계

        [Header("Rooms Configuration")]
        public RoomData[] rooms; // 각 방의 정보를 담는 배열
    }
}