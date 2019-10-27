using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _hitParticlesConePrefab = default;
    [SerializeField]
    private int _damageHP = default;

    public int DamageHP { get => _damageHP; }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        if (collision.gameObject.layer == Layer.Enemy)
        {
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
            enemy.OnBulletCollisionEnter(this, contact.point);
        }
        else
        {
            Instantiate(_hitParticlesConePrefab, contact.point, Quaternion.LookRotation(contact.normal), transform.parent);
        }
        this.StartCoroutineActionAfterFrames(() => {
            Destroy(gameObject);
        }, 1);
    }

    public BulletScript CreateInstance(Transform firearm, Rigidbody playerBody, Transform parent, float muzzleVelocity)
    {
        BulletScript bullet = Instantiate(this, firearm.position, firearm.rotation, parent);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        Vector3 bulletRay = bulletRB.position - firearm.parent.parent.position;
        Vector3 tangentialVelocity = Vector3.Cross(playerBody.angularVelocity, bulletRay);
        bulletRB.velocity = playerBody.velocity + tangentialVelocity + firearm.up * muzzleVelocity;
        bulletRB.angularVelocity = Vector3.zero;
        return bullet;
    }

    public void SetDamageHP(float mlp, float damageHP)
    {
        _damageHP = (int)(mlp * damageHP);
    }
}
