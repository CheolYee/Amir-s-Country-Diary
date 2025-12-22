using PBG_01_Breath;
using UnityEngine;

namespace PBG_01_Locker
{
    public class EnterLocker : MonoBehaviour
    {
        //[SerializeField] private float BreathProbability = 40f;
        [SerializeField] private bool isPlayer = false;
        [SerializeField] private bool inLocker = false;
        [SerializeField] private GameObject BreathCheack;
        [SerializeField] private GameObject player;

        [SerializeField] private BreathBar breathBar;
        [SerializeField] private Judgment judgment;

        //private float GimicOpen;


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isPlayer)
                {
                    if (!inLocker)
                    {
                        InLockerFalse();
                    }
                    else
                    {
                        InLockerTrue();
                    }
                }
            }
        }

        private void InLockerTrue()
        {
            inLocker = false;
            isPlayer = false;
            player.SetActive(true);
            BreathCheack.SetActive(false);
        }

        private void InLockerFalse()
        {
            isPlayer = true;
            inLocker = true;
            ShowGimic();
            player.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isPlayer = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if(!inLocker)
                    isPlayer = false;
            }
        }

        private void ShowGimic()
        {
            if (Random.value < 0.4f)
            {
                BreathCheack.SetActive(true);
                BreathCheack.GetComponentInChildren<BreathCheck>().SpawnHitZone();

            }
        }

        private void ShowUI()
        {

        }

    }
}
