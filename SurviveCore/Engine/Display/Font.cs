using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Display
{
  public class Font : IHasAssets
  {
    public string textureSheetName = "default_font";
    public Texture2D texture;

    public string glyphOrder = " abcdefghijklmnopqrstuvwxyz./-_()";
    public Dictionary<char, int> glyphSizeOverrides = new();
    public int glyphSize = 8;
    public int glyphGap = 0;

    public Font()
    {
      //todo: deserialisation calls the constructor before populating the fields, so they use the default value
      //UpdateAssets();
    }

    /// <summary>
    /// Called when this object should re-obtain its assets.
    /// </summary>
    /// <param name="id">The string id of the object. Handled on a per-subclass basis.</param>
    public void UpdateAssets()
    {
      // load assets
      texture = Warehouse.GetTexture(textureSheetName);
    }

    public virtual void ReplaceData(Font source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves
      textureSheetName = source.textureSheetName ?? textureSheetName;
      texture = source.texture ?? texture;

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
