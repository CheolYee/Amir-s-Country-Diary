using UnityEngine;

namespace _00._Work.Resources._02._Codes.Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>();
                if (_instance != null) return _instance;

                // ✅ 에디터에서(OnValidate 등) 자동 생성 금지
                if (!Application.isPlaying)
                    return null;

                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // ✅ Awake에서 중복 처리 + 인스턴스 확정
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
    }
}