using Microsoft.Xna.Framework.Audio;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Lua
{
  internal static class LuaCommon
  {
    /// <summary>
    /// Registers common methods for lua scripts, including debug logging, audio.
    /// </summary>
    /// <param name="lua">The lua scrips to add the methods to.</param>
    public static void Register(Script lua)
    {
      // debug/logging
      lua.Globals["DebugLog"] = (Func<object, bool>)Log;

      // audio
      lua.Globals["PlaySound"] = (Func<string, bool>)PlaySound;
      lua.Globals["PlayKeyedSound"] = (Func<string, string, bool>)PlaySound;
      lua.Globals["StopKeyedSound"] = (Func<string, bool>)AudioManager.StopKeyedSound;
      lua.Globals["StopAllSound"] = (Func<bool>)AudioManager.StopAllSounds;


      //lua.Globals["PlaySound"] = (Func<string, string, SoundEffectInstance>)AudioManager.PlaySound;
      //lua.Globals["PlaySound"] = (Func<string, string, SoundEffectInstance>)AudioManager.PlaySound;
      //lua.Globals["PlaySound"] = (Func<string, string, SoundEffectInstance>)AudioManager.PlaySound;
    }

    private static bool PlaySound(string soundFile)
    {
      return AudioManager.PlaySound(soundFile) != null;
    }

    private static bool PlaySound(string soundFile, string key)
    {
      return AudioManager.PlaySound(soundFile, key) != null;
    }

    private static bool Log(object thingToLog)
    {
      ELDebug.Log(thingToLog);
      return false;
    }


    public static void RegisterConverters()
    {
      //Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<SoundEffectInstance>(v => DynValue.NewBoolean(false));

    }

    /// <summary>
    /// Tries to find a method in the specified script, and runs it if found.
    /// </summary>
    /// <param name="script">The script to try running the method in.</param>
    /// <param name="methodName">The name of the method to try running.</param>
    /// <param name="args">Arguments to pass to `script.Call()`.</param>
    /// <returns>The value returned by the function, or DynValue.Nil.</returns>
    public static DynValue TryRunMethod(Script script, string methodName, params object[] args)
    {
      try
      {
        DynValue method = script.Globals.Get(methodName);
        if (method.Type != DataType.Function) return DynValue.Nil;

        DynValue res = script.Call(method, args);
        return res;
      }
      catch (Exception e)
      {
        ELDebug.Log("LUA error: \n" + e);
        return DynValue.Nil;
      }

    }

  }
}
