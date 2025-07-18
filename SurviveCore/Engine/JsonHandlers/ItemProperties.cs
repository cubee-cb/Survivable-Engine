using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class ItemProperties
  {
    // descriptions
    public string texture = "item_default";
    public List<string> tags = new();

    public int framesX = 1; // used to animate the item over time
    public int framesY = 1; // used for the charging animation of charged items

    // stats
    public SwingType swingType = SwingType.slash;
    public SwingCurve swingCurve = SwingCurve.linear;
    public HoldType holdType = HoldType.none;

    public int chargeDuration = 0;
    public int swingDuration = 30;

    // lua
    public string lua;



    public ItemProperties()
    {
    }

    public void ReplaceData(ItemProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      // descriptions
      texture = source.texture ?? texture;
      tags = source.tags ?? tags;

      // stats
      swingType = source.swingType;
      swingCurve = source.swingCurve;
      holdType = source.holdType;

      chargeDuration = source.chargeDuration < 0 ? 0 : source.chargeDuration;
      swingDuration = source.swingDuration <= 0 ? 30 : source.swingDuration;

      // lua
      lua = source.lua ?? lua;

    }


    public enum SwingType
    {
      slash = 0,
      poke = 1,
      hold = 2,
      consume = 3,
      mallet = 4
    }

    public enum HoldType
    {
      none = 0,
      inFront = 1,
      shoulder = 2
    }

    public enum SwingCurve
    {
      linear = 0, // constant swing speed
      easeIn = 1, // speed up then stop abruptly
      easeInOut = 2, // speed up then slow down
      backtrack = 3 // swing both directions; for polearms, swing twice
    }



  }
}
