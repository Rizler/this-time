using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype02
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Character))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField]
        private bool _shouldAttack;
        [SerializeField]
        private float _maxAttackRange;
        [SerializeField]
        private float _stopDistance;
        [SerializeField]
        private float _resumeMovementRange;
        //TODO: Add resume movement timer

        private NavMeshAgent _agent;
        private Character _player;
        private Transform _playerTransform;
        private Character _char;
        private bool _isKnockedDown;
        

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _player = GameObject.Find("Player").GetComponent<Character>();
            _playerTransform = _player.GetComponent<Transform>();
            _char = GetComponent<Character>();
            _char.OnKnockdownEvent.AddListener(OnKnockdownEvent);
            _char.OnGetUpEvent.AddListener(OnGetUpEvent);
        }
        
        void Update()
        {
            if (_isKnockedDown)
            {
                return;
            }
            _agent.destination = _playerTransform.position;
            _char.Velocity = _agent.velocity;

            if (_agent.remainingDistance <= _maxAttackRange)
            {
                transform.LookAt(_agent.destination);
                if (_shouldAttack)
                {
                    _char.Attack();
                }
                if (_agent.remainingDistance <= _stopDistance)
                {
                    _agent.isStopped = true;

                }
            }
            else if (_agent.remainingDistance >= _resumeMovementRange)
            {
                _agent.isStopped = false;
            }
        }

        private void OnKnockdownEvent()
        {
            _isKnockedDown = true;
            _agent.enabled = false;
        }

        private void OnGetUpEvent()
        {
            _isKnockedDown = false;
            _agent.enabled = true;
        }
    }
}