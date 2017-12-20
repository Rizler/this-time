using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype02
{
    using ButtonState = PlayerInputState.SingleFrameInput.ButtonState;

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Character))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float _movementVelocity = 20;
        [SerializeField]
        private float _airControlAcceleration = 100;
        [SerializeField]
        private float _jumpHeight = 7;
        [SerializeField]
        private float _minJumpHeight = 2;
        [SerializeField]
        private float _jumpPeakHorizontalDistance = 7;
        [SerializeField]
        private float _fallGravityMultiplier = 1;
        [SerializeField]
        private float _stopJumpGravityMultiplier = 2;
        [SerializeField]
        private float _rotationSpeed = 10;


        private Character _char;
        private CharacterController _charController;
        // TODO: Add input buffer
        private PlayerInputState _input;
        private Vector3 _velocity;
        private float _gravity;
        private float _initialJumpVelocity;
        private float _minJumpVelocityThreshold;
        private bool _stopJump;
        private Quaternion _targetRotation = Quaternion.identity;
        private bool _isKnockedDown;
        private float _moveAfterHitTimer = 0;
        private bool _isLunging;
        private Vector3 _lungeDirection;

        private void OnValidate()
        {
            CalculateJumpParameters();
        }

        // Use this for initialization
        private void Start()
        {
            _charController = GetComponent<CharacterController>();
            _char = GetComponent<Character>();
            _input = new PlayerInputState();
            _velocity = Vector3.zero;
            _char.OnKnockdownEvent.AddListener(OnKnockDownCallback);
            _char.OnGetUpEvent.AddListener(OnGetUpCallback);
            _char.OnHitDeliveredEvent.AddListener(OnHitDeliveredCallback);
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isKnockedDown)
            {
                return;
            }

            _input.UpdateInput();

            if (_input.SingleFrame.Jump == ButtonState.DOWN)
            {
                Jump();
            }
            else if (_input.SingleFrame.Jump == ButtonState.UP)
            {
                _stopJump = true;
            }

            if (_input.SingleFrame.Attack == ButtonState.DOWN)
            {
                Attack();
            }
            _char.Velocity = _velocity;
        }

        private void Attack()
        {
            Vector3 halfBoxExtents = new Vector3(2f, 0.5f, 1f);
            DebugExtension.DebugLocalCube(transform.localToWorldMatrix, halfBoxExtents * 2, Color.red, Vector3.zero + Vector3.forward, 5);
            //DebugExtension.DebugBounds(new Bounds(transform.TransformPoint(Vector3.zero) + transform.forward * 2, halfBoxExtents * 2), Color.red, 5);
            Collider[] collidersInRange = Physics.OverlapBox(transform.TransformPoint(Vector3.zero) + transform.forward * 2, new Vector3(2, 1, 2), transform.rotation);
            foreach (Collider collider in collidersInRange)
            {
                if (collider.GetComponent<EnemyAI>() != null)
                {
                    Debug.Log("rotate to enemy");
                    transform.LookAt(collider.transform.position);
                    if ((_velocity.x != 0 || _velocity.y != 0) && _charController.isGrounded)
                    {
                        Lunge(collider.transform.position);
                    }
                }
            }
            _char.Attack();
        }

        private void Lunge(Vector3 target)
        {
            _lungeDirection = target - transform.position;
            _lungeDirection.y = 0;
            _lungeDirection.Normalize();
            _isLunging = true;
            _velocity = Vector3.zero;
            StartCoroutine(DisableLunge(0.3f));
        }

        private IEnumerator DisableLunge(float timeToWait)
        {
            yield return new WaitForSeconds(0.3f);
            _isLunging = false;
        }

        private void FixedUpdate()
        {
            if (_isKnockedDown)
            {
                return;
            }

            Vector3 motion = Vector3.zero;
            if (_isLunging)
            {
                _charController.Move(_lungeDirection * (Time.fixedDeltaTime * 30));
                if (Vector3.Distance(transform.position, _lungeDirection) <= 1f)
                {
                    _isLunging = false;
                }
            }
            else if (_moveAfterHitTimer <= 0)
            { 
                motion = CalculateXZMotion(_input.Continous.Horizontal, _input.Continous.Vertical);
                RotateTowardsMovement(_input.Continous.Horizontal, _input.Continous.Vertical);
            }
            else
            {
                motion = Vector3.zero;
                _moveAfterHitTimer -= Time.fixedDeltaTime;
                _velocity = Vector3.zero;
            }
            
            motion.y = CalculateYMotion();
            _charController.Move(motion);
            if (_charController.isGrounded)
            {
                _velocity.y = 0;
            }
        }

        private void CalculateJumpParameters()
        {
            // calculate by jumpHeight and horizontal jump distance
            // h = 0.5gt^2 + v0t
            // v = v0 + gt
            // h = v^2 / 2g -- Energy equation (h from peak of jump downwards)
            _gravity = (-2 * _jumpHeight * Mathf.Pow(_movementVelocity, 2)) / (Mathf.Pow(_jumpPeakHorizontalDistance, 2));
            _initialJumpVelocity = (2 * _jumpHeight * _movementVelocity) / _jumpPeakHorizontalDistance;
            _minJumpVelocityThreshold = Mathf.Sqrt((_jumpHeight - _minJumpHeight) * -2 * _gravity);
        }

        private void Jump()
        {
            if (_charController.isGrounded)
            {
                _stopJump = false;
                _velocity.y = _initialJumpVelocity;
                _char.Jump();
            }
        }

        private float CalculateYMotion()
        {
            float _verticalAcceleration = _gravity;

            if (_stopJump && _velocity.y <= _minJumpVelocityThreshold && _velocity.y > 0)
            {
                _verticalAcceleration *= _stopJumpGravityMultiplier;
            }
            else if (_velocity.y <= 0)
            {
                _verticalAcceleration *= _fallGravityMultiplier;
            }
            float yMotion = PhysicsExt.CalculateMovementDistance(_velocity.y, _verticalAcceleration, Time.fixedDeltaTime);
            _velocity.y = PhysicsExt.CalculateVelocity(_velocity.y, _verticalAcceleration, Time.fixedDeltaTime);
            return yMotion;
        }

        private Vector3 CalculateXZMotion(float horizontalAxis, float verticalAxis)
        {
            Vector3 motion = new Vector3(horizontalAxis, 0, verticalAxis);
            motion.Normalize();

            if (_charController.isGrounded)
            {
                motion *= _movementVelocity;
                _velocity.x = motion.x;
                _velocity.z = motion.z;
                motion *= Time.fixedDeltaTime;
            }
            else
            {
                if (horizontalAxis == 0 && verticalAxis == 0)
                {
                    return new Vector3(_velocity.x, 0, _velocity.z) * Time.fixedDeltaTime;
                }

                float xMotion = PhysicsExt.CalculateMovementDistance(_velocity.x, motion.x * _airControlAcceleration, Time.fixedDeltaTime);
                float zMotion = PhysicsExt.CalculateMovementDistance(_velocity.z, motion.z * _airControlAcceleration, Time.fixedDeltaTime);

                float normalizedMaxVelocity = Mathf.Max(Mathf.Abs(motion.x), Mathf.Abs(motion.z)) * _movementVelocity;
                float maxDistance = normalizedMaxVelocity * Time.fixedDeltaTime;
                xMotion = Mathf.Clamp(xMotion, -maxDistance, maxDistance);
                zMotion = Mathf.Clamp(zMotion, -maxDistance, maxDistance);

                _velocity.x = PhysicsExt.CalculateVelocity(_velocity.x, motion.x * _airControlAcceleration, Time.fixedDeltaTime);
                _velocity.z = PhysicsExt.CalculateVelocity(_velocity.z, motion.z * _airControlAcceleration, Time.fixedDeltaTime);
                _velocity.x = Mathf.Clamp(_velocity.x, -normalizedMaxVelocity, normalizedMaxVelocity);
                _velocity.z = Mathf.Clamp(_velocity.z, -normalizedMaxVelocity, normalizedMaxVelocity);

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

        private void OnKnockDownCallback()
        {
            _isKnockedDown = true;
        }

        private void OnGetUpCallback()
        {
            _isKnockedDown = false;
        }

        private void OnHitDeliveredCallback()
        {
            _moveAfterHitTimer = 0.1f;
            _isLunging = false;
            Debug.Log("Hit!");
        }

    }
}
