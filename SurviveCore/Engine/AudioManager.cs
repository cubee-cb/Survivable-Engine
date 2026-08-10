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
using SoundFlow.Interfaces;

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

    /// <summary>
    /// construct format based on the sound's metadata
    /// </summary>
    /// <param name="provider">SoundDataProvider</param>
    /// <returns>AudioFormat</returns>
    private static AudioFormat InferFormat(ISoundDataProvider provider)
    {
      return new()
      {
        SampleRate = provider.FormatInfo.SampleRate,
        Channels = provider.FormatInfo.ChannelCount,
        Layout = provider.FormatInfo.ChannelCount == 1 ? ChannelLayout.Mono : ChannelLayout.Stereo,
        Format = SampleFormat.U8,
      };
    }

    public static SoundPlayer SoundFromAsset(string path)
    {
      AssetDataProvider soundAsset = new(engine, path);

      // put into player and add it to the mixer
      SoundPlayer player = new(engine, InferFormat(soundAsset), soundAsset);
      playbackDevice.MasterMixer.AddComponent(player);
      return player;
    }

    public static SoundPlayer SoundFromStream(Stream stream)
    {
      StreamDataProvider soundAsset = new(engine, stream);

      // put into player and add it to the mixer
      SoundPlayer player = new(engine, InferFormat(soundAsset), soundAsset);
      playbackDevice.MasterMixer.AddComponent(player);
      return player;
    }

    private static SoundPlayer PlaySoundInternal(string soundKey)
    {
      // get audio stream from warehouse
      SoundPlayer player = Warehouse.GetSoundEffect(soundKey);
      if (player == null) return null;

      // play
      player.Stop();
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
      return PlaySoundInternal(soundKey);
    }

    /// <summary>
    /// Creates an instance of a soundeffect stored in Warehouse. Can optionally be made unique by passing a key.
    /// </summary>
    /// <param name="soundFile">The filename for Warehouse to search for.</param>
    /// <param name="key">Optional key to make the sound unique. Use with StopKeyedSfx() to stop this sound later.</param>
    /// <returns>A reference to the SoundEffectInstance that was made.</returns>
    public static SoundPlayer PlaySound(string soundKey, string key)
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        ELDebug.Log("key is blank or null, not playing " + soundKey);
        return null;
      }

      // restart old sound instance instead of creating a new one, if it already exists
      if (soundPlayersKeyed.TryGetValue(key, out SoundPlayer existingPlayer))
      {
        existingPlayer.Stop();
        existingPlayer.Play();
        return existingPlayer;
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
      foreach (SoundPlayer player in soundPlayers)
      {
        player.Stop();
      }
      foreach (KeyValuePair<string, SoundPlayer> player in soundPlayersKeyed)
      {
        player.Value.Stop();
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
      // don't bother cleaning up sounds; sounds are currently not instanced
    }

  }
}
