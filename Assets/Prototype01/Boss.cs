using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    private Rigidbody _rigidBody;
    private const float JUMP_TIME = 5;
    private float _jumpTimer = 0;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _jumpTimer += Time.deltaTime;
        if (_jumpTimer >= JUMP_TIME)
        {
            _rigidBody.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
            _jumpTimer = 0;
        }
    }
}
