using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Entities;
using SurviveCore.Engine.JsonHandlers;
using SurviveCore.Engine.WorldGen;
using System;
using System.Collections.Generic;
using System.Text;
using static SurviveCore.Engine.JsonHandlers.GroundProperties;

namespace SurviveCore.Engine
{
  //todo: we could probably make a new class for each world type, but that's probably best left for the games/mods
  internal class World
  {
    string id;

    TileMap map;
    WorldGenerator generator;
    List<Entity> entities;
    List<Entity> activeEntities;

    WorldProperties properties;

    private GameInstance parentInstance;


    public World(string id, GameInstance instance)
    {
      parentInstance = instance;

      properties = Warehouse.GetJson<WorldProperties>(id);

      int width = 8;
      int height = 8;
      if (properties.area != null)
      {
        properties.area.TryGetValue("width", out width);
        properties.area.TryGetValue("height", out height);
      }

      map = new TileMap(width, height);

      entities = new();

      generator = new WorldGenerator(properties.generationRoutines);
      generator.Generate(map);
    }

    public void UpdateAssets()
    {
      //generator.UpdateAssets();

      //todo: update the world's properties when they have them.

      map.UpdateAssets();

      foreach (Entity entity in entities)
      {
        entity.UpdateAssets();
      }
    }

    public void Update(int tick, float deltaTime)
    {
      ELDebug.Log("update");
      //activeEntities.Clear();
      activeEntities = entities;

      // update world's entities
      //todo: create a partitioning system so only entities near the camera get updated
      foreach (Entity entity in activeEntities)
      {
        entity.Update(tick, deltaTime);
      }

      // collide entities
      for (int a = 0; a < activeEntities.Count; a++)
      {
        Entity entityA = activeEntities[a];

        // with all following entities
        for (int b = a + 1; b < activeEntities.Count; b++)
        {
          Entity entityB = activeEntities[b];

          /*/ skip if same
          if (entityA == entityB)
          {
            ELDebug.Log("this should never execute lol, but we tried to check the same entity in collisions"); continue;
          }
          //*/

          // collide
          //todo: get height of entity rather than hardcoded +-12px
          bool withinElevation = MathF.Abs(entityA.GetElevation() - entityB.GetElevation()) < 12;
          if (entityA.GetHitbox().Intersects(entityB.GetHitbox()) && withinElevation)
          {
            // don't execute OnCollisionEnter for already colliding entities
            bool clearA = entityA.RegisterCollidingEntity(entityB);
            bool clearB = entityB.RegisterCollidingEntity(entityA);

            if (clearA && clearB)
            {
              entityA.OnCollisionEnter(entityB);
              entityB.OnCollisionEnter(entityA);
            }
          }
          else
          {
            // do OnCollisionExit and unregister colliding entities
            if (entityA.GetCollidingEntityIDs().Contains(entityB.GetUID()))
            {
              entityA.OnCollisionExit(entityB);
              entityB.OnCollisionExit(entityA);

              entityA.UnregisterCollidingEntity(entityB);
              entityB.UnregisterCollidingEntity(entityA);
            }

          }



        }
      }


    }

    public void Draw(float tickProgress)
    {
      ELDebug.Log("draw");
      map.Draw(tickProgress);


      // draw world's entities
      foreach (Entity entity in activeEntities)
      {
        entity.Draw(tickProgress);
      }

    }

    public void AddEntity(Entity newEntity)
    {
      entities.Add(newEntity);
    }

    public void RemoveEntity(Entity removeEntity)
    {
      //removeEntity.Dispose();
      entities.Remove(removeEntity);
    }

    public TileMap GetMap()
    {
      return map;
    }

