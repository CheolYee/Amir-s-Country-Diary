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
    }
}
