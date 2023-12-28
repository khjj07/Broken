using Broken.Scripts.Interface;
using UnityEngine;

namespace Broken.Scripts.Ingame.Weapons
{
    public abstract class Weapon : MonoBehaviour, IWeapon
    {
        public abstract void OnEquip(Player player);

        public abstract void OnChange(IWeapon weapon);

        public abstract void OnUnequip(Player player);
    }
}
