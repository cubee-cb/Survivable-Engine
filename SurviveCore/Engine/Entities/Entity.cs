using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine;
using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.Items;
using SurviveCore.Engine.WorldGen;
using SurviveCore.Engine.JsonHandlers;
using static SurviveCore.Engine.JsonHandlers.GroundProperties;
using MoonSharp.Interpreter;

namespace SurviveCore.Engine.Entities
{
  internal abstract class Entity : IdentifiableObject
  {
    [JsonIgnore] protected World world;

    protected string id;
    protected EntityProperties properties;

    [JsonIgnore] protected List<string> tags = new();
    protected int t;

    // should these be vector3? thinking we want to support elevations
    [JsonIgnore] private Vector2 lastPosition;
    protected Vector2 position;
    protected Vector2 velocity;
    protected float elevation = 0;
    [JsonIgnore] protected float lastElevation = 0;
    protected float velocityElevation = 0;

    [JsonIgnore] protected FacingDirection direction = FacingDirection.Down;
    [JsonIgnore] protected SpriteRotationType rotationType = SpriteRotationType.None;
    [JsonIgnore] protected FacingDirection currentVisualDirection = FacingDirection.Down;
    [JsonIgnore] protected FacingDirection lastVisualDirection = FacingDirection.Down;

    [JsonIgnore] protected Dictionary<string, int> spriteDimensions = new();
    [JsonIgnore] public int feetOffsetY = 2;

    [JsonIgnore] protected bool grounded = false;

    protected float health;

    protected Inventory inventory;

    [JsonIgnore] protected Texture2D texture;
    [JsonIgnore] protected Texture2D shadowTexture;

    // lua scripts
    [JsonIgnore] protected Script lua;


    /// <summary>
    /// 
    /// </summary>
    public Entity(string id, World world) : base()
    {
      this.id = id;
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
    /// Called when this object should re-obtain its assets.
    /// </summary>
    /// <param name="id">The string id of the object. Handled on a per-subclass basis.</param>
    public virtual void UpdateAssets()
    {
      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      shadowTexture = Warehouse.GetTexture(properties.shadow);

      rotationType = properties.rotationType;
      spriteDimensions = properties.spriteDimensions;
      health = properties.maxHealth;
      tags = properties.tags;

      // create inventory
      inventory = new(properties.inventorySize);

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua))
      {
        lua = Warehouse.GetLua(properties.lua);
      }
    }

    /// <summary>
    /// Call before the base Update method.
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void PreUpdate(int tick, float deltaTime)
    {
      lastElevation = elevation;
      lastPosition = position;

    }


    /// <summary>
    /// Run this entity's main code.
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void Update(int tick, float deltaTime)
    {
      PreUpdate(tick, deltaTime);

      //luaTick.Call(luaTick.Globals["update"], this);

      PostUpdate(tick, deltaTime);
    }

