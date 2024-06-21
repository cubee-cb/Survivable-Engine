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
    private int t;

    // should these be vector3? thinking we want to support elevations
    private Vector2 lastPosition;
    protected Vector2 position;
    protected Vector2 velocity;

    [JsonIgnore] protected FacingDirection direction = FacingDirection.Down;
    [JsonIgnore] protected SpriteRotationType rotationType = SpriteRotationType.None;
    [JsonIgnore] protected FacingDirection currentVisualDirection = FacingDirection.Down;
    [JsonIgnore] protected FacingDirection lastVisualDirection = FacingDirection.Down;

    [JsonIgnore] protected Dictionary<string, int> spriteDimensions = new();
    [JsonIgnore] public int feetOffsetY = 2;

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
      t += 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch">spritebatch the game uses to render</param>
    /// <param name="tickProgress">a value from 0-1 showing the progress through the current tick, for smoothing purposes</param>
    public virtual void Draw(float tickProgress)
    {
      // get row number from facing direction
      List<FacingDirection> dirs = rotationTypeToLayout[rotationType];
      FacingDirection direct = GetSpriteDirection();
      int spriteY = dirs.IndexOf(direct);
      int frameX = (int)MathF.Floor(t % 20 / 10f);

      // set up clipping rectangle
      Rectangle clippingRect = new();
      spriteDimensions.TryGetValue("width", out clippingRect.Width);
      spriteDimensions.TryGetValue("height", out clippingRect.Height);
      spriteDimensions.TryGetValue("feetOffsetY", out feetOffsetY);

      clippingRect.Location = new Point(frameX, spriteY) * clippingRect.Size;

      // draw
      GameDisplay.Draw(texture, clippingRect, GetVisualPosition(tickProgress) - clippingRect.Size.ToVector2() / 2f + Vector2.UnitY * feetOffsetY);

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

    public virtual Inventory GetInventory()
    {
      return inventory;
    }

    /// <summary>
    /// Try to move in a direction. If a collision is detected, the entity will only move as far is it can and the amount it moved will be returned.
    /// </summary>
    /// <param name="delta">Distance to try to move.</param>
    /// <returns>Actual distance the entity moved.</returns>
    public virtual Vector2 TryMove(Vector2 delta)
    {
      if (delta == Vector2.Zero) return Vector2.Zero;
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
      const int totalPossibleDirections = 8;

      double angle = Math.Atan2(facingVector.Y, facingVector.X) / (Math.PI * 2);
      angle = Math.Floor(angle * totalPossibleDirections);
      if (angle < 0) angle += totalPossibleDirections;

      //ELDebug.Log("angle: " + angle + " / " + correspondingDirection[(int)angle]);

      return correspondingDirection[(int)angle];
    }

    protected FacingDirection GetSpriteDirection()
    {
      lastVisualDirection = currentVisualDirection;
      FacingDirection newDirection = directionToSpriteName[rotationType][(int)direction];

      // only update the facing direction if it is supposed to change
      if (newDirection != FacingDirection.Unchanged)
      {
        currentVisualDirection = newDirection;
      }

      return currentVisualDirection;
    }



    // directions corresponding to each rotation step
    private readonly List<FacingDirection> correspondingDirection = new()
    {
        FacingDirection.Right,
        FacingDirection.RightDown,
        FacingDirection.Down,
        FacingDirection.LeftDown,
        FacingDirection.Left,
        FacingDirection.LeftUp,
        FacingDirection.Up,
        FacingDirection.RightUp
    };

    // dictionary of the appropriate facing directions for sprites corresponding to each rotation type
    private readonly Dictionary<SpriteRotationType, List<FacingDirection>> directionToSpriteName = new() {
      {
        SpriteRotationType.None,
        new() {
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Right
        }
      },
      {
        SpriteRotationType.TwoWay,
        new() {
            FacingDirection.Right,
            FacingDirection.Right,
            FacingDirection.Unchanged,
            FacingDirection.Left,
            FacingDirection.Left,
            FacingDirection.Left,
            FacingDirection.Unchanged,
            FacingDirection.Right
        }
      },
      {
        SpriteRotationType.Cardinal,
        new() {
            FacingDirection.Right,
            FacingDirection.Unchanged,
            FacingDirection.Down,
            FacingDirection.Unchanged,
            FacingDirection.Left,
            FacingDirection.Unchanged,
            FacingDirection.Up,
            FacingDirection.Unchanged,
        }
      },
      {
        SpriteRotationType.Diagonal,
        new() {
            FacingDirection.Unchanged,
            FacingDirection.RightDown,
            FacingDirection.Unchanged,
            FacingDirection.LeftDown,
            FacingDirection.Unchanged,
            FacingDirection.LeftUp,
            FacingDirection.Unchanged,
            FacingDirection.RightUp
        }
      },
      {
        SpriteRotationType.EightWay,
        new() {
            FacingDirection.Right,
            FacingDirection.RightDown,
            FacingDirection.Down,
            FacingDirection.LeftDown,
            FacingDirection.Left,
            FacingDirection.LeftUp,
            FacingDirection.Up,
            FacingDirection.RightUp
        }
      },

    };

    // dictionary of the order of sprite rows for each rotation type
    private readonly Dictionary<SpriteRotationType, List<FacingDirection>> rotationTypeToLayout = new() {
      {
        SpriteRotationType.None,
        new() {
            FacingDirection.Unchanged,
        }
      },
      {
        SpriteRotationType.TwoWay,
        new() {
            FacingDirection.Right,
            FacingDirection.Left
        }
      },
      {
        SpriteRotationType.Cardinal,
        new() {
            FacingDirection.Down,
            FacingDirection.Up,
            FacingDirection.Right,
            FacingDirection.Left
        }
      },
      {
        SpriteRotationType.Diagonal,
        new() {
            FacingDirection.RightDown,
            FacingDirection.RightUp,
            FacingDirection.LeftDown,
            FacingDirection.LeftUp
        }
      },
      {
        SpriteRotationType.EightWay,
        new() {
            FacingDirection.Down,
            FacingDirection.LeftDown,
            FacingDirection.Left,
            FacingDirection.LeftUp,
            FacingDirection.Up,
            FacingDirection.RightUp,
            FacingDirection.Right,
            FacingDirection.RightDown
        }
      },

    };


  }
}
