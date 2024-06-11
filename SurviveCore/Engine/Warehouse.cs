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
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using SurviveDesktop;

namespace SurviveCore.Engine
{
  // this class is not static. make sure to call the constructor before using any of its functions so the placeholder assets can be properly initialised.
  public class Warehouse
  {
    // paths are formatted as:                        /CONTENT_PATH/nameSpace/TEXTURE_PATH/
    // for example, the default values would become:  /assets/default/spr/
    // paths are relative to the executable
    const string CONTENT_PATH = "assets"; // the base path where assets will be stored

    static string nameSpace = "default"; // the subfolder the assets are stored in, for modding purposes

    const string TEXTURE_PATH = "spr";
    const string AUDIO_PATH = "sfx";
    const string MUSIC_PATH = "music";
    const string LUA_PATH = "lua";
    const string JSON_PATH = "json";

    static Texture2D missingTexture;
    static Texture2D missingSound;
    static Texture2D missingMusic;

    static Dictionary<string, Texture2D> textures;
    static Dictionary<string, SoundEffect> sounds;
    static Dictionary<string, Song> music;
    static Dictionary<string, string> jsonData;
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
      jsonData = new Dictionary<string, string>();
      luaScripts = new Dictionary<string, string>();

    }

    public static bool LoadTexture(string fileName)
    {
      string internalName = string.Join('.', nameSpace, fileName);

      // exit if the filename is blank
      if (fileName == "")
      {
        ELDebug.Log("tried to load an empty texture filename in " + internalName, error: true);
        return default;
      }

      // if the file exists, create a temporary blank texture so the thing is invisible while it loads
      // if it doesn't, exit early instead. GetTexture should handle nonexistent assets and return the missing texture instead
      if (true)
      {
        //return false;
      }

      string relativePath = string.Join('/', CONTENT_PATH, nameSpace, TEXTURE_PATH, fileName);

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

    /// <summary>
    /// Gets a json file and turns it into an object, and loads it if it isn't already.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the json file to.</typeparam>
    /// <param name="fileName">Name of the file to load, excluding the extension. Extension is implied to be .json</param>
    /// <returns>An object based on the type provided to the function.</returns>
    public static T GetJson<T>(string fileName)
    {
      string internalName = string.Join('.', nameSpace, fileName);

      // exit if the filename is blank
      if (fileName == "")
      {
        ELDebug.Log("tried to load an empty json filename in " + internalName, error: true);
        return default;
      }

      // try to load the file
      if (!jsonData.ContainsKey(internalName))
      {
        // load json file content
        string relativePath = string.Join('/', CONTENT_PATH, nameSpace, JSON_PATH, fileName + ".json");
        string jsonString = Platform.LoadContentFile(relativePath);

        // add it to the loaded json dictionary
        jsonData.Add(internalName, jsonString);

        ELDebug.Log("loaded json file " + internalName);
      }

      // find the file, process, and return it
      if (jsonData.ContainsKey(internalName))
      {
        string jsonString = jsonData[internalName];

        T thing = JsonConvert.DeserializeObject<T>(jsonString);
        return thing;
      }

      else
      {
        ELDebug.Log("failed to obtain json file " + internalName, error: true);
        return default;
      }

    }

    /// <summary>
    /// Gets a Lua file and turns it into an object, and loads it if it isn't already.
    /// The script is run once to define functions.
    /// </summary>
    /// <param name="fileName">Name of the file to load, excluding the extension. Extension is implied to be .lua</param>
    /// <returns>A Script built based on the file contents.</returns>
    public static Script GetLua(string fileName)
    {
      string internalName = string.Join('.', nameSpace, fileName);

      // exit if the filename is blank
      if (fileName == "")
      {
        ELDebug.Log("tried to load an empty lua filename in " + internalName, error: true);
        return default;
      }

      // try to load the file if it isn't already loaded
      if (!luaScripts.ContainsKey(internalName))
      {
        // load lua file content
        string relativePath = string.Join('/', CONTENT_PATH, nameSpace, LUA_PATH, fileName + ".lua");
        string luaString = Platform.LoadContentFile(relativePath);

        // add it to the loaded json dictionary
        luaScripts.Add(internalName, luaString);

        ELDebug.Log("loaded lua file " + internalName);
      }

      if (luaScripts.ContainsKey(internalName))
      {
        string luaString = luaScripts[internalName];

        // execute lua script and put it into a Script object
        Script script = new Script();
        script.DoString(luaString);
        return script;
      }

      else
      {
        ELDebug.Log("failed to obtain lua file " + internalName, error: true);
        return default;
      }

    }

    /// <summary>
    /// set the namespace the warehouse is working in. this is the same as the game/mod assets folder name
    /// </summary>
    /// <param name="newNameSpace">the namespace</param>
    public static void SetNameSpace(string newNameSpace)
    {
      nameSpace = newNameSpace;
    }

  }
}
