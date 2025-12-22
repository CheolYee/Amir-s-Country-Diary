using PBG_01_PUSE;
using UnityEngine;

public class Puse : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private GameObject puse;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isPlayer)
            {
                inventory.PuseCount.Add(puse);
                Debug.Log("퓨즈 획득");
                puse.SetActive(false);
                isPlayer = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Puse"))
        {
            puse = collision.gameObject;
            isPlayer = true;
        }
    }

    private void ShowUI()
    {

    }
}
