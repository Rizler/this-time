using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Prototype02
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(AudioSource))]
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _animationSpeedFactor = 15f;
        [SerializeField]
        private Collider _meleeCollider;
        [SerializeField]
        private Image healthBarImg;

        [Header("Stats")]
        [SerializeField]
        private float _hp = 100;
        [SerializeField]
        private float _damage = 25;
        [SerializeField]
        private float _attackCooldown = 1;
        [SerializeField]
        private int _comboToKnockdown = 3;
        [SerializeField]
        private float _comboTimeWindow = 1.5f;
        [SerializeField]
        private float _knockdownDuration = 2f;

        [Header("Events")]
        public UnityEvent OnKnockdownEvent;
        public UnityEvent OnGetUpEvent;
        public UnityEvent OnHitDeliveredEvent;
        public UnityEvent OnHitReceivedEvent;
        public UnityEvent OnDeathEvent;

        private Vector3 _velocity;
        private float _maxHp;
        private float _lastAttackTime;
        private float _lastHitReceivedTime;
        private int _comboCounter;


        public Vector3 Velocity
        {
            get
            {
                return _velocity;
            }

            set
            {
                _velocity = value;
                _animator.SetFloat("Speed", new Vector2(Velocity.x, Velocity.z).magnitude / _animationSpeedFactor);
            }
        }

        private void Start()
        {
            _maxHp = _hp;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "Water")
            {
                Die();
            }
        }

        public void ReceiveHit(Character hitter)
        {
            OnHitReceivedEvent.Invoke();
            _hp -= hitter._damage;
            UpdateHealthBar();
            if (_hp <= 0)
            {
                Die();
                return;
            }

            if (Time.time - _lastHitReceivedTime <= _comboTimeWindow)
            {
                _comboCounter++;
            }
            else
            {
                _comboCounter = 1;
            }
            if (_comboCounter == _comboToKnockdown)
            {
                Knockdown();
            }
            else
            {
                _animator.SetTrigger("HitFront");
            }

            _lastHitReceivedTime = Time.time;
        }

        public void DeliveredHit(Character hitChar)
        {
            OnHitDeliveredEvent.Invoke();
        }

        public void Die()
        {
            OnDeathEvent.Invoke();
            _animator.SetTrigger("FallHitFront");
            Camera.main.GetComponent<FollowingCamera>().Shake(0.3f, 0.25f);
        }

        public void Knockdown()
        {
            StartCoroutine(KnockdownRoutine());
        }

        private IEnumerator KnockdownRoutine()
        {
            OnKnockdownEvent.Invoke();
            _animator.SetTrigger("FallHitFront");
            Camera.main.GetComponent<FollowingCamera>().Shake(0.15f, 0.25f);
            yield return new WaitForSeconds(1.5f);
            _animator.SetTrigger("GetUp");
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            OnGetUpEvent.Invoke();
        }

        public void Jump()
        {
            _animator.SetTrigger("Jump");
        }

        public void Attack()
        {
            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                _lastAttackTime = Time.time;
                StartCoroutine(AttackRoutine());
            }
        }

        private IEnumerator AttackRoutine()
        {
            _animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.1f);
            _meleeCollider.enabled = true;
            yield return new WaitForSeconds(0.2f);
            _meleeCollider.enabled = false;
        }

        private void UpdateHealthBar()
        {
            if (healthBarImg == null)
            {
                return;
            }
            healthBarImg.fillAmount = _hp / _maxHp;
        }
    }
}
