using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SurviveCore.Engine.JsonHandlers;
using SurviveCore.Engine.Lua;
using SurviveDesktop;

namespace SurviveCore.Engine
{
  // this class is not static. make sure to call the constructor before using any of its functions so the placeholder assets can be properly initialised.
  public class Warehouse
  {
    // paths are formatted as:                        /contentPath/nameSpace/TEXTURE_FOLDER/
    // for example, the default values would become:  /assets/default/spr/
    // paths are relative to the executable
    private static string CONTENT_PATH = "assetPacks"; // the base path where assets will be stored
    private static string currentContentPath = CONTENT_PATH;

    private static string nameSpace = "default"; // the subfolder the assets are stored in, for modding purposes

    private const string TEXTURE_FOLDER = "spr";
    private const string SOUND_FOLDER = "sfx";
    private const string MUSIC_FOLDER = "music";
    private const string LUA_FOLDER = "lua";
    private const string JSON_FOLDER = "json";

    private const char NAMESPACE_SEPARATOR = '.';

    public static Texture2D missingTexture;
    public static SoundEffect missingSound;
    public static Song missingMusic;

    private static Dictionary<string, Texture2D> textures;
    private static Dictionary<string, SoundEffect> sounds;
    private static Dictionary<string, Song> music;
    private static Dictionary<string, string> jsonData;
    private static Dictionary<string, string> luaScripts;

    private static Dictionary<string, string> namespaceToFolder = new();
    //private static Dictionary<string, string> folderToNamespace = new();

    private static GraphicsDevice graphicsDevice;


    public Warehouse(ContentManager content, GraphicsDevice outerGraphicsDevice)
    {
      // load fallback content, used when an asset cannot be found
      // content.Load should only be used here for built-in engine content like placeholders, not for the per-game/mod assets.
      missingTexture = content.Load<Texture2D>("spr/missing");
      missingSound = content.Load<SoundEffect>("sfx/missing");
      missingMusic = content.Load<Song>("music/missing");

      // set reference to the GraphicsDevice
      graphicsDevice = outerGraphicsDevice;

      textures = new Dictionary<string, Texture2D>();
      sounds = new Dictionary<string, SoundEffect>();
      music = new Dictionary<string, Song>();
      jsonData = new Dictionary<string, string>();
      luaScripts = new Dictionary<string, string>();
    }

    /// <summary>
    /// Builds a list of all the namespaces that can be found by Warehouse, along with their locations on disk, so it knows what game and mod assets are installed.
    /// This is useful in future to preload some definitions, like world types, biomes, and characters.
    /// </summary>
    public static List<string> FindAllPacks()
    {
      List<string> locations = new();

      locations = Platform.GetFolders(string.Join('/', currentContentPath));

      // load pack.json files
      foreach (string folder in locations)
      {
        string packJsonPath = Path.Combine(folder, "pack.json");

        if (Platform.Exists(packJsonPath))
        {
          AssetPackProperties properties = new(Platform.LoadContentFile(packJsonPath));

          namespaceToFolder.Add(properties.nameSpace, folder);
          //folderToNamespace.Add(folder, properties.nameSpace);

          ELDebug.Log("found pack at " + packJsonPath + " with namespace " + properties.nameSpace);
        }
        //AssetPackProperties properties = new AssetPackProperties(Platform.GetStream());
      }


      return locations;
    }

    /// <summary>
    /// Loads all assets that can be found by Warehouse.
    /// </summary>
    public static void LoadAll()
    {
    }

    /// <summary>
    /// Loads all assets that can be found by Warehouse from a specific folder.
    /// </summary>
    public static void LoadAllFrom()
    {
    }

    private static string NamespaceToPath(string nameSpace)
    {
      return namespaceToFolder[nameSpace];
    }

    /// <summary>
    /// set the namespace the warehouse is working in. this is the same as the game/mod assets folder name
    /// </summary>
    /// <param name="newNameSpace">the namespace</param>
    public static void SetNameSpace(string newNameSpace)
    {
      nameSpace = newNameSpace;
    }

    /// <summary>
    /// Used to handle leaving out the namespace in file references. To reference a namespace in json, use namespace/item.name.png
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string BuildInternalName(string fileName)
    {
      // if the filename has a namespace, use that.
      if (fileName.Contains(NAMESPACE_SEPARATOR))
      {
        return fileName;
      }
      // if it's missing a namespace, use the active namespace.
      // handy for if you want to easily change the mod's namespace later for whatever reason.
      else
      {
        return string.Join(NAMESPACE_SEPARATOR, nameSpace, fileName);
      }
    }

    /// <summary>
    /// Gets a texture from the stored assets, and tries to load it if it can't find it.
    /// </summary>
    /// <param name="fileName">Name of the file to load, excluding the extension. Extension is implied to be .png</param>
    /// <returns>The texture that was found, or the missing texture if not.</returns>
    public static Texture2D GetTexture(string fileName)
    {
      string internalName = BuildInternalName(fileName);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log(internalName + " contains an empty texture reference.", error: true);
        return missingTexture;
      }

