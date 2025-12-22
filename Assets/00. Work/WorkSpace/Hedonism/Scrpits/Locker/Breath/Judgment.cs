using System;
using System.Collections;
using PBG_01_Breath;
using UnityEditor.Build.Reporting;
using UnityEngine;

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

    void Start()
    {
        
    }



    void Update()
    {
        if (isCheack)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Destroy(_note);
                isCheack = false;
                success++;
                breathBar.swingSpeed += 50;
                StartCoroutine(NoteDelayCorutaine());
                Stop();
            }
        }
    }

    private void Stop()
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
