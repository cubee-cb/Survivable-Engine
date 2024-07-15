using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Items;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  internal class DroppedItem : Entity
  {
    Item itemData;

    public DroppedItem(Item item, World world) : base(item.id, world)
    {
      itemData = item;
      tags = item.properties.tags;

      velocity.X = (float)(Game1.rnd.NextDouble() * 2 - 1) * 3;
      velocity.Y = (float)(Game1.rnd.NextDouble() * 2 - 4);

      UpdateAssets();
    }

    public override void UpdateAssets()
    {
      itemData.UpdateAssets();
    }

    public override void Update(int tick, float deltaTime)
    {
      base.Update(tick, deltaTime);

      // fall around and bounce, etc...
      //world.properties.gravity;

    }


  }
}
