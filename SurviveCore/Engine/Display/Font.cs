using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Display
{
  internal class Font
  {
    public string textureSheetName;

    public string glyphOrder = " abcdefghijklmnopqrstuvwxyz./-_()";
    public Dictionary<char, int> glyphSizeOverrides = new();
    public int glyphSize = 8;
    public int glyphGap = 0;


    public virtual void ReplaceData(Font source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves
      textureSheetName = source.textureSheetName ?? textureSheetName;

      glyphOrder = source.glyphOrder ?? glyphOrder;
      glyphSizeOverrides = source.glyphSizeOverrides ?? glyphSizeOverrides;
      glyphSize = source.glyphSize;
      glyphGap = source.glyphGap;
    }

    public override string ToString()
    {
      return "Font: " + textureSheetName;
    }

  }
}
