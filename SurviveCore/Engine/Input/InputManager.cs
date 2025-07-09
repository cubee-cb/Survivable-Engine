using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SurviveCore.Engine.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Input
{
  internal class InputManager
  {
    readonly private PlayerIndex playerIndex = PlayerIndex.One;
    readonly private bool hasKeyboard = false;
    private GameDisplay pointerSource;

    KeyboardState keyboardState;
    GamePadState gamepadState;

    readonly List<Keys> keyboardBuffer;
    //List<MouseActions> mouseBuffer;
    readonly List<Buttons> gamepadBuffer;

    readonly Dictionary<string, List<Keys>> keyboardBindings = new()
    {
      { "left", new List<Keys>() { Keys.Left, Keys.S } },
      { "right", new List<Keys>() { Keys.Right, Keys.F } },
      { "up", new List<Keys>() { Keys.Up, Keys.E } },
      { "down", new List<Keys>() { Keys.Down, Keys.D } },
      { "interact", new List<Keys>() { Keys.E } },
      { "use", new List<Keys>() { Keys.X } },
      { "jump", new List<Keys>() { Keys.Space } },
      { "run", new List<Keys>() { Keys.LeftShift } }
    };
    readonly Dictionary<string, List<Buttons>> controllerBindings = new()
    {
      { "left", new List<Buttons>() { Buttons.DPadLeft, Buttons.LeftThumbstickLeft } },
      { "right", new List<Buttons>() { Buttons.DPadRight, Buttons.LeftThumbstickRight } },
      { "up", new List<Buttons>() { Buttons.DPadUp, Buttons.LeftThumbstickUp } },
      { "down", new List<Buttons>() { Buttons.DPadDown, Buttons.LeftThumbstickDown } },
      { "interact", new List<Buttons>() { Buttons.B } },
      { "use", new List<Buttons>() { Buttons.RightTrigger } },
      { "jump", new List<Buttons>() { Buttons.A } },
      { "run", new List<Buttons>() { Buttons.X } }
    };

    // this is set up this way so not every input type has to be specified.
    public InputManager(PlayerIndex playerIndex, GameDisplay pointerSource, bool hasKeyboard = false)
    {
      this.playerIndex = playerIndex;
      this.pointerSource = pointerSource;
      this.hasKeyboard = hasKeyboard;

      if (hasKeyboard) keyboardBuffer = new();
      gamepadBuffer = new();
    }

    /// <summary>
    /// Called every real frame, stores inputs that are detected for use later.
    /// </summary>
    public void BufferInputs()
    {
      // update input states
      if (hasKeyboard)
      {
        keyboardState = Keyboard.GetState();
      }
      if (pointerSource != null && pointerSource.HasPointer())
      {
        pointerSource.UpdatePointerState();
      }
      gamepadState = GamePad.GetState(playerIndex);

      // buffer keyboard
      foreach (Keys key in keyboardState.GetPressedKeys())
      {
        keyboardBuffer.Add(key);
      }

      /*/ todo: buffer mouse clicks and scrolls
      foreach (MouseActions action in Enum.GetValues(typeof(MouseActions)))
      {
        if (blah blah blah)
        {
          mouseBuffer.Add(action);
        }
      }
      
      //*/

      // buffer controller
      foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
      {
        if (gamepadState.IsButtonDown(button))
        {
          gamepadBuffer.Add(button);
        }
      }

    }

    /// <summary>
    /// Called at the end of a game tick. Resets the input buffers and <TODO> stores the previous input states.
    /// </summary>
    public void ResetInputs()
    {
      // todo: somehow store how many ticks each input has been held for, rather than just clearing them.
      // or just store last tick buffer, that'll work for the simple "just pressed?" use case.

      if (hasKeyboard)
      {
        keyboardBuffer.Clear();
      }
      gamepadBuffer.Clear();

    }

    /// <summary>
    /// Checks the provided action to see if any of its assigned inputs are pressed.
    /// </summary>
    /// <param name="action">The action to check.</param>
    /// <param name="justPressed">If true, return if the key was JUST pressed rather than if it is currently pressed.</param>
    /// <returns>Whether the key was pressed.</returns>
    public bool Action(string action, bool justPressed = false)
    {

      // check keyboard
      if (hasKeyboard && keyboardBindings.ContainsKey(action))
      {
        foreach (Keys key in keyboardBindings[action])
        {
          if (keyboardBuffer.Contains(key))
          {
            return true;
          }
        }
      }

      // check controller buttons
      if (controllerBindings.ContainsKey(action))
      {
        foreach (Buttons button in controllerBindings[action])
        {
          if (gamepadBuffer.Contains(button))
          {
            return true;
          }
        }

      }

      return false;
    }

    /// <summary>
    /// Get this input manager's aim cursor.
    /// </summary>
    /// <param name="origin">Optional offset that the cursor is centred around.</param>
    /// <returns>World coordinates. Origin or (0, 0) if no pointer is available.</returns>
    public Vector2 GetAimCursor(Vector2? origin = null)
    {
      Vector2 vecOrigin = origin == null ? Vector2.Zero : (Vector2)origin;

      if (pointerSource.HasPointer())
      {
        return pointerSource.ScreenToWorld(pointerSource.GetPointer()) - vecOrigin;
      }
      //todo: do we allow controllers to have a pointer of some kind?
      else
      {
        return vecOrigin;
      }
    }

    public bool IsAimCursorValid()
    {
      // todo: handle the cursor being over UI panels, etc.
      return true;
    }

  }
}