      // if the file exists, create a temporary blank texture so the thing is invisible while it loads
      // if it doesn't, exit early instead. GetTexture should handle nonexistent assets and return the missing texture instead

      // make a thread to load files in the background?
      // need to figure out how threads work
      //Thread thread = new Thread(new ThreadStart(ThreadedLoadTexture));

      // try to load the file if it isn't already loaded
      string relativePath = Path.Join(NamespaceToPath(nameSpace), TEXTURE_FOLDER, fileName + ".png");
      if (!textures.ContainsKey(internalName) && Platform.Exists(relativePath))
      {
        Stream stream = Platform.GetStream(relativePath);
        Texture2D loadedTexture = Texture2D.FromStream(graphicsDevice, stream);
        stream.DisposeAsync();

        textures.Add(internalName, loadedTexture);

        ELDebug.Log("loaded texture file " + internalName);
      }

      // find the loaded texture and return it
      if (textures.ContainsKey(internalName))
      {
        return textures[internalName];
      }
      else
      {
        ELDebug.Log("failed to obtain texture at " + relativePath, error: true);
        return missingTexture;
      }
    }


    /// <summary>
    /// Gets a sound effect from the stored assets, and tries to load it if it can't find it.
    /// </summary>
    /// <param name="fileName">Name of the file to load, excluding the extension. Extension is implied to be .wav</param>
    /// <returns>The sound that was found, or the missing sound if not.</returns>
    public static SoundEffect GetSoundEffect(string fileName)
    {
      string internalName = BuildInternalName(fileName);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log(internalName + " contains an empty sound reference.", error: true);
        return missingSound;
      }

      // make a thread to load files in the background?
      // need to figure out how threads work
      //Thread thread = new Thread(new ThreadStart(ThreadedLoadTexture));

      // try to load the file if it isn't already loaded
      string relativePath = Path.Join(NamespaceToPath(nameSpace), SOUND_FOLDER, fileName + ".wav");
      if (!sounds.ContainsKey(internalName) && Platform.Exists(relativePath))
      {
        Stream stream = Platform.GetStream(relativePath);
        SoundEffect loadedSound = SoundEffect.FromStream(stream);
        stream.DisposeAsync();

        sounds.Add(internalName, loadedSound);

        ELDebug.Log("loaded sound file " + internalName);
      }

      // find the loaded texture and return it
      if (sounds.ContainsKey(internalName))
      {
        return sounds[internalName];
      }
      else
      {
        ELDebug.Log("failed to obtain sound at " + relativePath, error: true);
        return missingSound;
      }
    }

    /// <summary>
    /// Gets a json file and turns it into an object, and loads it if it isn't already.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the json file to.</typeparam>
    /// <param name="fileName">Name of the file to load, excluding the extension. Extension is implied to be .json</param>
    /// <returns>An object deserialised from the json, based on the type provided to the function.</returns>
    public static T GetJson<T>(string fileName)
    {
      string internalName = BuildInternalName(fileName);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log(internalName + " contains an empty json reference.", error: true);
        return default;
      }

      // try to load the file
      string relativePath = Path.Join(NamespaceToPath(nameSpace), JSON_FOLDER, fileName + ".json");
      if (!jsonData.ContainsKey(internalName) && Platform.Exists(relativePath))
      {
        // load json file content
        string jsonString = Platform.LoadContentFile(relativePath);

        // add it to the loaded json dictionary
        jsonData.Add(internalName, jsonString);

        ELDebug.Log("loaded json file " + internalName);
      }

      // find the loaded json, process, and return it
      if (jsonData.ContainsKey(internalName))
      {
        string jsonString = jsonData[internalName];

        T thing = JsonConvert.DeserializeObject<T>(jsonString);
        return thing;
      }

      else
      {
        ELDebug.Log("failed to obtain json file at " + relativePath, error: true);
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
      string internalName = BuildInternalName(fileName);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log(internalName + " contains an empty lua reference.", error: true);
        return default;
      }

      // try to load the file if it isn't already loaded
      string relativePath = Path.Join(NamespaceToPath(nameSpace), LUA_FOLDER, fileName + ".lua");
      if (!luaScripts.ContainsKey(internalName) && Platform.Exists(relativePath))
      {
        // load lua file content
        string luaString = Platform.LoadContentFile(relativePath);

        // add it to the loaded json dictionary
        luaScripts.Add(internalName, luaString);

        ELDebug.Log("loaded lua file " + internalName);
      }

      // find the loaded lua, process, and return it
      if (luaScripts.ContainsKey(internalName))
      {
        string luaString = luaScripts[internalName];

        // execute lua script and put it into a Script object
        Script script = new Script(CoreModules.Preset_SoftSandbox);

        // register common methods
        LuaCommon.Register(script);

        script.DoString(luaString);
        return script;
      }

      else
      {
        ELDebug.Log("failed to obtain lua file at " + relativePath, error: true);
        return default;
      }

    }

  }
}
