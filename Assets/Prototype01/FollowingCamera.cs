using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{

    [SerializeField]
    private GameObject _target;

    private Vector3 _distance;

    // Use this for initialization
    void Start()
    {
        _distance = transform.position - _target.transform.position;
    }

    void LateUpdate()
    {
        transform.position = _target.transform.position + _distance;
    }
}
