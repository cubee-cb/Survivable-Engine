using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Networking
{
  internal class Ticket
  {
    // idk what to put here haha
    private int ownerID = -1;
    private string message = "";

    public Ticket(string message)
    {
      this.message = message;
    }

    public void SignTicket(int ownerID)
    {
      this.ownerID = ownerID;
    }

    public int GetSigner()
    {
      return this.ownerID;
    }

    public override string ToString()
    {
      return "Ticket: " + message;
    }

  }
}
