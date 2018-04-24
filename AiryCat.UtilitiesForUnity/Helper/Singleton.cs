using UnityEngine;

namespace AiryCat.UtilitiesForUnity.Helper
{
    public interface IDestroyableSingleton
    {
        bool CanDestroyed { get; }
    }

    public class Singleton<T> : MonoBehaviour where T : Singleton<T>,IDestroyableSingleton
    {
        private static T _instance;
        public void Awake()
        {
            if (!_instance)
            {
                _instance = gameObject.GetComponent<T>();
                if (!_instance.CanDestroyed)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Debug.LogError($"[Singleton] Second instance of '{typeof(T)}' created!");
                Destroy(this);
            }
        }

        private static bool _applicationIsQuitting;
        public void OnDestroy()
        {
            _applicationIsQuitting = true;
        }
        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                if (_applicationIsQuitting)
                {
                    return null;
                }
                _instance = (T) FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    var singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = $"[{typeof(T)}] - singleton";
                    if (!_instance.CanDestroyed)
                    {
                        DontDestroyOnLoad(singleton);
                    }
                    Debug.Log($"[Singleton] An instance of '{typeof(T)}' was created: {singleton}");
                }
                else
                {
                    Debug.Log($"[Singleton] Using instance of '{typeof(T)}': {_instance.gameObject.name}");
                }
                return _instance;
            }
        }
    }
}