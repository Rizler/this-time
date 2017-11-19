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
            _char.OnKnockdownEvent += OnKnockdownEvent;
            _char.OnGetUpEvent += OnGetUpEvent;
        }
        
        void Update()
        {
            if (_isKnockedDown)
            {
                return;
            }
            _agent.destination = _playerTransform.position;
            _char.Velocity = _agent.velocity;
            if (_agent.remainingDistance <= _agent.radius + 2f)
            {
                _char.Attack();
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