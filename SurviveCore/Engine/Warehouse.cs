using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
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
  public static class Warehouse
  {
    // paths are formatted as:                        /contentPath/nameSpace/TEXTURE_FOLDER/
    // for example, the default values would become:  /assets/default/spr/
    // paths are relative to the executable
    readonly private static string contentPath = "assetPacks"; // the base path where assets will be stored

    private static string nameSpace = "default"; // the subfolder the assets are stored in, for pack-loading purposes
    private static string currentCategory = "default";

    private const string TEXTURE_FOLDER = "spr";
    private const string SOUND_FOLDER = "sfx";
    private const string MUSIC_FOLDER = "music";
    private const string LUA_FOLDER = "lua";
    private const string JSON_FOLDER = "json";

    private const char NAMESPACE_SEPARATOR = '.';

    private static Texture2D missingTexture;
    private static SoundEffect missingSound;
    private static Song missingMusic;

    readonly private static Dictionary<string, Texture2D> textures = [];
    readonly private static Dictionary<string, SoundEffect> sounds = [];
    readonly private static Dictionary<string, Song> music = [];
    readonly private static Dictionary<string, string> jsonData = [];
    readonly private static Dictionary<string, string> luaScripts = [];

    private static GameProperties gameProps = null;
    readonly private static List<string> foundNamespaces = [];

    private static GraphicsDevice graphicsDevice;

    public static void SetGraphicsDevice(GraphicsDevice newGraphicsDevice)
    {
      graphicsDevice = newGraphicsDevice;
    }

    private static string[] GetContentPaths(bool prioritiseExternal = false)
    {
      if (prioritiseExternal)
      {
        return
        [
          Path.Combine(Platform.GetExternalPath(), contentPath),
          Path.Combine(Platform.GetBasePath(), contentPath)
        ];
      }

      return
      [
        Path.Combine(Platform.GetBasePath(), contentPath),
        Path.Combine(Platform.GetExternalPath(), contentPath)
      ];
    }

    /// <summary>
    /// Uses Content.Load to load the placeholder/fallback assets from the engine
    /// </summary>
    public static void LoadPlaceholders(ContentManager Content)
    {
      // load fallback content for warehouse, used when an asset cannot be found
      // content.Load is only used here for built-in engine content like placeholders.
      missingTexture = Content.Load<Texture2D>("spr/missing");
      missingSound = Content.Load<SoundEffect>("sfx/missing");
      missingMusic = Content.Load<Song>("music/missing");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onlyGame"></param>
    public static void LoadMod(string path)
    {
      // load content from subfolders
      foreach (string subfolder in Directory.GetDirectories(path))
      {
        currentCategory = Path.GetFileNameWithoutExtension(subfolder);
        if (Common.IsIgnorableDirectory(currentCategory))
        {
          ELDebug.Log($"subfolder {currentCategory} is ignorable, skipping");
          continue;
        }
        ELDebug.Log(currentCategory);

        LoadAssetsInFolder(Path.Join(subfolder, TEXTURE_FOLDER), LoadTexture);
        LoadAssetsInFolder(Path.Join(subfolder, SOUND_FOLDER), LoadSoundEffect);
        //LoadAssetsInFolder(Path.Join(subfolder, MUSIC_FOLDER), LoadSong);
        LoadAssetsInFolder(Path.Join(subfolder, JSON_FOLDER), LoadJson);
        LoadAssetsInFolder(Path.Join(subfolder, LUA_FOLDER), LoadLua);
      }
    }

    /// <summary>
    /// Loads a game. With nothing specified, it will load the first found game.
    /// </summary>
    /// <param name="desiredNameSpace">If specified, specifically load the game with this nameSpace.</param>
    public static void LoadGame(string desiredNameSpace = "")
    {
      //todo: make this an async task or something, so the game window can show a loading screen

      ELDebug.Log("searching for a game");
      foreach (string contentPath in GetContentPaths())
      {
        // skip if the directory doesn't exist
        if (!Directory.Exists(contentPath)) continue;

        // find all pack folders that have pack.json
        string[] packs = Directory.GetDirectories(contentPath);
        Array.Sort(packs);
        foreach (string packPath in packs)
        {
          ELDebug.Log("checking " + packPath);

          // skip if game.json doesn't exist
          if (!Platform.Exists(Path.Combine(packPath, "game.json")))
          {
            ELDebug.Log("this is not a game pack. skipping!");
            continue;
          }

          gameProps = GetJson<GameProperties>(LoadJson(Path.Combine(packPath, "game.json")));

          // skip if this isn't the game we're looking for
          if (!string.IsNullOrWhiteSpace(desiredNameSpace) && gameProps.nameSpace != desiredNameSpace)
          {
            ELDebug.Log("this is not the game pack we're looking for. skipping!");
            continue;
          }

          nameSpace = gameProps.nameSpace;
          foundNamespaces.Add(nameSpace);
          LoadMod(packPath);

        }

      }

      ELDebug.Log("=======================================");
    }

    /// <summary>
    /// Preloads all assets that can be found by Warehouse.
    /// </summary>
    public static void LoadAll()
    {
      //todo: make this an async task or something, so the game window can show a loading screen

      ELDebug.Log("loading content packs");
      foreach (string contentPath in GetContentPaths())
      {
        // skip if the directory doesn't exist
        if (!Directory.Exists(contentPath)) continue;

        // find all pack folders that have pack.json
        foreach (string packPath in Directory.GetDirectories(contentPath))
        {
          ELDebug.Log("checking " + packPath);

          // skip if pack.json doesn't exist
          if (!Platform.Exists(Path.Combine(packPath, "pack.json")))
          {
            ELDebug.Log("this not an asset pack. skipping!");
            continue;
          }

          // load pack.json
          ModProperties packProps = JsonConvert.DeserializeObject<ModProperties>(Platform.LoadFileDirectly(Path.Combine(packPath, "pack.json")));
          ELDebug.Log("found pack: " + packProps);

          nameSpace = packProps.nameSpace;
          foundNamespaces.Add(nameSpace);

          LoadMod(packPath);


        }

      }

      ELDebug.Log("=======================================");
    }

    public static void UnloadAll()
    {
      // existing objects will turn black rather than missing texture due to storing their own
      // texture reference after loading
      ELDebug.Log("unloading all asset packs");

      foreach (KeyValuePair<string, Texture2D> kvp in textures)
      {
        kvp.Value.Dispose();
        ELDebug.Log("unloaded texture " + kvp.Key);
      }
      textures.Clear();

      foreach (KeyValuePair<string, SoundEffect> kvp in sounds)
      {
        kvp.Value.Dispose();
        ELDebug.Log("unloaded sound effect " + kvp.Key);
      }
      sounds.Clear();

      foreach (KeyValuePair<string, Song> kvp in music)
      {
        kvp.Value.Dispose();
        ELDebug.Log("unloaded music track " + kvp.Key);
      }
      music.Clear();

      jsonData.Clear();
      ELDebug.Log("cleared json data");

      luaScripts.Clear();
      ELDebug.Log("cleared lua scripts");

    }

    /// <summary>
    /// Takes in a file path, and converts that into an object id of format "namespace.category.id". If a namespace is provided in the file name as "namespace.id", that will be used for this object. Otherwise, the active namespace will be used.
    /// </summary>
    /// <param name="file">The path to the original file.</param>
    /// <returns>An object id.</returns>
    private static string BuildInternalName(string file)
    {
      // get just the filename
      //todo: do we want this converted to lowercase? camelcase to underscores? do we care what style atrocities pack authors commit?
      file = Path.GetFileNameWithoutExtension(file);

      // if the filename has a namespace, use that namespace.
      // handy for having packs loaded later override content in already loaded packs, or inject content into other namespaces
      if (file.Contains(NAMESPACE_SEPARATOR))
      {
        string[] splitName = file.Split(NAMESPACE_SEPARATOR);

        // use the first part as the namespace, and the last as the id.
        // we can ignore extensions since we already removed them at the eginning of this method.
        return string.Join(NAMESPACE_SEPARATOR, splitName[0], currentCategory, splitName[^1]);
      }
      // if it's missing a namespace, use the active namespace.
      // handy for if you want to easily change the pack's namespace later for whatever reason.
      else
      {
        return string.Join(NAMESPACE_SEPARATOR, nameSpace, currentCategory, file);
      }

    }

    /// <summary>
    /// Scans a folder for files, and passes their path to the loadMethod().
    /// </summary>
    /// <param name="loadMethod">The method to use to import the asset when found.</param>
    public static void LoadAssetsInFolder(string basePath, Func<string, string> loadMethod)
    {
      string path = Path.Join(basePath);
      // skip this folder if it doesn't exist
      if (!Directory.Exists(path)) return;

      // try to load all the files in the folder
      foreach (string file in Directory.GetFiles(path))
      {
        try
        {
          string internalName = loadMethod(file);

          // the loadMethod handles its own output
          //ELDebug.Log("loaded " + subfolder + " " + internalName);
        }
        catch
        {
          ELDebug.Log(" failed to load " + basePath + " for category " + currentCategory + ". wrong file type?", category: ELDebug.Category.ERROR);
        }
      }
    }

    private static string LoadTexture(string filePath)
    {
      string internalName = BuildInternalName(filePath);

      if (Platform.Exists(filePath))
      {
        FileStream stream = new(filePath, FileMode.Open);
        Texture2D loadedTexture = Texture2D.FromStream(graphicsDevice, stream);
        stream.Dispose(); //DisposeAsync();

        // replace loaded asset if it already exists
        if (textures.ContainsKey(internalName))
        {
          textures[internalName] = loadedTexture;
        }
        else
        {
          textures.Add(internalName, loadedTexture);
        }

        ELDebug.Log("loaded texture file: " + internalName);
      }

      return internalName;
    }

    /// <summary>
    /// Gets a texture from the stored assets.
    /// </summary>
    /// <param name="fileName">Name of the texture to get.</param>
    /// <returns>The texture that was found, or the missing texture if not.</returns>
    public static Texture2D GetTexture(string internalName)
    {
      internalName = ProcessWildcard(internalName, textures);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(internalName))
      {
        ELDebug.Log("got an empty texture reference", category: ELDebug.Category.Warning);
        return missingTexture;
      }

      // find the loaded texture and return it
      if (textures.ContainsKey(internalName))
      {
        return textures[internalName];
      }
      else
      {
        ELDebug.Log("failed to obtain texture " + internalName, category: ELDebug.Category.Warning);
        return missingTexture;
      }
    }


    private static string LoadSoundEffect(string filePath)
    {
      string internalName = BuildInternalName(filePath);

      if (Platform.Exists(filePath))
      {
        FileStream stream = new(filePath, FileMode.Open);
        SoundEffect loadedSound = SoundEffect.FromStream(stream);
        stream.Dispose(); //DisposeAsync();

        // replace loaded asset if it already exists
        if (sounds.ContainsKey(internalName))
        {
          sounds[internalName] = loadedSound;
        }
        else
        {
          sounds.Add(internalName, loadedSound);
        }

        ELDebug.Log("loaded sound file " + internalName);
      }

      return internalName;
    }

    /// <summary>
    /// Gets a sound effect from the stored assets.
    /// </summary>
    /// <param name="fileName">Name of the sound to load.</param>
    /// <returns>The sound that was found, or the missing sound if not.</returns>
    public static SoundEffect GetSoundEffect(string internalName)
    {
      internalName = ProcessWildcard(internalName, sounds);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(internalName))
      {
        ELDebug.Log("got an empty sound reference", category: ELDebug.Category.Warning);
        return missingSound;
      }

      // find the loaded sound and return it
      if (sounds.ContainsKey(internalName))
      {
        return sounds[internalName];
      }
      else
      {
        ELDebug.Log("failed to obtain sound " + internalName, category: ELDebug.Category.Warning);
        return missingSound;
      }
    }

    private static string LoadJson(string filePath)
    {
      string internalName = BuildInternalName(filePath);

      if (Platform.Exists(filePath))
      {
        // load json file content
        string jsonString = Platform.LoadFileDirectly(filePath).Replace("@", nameSpace + ".");

        // replace loaded asset if it already exists
        if (jsonData.ContainsKey(internalName))
        {
          jsonData[internalName] = jsonString;
        }
        else
        {
          jsonData.Add(internalName, jsonString);
        }

        ELDebug.Log("loaded json file " + internalName);
      }

      return internalName;
    }

    /// <summary>
    /// Gets a json file and turns it into an object.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the json file to.</typeparam>
    /// <param name="fileName">Name of the file to load.</param>
    /// <returns>An object deserialised from the json, based on the type provided to the function.</returns>
    public static T GetJson<T>(string internalName)
    {
      internalName = ProcessWildcard(internalName, jsonData);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(internalName))
      {
        ELDebug.Log("got an empty json reference", category: ELDebug.Category.Warning);
        return JsonConvert.DeserializeObject<T>("{}");
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
        ELDebug.Log("failed to obtain json file " + internalName, category: ELDebug.Category.Warning);
        return JsonConvert.DeserializeObject<T>("{}");
      }

    }


    private static string LoadLua(string filePath)
    {
      string internalName = BuildInternalName(filePath);

      if (Platform.Exists(filePath))
      {
        // load lua file content
        // also process lua namespaces, in case it wants to reference external files
        string luaString = Platform.LoadFileDirectly(filePath).Replace("@", nameSpace + ".");

        // replace loaded asset if it already exists
        if (luaScripts.ContainsKey(internalName))
        {
          luaScripts[internalName] = luaString;
        }
        else
        {
          luaScripts.Add(internalName, luaString);
        }

        ELDebug.Log("loaded lua file " + internalName);
      }

      return internalName;
    }

    /// <summary>
    /// Gets a Lua file and turns it into an object.
    /// The script is immediately run once to define functions and initialise variables.
    /// </summary>
    /// <param name="fileName">Name of the file to load.</param>
    /// <returns>A Script built based on the file contents.</returns>
    public static Script GetLua(string internalName)
    {
      internalName = ProcessWildcard(internalName, luaScripts);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(internalName))
      {
        ELDebug.Log("got an empty lua reference", category: ELDebug.Category.Warning);
        return default;
      }

      // find the loaded lua, process, and return it
      if (luaScripts.ContainsKey(internalName))
      {
        string luaString = luaScripts[internalName];

        // execute lua script and put it into a Script object
        Script script = new(CoreModules.Preset_SoftSandbox);

        // register common methods
        LuaCommon.Register(script);


        try
        {
          script.DoString(luaString);
        }
        catch (Exception e)
        {
          ELDebug.Log("LUA error: \n" + e, ELDebug.Category.ERROR);
          return default;
        }

        return script;
      }

      else
      {
        ELDebug.Log("failed to obtain lua file " + internalName, category: ELDebug.Category.Warning);
        return default;
      }

    }

    public static GameProperties GetGameProps()
    {
      return gameProps;
    }

    private static string ProcessWildcard<T>(string internalName, Dictionary<string, T> dict)
    {
      string processedName = internalName.Replace("*.", "");

      foreach (string name in foundNamespaces)
      {
        if (dict.ContainsKey(name + "." + processedName))
        {
          return name + "." + processedName;

        }
      }

      return internalName;
    }




  }
}
