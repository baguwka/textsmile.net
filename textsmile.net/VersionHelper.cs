using System;

namespace textsmile.net {
   public static class VersionHelper {
      static VersionHelper() {
         Version = new Version(0, 4);
      }

      public static Version Version { get; private set; }
   }
}