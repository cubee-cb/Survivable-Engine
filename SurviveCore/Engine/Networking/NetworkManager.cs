using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Networking
{
  internal class NetworkManager
  {
    private Dictionary<int, ISynced> syncedObjects;
    private List<Ticket> caughtTickets;

    public NetworkManager()
    {
      syncedObjects = new();
    }

    public void Register(ISynced syncedObject)
    {
      syncedObjects.Add(syncedObject.GetNetworkID(), syncedObject);
    }

    public void ThrowTickets()
    {
      foreach (KeyValuePair<int, ISynced> kvp in syncedObjects)
      {
        ISynced syncedObject = kvp.Value;

        Ticket ticket = syncedObject.ScribeTicket();
        ticket.SignTicket(syncedObject.GetNetworkID());


      }

      // send
    }

    public void CatchTickets()
    {
      // get received tickets
      foreach (Ticket ticket in caughtTickets)
      {
        int ownerID = ticket.GetSigner();

        // give the ticket to the owner to read
        if (syncedObjects.ContainsKey(ownerID))
        {
          syncedObjects[ownerID].InterpretTicket(ticket);
        }

        // if the owner isn't around, summon them
        else
        {
          ELDebug.Log("caught a ticket for " + ownerID + ", who hasn't yet arrived. reminding them now...");
          //todo: create the object based on the ticket

        }

      }

      // update list entries, and make new ones if needed

    }

  }
}
