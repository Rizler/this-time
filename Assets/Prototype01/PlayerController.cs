using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _movementSpeed = 10;
    [SerializeField]
    private float _jumpForce = 11;
    [SerializeField]
    private float _airDragForce = 15;
    [SerializeField]
    private float _airControlForce = 25;
    [SerializeField]
    private float _rotationSpeed = 10;

    [Header("Combat")]
    [SerializeField]
    private float _attackCooldown = 1;
    [SerializeField]
    private float _attackDuration = 0.1f;


    private Rigidbody _rigidBody;
    private CapsuleCollider _collider;
    private Quaternion _targetRotation = Quaternion.identity;
    private float _lastAttackTime;
    private bool _isGrounded;
    private int _noSelfCollisionMask;


    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _noSelfCollisionMask = ~LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                StartCoroutine(Attack());
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (_isGrounded)
        {
            Vector3 newVelocity = new Vector3(horizontal, 0, vertical);
            newVelocity.Normalize();
            newVelocity *= _movementSpeed;
            newVelocity.y = _rigidBody.velocity.y;

            _rigidBody.velocity = newVelocity;
        }
        else
        {
            float xDirection = horizontal == 0 ? -_rigidBody.velocity.x : horizontal;
            float zDirection = vertical == 0 ? -_rigidBody.velocity.z : vertical;

            Vector3 newVelocity = new Vector3(xDirection, 0, zDirection);
            newVelocity.Normalize();
            float normalizedMaxSpeed = Mathf.Max(Mathf.Abs(newVelocity.x), Mathf.Abs(newVelocity.z)) * _movementSpeed;

            if (horizontal == 0)
            {
                newVelocity.x *= _airDragForce;
            }
            else
            {
                newVelocity.x *= _airControlForce;
            }
            if (vertical == 0)
            {
                newVelocity.z *= _airDragForce;
            }
            else
            {
                newVelocity.z *= _airControlForce;
            }

            _rigidBody.AddForce(newVelocity, ForceMode.Acceleration);

            // Limit the velocity after adding the force
            newVelocity = _rigidBody.velocity;
            newVelocity.x = Mathf.Clamp(newVelocity.x, -normalizedMaxSpeed, normalizedMaxSpeed);
            newVelocity.z = Mathf.Clamp(newVelocity.z, -normalizedMaxSpeed, normalizedMaxSpeed);

            _rigidBody.velocity = newVelocity;

        }

        if (horizontal != 0 || vertical != 0)
        {
            _targetRotation = Quaternion.LookRotation(new Vector3(horizontal, 0, vertical));
            _rigidBody.MoveRotation(Quaternion.Slerp(_rigidBody.rotation, _targetRotation, Time.fixedDeltaTime * _rotationSpeed));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _isGrounded = IsGrounded();
    }

    private void OnCollisionExit(Collision collision)
    {
        //_isGrounded = IsGrounded();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }

    private void Jump()
    {
        _rigidBody.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
        _isGrounded = false;
    }

    private bool IsGrounded()
    {
        Vector3 endPoint = new Vector3(_collider.bounds.center.x, _collider.bounds.min.y - 0.1f, _collider.bounds.min.z);
        return Physics.CheckCapsule(_collider.bounds.center, endPoint, _collider.radius, _noSelfCollisionMask);
    }

    private IEnumerator Attack()
    {
        BoxCollider meleeCollider = GetComponent<BoxCollider>();
        meleeCollider.enabled = true;
        AudioSource hitSound = GetComponent<AudioSource>();
        hitSound.Play();
        yield return new WaitForSeconds(_attackDuration);
        meleeCollider.enabled = false;
    }
}
