using System;
using Broken.Scripts.Interface;
using UnityEngine;

namespace Broken.Scripts.Ingame
{
    [RequireComponent(typeof(BoxCollider))]
    public class DropItem : MonoBehaviour, IInteractable
    {
        public IPickable item;
        private BoxCollider _collider;

        public void OnInteract(Player player)
        {
            
        }

        public void Awake()
        {
            _collider=GetComponent<BoxCollider>();
        }
    }
}
