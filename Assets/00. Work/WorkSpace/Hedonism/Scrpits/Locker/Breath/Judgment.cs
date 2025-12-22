using System.Collections;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using PBG_01_Breath;
using UnityEngine;


namespace PBG_01_Locker
{

    public class Judgment : MonoBehaviour
    {
        private bool isCheack = false;
        private GameObject _note;
        private BreathCheck _breathCheck;


        [SerializeField] public int touchCount = 0;
        [field: SerializeField] public int success { get; set; } = 0;
        [SerializeField] private BreathBar breathBar;
        [SerializeField] private GameObject breath;


        void Awake()
        {
            _breathCheck = GetComponentInParent<BreathCheck>();
        }

        void Update()
        {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (isCheack)
                        Tap();
                    else
                    {
                        Miss();
                    }
                }
        }

        private void Miss()
        {
            //플레이어의 스태미나가 크게 줄어들게
            Bus<StaminaConsumeEvent>.Raise(new StaminaConsumeEvent(30f));
            
        }

        private void Tap()
        {
            Destroy(_note);
            isCheack = false;
            success++;
            breathBar.swingSpeed += 50;
            StartCoroutine(NoteDelayCorutaine());
            Stop();
        }

        void OnEnable()
        {
            success = 0;
            touchCount = UnityEngine.Random.Range(3, 5);
        }


        public void Stop()
        {
            if (touchCount == success)
            {
                breath.SetActive(false);
                StopCoroutine(NoteDelayCorutaine());
            }
        }

        private IEnumerator NoteDelayCorutaine()
        {
            yield return new WaitForSeconds(1f);
            _breathCheck.SpawnHitZone();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Judgment"))
            {
                isCheack = true;
                _note = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Judgment"))
            {
                isCheack = false;
            }
        }
    }
}
