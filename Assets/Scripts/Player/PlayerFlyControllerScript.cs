using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlyControllerScript : PlayerControllerScript
{
    protected override void OnAwake()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        _rigidbody.useGravity = false;
    }

    private void OnDisable()
    {
        _rigidbody.useGravity = true;
    }

    private float GetFlyUpSpeed()
    {
        float flyUpVelocity = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            flyUpVelocity = _driveSpeed * (Input.GetKey(KeyCode.LeftShift) ? -1f : 1f);
        }
        return flyUpVelocity;
    }

    protected override void UpdateRigidbodyVelocity(float deltaTime)
    {
        _driveVelocity = GetDriveVelocity(transform);

        float upSpeed = Vector3.Dot(_rigidbody.velocity, transform.up);
        Vector3 upVelocity = upSpeed * transform.up;
        Vector3 planeVelocity = Vector3.Lerp(_rigidbody.velocity - upVelocity, _driveVelocity, deltaTime * DRIVE_ACCELERATION);

        upSpeed = Mathf.Lerp(upSpeed, GetFlyUpSpeed(), deltaTime * DRIVE_ACCELERATION * 0.5f);
        upVelocity = upSpeed * transform.up;

        _rigidbody.velocity = upVelocity + planeVelocity;
    }
}