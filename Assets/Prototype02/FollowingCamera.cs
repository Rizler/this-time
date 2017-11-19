using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype02
{
    public class FollowingCamera : MonoBehaviour
    {
        [SerializeField]
        bool _isChild;

        [SerializeField]
        private GameObject _target;

        private Vector3 _distance;

        private Quaternion _fixedRotation;

        private float _shakeIntensity;
        private float _shakeDuration;


        // Use this for initialization
        void Start()
        {
            _distance = transform.position - _target.transform.position;
            _fixedRotation = transform.rotation;
        }

        void LateUpdate()
        {
            if (_shakeDuration > 0)
            {
                _shakeDuration -= Time.deltaTime;
                Vector3 rotationAmount = Random.insideUnitSphere * _shakeIntensity;
                rotationAmount.z = 0;
                transform.rotation *= Quaternion.Euler(rotationAmount);
            }
            else
            {
                transform.rotation = _fixedRotation;
            }

            if (!_isChild)
            {
                transform.position = _target.transform.position + _distance;
            }
        }

        public void Shake(float duration, float intensity)
        {
            _shakeDuration = duration;
            _shakeIntensity = intensity;
        }
    }
}