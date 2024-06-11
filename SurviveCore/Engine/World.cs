using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

      entities = new List<Entity>();
      generator = new OverworldGenerator(); // default to overworld if not specified

      generator.Generate(map);

    }

    public World(int width, int height, WorldGenerator generator)
    {
      map = new TileMap(width, height);

      entities = new List<Entity>();
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


  }
}
