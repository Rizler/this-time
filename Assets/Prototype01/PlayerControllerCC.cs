using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerControllerCC : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

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

    [Header("Combat")]
    [SerializeField]
    private float _attackCooldown = 1;
    [SerializeField]
    private Collider _meleeCollider;


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
        // h = 0.5gt^2 + v0t
        // v = v0 + gt
        // h = v^2 / 2g -- Energy equation (h from peak of jump downwards)
        _gravity = (-2 * _jumpHeight * Mathf.Pow(_movementVelocity, 2)) / (Mathf.Pow(_jumpPeakHorizontalDistance, 2));
        _initialJumpVelocity = (2 * _jumpHeight * _movementVelocity) / _jumpPeakHorizontalDistance;

        _minJumpVelocityThreshold = Mathf.Sqrt((_jumpHeight -_minJumpHeight) * -2 * _gravity);
    }

    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _noSelfCollisionMask = ~LayerMask.GetMask("Player");
        _lastGroundedPosition = transform.position;
        _meleeCollider.enabled = false;
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
                StartCoroutine(Attack());
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

        _animator.SetFloat("Speed", new Vector2(_xVelocity, _zVelocity).magnitude / 15f);
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

            float xMotion = CalculateDistance(_xVelocity, motion.x * _airControlAcceleration, Time.fixedDeltaTime);
            float zMotion = CalculateDistance(_zVelocity, motion.z * _airControlAcceleration, Time.fixedDeltaTime);

            float normalizedMaxVelocity = Mathf.Max(Mathf.Abs(motion.x), Mathf.Abs(motion.z)) * _movementVelocity;
            float maxDistance = normalizedMaxVelocity * Time.fixedDeltaTime;
            xMotion = Mathf.Clamp(xMotion, -maxDistance, maxDistance);
            zMotion = Mathf.Clamp(zMotion, -maxDistance, maxDistance);

            _xVelocity = CalculateVelocity(_xVelocity, motion.x * _airControlAcceleration, Time.fixedDeltaTime);
            _zVelocity = CalculateVelocity(_zVelocity, motion.z * _airControlAcceleration, Time.fixedDeltaTime);
            _xVelocity = Mathf.Clamp(_xVelocity, -normalizedMaxVelocity, normalizedMaxVelocity);
            _zVelocity = Mathf.Clamp(_zVelocity, -normalizedMaxVelocity, normalizedMaxVelocity);

            motion.x = xMotion;
            motion.z = zMotion;
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

    private void Jump()
    {
        _yVelocity = _initialJumpVelocity;
    }

    private IEnumerator Attack()
    {
        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.1f);
        _meleeCollider.enabled = true;
        yield return new WaitForSeconds(0.2f);
        _meleeCollider.enabled = false;

        /*Vector3 hitBoxCenter = transform.TransformPoint(0, 2.7f, _charController.radius);
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
        }*/
    }
}
