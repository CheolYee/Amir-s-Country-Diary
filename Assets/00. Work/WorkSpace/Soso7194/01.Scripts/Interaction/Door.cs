using System;
using _00._Work.Resources._04._Templates.FadeManager;
using _00._Work.WorkSpace.Soso7194._01.Scripts.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction
{
    public class Door : MonoBehaviour, IInteraction
    {
        [SerializeField] private string indexNum;

        private void OnEnable()
        {
            
        }

        public void Interact()
        {
            Debug.Log("Door Interacted");
            FadeManager.Instance.FadeOut();
            PlayerPrefs.SetString("indexNum", indexNum);
            SceneManager.LoadScene("MoveScene");
        }
    }
}