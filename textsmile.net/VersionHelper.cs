using System;

namespace textsmile.net {
   public static class VersionHelper {
      static VersionHelper() {
         Version = new Version(0, 2);
      }

      public static Version Version { get; private set; }
   }
}