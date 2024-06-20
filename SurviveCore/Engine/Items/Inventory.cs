using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Items
{
  internal class Inventory
  {
    private List<Item> inventory;

    public Inventory(int size)
    {
      inventory = new();
      for (int i = 0; i < size; i++)
      {
        inventory.Add(null);
      }
    }

    public Item TakeItem(int slot)
    {
      if (slot < 0 || slot >= inventory.Count)
      {
        ELDebug.Log("tried to take item from out of bounds inventory slot " + slot);
        return null;
      }

      Item item = inventory[slot];
      inventory[slot] = null;
      return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slot">The slot to place the item in.</param>
    /// <returns>The item that was previously in the slot.</returns>
    public Item PlaceItem(int slot, Item item)
    {
      if (slot < 0 || slot >= inventory.Count)
      {
        ELDebug.Log("tried to place item in out of bounds inventory slot " + slot);
        return null;
      }

      Item existingItem = inventory[slot];
      inventory[slot] = item;
      return existingItem;
    }


  }
}