    /// <summary>
    /// Get the elevation of the tile at the specified position, in pixels. Adapts to slopes.
    /// </summary>
    /// <param name="position">The position to get the elevation of.</param>
    /// <returns>The elevation at the specified position.</returns>
    public int GetStandingTileElevation(Vector2 position)
    {
      GroundTile tile = map.Get(position);
      if (tile == null) return 0;

      switch (tile.GetSlope())
      {
        // return the tile's elevation if it's just a flat tile
        default:
        case SlopeType.None:
          return tile.GetElevation(pixels: true);

        // for slopes, we have to get a little more creative
        case SlopeType.Horizontal:
          {
            // 0-1 value of the position across the tile
            float progress = position.X % TileMap.TILE_WIDTH / TileMap.TILE_WIDTH;

            GroundTile tileLeft = map.Get(position - Vector2.UnitX * TileMap.TILE_WIDTH);
            GroundTile tileRight = map.Get(position + Vector2.UnitX * TileMap.TILE_WIDTH);

            int elevationLeft = tileLeft != null ? tileLeft.GetElevation(pixels: true) : 0;
            int elevationRight = tileRight != null ? tileRight.GetElevation(pixels: true) : 0;

            return (int)MathF.Floor(MathHelper.Lerp(elevationLeft, elevationRight, progress) + 0.5f);

            //return (int)MathF.Abs((elevationLeft - elevationRight) * progress);
          }

        //todo: duped code. on a scale of POOM to Palworld, how much can this be optimised?
        case SlopeType.Vertical:
          {
            // 0-1 value of the position across the tile
            float progress = position.Y % TileMap.TILE_HEIGHT / TileMap.TILE_HEIGHT;

            GroundTile tileUp = map.Get(position - Vector2.UnitY * TileMap.TILE_HEIGHT);
            GroundTile tileDown = map.Get(position + Vector2.UnitY * TileMap.TILE_HEIGHT);

            int elevationUp = tileUp != null ? tileUp.GetElevation(pixels: true) : 0;
            int elevationDown = tileDown != null ? tileDown.GetElevation(pixels: true) : 0;

            return (int)MathF.Floor(MathHelper.Lerp(elevationUp, elevationDown, progress) + 0.5f);

            //return (int)MathF.Abs((elevationUp - elevationDown) * progress);
          }
      }

    }

    public float GetGravity()
    {
      return properties.gravity;
    }

    public GameInstance GetInstance()
    {
      return parentInstance;
    }

    public Entity FindEntityWithTag(Entity callingEntity, string tag, MatchCondition condition = MatchCondition.Nearest)
    {
      // get list of targets matching the tag
      List<Entity> validTargets = new();
      foreach (Entity entity in entities)
      {
        if (entity.GetTags().Contains(tag))
        {
          validTargets.Add(entity);
        }
      }

      // return null if there's no valid targets
      if (validTargets.Count == 0) return null;

      // find one entity matching the match condition
      Entity returnEntity = validTargets[0];
      switch (condition)
      {
        // a random one
        default:
        case MatchCondition.Random:
          returnEntity = validTargets[Game1.rnd.Next(validTargets.Count)];
          return returnEntity;

        case MatchCondition.Nearest:
          foreach (Entity entity in validTargets)
          {
            // select this entity if it's closer
            if (Vector2.Distance(entity.GetPosition(), callingEntity.GetPosition()) < Vector2.Distance(returnEntity.GetPosition(), callingEntity.GetPosition()))
            {
              returnEntity = entity;
            }
          }
          return returnEntity;

        case MatchCondition.Farthest:
          foreach (Entity entity in validTargets)
          {
            // select this entity if it's further
            if (Vector2.Distance(entity.GetPosition(), callingEntity.GetPosition()) > Vector2.Distance(returnEntity.GetPosition(), callingEntity.GetPosition()))            {
              returnEntity = entity;
            }
          }
          return returnEntity;

        case MatchCondition.Strongest:
          foreach (Entity entity in validTargets)
          {
            // select this entity if it's more powerful
            if (entity.GetStrength() > returnEntity.GetStrength())
            {
              returnEntity = entity;
            }
          }
          return returnEntity;

        case MatchCondition.Weakest:
          foreach (Entity entity in validTargets)
          {
            // select this entity if it's weaker
            if (entity.GetStrength() < returnEntity.GetStrength())
            {
              returnEntity = entity;
            }
          }
          return returnEntity;

        case MatchCondition.MostDurable:
          foreach (Entity entity in validTargets)
          {
            // select this entity if it's more durable
            if (entity.GetDurability() > returnEntity.GetDurability())
            {
              returnEntity = entity;
            }
          }
          return returnEntity;

        case MatchCondition.MostFragile:
          foreach (Entity entity in validTargets)
          {
            // select this entity if it's less durable
            if (entity.GetDurability() < returnEntity.GetDurability())
            {
              returnEntity = entity;
            }
          }
          return returnEntity;
      }


    }


  }
}
