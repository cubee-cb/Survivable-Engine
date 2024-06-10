using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  //todo: we could probably make a new class for each world type, but that's probably best left for the games/mods
  internal class World
  {
    List<Entity> entities;
    GameInstance gameInstance;

    public World()
    {
      entities = new List<Entity>();
    }

    public World(GameInstance gameInstance)
    {
      entities = new List<Entity>();
      this.gameInstance = gameInstance;
    }

    public void LoadContent(ContentManager content)
    {
      // how should we handle this? i don't think we want to pass the contentmanager to all the entitys, that seems inefficient
      // especially as then we may have a sprite per instance of an entity, even when they're using the same sprite
      
      // get entitys to load their data
      foreach (Entity entity in entities)
      {
        //content.Load<Texture2D>(entity.textureSheetName);
      }

      // load world's data
    }

    public void Update(int tick, float deltaTime)
    {

      // update world's entitys
      //todo: create a partitioning system so only entities near the camera get updated
      foreach (Entity entity in entities)
      {
        entity.Update(tick, deltaTime);
      }

    }

    public void Draw(SpriteBatch spriteBatch, float tickProgress)
    {
      // draw world's entitys
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



  }
}
