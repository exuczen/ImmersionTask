using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MustHave.Utilities;

public class SpeedPowerupScript : PowerupScript
{
    protected override void OnPowerupEnd(PlayerScript player)
    {
        PlayerControllerScript controller = player.Controller;
        controller.SetSpeedMultiplier(1f);
        controller.SetJumpHeightMultiplier(1f);
    }

    protected override void OnPowerupStart(PlayerScript player)
    {
        PlayerControllerScript controller = player.Controller;
        controller.SetSpeedMultiplier(_multiplier);
        controller.SetJumpHeightMultiplier(_multiplier);
    }
}
