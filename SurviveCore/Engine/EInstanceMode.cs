using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal enum EInstanceMode
  {
    Host = 0, // instance who is playing alone or locally hosting. / fully functional
    Client = 1, // instance who is joining an existing server. / no worldgen / sync world+game state from host
    Dedicated = 2, // instance dedicated only to hosting the world. / no rendering / no player / sleeps when no players
  }
}
