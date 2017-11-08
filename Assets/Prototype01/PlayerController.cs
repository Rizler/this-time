using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _movementSpeed = 10;
    [SerializeField]
    float _rotationSpeed = 20;

    private Rigidbody _playerRb;

    // Use this for initialization
    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _playerRb.velocity = new Vector3(_movementSpeed * horizontal, _playerRb.velocity.y, _movementSpeed * vertical);

        if (horizontal != 0 || vertical != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_playerRb.velocity.x, 0, _playerRb.velocity.z));
            _playerRb.MoveRotation(Quaternion.Slerp(_playerRb.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed));
        }
    }
}
