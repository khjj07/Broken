using System;
using UnityEngine;

namespace Broken.Scripts.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = FindObjectOfType<T>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                return _instance;
            }
        }
    }
}
