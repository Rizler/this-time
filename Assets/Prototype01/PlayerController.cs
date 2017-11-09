using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 10;
    [SerializeField]
    private float _rotationSpeed = 20;
    [SerializeField]
    private float _jumpForce = 10;
    [SerializeField]
    private float _attackCooldown = 1;
    
    private Rigidbody _playerRb;
    private float _lastAttackTime;
    private int _noSelfCollisionMask;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _noSelfCollisionMask = ~LayerMask.GetMask("Player");
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                Attack();
            }
        };
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                Jump();
            }
        };
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newVelocity = new Vector3(horizontal, 0, vertical);
        newVelocity.Normalize();
        newVelocity *= _movementSpeed;
        newVelocity.y = _playerRb.velocity.y;

        _playerRb.velocity = newVelocity;

        if (horizontal != 0 || vertical != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_playerRb.velocity.x, 0, _playerRb.velocity.z));
            _playerRb.MoveRotation(Quaternion.Slerp(_playerRb.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed));
        }
    }

    private void Jump()
    {
        _playerRb.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        var collider = GetComponent<CapsuleCollider>();
        Vector3 endPoint = new Vector3(collider.bounds.center.x, collider.bounds.min.y - 0.1f, collider.bounds.min.z);
        return Physics.CheckCapsule(collider.bounds.center, endPoint, collider.radius, _noSelfCollisionMask);
    }

    private void Attack()
    {
        _lastAttackTime = Time.time;
    }
}
