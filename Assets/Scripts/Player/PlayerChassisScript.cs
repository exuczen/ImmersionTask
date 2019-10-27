using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChassisScript : MonoBehaviour
{
    private PlayerControllerScript _player = default;
    private Transform _playerTransform = default;

    private void Awake()
    {
        _playerTransform = transform.parent;
        _player = _playerTransform.GetComponent<PlayerControllerScript>();
    }

    private void Update()
    {
        if (_player.DriveVelocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(_player.DriveVelocity, _playerTransform.up);
        }
    }
}
