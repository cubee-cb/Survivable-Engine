using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace SurviveCore.Engine
{
  // this class is not static. make sure to call the constructor before using any of its functions so the placeholder assets can be properly initialised.
  public class Warehouse
  {
    const string CONTENT_PATH = "assets";
    const string TEXTURE_PATH = "spr";
    const string AUDIO_PATH = "sfx";
    const string MUSIC_PATH = "music";
    const string LUA_PATH = "lua";

    static Texture2D missingTexture;
    static Texture2D missingSound;
    static Texture2D missingMusic;

    static Dictionary<string, Texture2D> textures;
    static Dictionary<string, SoundEffect> sounds;
    static Dictionary<string, Song> music;
    static Dictionary<string, string> luaScripts;


    public Warehouse(ContentManager content)
    {
      // content.Load should only be used for built-in engine content like placeholders, not for game/mod assets.
      missingTexture = content.Load<Texture2D>("tex/missing");
      //missingSound = content.Load<Texture2D>("sfx/missing");
      //missingMusic = content.Load<Texture2D>("music/missing");

      textures = new Dictionary<string, Texture2D>();
      sounds = new Dictionary<string, SoundEffect>();
      music = new Dictionary<string, Song>();
      luaScripts = new Dictionary<string, string>();

    }

    public static bool LoadTexture(string fileName)
    {
      // if the file exists, create a temporary blank texture so the thing is invisible while it loads
      // if it doesn't, exit early instead. GetTexture should handle nonexistent assets and return the missing texture instead
      if (true)
      {
        //return false;
      }

      string relativePath = string.Join('/', CONTENT_PATH, TEXTURE_PATH);

      // make a thread to load files in the background?
      // need to figure out how threads work
      //Thread thread = new Thread(new ThreadStart(ThreadedLoadTexture));

      // call platform-specific code here
      textures.Add(fileName, missingTexture);

      return true;
    }

    public static Texture2D GetTexture(string fileName)
    {
      if (textures.ContainsKey(fileName))
      {
        return textures[fileName];
      }

      return missingTexture;
    }

    public static string GetJson<T>()
    {

      return jsonString;
    }

  }
}
