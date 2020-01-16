using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave;
using MustHave.Utilities;

public class PlayerTurretScript : MonoBehaviour
{
    [SerializeField] private BoolMessageEvent _enemyTargetMessage = default;
    [SerializeField] private PlayerFirearmScript[] _firearms = default;
    [SerializeField] private Transform _targetMark = default;
    [SerializeField] private float _raycastDistance = default;

    private int _raycastMask = default;

    private void Awake()
    {
        _raycastMask = Layer.EnemyMask | Layer.FloorMask | Layer.WallMask | Layer.StairsMask | Layer.EnvCubeMask;
    }

    public void SetFirearmsDamageFactor(float mlp)
    {
        foreach (var firearm in _firearms)
        {
            firearm.DamageFactor = mlp;
        }
    }

    private void FixedUpdate()
    {
        Ray camRay = CameraUtils.MainOrCurrent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, out RaycastHit hit, _raycastDistance, _raycastMask))
        {
            _targetMark.position = hit.point;

            _enemyTargetMessage.Data = hit.transform.gameObject.layer == Layer.Enemy;

            Vector3 ray = hit.point - transform.position;
            transform.rotation = Quaternion.LookRotation(ray, transform.parent.up);
            Vector3 localEuler = transform.localEulerAngles;
            localEuler.x = localEuler.z = 0f;
            transform.localEulerAngles = localEuler;

            foreach (var firearm in _firearms)
            {
                Transform arm = firearm.transform.parent;
                ray = hit.point - arm.position;

                arm.rotation = Quaternion.LookRotation(ray, transform.parent.up);

                localEuler = Maths.AnglesModulo360(arm.localRotation.eulerAngles);
                localEuler = Mathv.Clamp(localEuler, new Vector3(-180, -70, -180), new Vector3(180, 70, 180));
                arm.localRotation = Quaternion.Euler(localEuler);
            }
        }
    }
}
