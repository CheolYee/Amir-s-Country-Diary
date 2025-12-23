using System.Collections;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using TMPro;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class AlertMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject root;

        private Coroutine _co;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
        }

        private void OnEnable() => Bus<AlertMessageEvent>.OnEvent += OnAlert;
        private void OnDisable() => Bus<AlertMessageEvent>.OnEvent -= OnAlert;

        private void OnAlert(AlertMessageEvent evt)
        {
            if (text == null || root == null) return;

            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Routine(evt.Text, evt.Duration));
        }

        private IEnumerator Routine(string msg, float duration)
        {
            root.SetActive(true);
            text.text = msg;

            yield return new WaitForSeconds(duration);

            root.SetActive(false);
            _co = null;
        }
    }
}