using System;
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
        private AnimatorOverrideController animatorController;

        [SerializeField] private int attackFrame;

        public override void OnEquip(Player player)
        {
            var unequipStream = player.equipedWeapon.Where(w => w != this);

            var click1Stream = GlobalInputBinder.Instance.CreateGetMouseButtonDownStream(0).TakeUntil(unequipStream);

            //var click2Stream = click1Stream.SelectMany(click1Stream.ThrottleFrame(attackFrame)).SkipUntil(click1Stream).Take(1);

            //var click3Stream = click2Stream.SelectMany(click1Stream.ThrottleFrame(attackFrame)).SkipUntil(click2Stream).Take(1);

            Debug.Log("Equip");
            click1Stream.Subscribe(_ => player.AnimateAttack(1)).AddTo(player.gameObject);
            //click2Stream.Subscribe(_ => player.AnimateAttack(2)).AddTo(player.gameObject);
            //click3Stream.Subscribe(_ => player.AnimateAttack(3)).AddTo(player.gameObject);
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
