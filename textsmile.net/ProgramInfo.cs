using System.Reflection;
using System.Runtime.InteropServices;

namespace textsmile.net {
   public static class ProgramInfo {
      static public string AssemblyGuid {
         get {
            var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
            return attributes.Length == 0 
               ? string.Empty 
               : ((GuidAttribute)attributes[0]).Value;
         }
      }
   }
}