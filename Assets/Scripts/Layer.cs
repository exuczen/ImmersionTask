using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Layer
{
    public static readonly int Floor = LayerMask.NameToLayer("Floor");
    public static readonly int Wall = LayerMask.NameToLayer("Wall");
    public static readonly int Powerup = LayerMask.NameToLayer("Powerup");
    public static readonly int Bullet = LayerMask.NameToLayer("Bullet");
    public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int EnvCube = LayerMask.NameToLayer("EnvCube");
    public static readonly int Stairs = LayerMask.NameToLayer("Stairs");

    public static readonly int FloorMask = LayerMask.GetMask("Floor");
    public static readonly int WallMask = LayerMask.GetMask("Wall");
    public static readonly int PowerupMask = LayerMask.GetMask("Powerup");
    public static readonly int BulletMask = LayerMask.GetMask("Bullet");
    public static readonly int EnemyMask = LayerMask.GetMask("Enemy");
    public static readonly int PlayerMask = LayerMask.GetMask("Player");
    public static readonly int EnvCubeMask = LayerMask.GetMask("EnvCube");
    public static readonly int StairsMask = LayerMask.GetMask("Stairs");
}
