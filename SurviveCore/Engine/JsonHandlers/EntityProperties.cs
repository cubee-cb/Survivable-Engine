using SurviveCore.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public abstract class EntityProperties
  {
    // descriptions
    public string textureSheetName = "entity_default";
    public List<string> tags = new();
    public int feetOffsetY = 2;
    public Dictionary<string, int> hitbox = new();

    public SpriteRotationType rotationType;
    public Dictionary<string, int> spriteDimensions = new();

    // stats
    public int maxHealth = 10;
    public float regenerationPerSecond = 1;

    public bool affectedByGravity = true;
    public float movementSpeedWalk = 1f;
    public float movementSpeedRun = 1.5f;

    public int inventorySize = 5;

    // lua
    public string lua;



    public EntityProperties()
    {
    }

    public virtual void ReplaceData(EntityProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      // descriptions
      textureSheetName = source.textureSheetName ?? textureSheetName;
      tags = source.tags ?? tags;
      feetOffsetY = source.feetOffsetY;
      hitbox = source.hitbox ?? hitbox;

      rotationType = source.rotationType;
      spriteDimensions = source.spriteDimensions ?? spriteDimensions;

      // stats
      maxHealth = source.maxHealth;
      regenerationPerSecond = source.regenerationPerSecond;

      movementSpeedWalk = source.movementSpeedWalk;
      movementSpeedRun = source.movementSpeedRun;

      inventorySize = source.inventorySize;

      // lua
      lua = source.lua ?? lua;

    }

  }
}
