using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using UnityEngine;

public class Die : MonoBehaviour
{
    public static Die Instance;

    [SerializeField] private GameObject dieCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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

    public void SetTrue()
    {
        dieCanvas.SetActive(true);
    }


    private void SetFalse()
    {
        dieCanvas.SetActive(false);
    }
}
