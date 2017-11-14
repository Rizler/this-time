using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerControllerCC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _movementVelocity = 20;
    [SerializeField]
    private float _airControlAcceleration = 5;
    [SerializeField]
    private float _jumpHeight = 10;
    [SerializeField]
    private float _minJumpHeight = 10;
    [SerializeField]
    private float _jumpPeakHorizontalDistance = 10;
    /*[SerializeField]
    private float _jumpPeakTime = 0.5f;*/
    [SerializeField]
    private float _fallGravityMultiplier = 3;
    [SerializeField]
    private float _stopJumpGravityMultiplier = 3;
    [SerializeField]
    private float _rotationSpeed = 10;
    [SerializeField]
    private Animator _animator;

    [Header("Combat")]
    [SerializeField]
    private float _attackCooldown = 1;


    private CharacterController _charController;
    private float _xVelocity;
    private float _yVelocity;
    private float _zVelocity;
    private float _gravity;
    private float _initialJumpVelocity;
    private bool _isJumpPressed;
    private float _minJumpVelocityThreshold;
    private Quaternion _targetRotation = Quaternion.identity;
    private float _lastAttackTime;
    private Vector3 _lastGroundedPosition;
    private float _groundedTimer;
    private int _noSelfCollisionMask;


    private void OnValidate()
    {
        CalculateJumpParameters();
    }

    private void CalculateJumpParameters()
    {
        //calculate by jumpHeight and jumpPeakTime
        /*_gravity = (-2 * _jumpHeight) / (Mathf.Pow(_jumpPeakTime, 2));
        _initialJumpVelocity = (2 * _jumpHeight) / _jumpPeakTime; */

        // calculate by jumpHeight and horizontal jump distance
        _gravity = (-2 * _jumpHeight * Mathf.Pow(_movementVelocity, 2)) / (Mathf.Pow(_jumpPeakHorizontalDistance, 2));
        _initialJumpVelocity = (2 * _jumpHeight * _movementVelocity) / _jumpPeakHorizontalDistance;

        // h = 0.5gt^2 + v0t
        // v = v0 + gt
        float minJumpTime = ((-2 * _initialJumpVelocity) + Mathf.Sqrt((4 * Mathf.Pow(_initialJumpVelocity, 2)) + (8 * _gravity * _minJumpHeight))) / (2 * _gravity);
        _minJumpVelocityThreshold = _initialJumpVelocity + _gravity * minJumpTime;

        Debug.Log("_gravity: " + _gravity);
        Debug.Log("_initialJumpVelocity: " + _initialJumpVelocity);
        Debug.Log("minJumpTime: " + minJumpTime);
        Debug.Log("_minJumpVelocityThreshold: " + _minJumpVelocityThreshold);
    }

    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _noSelfCollisionMask = ~LayerMask.GetMask("Player");
        _lastGroundedPosition = transform.position;
    }

    void Update()
    {
        if (_charController.isGrounded)
        {
            _groundedTimer += Time.deltaTime;
            if (_groundedTimer >= 0.3)
            {
                _lastGroundedPosition = transform.position;
                _groundedTimer = 0;
            }
        }
        else
        {
            _groundedTimer = 0;
        }

        if (Input.GetButtonDown("Attack"))
        {
            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                Attack();
            }
        }
        _isJumpPressed = Input.GetButton("Jump");
        if (Input.GetButtonDown("Jump"))
        {
            if (_charController.isGrounded)
            {
                _animator.SetTrigger("Jump");
                Jump();
            }
        }

        if (_charController.isGrounded)
        {
            _animator.SetFloat("Speed", new Vector2(_xVelocity, _zVelocity).magnitude / 20f);
        }
        else
        {
            _animator.SetFloat("Speed", 0.0f);
        }
        



    }

    private float CalculateDistance(float velocity, float acceleration, float deltaTime)
    {
        return (velocity * deltaTime) + (0.5f * acceleration * Mathf.Pow(deltaTime, 2));
    }

    private float CalculateVelocity(float currentVelocity, float acceleration, float deltaTime)
    {
        return currentVelocity + (acceleration * deltaTime);
    }

    private float CalculateVerticalMotion()
    {
        float _verticalAcceleration = _gravity;
        if (!_isJumpPressed && _yVelocity <= _minJumpVelocityThreshold && _yVelocity > 0)
        {
            _verticalAcceleration *= _stopJumpGravityMultiplier;
        }
        else if (_yVelocity <= 0)
        {
            _verticalAcceleration *= _fallGravityMultiplier;
        }
        float yMotion = CalculateDistance(_yVelocity, _verticalAcceleration, Time.fixedDeltaTime);
        _yVelocity = CalculateVelocity(_yVelocity, _verticalAcceleration, Time.fixedDeltaTime);
        return yMotion;
    }

    private Vector3 CalculateXZMotion(float horizontalAxis, float verticalAxis)
    {
        Vector3 motion = new Vector3(horizontalAxis, 0, verticalAxis);
        motion.Normalize();
        if (_charController.isGrounded)
        {
            motion *= _movementVelocity;
            _xVelocity = motion.x;
            _zVelocity = motion.z;
            motion *= Time.fixedDeltaTime;
        }
        else
        {
            if (horizontalAxis == 0 && verticalAxis == 0)
            {
                return new Vector3(_xVelocity, 0, _zVelocity) * Time.fixedDeltaTime;
            }

            float normalizedMaxVelocity = Mathf.Max(Mathf.Abs(motion.x), Mathf.Abs(motion.z)) * _movementVelocity;
            motion *= _airControlAcceleration;

            //TODO: need to calculate the maximum acceleration so we don't go over the maximum speed before calculating the distance
            //motion.x = CalculateDistance(_xVelocity, motion.x * _airControlAcceleration, Time.fixedDeltaTime);
            //motion.z = CalculateDistance(_zVelocity, motion.z * _airControlAcceleration, Time.fixedDeltaTime);
            float xMotion = CalculateDistance(_xVelocity, 0, Time.fixedDeltaTime);
            float zMotion = CalculateDistance(_zVelocity, 0, Time.fixedDeltaTime);

            _xVelocity = CalculateVelocity(_xVelocity, motion.x * _airControlAcceleration, Time.fixedDeltaTime);
            _zVelocity = CalculateVelocity(_zVelocity, motion.z * _airControlAcceleration, Time.fixedDeltaTime);

            motion.x = xMotion;
            motion.z = zMotion;

            _xVelocity = Mathf.Clamp(_xVelocity, -normalizedMaxVelocity, normalizedMaxVelocity);
            _zVelocity = Mathf.Clamp(_zVelocity, -normalizedMaxVelocity, normalizedMaxVelocity);
        }
        return motion;
    }

    private void RotateTowardsMovement(float hotizontalAxis, float verticalAxis)
    {
        if (hotizontalAxis != 0 || verticalAxis != 0)
        {
            _targetRotation = Quaternion.LookRotation(new Vector3(hotizontalAxis, 0, verticalAxis));
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 motion = CalculateXZMotion(horizontal, vertical);
        motion.y = CalculateVerticalMotion();
        _charController.Move(motion);

        if (_charController.isGrounded)
        {
            _yVelocity = 0;
        }

        RotateTowardsMovement(horizontal, vertical);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Water")
        {
            transform.position = _lastGroundedPosition;
        }
    }

    private void Jump()
    {
        _yVelocity = _initialJumpVelocity;
    }

    private void Attack()
    {
        Vector3 hitBoxCenter = transform.TransformPoint(0, 2.7f, _charController.radius);
        Vector3 hitBoxHalfExtents = new Vector3(1.25f, 1.7f, 1.1f);
        RaycastHit[] hits = Physics.BoxCastAll(hitBoxCenter, hitBoxHalfExtents, transform.forward, Quaternion.identity, 0, _noSelfCollisionMask);
        if (hits.Length > 0)
        {
            AudioSource hitSound = GetComponent<AudioSource>();
            hitSound.Play();
        }
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.transform.GetComponent<Enemy>().Hit();
            }
        }
    }
}
