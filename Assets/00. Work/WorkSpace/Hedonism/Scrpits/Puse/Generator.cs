using System.Collections.Generic;
using UnityEngine;


namespace PBG_01_PUSE
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private List<GameObject> puse;
        [SerializeField] private bool isPlayer = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isPlayer)
                {
                    if (inventory.PuseCount.Count == 0)
                    {
                        Debug.Log("퓨즈 없음");
                        return;
                    }
                    else
                    {
                        SetPuse();
                        Debug.Log("퓨즈 끼움");
                        isPlayer = false;
                    }
                }
            }
        }


        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isPlayer = true;

            }
        }

        private void SetPuse()
        {
            this.puse.Add(inventory.PuseCount[0]);
            inventory.PuseCount.RemoveAt(0);
        }

        private void PuseIsNull()
        {

        }
    }
}
