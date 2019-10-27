using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

public class CameraDriverScript : MonoBehaviour
{
    private const float MOUSE_ROTATION_RATE = 240f;
    private const float TRANSLATION_DAMPING = 10f;
    private const float ROTATION_DAMPING = 15f;

    public void UpdateTransformRotationWithMouse(Transform target, float mouseY, float deltaTime)
    {
        Vector3 eulerAngles = transform.eulerAngles;
        if (mouseY != 0f)
        {
            eulerAngles.x += mouseY * MOUSE_ROTATION_RATE * Time.deltaTime;
            eulerAngles.x = Maths.AngleModulo360(eulerAngles.x);
            eulerAngles.x = Mathf.Clamp(eulerAngles.x, -15f, 90f);
        }
        transform.eulerAngles = eulerAngles;
    }

    public void FollowTargetUpdate(Transform target, float deltaTime)
    {
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.y = Mathf.LerpAngle(eulerAngles.y, target.eulerAngles.y, ROTATION_DAMPING * deltaTime);
        transform.eulerAngles = eulerAngles;
        transform.position = Vector3.Lerp(transform.position, target.position, TRANSLATION_DAMPING * deltaTime);
    }

    public void FollowTargetUpdate(Rigidbody rigidbody, float deltaTime)
    {
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.y = Mathf.LerpAngle(eulerAngles.y, rigidbody.rotation.eulerAngles.y, ROTATION_DAMPING * deltaTime);
        transform.eulerAngles = eulerAngles;
        transform.position = Vector3.Lerp(transform.position, rigidbody.position, TRANSLATION_DAMPING * deltaTime);
    }

}