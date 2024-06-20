using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Input
{
  public class InputManager
  {
    public string internalName = "default_inputManager";
    private PlayerIndex playerIndex = PlayerIndex.One;
    private bool hasKeyboard = false;

    KeyboardState keyboardState;
    MouseState mouseState;
    GamePadState gamePadState;

    Dictionary<string, List<Keys>> keyboardBindings = new()
    {
      { "left", new List<Keys>() { Keys.Left } },
      { "right", new List<Keys>() { Keys.Right } },
      { "up", new List<Keys>() { Keys.Up } },
      { "down", new List<Keys>() { Keys.Down } },
      { "interact", new List<Keys>() { Keys.E } },
      { "use", new List<Keys>() { Keys.Space } },
      { "run", new List<Keys>() { Keys.LeftShift } }
    };
    Dictionary<string, List<Buttons>> controllerBindings = new();

    // this is set up this way so not every input type has to be specified.
    public InputManager(PlayerIndex playerIndex, bool hasKeyboard = false)
    {
      this.playerIndex = playerIndex;
      this.hasKeyboard = hasKeyboard;
    }

    public void UpdateInputs()
    {
      //todo: set "last" states

      keyboardState = Keyboard.GetState();
      mouseState = Mouse.GetState();
      gamePadState = GamePad.GetState(playerIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">The action to check.</param>
    /// <param name="justPressed">If true, return if the key was JUST pressed rather than if it is currently pressed.</param>
    /// <returns></returns>
    public bool CheckInputAction(string action, bool justPressed)
    {
      // check keyboard
      if (keyboardState != null)
      {
        foreach (KeyValuePair<string, List<Keys>> kvp in keyboardBindings)
        {
          foreach (Keys key in kvp.Value)
          {
            if (keyboardState.IsKeyDown(key))
            {
              return true;
            }
          }
        }

      }

      // check controller buttons

      return false;
    }

  }
}
