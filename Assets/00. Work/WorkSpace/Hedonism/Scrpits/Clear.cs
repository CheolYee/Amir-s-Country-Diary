using _00._Work.Resources._02._Codes.Utils;
using _00._Work.Resources._04._Templates.FadeManager;
using UnityEngine;

public class Clear : MonoBehaviour
{
    public static Clear Instance;

    [SerializeField] private GameObject clearCanvas;

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
        clearCanvas.SetActive(false);
    }

    public void OnClear()
    {
        FadeManager.Instance.FadeInOut();
        Invoke("SetTrue", FadeManager.Instance.fadeDuration);
    }

    public void ReturnTitle()
    {
        Time.timeScale = 1f;
        FadeManager.Instance.FadeToSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetTrue()
    {
        clearCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
