using System.Diagnostics;
using textsmile.net.Annotations;

namespace textsmile.net.Model {
   [UsedImplicitly]
   public class IoManager {
      private readonly ISerializer _io;
      private readonly IDataProvider _provider;

      public IoManager(ISerializer io, IDataProvider provider) {
         _io = io;
         _provider = provider;
      }

      public void Save<T>(string key, T data) where T : class {
         var serialized = _io.Serialize(data);
         if (string.IsNullOrEmpty(serialized)) {
            Debug.WriteLine("serialized data is empty");
            return;
         }
         _provider.Write(key, serialized);
      }

      public bool TryLoad<T>(string key, out T data) where T : class {
         var readResult = _provider.Read(key);
         var serializedData = readResult;
         data =_io.Deserialize<T>(serializedData);
         return data != null;
      }
   }
}