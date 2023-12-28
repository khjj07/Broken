using System.Diagnostics.CodeAnalysis;
using Broken.Scripts.Ingame.Weapons;
using Broken.Scripts.Interface;
using UnityEngine;

namespace Broken.Scripts.Ingame
{
    public class DropWeapon : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Weapon weapon;
        public void OnInteract(Player player)
        {
            player.equipSubject.OnNext(weapon);
        }
        
        void Start()
        {
        
        }

        void Update()
        {
        
        }
    }
}
