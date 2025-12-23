using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Door
{
    public class RandomSelect : MonoBehaviour
    {
        [SerializeField] private MoveRoom[] doors;
        [SerializeField] private int lockPickDoorCount = 2; 

        private void Awake()
        {
            if (doors == null || doors.Length == 0)
            {
                Debug.LogWarning("Doors array is not set or empty.");
                return;
            }

            // 1. Enter 타입인 문의 인덱스만 후보로 수집
            List<int> validIndices = new List<int>();
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].type == MoveRoom.DoorType.Enter)
                {
                    validIndices.Add(i);
                }
            }

            // 2. 랜덤 선택 (유효한 문 개수와 설정된 개수 중 작은 값만큼)
            List<int> selectedIndices = new List<int>();
            int count = Mathf.Min(lockPickDoorCount, validIndices.Count);
            
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, validIndices.Count);
                selectedIndices.Add(validIndices[randomIndex]);
                validIndices.RemoveAt(randomIndex);
            }

            // 3. 전체 문 초기화
            for (int i = 0; i < doors.Length; i++)
            {
                // 선택된 인덱스에 포함되어 있다면 true, 아니면 false
                // Exit 타입은 애초에 후보에 없었으므로 무조건 false가 전달됨
                bool isSelected = selectedIndices.Contains(i);
                doors[i].InitializeLock(isSelected);

                if (isSelected)
                {
                    Debug.Log($"Door {i} is Locked (LockPick Required).");
                }
            }
        }
    }
}