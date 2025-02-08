using System;
using System.Collections.Generic;
using System.Text;
using SurviveCore.Engine.Items;

namespace SurviveCore.Engine.UI
{


  public abstract class UILayout
  {
    // descriptions
    public string texture = "ui_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    public List<UISlot> slots;


    public int inventorySize = 10;
    public Inventory inventory = new();

    // lua
    public string lua;



    public UILayout()
    {
    }

    public virtual void ReplaceData(UILayout source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      // descriptions
      texture = source.texture ?? texture;
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
