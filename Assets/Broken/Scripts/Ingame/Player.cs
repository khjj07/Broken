using System;
using System.Collections.Generic;
using Broken.Scripts.Ingame.Weapons;
using Broken.Scripts.Interface;
using Broken.Scripts.Systems.Global;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
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

        public ReactiveProperty<Weapon> equipedWeapon;

        [SerializeField] private Transform hand;

        [SerializeField]
        private List<Weapon> weaponSlot = new();

        [SerializeField]
        private int weaponSlotCapacity = 1;


        private bool _dodgable= true;
        private bool _attackable= true;

        private Vector3 _position;
        private Animator _animator;
        private State _state;

        public Subject<IEquipable> equipSubject = new();
        public Subject<State> stateSubject = new();

        private static readonly int AnimWalk = Animator.StringToHash("Walk");
        private static readonly int AnimDodge = Animator.StringToHash("Dodge");
        private static readonly int AnimAttack = Animator.StringToHash("Attack");


        // Start is called before the first frame update
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            var moveHorizontalStream = GlobalInputBinder.Instance.CreateGetAxisStream("Horizontal").Where(x => math.abs(x) > 0);
            {
                moveHorizontalStream.Subscribe(MoveX).AddTo(gameObject);
                moveHorizontalStream.Where(_ => _dodgable && _state != State.Attack).Subscribe(_ => SetState(State.Walk)).AddTo(gameObject);
            }

            var moveVerticalStream = GlobalInputBinder.Instance.CreateGetAxisStream("Vertical").Where(x => math.abs(x) > 0);
            {
                moveVerticalStream.Subscribe(MoveZ).AddTo(gameObject);
                moveVerticalStream.Where(_ => _dodgable && _state!=State.Attack).Subscribe(_ => SetState(State.Walk)).AddTo(gameObject);
            }

            var dodgeStream = GlobalInputBinder.Instance.CreateGetKeyDownStream(KeyCode.Space);
            {
                dodgeStream.Subscribe(_ => Dodge()).AddTo(gameObject);
                dodgeStream.Subscribe(_ => SetState(State.Dodge)).AddTo(gameObject);
            }

            var attackStream = GlobalInputBinder.Instance.CreateGetMouseButtonDownStream(0);
            {
                attackStream.Select(_ => GetAttackDirection(Input.mousePosition)).Subscribe(x => Look(x)).AddTo(gameObject);
            }

            this.OnTriggerEnterAsObservable().Subscribe(CheckInteractableTarget).AddTo(gameObject);

            var doNotMoveStream = GlobalInputBinder.Instance.CreateGetAnyAxisStream().Where(v => v.magnitude <= 0);

            //doNotMoveStream.Subscribe(_=>Debug.Log(_));
            doNotMoveStream.Where(_=>_state!=State.Attack).Subscribe(_ => SetState(State.Idle)).AddTo(gameObject);

            this.UpdateAsObservable().Where(_ => _state==State.Walk).Subscribe(_ => UpdateDirection()).AddTo(gameObject);

            equipSubject.Subscribe(EquipWeapon);
            stateSubject.Subscribe(Animate);
            equipedWeapon = new ReactiveProperty<Weapon>(weaponSlot[0]);
            equipedWeapon.Subscribe(x=>
            {
                x.OnEquip(this);
            });
            
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


        public void Attack(int number)
        {
            _state = State.Attack; 
            _animator.SetInteger(AnimAttack, number);
        }

        public void AttackEnd()
        {
            _state = State.Idle;
            _animator.SetInteger(AnimAttack, 0);
        }

        private void KeepWeapon(IEquipable equipment)
        {
            weaponSlot.Add(equipment as Weapon);
        }
        private void EquipWeapon(IEquipable equipment)
        {
            equipedWeapon.Value = equipment as Weapon;
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
                    _dodgable = true;
                });
            _dodgable = false;
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

        private void Look(Vector3 direction)
        { 
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private void Update()
        {
            stateSubject.OnNext(_state);
        }
    }
}
