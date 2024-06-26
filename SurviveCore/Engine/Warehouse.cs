﻿using System;
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
    private static string contentPath = "assetPacks"; // the base path where assets will be stored

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

    private const string FOLDER_CHARACTER = "character";
    private const string FOLDER_GROUND = "ground";
    private const string FOLDER_TILE = "tile";
    private const string FOLDER_DIMENSION = "dimension";
    private const string FOLDER_BIOME = "biome";
    private const string FOLDER_ITEM = "item";
    private const string FOLDER_MOB = "mob";
    readonly private List<string> contentTypeSubfolders = new()
    {
      FOLDER_CHARACTER,
      FOLDER_GROUND,
      FOLDER_TILE,
      FOLDER_DIMENSION,
      FOLDER_BIOME,
      FOLDER_ITEM,
      FOLDER_MOB
    };

    readonly private Dictionary<string, string> nameSpaceToDirectory = new();

    private static List<string> contentPaths = new()
    {
      Path.Combine(Platform.BASE_FOLDER, contentPath),
      Path.Combine(Platform.EXTERNAL_FOLDER, contentPath)
    };

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
    /// Preloads all assets that can be found by Warehouse.
    /// </summary>
    public void LoadAll()
    {

      foreach (string contentPath in contentPaths)
      {
        // skip if the directory doesn't exist
        if (!Directory.Exists(contentPath)) continue;

        // find all mod folders that have mod.json
        List<string> modPaths = new(Directory.GetDirectories(contentPath));

        foreach (string modPath in modPaths)
        {
          // skip this mod if the mod.json doesn't exist
          if (!Platform.Exists(Path.Combine(modPath, "mod.json"))) continue;

          // load mod.json
          ModProperties modProps = new(Platform.LoadFileDirectly(Path.Combine(modPath, "mod.json")));
          ELDebug.Log("found mod: " + modProps);

          nameSpace = modProps.nameSpace;

          // add namespace directory
          nameSpaceToDirectory.Add(nameSpace, modPath);

          // load content from folders
          foreach (string contentType in contentTypeSubfolders)
          {
            LoadAssetsInFolder(Path.Join(modPath, TEXTURE_FOLDER), contentType, LoadTexture);
            LoadAssetsInFolder(Path.Join(modPath, SOUND_FOLDER), contentType, LoadSoundEffect);
            //LoadAssetsInFolder(Path.Join(modPath, MUSIC_FOLDER), contentType, LoadSong);
            LoadAssetsInFolder(Path.Join(modPath, JSON_FOLDER), contentType, LoadJson);
            LoadAssetsInFolder(Path.Join(modPath, LUA_FOLDER), contentType, LoadLua);
          }

          //todo: each object should update all internal references with the namespace
          // if they don't, it won't know which namespace the file is from if it's omitted
          // alternatively, perform a "best-guess" by loading the first matching name from any namespace.

          // how to handle the different types? character, item, etc.
          // they're all in the same list, what if we have a tile and item both called Furnace?
          // whichever is loaded last will take precedence, overwriting the first one

        }




      }

      ELDebug.Log("=======================================");
    }

    /// <summary>
    /// Used to handle leaving out the namespace in file references. To reference a namespace in json, use namespace/item.name.png
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string BuildInternalName(string fileName)
    {
      // get just the filename
      //todo: do we want this converted to lowercase? camelcase to underscores? do we care what style atrocities mod authors commit?
      fileName = Path.GetFileNameWithoutExtension(fileName);

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
    /// Scans a folder for files, and passes their path to the loadMethod().
    /// </summary>
    /// <param name="loadMethod">The method to use to import the asset when found.</param>
    public static void LoadAssetsInFolder(string basePath, string subfolder, Func<string, string> loadMethod)
    {
      string path = Path.Join(basePath, subfolder);
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
          ELDebug.Log(" failed to load " + subfolder + " " + basePath + ". wrong file type?", error: true);
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
    public static Texture2D GetTexture(string fileName)
    {
      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log("empty texture reference in " + fileName, error: true);
        return missingTexture;
      }

      string internalName = BuildInternalName(fileName);

      // find the loaded texture and return it
      if (textures.ContainsKey(internalName))
      {
        return textures[internalName];
      }
      else
      {
        ELDebug.Log("failed to obtain texture " + fileName, error: true);
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
    public static SoundEffect GetSoundEffect(string fileName)
    {
      string internalName = BuildInternalName(fileName);

      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log("empty sound reference in " + fileName, error: true);
        return missingSound;
      }

      // find the loaded sound and return it
      if (sounds.ContainsKey(internalName))
      {
        return sounds[internalName];
      }
      else
      {
        ELDebug.Log("failed to obtain sound " + fileName, error: true);
        return missingSound;
      }
    }

    private static string LoadJson(string filePath)
    {
      string internalName = BuildInternalName(filePath);

      if (Platform.Exists(filePath))
      {
        // load json file content
        string jsonString = Platform.LoadFileDirectly(filePath);

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
    public static T GetJson<T>(string fileName)
    {
      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log("empty json reference in " + fileName, error: true);
        return default;
      }

      string internalName = BuildInternalName(fileName);

      // find the loaded json, process, and return it
      if (jsonData.ContainsKey(internalName))
      {
        string jsonString = jsonData[internalName];

        T thing = JsonConvert.DeserializeObject<T>(jsonString);
        return thing;
      }

      else
      {
        ELDebug.Log("failed to obtain json file " + fileName, error: true);
        return default;
      }

    }


    private static string LoadLua(string filePath)
    {
      string internalName = BuildInternalName(filePath);

      if (Platform.Exists(filePath))
      {
        // load json file content
        string luaString = Platform.LoadFileDirectly(filePath);

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
    public static Script GetLua(string fileName)
    {
      // exit if the filename is blank
      if (string.IsNullOrWhiteSpace(fileName))
      {
        ELDebug.Log("empty lua reference in " + fileName, error: true);
        return default;
      }

      string internalName = BuildInternalName(fileName);

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
        ELDebug.Log("failed to obtain lua file " + fileName, error: true);
        return default;
      }

    }

  }
}
