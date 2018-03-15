using UnityEngine;

namespace AiryCat.Utilities.Helper
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public void Awake()
        {
            if (!_instance)
            {
                _instance = gameObject.GetComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogError($"[Singleton] Second instance of '{typeof(T)}' created!");
                Destroy(this);
            }
        }

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = (T) FindObjectOfType(typeof(T));

                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    Debug.LogError($"[Singleton] multiple instances of '{typeof(T)}' found!");
                }

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = $"[{typeof(T).ToString()}] - singleton";
                    DontDestroyOnLoad(singleton);
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