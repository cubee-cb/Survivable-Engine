using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  internal class Hitbox
  {
    protected bool active = true;
    protected Rectangle bounds = new();
    protected HitboxType type = HitboxType.TileCollision;


    public Hitbox(Rectangle bounds)
    {
      this.bounds = bounds;
    }

  }

  internal class HitboxCircle : Hitbox
  {
    public HitboxCircle(Rectangle bounds) : base(bounds)
    {
      this.bounds = bounds;
    }
    
    //todo: handle circles in hitbox collisions

  }

  public enum HitboxType
  {
    TileCollision = 0,
    SendDamage = 1,
    ReceiveDamage = 2,
  }

}
