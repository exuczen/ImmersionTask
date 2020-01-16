using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave;
using MustHave.Utilities;

public class EnemyAgentScript : MonoBehaviour
{
    [SerializeField] private float _driveSpeed = default;
    [SerializeField] private Transform _leftSensor = default;
    [SerializeField] private Transform _rightSensor = default;

    private const float VELOCITY_DAMPING = 5f;

    private Transform _target = default;
    private Rigidbody _rigidbody = default;

    private void Awake()
    {
        _target = GetComponentInParent<EnemyHandlerScript>().Target;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 ray = _target.position - transform.position;
        //ray.y = 0;
        float raySqrLength = ray.sqrMagnitude;
        float fixedDeltaTranslation = _driveSpeed * Time.fixedDeltaTime;

        if (raySqrLength > 4f * Maths.PowF(fixedDeltaTranslation, 2))
        {
            _rigidbody.rotation = Quaternion.LookRotation(ray, _target.up);
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, GetDriveVelocity(ray), VELOCITY_DAMPING * Time.fixedDeltaTime);
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;

        }
        Vector3 euler = _rigidbody.rotation.eulerAngles;
        _rigidbody.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = true;
    }

    private bool GetClearRaycastHitTangentAxis(RaycastHit hit, int linecastMask, int sign, out Vector3 hitTangentAxis)
    {
        hitTangentAxis = sign * Vector3.Cross(hit.transform.up, hit.normal).normalized;
        bool leftTangentDetection = Physics.Raycast(_leftSensor.position, hitTangentAxis, out RaycastHit leftLeftHit, 1f, linecastMask);
        bool rightTangentDetection = Physics.Raycast(_rightSensor.position, hitTangentAxis, out RaycastHit rightLeftHit, 1f, linecastMask);
        bool leftClear = !leftTangentDetection || (leftTangentDetection && leftLeftHit.collider != hit.collider);
        bool rightClear = !rightTangentDetection || (rightTangentDetection && rightLeftHit.collider != hit.collider);
        return leftClear && rightClear;
    }

    private Vector3 GetDriveVelocity(Vector3 ray)
    {
        Camera camera = CameraUtils.MainOrCurrent;
        int linecastMask = Layer.WallMask | Layer.StairsMask;
        bool leftDetection = Physics.Linecast(_leftSensor.position, _target.position, out RaycastHit leftHit, linecastMask);
        bool rightDetection = Physics.Linecast(_rightSensor.position, _target.position, out RaycastHit rightHit, linecastMask);
        Vector3 velocity = Vector3.zero;
        if (!leftDetection && !rightDetection)
        {
            velocity = _driveSpeed * ray.normalized;
        }
        else
        {
            if (!leftDetection && rightDetection)
            {
                if (GetClearRaycastHitTangentAxis(rightHit, linecastMask, 1, out Vector3 hitTangentAxis))
                    velocity = _driveSpeed * hitTangentAxis;
            }
            else if (leftDetection && !rightDetection)
            {
                if (GetClearRaycastHitTangentAxis(leftHit, linecastMask, -1, out Vector3 hitTangentAxis))
                    velocity = _driveSpeed * hitTangentAxis;
            }
        }
        return velocity;
    }
}
