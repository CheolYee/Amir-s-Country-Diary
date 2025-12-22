using Unity.VisualScripting;
using UnityEngine;

namespace PBG_01_LockPick
{
    public class Pin : MonoBehaviour
    {
        [SerializeField] private Transform pinTransform;

        void Update()
        {
            transform.position = pinTransform.transform.position;
        }
    }
}
