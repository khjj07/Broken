using Broken.Scripts.Systems.Etc;
using Broken.Scripts.Systems.Global;
using UniRx;
using UnityEngine;

namespace Broken.Scripts.Systems.UI
{
    public class SplashManager : SceneLoadable
    {
        [SerializeField] private string targetScene;
        void Start()
        {
            LoadSceneAsync(targetScene);
        }

        public override void OnSceneLoaded(AsyncOperation asyncOperation)
        {
            Debug.Log("Almost Loaded");
            GlobalInputBinder.Instance.CreateGetMouseButtonDownStream(0)
                .Subscribe(_ => asyncOperation.allowSceneActivation = true)
                .AddTo(gameObject);
        }

    }
}
