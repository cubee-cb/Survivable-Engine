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
using SoundFlow.Enums;

namespace SurviveCore.Engine
{
  // based on SoundEffectInstance manager code from Ninja Cat Remewstered
  internal static class AudioManager
  {

    private static readonly List<SoundPlayer> soundPlayers = [];
    private static readonly Dictionary<string, SoundPlayer> soundPlayersKeyed = [];

    private static AudioFormat format;
    private static MiniAudioEngine engine;
    private static AudioPlaybackDevice playbackDevice;

    public static void Initialise()
    {
      // this works for WAV files exported from PICO-8.
      format = new()
      {
        Format = SampleFormat.U8,
        SampleRate = 44100 / 2,
        Channels = 1,
        Layout = ChannelLayout.Mono,
      };

      // init soundflow audio and playback device
      engine = new();
      engine.UpdateAudioDevicesInfo();
      DeviceInfo defaultDevice = engine.PlaybackDevices.FirstOrDefault(deviceInfo => deviceInfo.IsDefault); // attempt to use default device
      playbackDevice = engine.InitializePlaybackDevice(defaultDevice, format);
      playbackDevice.Start();

      // then stop all audio
      //playbackDevice.Stop();
    }

    public static AssetDataProvider FromAsset(string path)
    {
      return new(engine, path);
    }

    public static StreamDataProvider FromAsset(Stream stream)
    {
      return new(engine, stream);
    }

    private static SoundPlayer PlaySoundInternal(string soundKey)
    {
      // get audio stream from warehouse
      AssetDataProvider soundAsset = Warehouse.GetSoundEffect(soundKey);
      if (soundAsset == null) return null;

      // constructs format based on the sound's metadata
      AudioFormat format = new()
      {
        SampleRate = soundAsset.FormatInfo.SampleRate,
        Channels = soundAsset.FormatInfo.ChannelCount,
        Layout = soundAsset.FormatInfo.ChannelCount == 1 ? ChannelLayout.Mono : ChannelLayout.Stereo,
        Format = SampleFormat.U8,
      };

      // put into player and add it to the mixer
      SoundPlayer player = new(engine, format, soundAsset);
      playbackDevice.MasterMixer.AddComponent(player);

      // play
      player.Play();
      return player;
    }

    /// <summary>
    /// Creates an instance of a soundeffect stored in Warehouse. Can optionally be made unique by passing a key.
    /// </summary>
    /// <param name="soundKey">The filename for Warehouse to search for.</param>
    /// <param name="key">Optional key to make the sound unique. Use with StopKeyedSfx() to stop this sound later.</param>
    /// <returns>A reference to the SoundEffectInstance that was made.</returns>
    public static SoundPlayer PlaySound(string soundKey)
    {
      // only play the sound if there's a free slot, or it would replace an existing sound
      if (soundPlayers.Count + soundPlayersKeyed.Count >= Platform.MAX_SFX_INSTANCES)
      {
        ELDebug.Log("max sound instances reached, not playing " + soundKey);
        return null;
      }

      ELDebug.Log($"playing {soundKey}");
      SoundPlayer player = PlaySoundInternal(soundKey);
      ELDebug.Log($"player: {player.State} {player.Volume} {player.IsDisposed} {player.DataProvider.Length} {player.Name}");
      ELDebug.Log($"mixer: {playbackDevice.MasterMixer.Components.Count} {playbackDevice.MasterMixer.Enabled}");
      soundPlayers.Add(player);
      return player;
    }

    /// <summary>
    /// Creates an instance of a soundeffect stored in Warehouse. Can optionally be made unique by passing a key.
    /// </summary>
    /// <param name="soundFile">The filename for Warehouse to search for.</param>
    /// <param name="key">Optional key to make the sound unique. Use with StopKeyedSfx() to stop this sound later.</param>
    /// <returns>A reference to the SoundEffectInstance that was made.</returns>
    public static SoundPlayer PlaySound(string soundKey, string key)
    {
      // only play the sound if there's a free slot, or it would replace an existing sound
      if (soundPlayers.Count + soundPlayersKeyed.Count >= Platform.MAX_SFX_INSTANCES && !soundPlayersKeyed.ContainsKey(key))
      {
        ELDebug.Log("max sound instances reached, not playing " + soundKey);
        return null;
      }

      if (string.IsNullOrWhiteSpace(key))
      {
        ELDebug.Log("key is blank or null, not playing " + soundKey);
        return null;
      }

      // restart old sound instance instead of creating a new one, if it already exists
      if (soundPlayersKeyed.TryGetValue(key, out SoundPlayer oldPlayer))
      {
        ELDebug.Log("conflicting keyed sound instance " + soundKey + " exists, replacing old instance");
        oldPlayer.Stop();
        oldPlayer.Play();
        soundPlayersKeyed.Remove(key);
        return oldPlayer;
      }

      SoundPlayer player = PlaySoundInternal(soundKey);
      soundPlayersKeyed.Add(key, player);
      return player;
    }

    /// <summary>
    /// Stop all playing sound effects.
    /// </summary>
    /// <returns>True</returns>
    public static bool StopAllSounds()
    {
      foreach (SoundPlayer sf in soundPlayers)
      {
        sf.Stop();
      }
      foreach (KeyValuePair<string, SoundPlayer> sf in soundPlayersKeyed)
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
      if (soundPlayersKeyed.TryGetValue(key, out SoundPlayer player))
      {
        player.Stop();
        return true;
      }
      return false;
    }

    /// <summary>
    /// Get state of keyed sound.
    /// </summary>
    /// <param name="key">The sound key to search for.</param>
    /// <returns>SoundState of the sound effect, or SoundState.Stopped if not found.</returns>
    public static PlaybackState KeyedSoundState(string key)
    {
      if (soundPlayersKeyed.TryGetValue(key, out SoundPlayer player))
      {
        return player.State;
      }
      return PlaybackState.Stopped;
    }

    /// <summary>
    /// Get how many sounds are currently playing.
    /// </summary>
    /// <returns>The amount of sounds which are currently playing.</returns>
    public static int PlayingSoundCount()
    {
      return soundPlayers.Count + soundPlayersKeyed.Count;
    }

    /// <summary>
    /// Cleans up any finished sound instances that are taking up space in the sound lists.
    /// </summary>
    public static void Cleanup()
    {
      // clean up finished SoundEffectInstances
      // loop in reverse so we don't skip the next one after removing one
      for (int i = soundPlayers.Count - 1; i > -1; i--)
      {
        SoundPlayer sf = soundPlayers[i];
        if (sf.State == PlaybackState.Stopped)
        {
          playbackDevice.MasterMixer.RemoveComponent(sf);
          sf.Dispose();
          soundPlayers.RemoveAt(i);
        }
      }

      // don't bother cleaning up keyed sounds; they will be reused next time something plays them
    }

  }
}
