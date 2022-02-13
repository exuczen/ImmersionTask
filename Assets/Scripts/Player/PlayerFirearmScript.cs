using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;
using MustHave;
public class PlayerFirearmScript : MonoBehaviour
{
    [SerializeField] private BulletScript _bulletPrefab = default;
    [SerializeField] private Transform _bulletsContainer = default;
    [SerializeField, Range(0, 2)] private int _mouseButtonIndex = default;
    [SerializeField, Range(0f, 1f)] private float _shootingInterval = default;
    [SerializeField] private float _muzzleVelocity = default;

    private Rigidbody _playerRigidbody = default;
    private Coroutine _shootingRoutine = default;
    private bool _isShooting = default;
    private Vector3 _localPosition = default;
    private float _damageFactor = default;

    public float DamageFactor { set { _damageFactor = value; } }

    private void Awake()
    {
        _playerRigidbody = transform.GetComponentInParent<Rigidbody>();
        _localPosition = transform.localPosition;
    }

    private void Start()
    {
        _damageFactor = 1f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(_mouseButtonIndex))
        {
            StartShooting();
        }
        _isShooting = Input.GetMouseButton(_mouseButtonIndex);
    }

    private void StartShooting()
    {
        StopShootingRoutine();
        _isShooting = true;
        _shootingRoutine = StartCoroutine(ShootingRoutine());
    }

    private void StopShootingRoutine()
    {
        transform.localPosition = _localPosition;
        if (_shootingRoutine != null)
        {
            StopCoroutine(_shootingRoutine);
            _shootingRoutine = null;
        }
        _isShooting = false;
    }

    private IEnumerator ShootingRoutine()
    {
        while (_isShooting)
        {
            BulletScript bullet = _bulletPrefab.CreateInstance(transform, _playerRigidbody, _bulletsContainer, _muzzleVelocity);
            bullet.SetDamageHP(_damageFactor, _bulletPrefab.DamageHP);
            yield return CoroutineUtils.UpdateRoutine(_shootingInterval, (elapsedTime, transition) => {
                float translationZ = -0.15f * Maths.GetTransition(TransitionType.SIN_IN_PI_RANGE, transition);
                transform.localPosition = _localPosition;
                transform.Translate(0f, translationZ, 0f, Space.Self);
            });
            transform.localPosition = _localPosition;
        }
        _shootingRoutine = null;
    }
}
