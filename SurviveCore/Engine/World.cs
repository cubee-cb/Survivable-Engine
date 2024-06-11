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
    GameInstance gameInstance;

    public World()
    {
      entities = new List<Entity>();
      generator = new OverworldGenerator(); // default to overworld if not specified

      generator.Generate(map);

    }

    public World(GameInstance gameInstance, WorldGenerator generator)
    {
      entities = new List<Entity>();
      this.gameInstance = gameInstance;
      this.generator = generator;

      this.generator.Generate(map);
    }

    public void LoadContent(ContentManager content)
    {
      // load world's data
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

    public void Draw(SpriteBatch spriteBatch, float tickProgress)
    {
      map.Draw(spriteBatch, tickProgress);


      // draw world's entities
      foreach (Entity entity in entities)
      {
        entity.Draw(spriteBatch, tickProgress);
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
