using _00._Work.Resources._04._Templates.FadeManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _00._Work.Resources._02._Codes
{
    public class MenuUIBtn : MonoBehaviour
    {
        [SerializeField] private int mainMenuSceneIndex;
        [SerializeField] private GameObject menu;
        [SerializeField] private Button mainButton;
        [SerializeField] private bool exitToMainMenu = true; 
        public Slider bgmSlider;
        public Slider sfxSlider;
        
        private bool _isPressedEsc;

        private void Start()
        {
            bgmSlider.value = SoundManager.Instance.GetBGMVolume();
            sfxSlider.value = SoundManager.Instance.GetSfxVolume();

            bgmSlider.onValueChanged.AddListener((v) => SoundManager.Instance.SetBgmVolume(v));
            sfxSlider.onValueChanged.AddListener((v) => SoundManager.Instance.SetSfxVolume(v));
            
            menu.SetActive(false);
            _isPressedEsc = false;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (_isPressedEsc)
                {
                    ContinueButton();
                }
                else
                {
                    MainMenu();
                }
            }
        }

        public void MainMenu()
        {
            _isPressedEsc = true;

            menu.SetActive(true);

            Time.timeScale = 0f;
        }
        
        public void ContinueButton()
        {
            _isPressedEsc = false;
            
            Time.timeScale = 1f;

            if (mainButton != null)
                mainButton.gameObject.SetActive(true);

            menu.SetActive(false);
        }

        public void ExitButton()
        {
            if (!exitToMainMenu)
            {
                Time.timeScale = 1f;
                FadeManager.Instance?.FadeToSceneAsync(mainMenuSceneIndex);
                return;
            }
            Time.timeScale = 1f;
            SoundManager.Instance.PlayBgm(BgmId.Normal);

            FadeManager.Instance?.FadeToSceneAsync(mainMenuSceneIndex);
        }
    }
}
