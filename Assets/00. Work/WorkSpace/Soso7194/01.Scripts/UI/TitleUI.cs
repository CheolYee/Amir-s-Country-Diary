using System;
using _00._Work.Resources._04._Templates.FadeManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] private Canvas settingCanvas;
        
        public void StartGame()
        {
            FadeManager.Instance.FadeToSceneAsync(2);
        }

        public void Setting()
        {
            settingCanvas.enabled = true;
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}