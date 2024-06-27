using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Entities;
using SurviveCore.Engine.WorldGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  //todo: we could probably make a new class for each world type, but that's probably best left for the games/mods
  internal class World
  {
    TileMap map;
    WorldGenerator generator;
    List<Entity> entities;

    public World()
    {
      map = new TileMap(100, 100);

      entities = new();
      generator = new OverworldGenerator(); // default to overworld if not specified

      generator.Generate(map);

    }

    public World(int width, int height, WorldGenerator generator)
    {
      map = new TileMap(width, height);

      entities = new();
      this.generator = generator;

      this.generator.Generate(map);
    }

    public void Update(int tick, float deltaTime)
    {
      // update world's entities
      //todo: create a partitioning system so only entities near the camera get updated
      foreach (Entity entity in entities)
      {
        entity.Update(tick, deltaTime);
      }

    }

    public void Draw(float tickProgress)
    {
      map.Draw(tickProgress);


      // draw world's entities
      foreach (Entity entity in entities)
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
    /// Get the elevation of the tile at the specified position, in pixels.
    /// </summary>
    /// <param name="position">The position to get the elevation of.</param>
    /// <returns>The elevation at the specified position.</returns>
    public int GetStandingTileElevation(Vector2 position)
    {
      GroundTile tile = map.Get(position);
      return tile != null? tile.GetElevation(pixels: true) : 0;
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
