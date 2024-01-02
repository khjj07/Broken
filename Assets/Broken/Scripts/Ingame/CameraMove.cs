using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Broken.Scripts.Ingame
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        void Start()
        {
            this.UpdateAsObservable().Subscribe(_ =>
            {
                transform.position=new Vector3(target.position.x, transform.position.y, target.position.z-15);
            });
        }
    }
}
