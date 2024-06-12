using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Networking
{
  internal interface ISynced
  {
    /// <summary>
    /// Converts this object into a Ticket, for sending over the network.
    /// </summary>
    /// <returns>A Ticket.</returns>
    public Ticket ScribeTicket();

    /// <summary>
    /// Reads a ticket and updates this object according to its content.
    /// </summary>
    /// <param name="ticket">Received Ticket.</param>
    /// <returns>Success.</returns>
    public bool InterpretTicket(Ticket ticket);

    /// <summary>
    /// Return a (hopefully) unique network ID for this object.
    /// </summary>
    /// <returns>An int HashCode.</returns>
    public int GetNetworkID()
    {
      return GetHashCode();
    }

  }
}
