using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraColliderScript : MonoBehaviour
{
    private Camera _camera = default;
    private Vector3 _fixedLocalPosition = default;
    private Vector3 _fixedLocalEuler = default;

    private const float TRANSLATION_DAMPING = 5f;
    private const float ROTATION_DAMPING = 10f;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _fixedLocalPosition = transform.localPosition;
        _fixedLocalEuler = transform.localEulerAngles;
    }

    private void FixedUpdate()
    {
        Vector3 parentPosition = transform.parent.position;
        Vector3 fixedPosition = transform.parent.TransformPoint(_fixedLocalPosition);
        Vector3 destPosition;
        if (Physics.Linecast(parentPosition, fixedPosition, out RaycastHit hit, Layer.WallMask | Layer.FloorMask | Layer.StairsMask))
        {
            if (Vector3.Dot(hit.normal, Vector3.up) < -0.99f)
                destPosition = hit.point;
            else
                destPosition = new Vector3(hit.point.x, fixedPosition.y, hit.point.z);
        }
        else
        {
            destPosition = fixedPosition;
        }
        transform.position = Vector3.Lerp(transform.position, destPosition, Time.fixedDeltaTime * TRANSLATION_DAMPING);

        Vector3 localPosition = transform.localPosition;
        float destLocalEulerX = _fixedLocalEuler.x + Vector3.Angle(localPosition, _fixedLocalPosition) * 0.65f;
        Vector3 localEuler = transform.localEulerAngles;
        localEuler.x = Mathf.Lerp(localEuler.x, destLocalEulerX, Time.fixedDeltaTime * ROTATION_DAMPING);
        transform.localEulerAngles = localEuler;
    }
}
