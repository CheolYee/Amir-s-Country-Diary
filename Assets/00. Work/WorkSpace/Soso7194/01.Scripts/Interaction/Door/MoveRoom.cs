using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Manager;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door
{
    public class MoveRoom : MonoBehaviour
    {
        [Header("Settings")]
        public InputSo inputSo; // InputSO 연결
        public enum DoorType { Enter, Exit }
        public DoorType type;

        [Header("If Enter Type")]
        public int roomIndexToGo = 0; // 이동할 방 번호 (RoomSpawnPoints 기준)

        private bool isPlayerNearby = false;
        private GameObject playerObject;

        private void OnEnable()
        {
            if (inputSo != null)
                inputSo.OnInteractionKeyPressed += HandleInteraction;
        }

        private void OnDisable()
        {
            if (inputSo != null)
                inputSo.OnInteractionKeyPressed -= HandleInteraction;
        }

        private void HandleInteraction()
        {
            // 플레이어가 근처에 있고, 플레이어 오브젝트를 찾았을 때만 실행
            if (isPlayerNearby && playerObject != null)
            {
                if (type == DoorType.Enter)
                {
                    DoorManager.Instance.EnterRoom(playerObject, roomIndexToGo);
                }
                else
                {
                    DoorManager.Instance.ExitRoom(playerObject);
                }
            }
        }

        // 물리 충돌 감지
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
                playerObject = other.gameObject; // 플레이어 캐싱
                Debug.Log("문 근처: 상호작용 가능");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
                playerObject = null;
            }
        }
    }
}