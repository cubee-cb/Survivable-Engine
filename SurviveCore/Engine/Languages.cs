using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  public static class Languages
  {
    //todo: languages support
    private static Language currentLanguage;

    public static void SetCurrentLanguage(Language newLanguage)
    {
      currentLanguage = newLanguage;
    }

    public static string Translate(string key)
    {
      // key when no language
      if (currentLanguage.associations == null || currentLanguage.associations.Count == 0) return key;

      // final string when yes language and exists
      if (currentLanguage.associations.TryGetValue(key, out string output))
      {
        return output;
      }

      // key otherwise
      return key;
    }

    public struct Language
    {
      public Dictionary<string, string> associations;
    }
  }

}
