using UnityEngine;

namespace PBG_01_Locker
{
    public class EnterLocker : MonoBehaviour
    {
        //[SerializeField] private float BreathProbability = 40f;
        [SerializeField] private bool isPlayer = false;
        [SerializeField] private bool inLocker = false;
        private GameObject player;
        void Update()
        {
            if (isPlayer)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!inLocker)
                    {
                        inLocker = true;
                        player.SetActive(false);
                    }
                    else
                    {
                        inLocker = false;
                        player.SetActive(true);
                    }

                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isPlayer = true;
                player = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isPlayer = false;
            }
        }

        private void ShowUI()
        {
            
        }

    }
}
