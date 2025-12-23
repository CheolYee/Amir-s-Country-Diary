using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private Image tutorialImage;
        [SerializeField] private Sprite[] tutorialUI;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private string[] tutorialTextArray;
        [SerializeField] private int tutorialIndex;

        private void Start()
        {
            tutorialImage.sprite = tutorialUI[tutorialIndex];
            tutorialText.text = tutorialTextArray[tutorialIndex];
            tutorialPanel.SetActive(false);
        }

        public void HideTutorial()
        {
            tutorialPanel.SetActive(false);
        }
        
        public void ShowTutorial()
        {
            tutorialPanel.SetActive(true);
        }

        public void NextTutorial()
        {
            int index = tutorialIndex++;
            tutorialIndex = Mathf.Clamp(tutorialIndex, 0, tutorialUI.Length - 1);
            tutorialImage.sprite = tutorialUI[tutorialIndex];
            tutorialText.text = tutorialTextArray[tutorialIndex];
        }
        
        public void PrevTutorial()
        {
            int index = tutorialIndex--;
            tutorialIndex = Mathf.Clamp(tutorialIndex, 0, tutorialUI.Length - 1);
            tutorialImage.sprite = tutorialUI[tutorialIndex];
            tutorialText.text = tutorialTextArray[tutorialIndex];
        }
    }
}