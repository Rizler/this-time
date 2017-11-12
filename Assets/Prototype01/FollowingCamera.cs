using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{

    [SerializeField]
    bool _isChild;

    [SerializeField]
    private GameObject _target;

    private Vector3 _distance;

    private Quaternion _fixedRotation;

    // Use this for initialization
    void Start()
    {
        _distance = transform.position - _target.transform.position;
        _fixedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (_isChild) {
            transform.rotation = _fixedRotation;
        } else {
            transform.position = _target.transform.position + _distance;
        }
        
    }
}
