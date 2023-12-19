using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Broken.Scripts.Systems.Etc
{
    public abstract class SceneLoadable : MonoBehaviour
    {
        [SerializeField]
        protected float minimumDelaySecond;

        // Start is called before the first frame update
        public void LoadSceneAsync(string targetScene)
        {
            Observable.FromCoroutine<AsyncOperation>(x=>LoadSceneAsync(x,targetScene))
                .Where(x => x.progress >= 0.9f)
                .Subscribe(OnSceneLoaded);
        }

        public abstract void OnSceneLoaded(AsyncOperation asyncOperation);

        private IEnumerator LoadSceneAsync(IObserver<AsyncOperation> observer,string targetScene)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene);
            asyncOperation.allowSceneActivation = false;
            observer.OnNext(asyncOperation);
            yield return new WaitForSeconds(minimumDelaySecond);
            while (asyncOperation.progress < 0.9f)
            {
                observer.OnNext(asyncOperation);
                yield return asyncOperation;
            }
            observer.OnNext(asyncOperation);
            yield return null;
        }
    }
}
