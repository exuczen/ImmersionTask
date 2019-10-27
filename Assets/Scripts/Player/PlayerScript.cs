using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private PlayerTurretScript _turret = default;
    [SerializeField]
    private PlayerChassisScript _chassis = default;

    private PlayerControllerScript _controller = default;
    private PlayerFlyControllerScript _flyController = default;
    private Rigidbody _rigidbody = default;

    public PlayerTurretScript Turret { get => _turret; }
    public PlayerChassisScript Chassis { get => _chassis; }
    public PlayerControllerScript Controller { get => _controller; }
    public PlayerFlyControllerScript FlyController { get => _flyController; }
    public Rigidbody Rigidbody { get => _rigidbody; }

    private void Awake()
    {
        _controller = GetComponent<PlayerControllerScript>();
        _flyController = GetComponent<PlayerFlyControllerScript>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layer.Powerup && other.gameObject.activeSelf)
        {
            PowerupScript powerup = other.GetComponentInParent<PowerupScript>();
            PowerupHandlerScript powerupHandler = powerup.GetComponentInParent<PowerupHandlerScript>();
            powerupHandler.OnPlayerEnter(this, powerup);
        }
    }
}
