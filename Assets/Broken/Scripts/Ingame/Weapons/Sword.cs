using System;
using System.Collections;
using Broken.Scripts.Interface;
using Broken.Scripts.Systems.Global;
using UniRx;
using UniRx.Triggers;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

namespace Broken.Scripts.Ingame.Weapons
{
    public class Sword : Weapon
    {
        [SerializeField]
        private AnimatorOverrideController animatorController;

        [SerializeField] private float attackTermSec = 1f;

        private int _count = 0;

        public override void OnEquip(Player player)
        {
            var unequipStream = player.equipedWeapon.Where(w => w != this);

            var clickStream = GlobalInputBinder.Instance.CreateGetMouseButtonDownStream(0).TakeUntil(unequipStream);

            clickStream.Throttle(TimeSpan.FromSeconds(0.1f)).Subscribe(_=>
            {
                if (_count > 2)
                {
                    _count=1;
                }
                else
                {
                    _count++;
                }
                player.Attack(_count);
                Debug.Log("Click :"+_count);
            }).AddTo(player.gameObject);

            clickStream.Subscribe(_=>
            {
                Observable.Timer(TimeSpan.FromSeconds(attackTermSec)).TakeUntil(clickStream).Subscribe(_=>
                {
                    _count = 0;
                    player.AttackEnd();
                    Debug.Log("NonClick :" + _count);
                }).AddTo(player.gameObject);
            }).AddTo(player.gameObject);
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
