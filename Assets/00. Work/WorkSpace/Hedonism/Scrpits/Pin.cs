using Unity.VisualScripting;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] private Transform pinTransform;

    void Start()
    {

    }

    void Update()
    {
        transform.position = pinTransform.transform.position;

        LookPoint();

    }

    private void LookPoint()
    {
        Vector2 pos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = pos - (Vector2)transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }
}
