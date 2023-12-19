using Broken.Scripts.Systems.Global;
using UniRx;
using UnityEngine;

namespace Broken.Scripts.Ingame
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
            GlobalInputBinder.Instance.CreateGetAxisStream("Horizontal").Subscribe(MoveX);
            GlobalInputBinder.Instance.CreateGetAxisStream("Vertical").Subscribe(MoveZ);
        }

        void MoveX(float direction)
        {
            transform.Translate(Vector3.right*direction*speed*Time.deltaTime);
        }

        void MoveZ(float direction)
        {
            transform.Translate(Vector3.forward*direction*speed*Time.deltaTime);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
