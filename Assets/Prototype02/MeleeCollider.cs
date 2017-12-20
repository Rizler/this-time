using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype02
{
    [RequireComponent(typeof(Collider))]
    public class MeleeCollider : MonoBehaviour
    {
        private Character _parentCharacter;
        private Collider _collider;
        
        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _parentCharacter = GetComponentInParent<Character>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Character charHit = other.GetComponent<Character>();
            if (charHit != null && charHit.GetInstanceID() != _parentCharacter.GetInstanceID())
            {
                charHit.ReceiveHit(_parentCharacter);
                _parentCharacter.DeliveredHit(charHit);
                _collider.enabled = false;
            }
        }
    }
}