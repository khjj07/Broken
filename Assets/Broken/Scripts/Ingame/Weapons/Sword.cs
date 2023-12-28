using Broken.Scripts.Interface;
using Broken.Scripts.Systems.Global;
using UniRx;
using UniRx.Triggers;
using UnityEditor.Animations;
using UnityEngine;

namespace Broken.Scripts.Ingame.Weapons
{
    public class Sword : Weapon
    {
        [SerializeField]
        private AnimatorController animatorController;


        public override void OnEquip(Player player)
        {
            var unequipStream = player._equipedWeapon.Where(w => w != this);

            var clickStream = GlobalInputBinder.Instance.CreateGetMouseButtonDownStream(0);
            clickStream.TakeUntil(unequipStream)
                .Subscribe(_ => player.AnimateAttack(0)).AddTo(player.gameObject);

        }

        public override void OnUnequip(Player player) 
        {
            throw new System.NotImplementedException();
        }

        public override void OnChange(IWeapon weapon)
        {
            throw new System.NotImplementedException();
        }
    }
}
