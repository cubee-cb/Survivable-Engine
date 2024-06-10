using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore
{
  internal abstract class Entity : IdentifiableObject
  {
    // should this be vector3? thinking we want to support elevations
    protected Vector2 position;
    protected Vector2 velocity;

    protected float health;

    protected Texture2D texture;
    protected EntityProperties properties;

    /// <summary>
    /// 
    /// </summary>
    public Entity(string id) : base()
    {
      position = Vector2.Zero;
      velocity = Vector2.Zero;

      properties = Warehouse.GetJson<EntityProperties>(id);

      texture = Warehouse.GetTexture(properties.textureSheetName);
      health = properties.maxHealth;
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch">spritebatch the game uses to render</param>
    /// <param name="tickProgress">a value from 0-1 showing the progress through the current tick, for smoothing purposes</param>
    public virtual void Draw(SpriteBatch spriteBatch, float tickProgress)
    {

      // todo: handle spritesheets and multiple textures
      spriteBatch.Draw(texture, position, Color.White);
    }

    /// <summary>
    /// try to move in a direction. if a collision is detected, the entity will only move as far is it can and the amount it moved will be returned.
    /// </summary>
    /// <param name="delta">distance to try to move</param>
    /// <returns>actual distance the entity moved</returns>
    public virtual Vector2 TryMove(Vector2 delta)
    {
      // todo: check for collisions with objects this object is allowed to collide with

      position += delta;
      return delta;
    }


  }
}
