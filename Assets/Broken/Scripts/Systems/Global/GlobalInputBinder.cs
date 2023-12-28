using System;
using Broken.Scripts.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Broken.Scripts.Systems.Global
{
    public class GlobalInputBinder : Singleton<GlobalInputBinder>
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IObservable<Vector2> CreateGetAnyAxisStream()
        {
            return this.UpdateAsObservable().Select(_ => new Vector2(Input.GetAxis("Vertical") , Input.GetAxis("Horizontal")));
        }

        public IObservable<float> CreateGetAxisStream(string axis)
        {
            return this.UpdateAsObservable().Select(_ => Input.GetAxis(axis));
        }
        public IObservable<Unit> CreateGetKeyStream(KeyCode key)
        {
            return this.UpdateAsObservable().Where(_ => Input.GetKey(key));
        }
        public IObservable<Unit> CreateGetKeyDownStream(KeyCode key)
        {
            return this.UpdateAsObservable().Where(_ => Input.GetKeyDown(key));
        }
        public IObservable<Unit> CreateGetKeyUpStream(KeyCode key)
        {
            return this.UpdateAsObservable().Where(_ => Input.GetKeyUp(key));
        }
        public IObservable<Unit> CreateGetMouseButtonStream(int btn)
        {
            return this.UpdateAsObservable().Where(_ => Input.GetMouseButton(btn));
        }
        public IObservable<Unit> CreateGetMouseButtonDownStream(int btn)
        {
            return this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(btn));
        }
        public IObservable<Unit> CreateGetMouseButtonUpStream(int btn)
        {
            return this.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(btn));
        }
        public void CreateGetEventAxisStream(string axis, UnityEvent e)
        {
            this.UpdateAsObservable().Select(_ => Input.GetAxis(axis)).Subscribe(_ => e.Invoke());
        }
        public void CreateEventKeyStream(KeyCode key, UnityEvent e)
        {
            this.UpdateAsObservable().Where(_ => Input.GetKey(key)).Subscribe(_ => e.Invoke());
        }
        public void CreateEventKeyDownStream(KeyCode key, UnityEvent e)
        {
            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(key)).Subscribe(_ => e.Invoke());
        }
        public void CreateEventKeyUpStream(KeyCode key, UnityEvent e)
        {
            this.UpdateAsObservable().Where(_ => Input.GetKeyUp(key)).Subscribe(_ => e.Invoke());
        }
        public void CreateGetMouseButtonStream(int btn, UnityEvent e)
        {
            this.UpdateAsObservable().Where(_ => Input.GetMouseButton(btn)).Subscribe(_ => e.Invoke());
        }
        public void CreateGetMouseButtonDownStream(int btn, UnityEvent e)
        {
            this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(btn)).Subscribe(_ => e.Invoke());
        }
        public void CreateGetMouseButtonUpStream(int btn, UnityEvent e)
        {
            this.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(btn)).Subscribe(_ => e.Invoke());
        }

    }
}
