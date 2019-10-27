using UnityEngine;
using MustHave.Utilities;

public class FlyPowerupScript : PowerupScript
{
    private void Start()
    {
        _logPrefix = "+" + _logName + " : ";
    }

    protected override void OnPowerupEnd(PlayerScript player)
    {
        player.Controller.enabled = true;
        player.FlyController.enabled = false;
    }

    protected override void OnPowerupStart(PlayerScript player)
    {
        this.StartCoroutineActionAfterPredicate(() => {
            player.Controller.enabled = false;
            player.FlyController.enabled = true;
        }, () => Vector3.Dot(player.Rigidbody.velocity, Physics.gravity) < -0.0001f);
    }
}
