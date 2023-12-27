using System;
using System.Collections.Generic;
using Broken.Scripts.Interface;
using Broken.Scripts.Systems.Global;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Broken.Scripts.Ingame
{
    public class Player : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Walk,
            Dodge,
            Attack,
        }

        [SerializeField] private float speed = 1.0f;
        [SerializeField] private float dodgeCool = 1.0f;
        [SerializeField] private float dodgeDistance = 1.0f;
        [SerializeField] private float dodgeSpeed = 1.0f;



        [SerializeField]
        private List<Weapon> weaponSlot;
        [SerializeField]
        private int weaponSlotCapacity = 1;

        private bool _dodge= false;
        private bool _attack= false;

        private Vector3 _position;
        private Animator _animator;
        private State _state;

        public Subject<IEquipable> equipSubject = new Subject<IEquipable>();
        public Subject<State> stateSubject = new Subject<State>();
        private static readonly int AnimWalk = Animator.StringToHash("Walk");
        private static readonly int AnimDodge = Animator.StringToHash("Dodge");

        // Start is called before the first frame update
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            this.OnTriggerEnterAsObservable().Subscribe(CheckInteractableTarget).AddTo(gameObject);

            this.UpdateAsObservable().Where(_=> !_dodge).Subscribe(_ => SetState(State.Idle)).AddTo(gameObject);
            this.UpdateAsObservable().Where(_=> !_attack).Subscribe(_ => UpdateDirection()).AddTo(gameObject);

            var moveHorizontalStream = GlobalInputBinder.Instance.CreateGetAxisStream("Horizontal");
            {
                moveHorizontalStream.Subscribe(MoveX).AddTo(gameObject);
                moveHorizontalStream.Where(x => math.abs(x) > 0).Where(_ => !_dodge).Subscribe(_ => SetState(State.Walk)).AddTo(gameObject);
            }

            var moveVerticalStream = GlobalInputBinder.Instance.CreateGetAxisStream("Vertical");
            {
                moveVerticalStream.Subscribe(MoveZ).AddTo(gameObject);
                moveVerticalStream.Where(x=>math.abs(x)>0).Where(_ => !_dodge).Subscribe(_ => SetState(State.Walk)).AddTo(gameObject);
            }

            var dodgeStream = GlobalInputBinder.Instance.CreateGetKeyDownStream(KeyCode.Space);
            {
                dodgeStream.Subscribe(_ => Dodge()).AddTo(gameObject);
                dodgeStream.Subscribe(_ => SetState(State.Dodge)).AddTo(gameObject);
            }

            var attackStream = GlobalInputBinder.Instance.CreateGetMouseButtonStream(0);
            {
                attackStream.Select(_ => GetAttackDirection(Input.mousePosition)).Subscribe(x => TryAttack(x)).AddTo(gameObject);
                attackStream.Subscribe(_ => SetState(State.Attack)).AddTo(gameObject);
            }
            equipSubject.Subscribe(TryEquip);

            stateSubject.Subscribe(Animate);
        }

        private void Animate(State state)
        {
            switch (state)
            {
                case State.Idle:
                    _animator.SetBool(AnimWalk, false);
                    _animator.SetBool(AnimDodge, false);
                    break;
                case State.Walk:
                    _animator.SetBool(AnimWalk, true);
                    _animator.SetBool(AnimDodge, false);
                    break;
                case State.Dodge:
                    _animator.SetBool(AnimDodge, true);
                    break;
            }
        }
        private void KeepWeapon(IEquipable equipment)
        {
            weaponSlot.Add(equipment as Weapon);
        }
        private void EquipWeapon(IEquipable equipment)
        {
            if (weaponSlot[0] != null)
            {
                weaponSlot[0] = equipment as Weapon;
                var prevWeapon = weaponSlot[0];
                prevWeapon.OnUnequip();
                weaponSlot[0].OnEquip();
            }
            else
            {
                weaponSlot[0] = equipment as Weapon;
                weaponSlot[0].OnEquip();
            }
        }
        private void TryEquip(IEquipable equipment)
        {
            if (weaponSlot.Count != 0 && weaponSlot.Count < weaponSlotCapacity)
            {
                KeepWeapon(equipment);
            }
            else
            {
                EquipWeapon(equipment);
            }
        }
        private void CheckInteractableTarget(Collider collider)
        {
            var item = collider.GetComponent<IInteractable>();
            if (item != null)
            {
                var leaveFromItemStream = this.OnTriggerExitAsObservable()
                    .Where(x => x.GetComponent<IInteractable>() == item);

                GlobalInputBinder.Instance.CreateGetKeyDownStream(KeyCode.E)
                    .TakeUntil(leaveFromItemStream)
                    .Subscribe(x => Interact(item))
                    .AddTo(gameObject);
            }
        }
        private void Interact(IInteractable item)
        {
            item.OnInteract(this);
        }
        private void Dodge()
        {
            transform.DOMove(transform.forward * dodgeDistance, dodgeSpeed)
                .SetRelative(true)
                .SetSpeedBased(true)
                .OnComplete(() =>
                {
                    _dodge = false;
                });
            _dodge = true;
        }
        private void MoveX(float direction)
        {
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);
        }
        private void MoveZ(float direction)
        {
            transform.Translate(Vector3.forward * direction * speed * Time.deltaTime, Space.World);
        }
        private void UpdateDirection()
        {
            var currentPosition = transform.position;
            if (Vector3.Distance(currentPosition, _position) > 0.001f)
            {
                var direction = Vector3.Normalize(currentPosition - _position);
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            _position = currentPosition;
        }

        private void SetState(State newState)
        {
            _state = newState;
        }

        private Vector3 GetAttackDirection(Vector3 mousePosition)
        {
            var ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit, 100f))
            {
                mousePosition = hit.point;
                mousePosition.y = transform.position.y;
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                return (mousePosition- transform.position).normalized;
            }
            return Vector3.zero;
        }

        private void TryAttack(Vector3 direction)
        {
            if (!_attack)
            {
                _attack = true;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => _attack = false);
            }
        }

        private void Update()
        {
            stateSubject.OnNext(_state);
        }
    }
}
