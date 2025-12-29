using System;
using _00._Work.Resources._02._Codes;
using _00._Work.Resources._04._Templates.FadeManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class TitleUI : MonoBehaviour
    {

        public void StartGame()
        {
            FadeManager.Instance.FadeToSceneAsync(1);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}