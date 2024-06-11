using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;

namespace SurviveCore
{
  internal abstract class Entity : IdentifiableObject
  {
    [JsonIgnore] World world;

    // should these be vector3? thinking we want to support elevations
    private Vector2 lastPosition;
    protected Vector2 position;
    protected Vector2 velocity;

    protected float health;

    [JsonIgnore] protected Texture2D texture;
    //protected EntityProperties properties;


    /// <summary>
    /// 
    /// </summary>
    public Entity(string id, World world) : base()
    {
      this.world = world;

      position = Vector2.Zero;
      velocity = Vector2.Zero;

      //properties = Warehouse.GetJson<EntityProperties>(id);

      //texture = Warehouse.GetTexture(properties.textureSheetName);
      //health = properties.maxHealth;

      //luaTick.DoString(entityProperties.luaTick);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spawnLocation"></param>
    public Entity(Vector2 spawnLocation) : base()
    {
      position = spawnLocation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void Update(int tick, float deltaTime)
    {
      //luaTick.Call(luaTick.Globals["update"], this);

      lastPosition = position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch">spritebatch the game uses to render</param>
    /// <param name="tickProgress">a value from 0-1 showing the progress through the current tick, for smoothing purposes</param>
    public virtual void Draw(float tickProgress)
    {

      // todo: handle spritesheets and multiple textures
      //spriteBatch.Draw(texture, visualPosition, Color.White);
      GameDisplay.Draw(texture, new Rectangle(0, 0, 16, 16), GetVisualPosition(tickProgress));

    }

    public virtual Vector2 GetPosition()
    {
      return position;
    }

    public virtual Vector2 GetVisualPosition(float tickProgress)
    {
      // lerp position to the actual one, based on the current progress of the tick.
      // doesn't need to be global, because collisions are per-frame. visualPosition *should* equal position every time collisions are checked.
      return Vector2.Lerp(lastPosition, position, tickProgress);
    }

    /// <summary>
    /// Try to move in a direction. If a collision is detected, the entity will only move as far is it can and the amount it moved will be returned.
    /// </summary>
    /// <param name="delta">Distance to try to move.</param>
    /// <returns>Actual distance the entity moved.</returns>
    public virtual Vector2 TryMove(Vector2 delta)
    {
      // todo: check for collisions with objects this object is allowed to collide with

      position += delta;
      return delta;
    }


  }
}
