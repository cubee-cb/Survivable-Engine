using SurviveCore.Engine.Networking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal abstract class SyncedObject : ISynced
  {
    private int id;

    public SyncedObject()
    {
      id = GetHashCode();
    }

    public abstract bool InterpretTicket(Ticket ticket);

    public abstract Ticket ScribeTicket();

    int GetID()
    {
      return id;
    }

  }
}
