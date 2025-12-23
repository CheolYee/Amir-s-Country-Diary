using System;
using _00._Work.Resources._02._Codes;
using UnityEngine;
using UnityEngine.UIElements;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class SelectImageManager : MonoBehaviour
    {
        [SerializeField] private GameObject _image;

        public void Enter(GameObject owner)
        {
            _image.SetActive(true);
            _image.transform.position = owner.transform.position;
        }
        
        public void Exit() => _image.SetActive(false);
    }
}