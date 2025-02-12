using System;
using System.Collections.Generic;
using System.Text;
using SurviveCore.Engine.Items;
using SurviveCore.Engine.UI;

namespace SurviveCore.Engine.JsonHandlers
{


  public class UIProperties
  {
    // descriptions
    public string texture = "ui_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    public List<UISlot> slots;


    public int inventorySize = 10;
    readonly private Inventory inventory;

    // lua
    public string lua;



    public UIProperties()
    {
      inventory = new(inventorySize);

    }

    public virtual void ReplaceData(UIProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      // descriptions
      texture = source.texture ?? texture;
      tags = source.tags ?? tags;

      inventorySize = source.inventorySize;

      // lua
      lua = source.lua ?? lua;

    }

  }
}
