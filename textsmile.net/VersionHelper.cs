using System;

namespace textsmile.net {
   public static class VersionHelper {
      static VersionHelper() {
         Version = new Version(0, 3);
      }

      public static Version Version { get; private set; }
   }
}