    /// <summary>
    /// Call after the base Update method.
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void PostUpdate(int tick, float deltaTime)
    {

      // apply gravity and ground collisions
      if (properties.affectedByGravity)
      {
        // apply gravity
        velocityElevation -= world.GetGravity();

        // land on ground
        int elevationHere = world.GetStandingTileElevation(position);
        if (velocityElevation < 0 && elevation + velocityElevation <= elevationHere)
        {
          elevation = elevationHere;
          velocityElevation = 0;
          grounded = true;
        }
        else
        {
          // fall
          elevation += velocityElevation;
          grounded = false;
        }

      }


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

      int width = 16;
      int height = 16;
      if (spriteDimensions != null)
      {
        spriteDimensions.TryGetValue("width", out width);
        spriteDimensions.TryGetValue("height", out height);
        spriteDimensions.TryGetValue("feetOffsetY", out feetOffsetY);
      }

      // set up clipping rectangle
      // rendering breaks slightly with non power-of-2 sprites, try to use powers of 2 where possible.
      Rectangle clippingRect = new(
        frameX * width,
        (int)MathF.Floor(MathF.Max(spriteY, 0)) * height,
        width,
        height
      );

      //ELDebug.Log(id + ": " + width + " " + clippingRect.Y);

      // draw, with the bottom of the sprite as its centre
      float myElevation = GetVisualElevation(tickProgress);
      GameDisplay.Draw(shadowTexture, shadowTexture.Bounds, GetVisualPosition(tickProgress) - Vector2.UnitY, visualOffsetX: -shadowTexture.Width / 2, visualOffsetY: 1-world.GetStandingTileElevation(GetVisualPosition(tickProgress)) - shadowTexture.Height / 2, colour: Color.White * 0.5f);
      GameDisplay.Draw(texture, clippingRect, GetVisualPosition(tickProgress), visualOffsetX: -width / 2, visualOffsetY: feetOffsetY - myElevation - height);

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

    public virtual float GetElevation()
    {
      return elevation;
    }
    public virtual float GetVisualElevation(float tickProgress)
    {
      // lerp between elevations
      return MathHelper.Lerp(lastElevation, elevation, tickProgress);
    }

    public virtual Inventory GetInventory()
    {
      return inventory;
    }

    public virtual List<string> GetTags()
    {
      return tags;
    }

    public Dictionary<string, int> GetHitbox()
    {
      // this can't read the properties
      if (properties != null)
      {
        return properties.hitbox;
      }
      else
      {
        ELDebug.Log("null properties: " + id);
        return new();
      }
    }

    public virtual float GetDurability()
    {
      return health;
    }

    public virtual float GetStrength()
    {
      return health;
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

      // ground tile collisions
      //delta = world.HandleEntityMovement(this, delta);

      TileMap map = world.GetMap();

      // ground tiles
      GroundTile tileCurrent = map.Get(GetPosition());
      GroundTile checkTile;

      Point hitbox;

      properties.hitbox.TryGetValue("width", out hitbox.X);
      properties.hitbox.TryGetValue("height", out hitbox.Y);

      //todo: if going too fast, entity can snap to higher elevations before colliding with walls?
      // seen with test.chaser's charge state

      // horizontal
      for (int d = 0; d <= MathF.Abs(delta.X); d++)
      {
        // right
        if (delta.X > 0)
        {
          checkTile = map.Get((int)(position.X + d + hitbox.X / 2), (int)(position.Y), pixel: true);

          if (
            tileCurrent?.GetSlope() != SlopeType.Horizontal &&
            checkTile?.GetSlope() != SlopeType.Horizontal &&
            elevation < checkTile?.GetElevation(pixels: true)
          )
          {
            delta.X = 0;
            position.X = TileMap.SnapPosition(position).X + TileMap.TILE_WIDTH - hitbox.X / 2;
          }
        }

        // left
        else if (delta.X < 0)
        {
          checkTile = map.Get((int)(position.X - d - hitbox.X / 2), (int)(position.Y), pixel: true);

          if (
            tileCurrent?.GetSlope() != SlopeType.Horizontal &&
            checkTile?.GetSlope() != SlopeType.Horizontal &&
            elevation < checkTile?.GetElevation(pixels: true)
          )
          {
            delta.X = 0;
            position.X = TileMap.SnapPosition(position).X + hitbox.X / 2;
          }
        }
      }

      // vertical
      for (int d = 0; d <= MathF.Abs(delta.Y); d++)
      {
        // down
        if (delta.Y > 0)
        {
          checkTile = map.Get((int)(position.X), (int)(position.Y + d + hitbox.Y / 2), pixel: true);

          if (
            tileCurrent?.GetSlope() != SlopeType.Vertical &&
            checkTile?.GetSlope() != SlopeType.Vertical &&
            elevation < checkTile?.GetElevation(pixels: true)
          )
          {
            delta.Y = 0;
            position.Y = TileMap.SnapPosition(position).Y + TileMap.TILE_HEIGHT - hitbox.Y / 2;
          }
        }

        // up
        else if (delta.Y < 0)
        {
          checkTile = map.Get((int)(position.X), (int)(position.Y - d - hitbox.Y / 2), pixel: true);

          if (
            tileCurrent?.GetSlope() != SlopeType.Vertical &&
            checkTile?.GetSlope() != SlopeType.Vertical &&
            elevation < checkTile?.GetElevation(pixels: true)
          )
          {
            delta.Y = 0;
            position.Y = TileMap.SnapPosition(position).Y + hitbox.Y / 2;
          }
        }
      }


      /*/todo: tileEntity/entity collision
      foreach (Entity otherEntity in world.GetCollidingEntities(this))
      {
        // get entity's hitbox and eject self from it based on velocity.
        ResolveCollision(otherEntity.hitboxOrWhatever);
      }
      //*/


      position += delta;

      if (delta != Vector2.Zero) direction = GetFacingDirection(delta);

      return delta;
    }

    public void SetX(float x)
    {
      position.X = x;
    }
    public void SetY(float y)
    {
      position.Y = y;
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
