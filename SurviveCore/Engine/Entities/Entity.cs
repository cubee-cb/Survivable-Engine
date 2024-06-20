using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine;
using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.Items;

namespace SurviveCore.Engine.Entities
{
  internal abstract class Entity : IdentifiableObject
  {


    [JsonIgnore] World world;

    private string id;

    // should these be vector3? thinking we want to support elevations
    private Vector2 lastPosition;
    protected Vector2 position;
    protected Vector2 velocity;
    protected FacingDirection direction = FacingDirection.Right;
    protected SpriteRotationType rotationType = SpriteRotationType.None;

    protected float health;

    protected Inventory inventory;

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
    public Entity(string id, Vector2 spawnLocation) : base()
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

      direction = GetFacingDirection(delta);

      return delta;
    }

    /// <summary>
    /// Gets a FacingDirection based on a vector and the object's rotation type.
    /// </summary>
    /// <param name="facingVector">The direction vector to face along.</param>
    /// <returns>A FacingDirection.</returns>
    protected FacingDirection GetFacingDirection(Vector2 facingVector)
    {

      double angle = Math.Atan2(facingVector.Y, facingVector.X) / (Math.PI * 2);
      angle = Math.Floor(angle * correspondingDirection[rotationType].Length);
      if (angle < 0) angle += correspondingDirection[rotationType].Length;

      //ELDebug.Log("angle: " + angle + " / " + correspondingDirection[(int)angle]);

      return correspondingDirection[rotationType][(int)angle];
    }




    // dictionary of the appropriate facing directions corresponding to each rotation type
    private readonly Dictionary<SpriteRotationType, FacingDirection[]> correspondingDirection = new() {
      {
        SpriteRotationType.EightWay,
        new FacingDirection[] {
            FacingDirection.Right,
            FacingDirection.RightDown,
            FacingDirection.Down,
            FacingDirection.LeftDown,
            FacingDirection.Left,
            FacingDirection.LeftUp,
            FacingDirection.Up,
            FacingDirection.RightUp
        }
      }
    };
  }
}
