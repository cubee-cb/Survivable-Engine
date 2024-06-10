using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurviveDesktop
{
  public static class Platform
  {
    public const string PLATFORM_NAME = "desktop";


    // load a text file from the root folder
    // i.e. pass @"Content/lua/textFile.lua"
    public static string LoadContentFile(string path)
    {
      Stream stream = TitleContainer.OpenStream(path);

      string fileContents = "";
      using (StreamReader reader = new StreamReader(stream))
      {
        fileContents = reader.ReadToEnd();
      }
      stream.Close();

      return fileContents;
    }

  }
}
