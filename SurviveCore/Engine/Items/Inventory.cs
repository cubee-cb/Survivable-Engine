using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Display;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurviveCore.Engine.Items
{
  internal class Inventory
  {
    readonly private List<Item> items;

    public Inventory(int size)
    {
      items = new();
      for (int i = 0; i < size; i++)
      {
        items.Add(null);
      }
    }

    /// <summary>
    /// Take an item out of a specific inventory slot.
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public Item TakeItem(int slot)
    {
      if (slot < 0 || slot >= items.Count)
      {
        ELDebug.Log("tried to take item from out of bounds items slot " + slot);
        return null;
      }

      Item item = items[slot];
      items[slot] = null;
      return item;
    }

    /// <summary>
    /// Place an item into a specific inventory slot, swapping with what's there.
    /// </summary>
    /// <param name="slot">The slot to place the item in.</param>
    /// <returns>The item that was previously in the slot.</returns>
    public Item PlaceItem(int slot, Item item)
    {
      if (slot < 0 || slot >= items.Count)
      {
        ELDebug.Log("tried to place item in out of bounds items slot " + slot);
        return null;
      }

      Item existingItem = items[slot];
      items[slot] = item;
      return existingItem;
    }

    /// <summary>
    /// Places an item in the next available slot. If there's no space, return false.
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <returns>True if the item could be placed.</returns>
    public bool AddItem(Item item)
    {
      for (int slot = 0; slot < items.Count; slot++)
      {
        if (items[slot] == null)
        {
          items[slot] = item;
          return true;
        }
      }

      return false;
    }

    public List<Item> GetItems()
    {
      return items;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void Update(int tick, float deltaTime)
    {
      foreach (Item item in items)
      {
        item.Update(tick, deltaTime);
      }
    }

    /// <summary>
    /// deprecated; use a UILayout to draw inventories
    /// </summary>
    /// <param name="position"></param>
    /// <param name="unitsWide">Amount of units wide to display the inventory at.</param>
    /// <param name="tickProgress">A value from 0-1 showing the progress through the current tick, for smoothing purposes.</param>
    public virtual void Draw(Vector2 position, int unitsWide, float tickProgress)
    {
      ELDebug.Log("self-drawing inventories are deprecated; use a UILayout to draw inventories");

      // a "unit" here is the amount of screen pixels that correspond to an in-world pixel.
      float unitScale = 4; //todo: get the game scale somehow

      int cellSize = 20;
      int cellsWide = (int)(MathF.Max(1, MathF.Floor(unitsWide / cellSize)));

      int index = 0;
      foreach (Item item in items)
      {
        if (item != null)
        {
          Vector2 thisCellPosition = position + new Vector2(index % cellsWide, MathF.Floor(index / cellsWide)) * unitScale;

          item.Draw(position, tickProgress);
        }

        index++;
      }
      //*/

    }

  }
}
