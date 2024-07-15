using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Input
{
  public class InputManager
  {
    private PlayerIndex playerIndex = PlayerIndex.One;
    private bool hasKeyboard = false;

    KeyboardState keyboardState;
    MouseState mouseState;
    GamePadState gamePadState;

    Dictionary<string, List<Keys>> keyboardBindings = new()
    {
      { "left", new List<Keys>() { Keys.Left, Keys.S } },
      { "right", new List<Keys>() { Keys.Right, Keys.F } },
      { "up", new List<Keys>() { Keys.Up, Keys.E } },
      { "down", new List<Keys>() { Keys.Down, Keys.D } },
      { "interact", new List<Keys>() { Keys.E } },
      { "use", new List<Keys>() { Keys.Space } },
      { "run", new List<Keys>() { Keys.LeftShift } }
    };
    Dictionary<string, List<Buttons>> controllerBindings = new()
    {
      { "left", new List<Buttons>() { Buttons.DPadLeft, Buttons.LeftThumbstickLeft } },
      { "right", new List<Buttons>() { Buttons.DPadRight, Buttons.LeftThumbstickRight } },
      { "up", new List<Buttons>() { Buttons.DPadUp, Buttons.LeftThumbstickUp } },
      { "down", new List<Buttons>() { Buttons.DPadDown, Buttons.LeftThumbstickDown } },
      { "interact", new List<Buttons>() { Buttons.A } },
      { "use", new List<Buttons>() { Buttons.RightTrigger } },
      { "run", new List<Buttons>() { Buttons.X } }
    };

    // this is set up this way so not every input type has to be specified.
    public InputManager(PlayerIndex playerIndex, bool hasKeyboard = false)
    {
      this.playerIndex = playerIndex;
      this.hasKeyboard = hasKeyboard;
    }

    public void UpdateInputs()
    {
      //todo: set "last" states

      if (hasKeyboard) keyboardState = Keyboard.GetState();
      mouseState = Mouse.GetState();
      gamePadState = GamePad.GetState(playerIndex);
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
          if (keyboardState.IsKeyDown(key))
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
          if (gamePadState.IsButtonDown(button))
          {
            return true;
          }
        }

      }

      return false;
    }


  }
}
