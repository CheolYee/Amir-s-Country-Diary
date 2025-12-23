using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using UnityEngine;

public class Die : MonoSingleton<Die>
{
    [SerializeField] private GameObject dieCanvas;

    // 플레이어가 죽었을 때 dieCanvas 활성화
    void Start()
    {
        dieCanvas.SetActive(false);
    }

    public void OnDie()
    {
        FadeManager.Instance.FadeInOut();
        Invoke("SetTrue", FadeManager.Instance.fadeDuration);
    }

    public void Revive()
    {
        FadeManager.Instance.FadeInOut();
        Invoke("SetFalse", FadeManager.Instance.fadeDuration);
    }

    public void QuitGame()
    {
        FadeManager.Instance.FadeToSceneAsync(0);
    }

    private void SetTrue()
    {
        dieCanvas.SetActive(true);
    }


    private void SetFalse()
    {
        dieCanvas.SetActive(false);
    }
}
