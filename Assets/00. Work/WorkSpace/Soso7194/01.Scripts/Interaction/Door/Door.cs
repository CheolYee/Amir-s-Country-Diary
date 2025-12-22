using _00._Work.Resources._02._Codes.Utils;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    public InputSo inputSo; // InputSO 연결
    public enum DoorType { Enter, Exit }
    public DoorType type;

    [Header("If Enter Type")]
    public string roomSceneName = "RoomScene"; // 이동할 씬 이름
    public int roomIndexToGo = 0; // 몇 번째 방으로 갈지

    private bool isPlayerNearby = false;

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

    // F키(InputSO 이벤트)가 눌리면 실행
    private void HandleInteraction()
    {
        if (isPlayerNearby)
        {
            MoveScene();
        }
    }

    void MoveScene()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (type == DoorType.Enter)
        {
            // 데이터 저장 (싱글톤 접근)
            DoorManager.Instance.lastWorldPosition = player.transform.position;
            DoorManager.Instance.lastWorldSceneName = SceneManager.GetActiveScene().name;
            DoorManager.Instance.targetRoomIndex = roomIndexToGo;
            DoorManager.Instance.isReturning = false;

            SceneManager.LoadScene(roomSceneName);
        }
        else if (type == DoorType.Exit)
        {
            // 복귀 모드 설정
            DoorManager.Instance.isReturning = true;
            DoorManager.Instance.targetRoomIndex = -1;

            SceneManager.LoadScene(DoorManager.Instance.lastWorldSceneName);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player is nearby!");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is nearby!");
            isPlayerNearby = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is not nearby!");
            isPlayerNearby = false;
        }
    }
}