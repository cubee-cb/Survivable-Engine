using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Items
{
  internal class Item : IdentifiableObject
  {

    [JsonIgnore] public ItemProperties properties;
    [JsonIgnore] private readonly Texture2D texture;
    [JsonIgnore] private readonly Script lua;

    public Item(string id)
    {

      // set initial properties
      properties = Warehouse.GetJson<ItemProperties>(id);

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      if (properties.sounds != null)
      {
        foreach (string fileName in properties.sounds)
        {
          Warehouse.GetSoundEffect(fileName);
        }
      }

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua)) lua = Warehouse.GetLua(properties.lua);
    }



  }
}
