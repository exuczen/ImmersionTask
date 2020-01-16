using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MustHave;

public class ElevatorScript : MonoBehaviour
{
    [SerializeField] private float _groundFloorY = default;
    [SerializeField] private float _firstFloorY = default;

    private const float CONTACT_DURATION = 1f;
    private const float SHIFT_DURATION = 2f;
    private const float WAIT_DURATION = 3f;

    private float _contactStartTime = default;
    private bool _isShifting = default;
    private bool _isInContact = default;
    private Rigidbody _rigidbody = default;

    private float _shiftStartTime = default;
    private float _shiftBegPosY = default;
    private float _shiftEndPosY = default;

    private bool IsOnFirstFloor { get => transform.localPosition.y > _firstFloorY - 0.0001f; }
    private bool IsOnGroundFloor { get => transform.localPosition.y < _groundFloorY + 0.0001f; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void StartShifting()
    {
        if (!_isShifting)
        {
            Vector3 localPosition = transform.localPosition;
            if (IsOnGroundFloor)
                _shiftEndPosY = _firstFloorY;
            else if (IsOnFirstFloor)
                _shiftEndPosY = _groundFloorY;
            else
                return;
            _shiftBegPosY = localPosition.y;
            _shiftStartTime = Time.time;
            _isShifting = true;
        }
    }

    private void FixedUpdate()
    {
        if (_isShifting)
        {
            Vector3 position = _rigidbody.position;
            float elapsedTime = Time.time - _shiftStartTime;
            if (elapsedTime < SHIFT_DURATION)
            {
                float shift = Maths.GetTransition(TransitionType.COS_IN_PI_RANGE, elapsedTime, SHIFT_DURATION);
                position.y = Mathf.Lerp(_shiftBegPosY, _shiftEndPosY, shift);
            }
            else
            {
                position.y = _shiftEndPosY;
                if (elapsedTime >= SHIFT_DURATION + WAIT_DURATION)
                    _isShifting = false;
            }
            _rigidbody.MovePosition(position);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int collisionObjectLayer = collision.gameObject.layer;
        if (collisionObjectLayer == Layer.Player)
        {
            if (!_isInContact)
            {
                List<ContactPoint> contacts = collision.contacts.ToList();
                List<ContactPoint> floorContacts = contacts.FindAll(c => Vector3.Dot(-c.normal, transform.up) > 0.99f);
                if (floorContacts.Count >= 2)
                {
                    _isInContact = true;
                    _contactStartTime = Time.time;
                }
            }
            else if (!_isShifting && Time.time - _contactStartTime > CONTACT_DURATION)
            {
                StartShifting();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == Layer.Player)
        {
            _isInContact = false;
        }
    }
}
