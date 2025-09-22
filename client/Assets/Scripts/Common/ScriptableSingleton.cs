using System.Linq;
using UnityEngine;

namespace Common
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = Resources.LoadAll<T>(string.Empty).FirstOrDefault();
                
                if (_instance == null)
                {
                    _instance = CreateInstance<T>();
                    _instance.hideFlags = HideFlags.HideAndDontSave; // optional: don't save this ephemeral instance
                }

                return _instance;
            }
        }
    }
}