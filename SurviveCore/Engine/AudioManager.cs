using SurviveDesktop;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;
using System.Linq;
using SoundFlow.Components;
using SoundFlow.Providers;
using System.IO;
using SoundFlow.Abstracts.Devices;

namespace SurviveCore.Engine
{
  // based on SoundEffectInstance manager code from Ninja Cat Remewstered
  internal static class AudioManager
  {

    private static readonly List<SoundEffectInstance> soundInstances = [];
    private static readonly Dictionary<string, SoundEffectInstance> keyedSoundInstances = [];

    private static SoundPlayer player;

    public static void Initialise()
    {
      AudioFormat format = AudioFormat.DvdHq;

      // init soundflow audio and playback device
      using MiniAudioEngine engine = new();
      engine.UpdateAudioDevicesInfo();
      DeviceInfo defaultDevice = engine.PlaybackDevices.FirstOrDefault(deviceInfo => deviceInfo.IsDefault); // attempt to use default device
      using AudioPlaybackDevice playbackDevice = engine.InitializePlaybackDevice(defaultDevice, format);
      playbackDevice.Start();

      // get audio stream from file and put into player
      using StreamDataProvider dataProvider = new(engine, format, File.OpenRead("/mnt/big-chungus/projects/monogame/Survivable-Engine/SurviveCore/Content/sfx/missing.wav"));
      player = new(engine, format, dataProvider);

      // add it to the mixer
      playbackDevice.MasterMixer.AddComponent(player);

      // play
      player.Play();

      // it does play, but presumably the audio system doesn't live long anough to actually be audible,
      // outside of pausing here

      // then stop all audio
      playbackDevice.Stop();
    }

    /// <summary>
    /// Creates an instance of a soundeffect stored in Warehouse. Can optionally be made unique by passing a key.
    /// </summary>
    /// <param name="soundFile">The filename for Warehouse to search for.</param>
    /// <param name="key">Optional key to make the sound unique. Use with StopKeyedSfx() to stop this sound later.</param>
    /// <returns>A reference to the SoundEffectInstance that was made.</returns>
    public static SoundEffectInstance PlaySound(string soundFile)
    {
      // only play the sound if there's a free slot, or it would replace an existing sound
      if (soundInstances.Count + keyedSoundInstances.Count < Platform.MAX_SFX_INSTANCES)
      {
        SoundEffect soundEffect = Warehouse.GetSoundEffect(soundFile);

        SoundEffectInstance sf = soundEffect.CreateInstance();
        soundInstances.Add(sf);
        sf.Play();

        return sf;
      }
      else
      {
        ELDebug.Log("max sound instances reached, not playing " + soundFile);
      }

      return null;
    }

    /// <summary>
    /// Creates an instance of a soundeffect stored in Warehouse. Can optionally be made unique by passing a key.
    /// </summary>
    /// <param name="soundFile">The filename for Warehouse to search for.</param>
    /// <param name="key">Optional key to make the sound unique. Use with StopKeyedSfx() to stop this sound later.</param>
    /// <returns>A reference to the SoundEffectInstance that was made.</returns>
    public static SoundEffectInstance PlaySound(string soundFile, string key)
    {
      // only play the sound if there's a free slot, or it would replace an existing sound
      if (soundInstances.Count + keyedSoundInstances.Count < Platform.MAX_SFX_INSTANCES || keyedSoundInstances.ContainsKey(key))
      {
        SoundEffect soundEffect = Warehouse.GetSoundEffect(soundFile);

        SoundEffectInstance sf = soundEffect.CreateInstance();

        if (string.IsNullOrWhiteSpace(key))
        {
          ELDebug.Log("key is blank or null, not playing " + soundFile);
          return null;
        }
        else
        {
          // remove old sound instance if it already exists
          if (keyedSoundInstances.ContainsKey(key))
          {
            ELDebug.Log("conflicting keyed sound instance " + soundFile + " exists, replacing old instance");
            keyedSoundInstances[key].Stop();
            keyedSoundInstances[key].Dispose();
            keyedSoundInstances.Remove(key);
          }
          keyedSoundInstances.Add(key, sf);
        }

        sf.Play();

        return sf;
      }
      else
      {
        ELDebug.Log("max sound instances reached, not playing " + soundFile);
      }

      return null;
    }

    /// <summary>
    /// Stop all playing sound effects.
    /// </summary>
    /// <returns>True</returns>
    public static bool StopAllSounds()
    {
      foreach (SoundEffectInstance sf in soundInstances)
      {
        sf.Stop();
      }
      foreach (KeyValuePair<string, SoundEffectInstance> sf in keyedSoundInstances)
      {
        sf.Value.Stop();
      }
      return true;
    }

    /// <summary>
    /// Stop a specific keyed sound.
    /// </summary>
    /// <param name="key">The sound key to search for.</param>
    /// <returns>Whether the sound existed or not.</returns>
    public static bool StopKeyedSound(string key)
    {
      if (keyedSoundInstances.ContainsKey(key))
      {
        keyedSoundInstances[key].Stop();
        return true;
      }
      return false;
    }

    /// <summary>
    /// Get state of keyed sound.
    /// </summary>
    /// <param name="key">The sound key to search for.</param>
    /// <returns>SoundState of the sound effect, or SoundState.Stopped if not found.</returns>
    public static SoundState KeyedSoundState(string key)
    {
      if (keyedSoundInstances.ContainsKey(key))
      {
        return keyedSoundInstances[key].State;
      }
      else
      {
        return SoundState.Stopped;
      }
    }

    /// <summary>
    /// Get how many sounds are currently playing.
    /// </summary>
    /// <returns>The amount of sounds which are currently playing.</returns>
    public static int PlayingSoundCount()
    {
      return soundInstances.Count + keyedSoundInstances.Count;
    }

    /// <summary>
    /// Cleans up any finished sound instances that are taking up space in the sound lists.
    /// </summary>
    public static void Cleanup()
    {
      // clean up finished SoundEffectInstances
      // loop in reverse so we don't skip the next one after removing one
      for (int i = soundInstances.Count - 1; i > -1; i--)
      {
        SoundEffectInstance sf = soundInstances[i];
        if (sf.State == SoundState.Stopped)
        {
          sf.Dispose();
          soundInstances.RemoveAt(i);
        }
      }

      // clean up keyed sounds
      foreach (string key in keyedSoundInstances.Keys)
      {
        SoundEffectInstance sf = keyedSoundInstances[key];
        if (sf.State == SoundState.Stopped)
        {
          sf.Dispose();
          keyedSoundInstances.Remove(key);
        }
      }


    }

  }
}
