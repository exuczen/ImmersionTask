using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private int _fullHealthPoints = default;
    [SerializeField] private TextMesh _healthTextMesh = default;
    [SerializeField] private ParticleSystem _hitParticlesHemispherePrefab = default;

    private int _healthPoints = default;
    private Material _material = default;
    private EnemyHandlerScript _enemyHandler = default;
    private bool _isTouchingFloor = default;

    private int Health { get => 100 * _healthPoints / _fullHealthPoints; }
    public int FullHealthPoints { get => _fullHealthPoints; }

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        _enemyHandler = GetComponentInParent<EnemyHandlerScript>();
    }
    private void Start()
    {
        _material.color = Color.green;
        _healthPoints = _fullHealthPoints;
        _healthTextMesh.text = Health + "%";
    }

    private void Update()
    {
        if (transform.position.y < 0f)
        {
            DestroyGameObject();
        }
    }

    private void DestroyGameObject()
    {
        _enemyHandler.CreateEnemyAtRandomCell(_fullHealthPoints);
        InstantiateDeathParticles();
        Destroy(gameObject);
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _isTouchingFloor);
        DestroyGameObject();
    }

    private void TakeDamage(int damageHP)
    {
        int prevHealth = Health;
        _healthPoints = Mathf.Max(0, _healthPoints - damageHP);
        int health = Health;
        if (prevHealth > 50 && health <= 50 && health > 0)
        {
            _material.color = Color.yellow;
        }
        else if (prevHealth > 0 && health <= 0)
        {
            _material.color = Color.red;
            GetComponent<EnemyAgentScript>().enabled = false;
            StartCoroutine(DestroyRoutine());
        }
        _healthTextMesh.text = health + "%";
    }

    public void OnBulletCollisionEnter(BulletScript bullet, Vector3 collisionPoint)
    {
        if (_healthPoints > 0)
            TakeDamage(bullet.DamageHP);
        InstantiateHitParticles(collisionPoint);
    }

    private ParticleSystem InstantiateDeathParticles()
    {
        ParticleSystem particles = InstantiateHitParticles(transform.position);
        var burst = particles.emission.GetBurst(0);
        burst.count = _fullHealthPoints / 2;
        particles.emission.SetBurst(0, burst);
        return particles;
    }

    private ParticleSystem InstantiateHitParticles(Vector3 position)
    {
        ParticleSystem particles = Instantiate(_hitParticlesHemispherePrefab, position, Quaternion.identity, _enemyHandler.ParticlesContainer);
        var particlesModule = particles.main;
        particlesModule.startColor = new ParticleSystem.MinMaxGradient(Color.white, _material.color);
        return particles;
    }

    private void OnCollisionStay(Collision collision)
    {
        int objectLayer = collision.gameObject.layer;
        if (objectLayer == Layer.Floor || objectLayer == Layer.Stairs)
        {
            _isTouchingFloor = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int objectLayer = collision.gameObject.layer;
        if (objectLayer == Layer.Floor || objectLayer == Layer.Stairs)
        {
            _isTouchingFloor = false;
        }
    }
}