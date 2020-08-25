using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;
using MustHave;
using System.Linq;
using System;

public class PlayerControllerScript : MonoBehaviour
{
    [SerializeField] private TextMessageEvent _playerStateMessage = default;

    protected CameraDriverScript _cameraDriver = default;
    protected Rigidbody _rigidbody = default;

    private const float MOUSE_ROTATION_RATE = 4.8f;
    private const float TURN_SPEED = 2.0f;
    private const float TURN_ACCELERATION = 10f;
    private const float JUMP_HEIGHT = 1.35f;
    protected const float DRIVE_SPEED = 5f;
    protected const float DRIVE_ACCELERATION = 10f;

    protected float _turnSpeed = TURN_SPEED;
    protected float _driveSpeed = DRIVE_SPEED;
    protected float _jumpHeight = JUMP_HEIGHT;
    protected bool _isTouchingFloorPlane = default;
    protected bool _isTouchingFloor = default;
    protected Vector3 _driveVelocity = default;

    public Vector3 DriveVelocity { get => _driveVelocity; }

    protected virtual void OnAwake() { }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Camera camera = CameraUtils.MainOrCurrent;
        if (camera && camera.transform.parent)
            _cameraDriver = camera.GetComponentInParent<CameraDriverScript>();
        OnAwake();
    }

    private void Update()
    {
        SetPlayerStateMessageData();
    }

    protected void SetPlayerStateMessageData()
    {
        Vector3 localVelocity = transform.InverseTransformVector(_rigidbody.velocity);
        bool moving = Mathf.Abs(localVelocity.x) > float.Epsilon || Mathf.Abs(localVelocity.z) > float.Epsilon;
        _playerStateMessage.Data =
            "Moving: " + moving +
            "\nJumping: " + !_isTouchingFloorPlane +
            "\nVelocity: " + localVelocity.ToString("F2") +
            "\nAngular Velocity: " + _rigidbody.angularVelocity.ToString("F2");
    }

    private void UpdateTransformRotationWithMouse()
    {
        Vector3 localEuler = transform.localEulerAngles;
        localEuler.x = localEuler.z = 0f;
        float mouseY = 0f, mouseX;
        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            mouseY = Input.GetAxis("Mouse Y");
            mouseX = Input.GetAxis("Mouse X");
            localEuler.y -= mouseX * MOUSE_ROTATION_RATE;
        }
        transform.localEulerAngles = localEuler;
        if (_cameraDriver)
            _cameraDriver.UpdateTransformRotationWithMouse(mouseY, MOUSE_ROTATION_RATE);
    }

    private void JumpOnKeyDown(KeyCode keyCode)
    {
        if (_isTouchingFloorPlane && Input.GetKeyDown(keyCode))
        {
            //0=v-gt => t=v/g
            //h=v^2/g - 0.5*g(v/g)^2 = 0.5*v^2/g
            //v=sqrt(2*gh);
            float gravityDotUp = Vector3.Dot(transform.up, Physics.gravity);
            if (gravityDotUp < 0f)
            {
                float jumpVelocity = Mathf.Sqrt(-2f * _jumpHeight * gravityDotUp);
                _rigidbody.AddRelativeForce(0f, jumpVelocity, 0f, ForceMode.VelocityChange);
            }
        }
    }

    private float GetTurnVelocity()
    {
        float turnVelocity = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            turnVelocity -= _turnSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            turnVelocity += _turnSpeed;
        }
        return turnVelocity;
    }

    protected Vector3 GetDriveVelocity(Transform driverTransform)
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        float inputDenom = Mathf.Max(1f, Mathf.Sqrt(inputX * inputX + inputZ * inputZ));

        Vector3 driveVelocity = Vector3.zero;
        if (Mathf.Abs(inputX) > 0f)
        {
            driveVelocity += driverTransform.right * inputX * _driveSpeed / inputDenom;
        }
        if (Mathf.Abs(inputZ) > 0f)
        {
            driveVelocity += driverTransform.forward * inputZ * _driveSpeed / inputDenom;
        }
        return driveVelocity;
    }

    private void ResetRigidbodyRotationXZ()
    {
        Vector3 rigidbodyEuler = _rigidbody.rotation.eulerAngles;
        _rigidbody.rotation = Quaternion.Euler(0f, rigidbodyEuler.y, 0f);
    }

    protected virtual void UpdateRigidbodyVelocity(float deltaTime)
    {
        _driveVelocity = GetDriveVelocity(transform);
        if (_isTouchingFloor)
        {
            Vector3 upVelocity = Vector3.Dot(_rigidbody.velocity, transform.up) * transform.up;
            Vector3 planeVelocity = Vector3.Lerp(_rigidbody.velocity - upVelocity, _driveVelocity, deltaTime * DRIVE_ACCELERATION);
            _rigidbody.velocity = upVelocity + planeVelocity;
        }
        JumpOnKeyDown(KeyCode.Space);
    }

    private void UpdateRigidbodyAngularVelocity(float deltaTime)
    {
        Vector3 angularVelocity = _rigidbody.angularVelocity;
        angularVelocity.x = angularVelocity.z = 0f;
        angularVelocity.y = Mathf.Lerp(angularVelocity.y, GetTurnVelocity(), TURN_ACCELERATION * deltaTime);
        _rigidbody.angularVelocity = angularVelocity;
    }

    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        UpdateTransformRotationWithMouse();
        ResetRigidbodyRotationXZ();
        UpdateRigidbodyVelocity(fixedDeltaTime);
        UpdateRigidbodyAngularVelocity(fixedDeltaTime);
        if (_cameraDriver)
            _cameraDriver.FollowTargetUpdate(transform, fixedDeltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        int objectLayer = collision.gameObject.layer;
        if (objectLayer == Layer.Floor || objectLayer == Layer.Stairs)
        {
            _isTouchingFloor = true;
            if (!_isTouchingFloorPlane)
            {
                List<ContactPoint> contacts = collision.contacts.ToList();
                List<ContactPoint> floorPlaneContacts = contacts.FindAll(c => Vector3.Dot(c.normal, transform.up) > 0.99f);
                _isTouchingFloorPlane |= floorPlaneContacts.Count >= 2;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int objectLayer = collision.gameObject.layer;
        if (objectLayer == Layer.Floor || objectLayer == Layer.Stairs)
        {
            _isTouchingFloor = false;
            _isTouchingFloorPlane = false;
        }
    }

    public void SetSpeedMultiplier(float mlp)
    {
        _driveSpeed = mlp * DRIVE_SPEED;
        _turnSpeed = mlp * TURN_SPEED;
    }

    public void SetJumpHeightMultiplier(float mlp)
    {
        _jumpHeight = mlp * JUMP_HEIGHT;
    }
}
