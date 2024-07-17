using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
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
    private static string contentPath = "assetPacks"; // the base path where assets will be stored

    private static string nameSpace = "default"; // the subfolder the assets are stored in, for packding purposes
    private static string currentCategory = "default";

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

    private const string FOLDER_COMMON = "common";
    private const string FOLDER_CHARACTER = "character";
    private const string FOLDER_GROUND = "ground";
    private const string FOLDER_TILE = "tile";
    private const string FOLDER_DIMENSION = "dimension";
    private const string FOLDER_BIOME = "biome";
    private const string FOLDER_ITEM = "item";
    private const string FOLDER_MOB = "mob";
    private const string FOLDER_WORLDGEN = "worldgen";
    readonly static private List<string> contentTypeSubfolders = new()
    {
      FOLDER_COMMON,
      FOLDER_CHARACTER,
      FOLDER_GROUND,
      FOLDER_TILE,
      FOLDER_DIMENSION,
      FOLDER_BIOME,
      FOLDER_ITEM,
      FOLDER_MOB,
      FOLDER_WORLDGEN
    };

    private static GameProperties gameProps = null;

    private static List<string> contentPaths = new()
    {
      Path.Combine(Platform.BASE_FOLDER, contentPath),
      Path.Combine(Platform.EXTERNAL_FOLDER, contentPath)
    };

    private static GraphicsDevice graphicsDevice;


    public Warehouse(ContentManager content, GraphicsDevice outerGraphicsDevice)
    {
      // load fallback content, used when an asset cannot be found
      // content.Load should only be used here for built-in engine content like placeholders, not for the per-game/pack assets.
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
    /// Preloads all assets that can be found by Warehouse.
    /// </summary>
    public static void LoadAll()
    {
      //todo: make this an async task or something, so the game window can show a loading screen

      ELDebug.Log("loading content packs");
      foreach (string contentPath in contentPaths)
      {
        // skip if the directory doesn't exist
        if (!Directory.Exists(contentPath)) continue;

        // find all pack folders that have pack.json
        List<string> packPaths = new(Directory.GetDirectories(contentPath));

        foreach (string packPath in packPaths)
        {
          ELDebug.Log("checking " + packPath);

          // skip this pack if the pack.json doesn't exist
          if (!Platform.Exists(Path.Combine(packPath, "pack.json"))) continue;

          // load pack.json
          ModProperties packProps = JsonConvert.DeserializeObject<ModProperties>(Platform.LoadFileDirectly(Path.Combine(packPath, "pack.json")));
          ELDebug.Log("found pack: " + packProps);

          nameSpace = packProps.nameSpace;

          // check if this mod is a game
          if (Platform.Exists(Path.Combine(packPath, "game.json")))
          {
            if (gameProps != null)
            {
              ELDebug.Log("a game pack has already been loaded. only content will be loaded from this pack - some may be inaccessible or cause conflicts!", category: ELDebug.Category.Warning);
            }
            else
            {
              gameProps = GetJson<GameProperties>(LoadJson(Path.Combine(packPath, "game.json")));
              ELDebug.Log("this is a game pack, using it for game data");
            }
          }

          // load content from folders
          foreach (string contentType in contentTypeSubfolders)
          {
            ELDebug.Log(contentType);
            currentCategory = contentType;
            string categoryPath = Path.Join(packPath, currentCategory);

            LoadAssetsInFolder(Path.Join(categoryPath, TEXTURE_FOLDER), LoadTexture);
            LoadAssetsInFolder(Path.Join(categoryPath, SOUND_FOLDER), LoadSoundEffect);
            //LoadAssetsInFolder(Path.Join(categoryPath, MUSIC_FOLDER), LoadSong);
            LoadAssetsInFolder(Path.Join(categoryPath, JSON_FOLDER), LoadJson);
            LoadAssetsInFolder(Path.Join(categoryPath, LUA_FOLDER), LoadLua);
          }


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
        stream.DisposeAsync();

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
        stream.DisposeAsync();

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
        // plus a quick, hacky way of handling namespace wildcards
        // wildcard is replaced with the current namespace, typically the pack's namespace
        // example, with two packs "foo" and "bar":
        // foo may refer to its content as either "foo.thing" or "*.thing"
        // if bar references "*.thing", it gets "bar.thing" unless it specifies "foo.thing"
        // if foo wants to become "xyzzy", it can do so without breaking its own content -
        // - as long as it uses "*.thing" instead of "foo.thing".
        // however, bar needs to be updated to continue using "xyzzy.thing".
        string jsonString = Platform.LoadFileDirectly(filePath).Replace("*", nameSpace);

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
        string luaString = Platform.LoadFileDirectly(filePath).Replace("*", nameSpace);

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





  }
}
