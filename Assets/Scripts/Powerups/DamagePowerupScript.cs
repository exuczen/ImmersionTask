using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MustHave.Utilities;

public class DamagePowerupScript : PowerupScript
{
    protected override void OnPowerupEnd(PlayerScript player)
    {
        player.Turret.SetFirearmsDamageFactor(1f);
    }

    protected override void OnPowerupStart(PlayerScript player)
    {
        player.Turret.SetFirearmsDamageFactor(_multiplier);
    }
}
