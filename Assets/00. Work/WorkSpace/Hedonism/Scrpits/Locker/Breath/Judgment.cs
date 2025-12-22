using System;
using System.Collections;
using PBG_01_Breath;
using UnityEngine;

public class Judgment : MonoBehaviour
{
    private bool isCheack = false;
    private GameObject _note;
    private BreathCheck _breathCheck;
    [SerializeField] private BreathBar breathBar;

    void Awake()
    {
        _breathCheck = GetComponentInParent<BreathCheck>();
    }



    void Update()
    {
        if (isCheack)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Destroy(_note);
                isCheack = false;
                breathBar.swingSpeed += 50;
                StartCoroutine(NoteDelayCorutaine());
            }
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
