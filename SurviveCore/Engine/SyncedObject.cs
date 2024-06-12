using SurviveCore.Engine.Networking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal abstract class SyncedObject : ISynced
  {
    private int id;
    private NetworkManager networkManagerRef;

    public SyncedObject(NetworkManager networkManager)
    {
      id = GetHashCode();
      networkManagerRef = networkManager;

      networkManager.Register(this);
    }

    public abstract bool InterpretTicket(Ticket ticket);

    public abstract Ticket ScribeTicket();

    int GetID()
    {
      return id;
    }

  }
}
