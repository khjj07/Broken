using Broken.Scripts.Systems.Etc;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Broken.Scripts.Systems.UI
{
    public class TitleManager : SceneLoadable
    {
        [SerializeField]
        private string ingameScene;
        [SerializeField]
        private Button gameStartButton;
      
        // Start is called before the first frame update
        void Start()
        {
            gameStartButton.OnPointerClickAsObservable().Subscribe(_=>LoadSceneAsync(ingameScene));
        }

        public override void OnSceneLoaded(AsyncOperation asyncOperation)
        {
            asyncOperation.allowSceneActivation = true;
        }
    }
}
