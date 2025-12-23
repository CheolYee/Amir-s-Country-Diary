using UnityEngine;

namespace _00._Work.Resources._02._Codes.Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
            _isQuitting = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HookQuit()
        {
            Application.quitting += () => _isQuitting = true;
        }

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;
                if (_instance != null) return _instance;

                // ✅ 찾기만 한다 (없으면 null)
                _instance = FindFirstObjectByType<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != (this as T))
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
    }
}