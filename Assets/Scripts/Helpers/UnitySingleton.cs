using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Generic singleton class. Use this class whenever you need just one instance of gameObject in game, no matter of scene.
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public abstract class UnitySingleton<T> : MonoBehaviour where T : Component
    {
        #region Fields

        private static T _instance;

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        // Create instance of gameObject and attach this script to it.
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            // Make sure only one copy is persisted even during scene change.
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
