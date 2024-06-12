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
      lua.Globals["DebugLog"] = (Func<object, bool, bool>)ELDebug.Log;

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


    public static void RegisterConverters()
    {
      //Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<SoundEffectInstance>(v => DynValue.NewBoolean(false));

    }

  }
}
