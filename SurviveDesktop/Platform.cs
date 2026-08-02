using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SurviveCore.Engine;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurviveDesktop
{
  public static class Platform
  {
    public const string PLATFORM_NAME = "desktop";
    public const int MAX_SFX_INSTANCES = 256;

    //const string SAVE_NAME = "save.json";

    public static string GetBasePath()
    {
      return AppDomain.CurrentDomain.BaseDirectory;
    }

    public static string GetExternalPath()
    {
      string gameStorage = Path.Join("cubee", "surviveEngine");

      // use game's studio folder instead of mine if set
      GameProperties gameProps = Warehouse.GetGameProps();
      if (gameProps != null) gameStorage = Path.Join(gameProps.studioName, gameProps.nameSpace);

      return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), gameStorage);
    }

    /// <summary>
    /// Load a text file from the game root. i.e. pass @"Content/lua/textFile.lua".
    /// </summary>
    /// <param name="path">The path to a file.</param>
    /// <returns>The content of the file as a string.</returns>
    public static string LoadContentFile(string path)
    {
      if (Exists(path))
      {
        Stream stream = TitleContainer.OpenStream(path);

        string fileContents = "";
        using (StreamReader reader = new(stream))
        {
          fileContents = reader.ReadToEnd();
        }
        stream.Close();

        return fileContents;
      }

      else
      {
        ELDebug.Log("couldn't open stream: file \"" + path + "\" does not exist or is inaccessible.", ELDebug.Category.ERROR);
        return "<error>";
      }
    }

    /// <summary>
    /// Load a text file from the drive root. i.e. pass @"/home/user/game/lua/textFile.lua".
    /// </summary>
    /// <param name="path">The absolute path to a file.</param>
    /// <returns>The content of the file as a string.</returns>
    public static string LoadFileDirectly(string path)
    {
      if (Exists(path))
      {
        string fileContents = File.ReadAllText(path);

        return fileContents;
      }

      else
      {
        ELDebug.Log("couldn't open file: \"" + path + "\" does not exist or is inaccessible.", ELDebug.Category.ERROR);
        return "<error>";
      }
    }

    /// <summary>
    /// Wrapper for TitleContainer.GetStream(). Get a Stream to a file.
    /// </summary>
    /// <param name="path">The path to a file.</param>
    /// <returns>A Stream for the specified file.</returns>
    public static Stream GetStream(string path)
    {
      if (Exists(path))
      {
        return TitleContainer.OpenStream(path);
      }
      else
      {
        ELDebug.Log("couldn't open stream: file \"" + path + "\" does not exist or is inaccessible.", ELDebug.Category.ERROR);
        return null;
      }
    }

    /// <summary>
    /// Wrapper for File.Exists(). Check whether the file path exists.
    /// </summary>
    /// <param name="path">The path to a file.</param>
    /// <returns>Whether the file exists.</returns>
    public static bool Exists(string path)
    {
      return File.Exists(path);
    }

  }
}
