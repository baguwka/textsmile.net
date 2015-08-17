using System;
using System.Runtime.Serialization;

namespace textsmile.net.Model {
   [Serializable]
   public class DataCorruptedException : Exception {
      public DataCorruptedException() {
      }

      public DataCorruptedException(string message) : base(message) {
      }

      public DataCorruptedException(string message, Exception inner) : base(message, inner) {

      }

      protected DataCorruptedException(
         SerializationInfo info,
         StreamingContext context) : base(info, context) {
      }
   }
}