using System;
using JetBrains.Annotations;

namespace textsmile.net.Model {
   [UsedImplicitly]
   public class SaveLoadController {
      public const string WRITE_IF_CORRUPTED = "";

      private readonly ISerializer _io;
      private readonly IDataProvider _provider;

      public SaveLoadController([NotNull] ISerializer io, [NotNull] IDataProvider provider) {
         if (io == null) {
            throw new ArgumentNullException(nameof(io));
         }
         if (provider == null) {
            throw new ArgumentNullException(nameof(provider));
         }
         _io = io;
         _provider = provider;
      }

      /// <summary>
      /// Save the data to <see cref="IDataProvider"/> with associated key. This is most secured way to store data because of DataCorrupt handler
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="key">Key value to associate with data.</param>
      /// <param name="data">Whatever data you want to save.</param>
      /// <param name="dataCorruptedHandler">On-Data corrupted handler. Handle and decide erase all data or not</param>
      public void Save<T>(string key, T data, [NotNull] Func<DataCorruptedException, bool> dataCorruptedHandler) where T : class {
         if (dataCorruptedHandler == null) {
            throw new ArgumentNullException(nameof(dataCorruptedHandler));
         }

         var serialized = string.Empty;

         try {
            serialized = _io.Serialize(data);
         }
         catch (DataCorruptedException ex) {
            var result = dataCorruptedHandler(ex);
            if (result) {
               return;
            }
         }

         _provider.Write(key, serialized);
      }

      /// <summary>
      /// Load the data by key from <see cref="IDataProvider"/>. This is most secured way to store data because of DataCorrupt handler
      /// </summary>
      /// <param name="key">Key value associated with data</param>
      /// <param name="dataCorruptedHandler">On-Data corrupted handler. Handle and decide erase all data or not</param>
      public T Load<T>(string key, [NotNull] Func<DataCorruptedException, bool> dataCorruptedHandler) where T : class {
         if (dataCorruptedHandler == null) {
            throw new ArgumentNullException(nameof(dataCorruptedHandler));
         }

         T data = null;

         var readResult = _provider.Read(key);
         var serializedData = readResult;

         try {
            data = _io.Deserialize<T>(serializedData);
         }
         catch (DataCorruptedException ex) {
            var result = dataCorruptedHandler(ex);
            if (result) {
               _provider.Write(key, WRITE_IF_CORRUPTED);
            }
         }

         return data;
      }

      /// <summary>
      /// Load the data by key from <see cref="IDataProvider"/>. This is most secured way to store data because of DataCorrupt handler
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="key">Key value associated with data</param>
      /// <param name="data">Whatever data you want to load.</param>
      /// <param name="dataCorruptedHandler">On-Data corrupted handler. Handle and decide erase all data or not</param>
      public bool TryLoad<T>(string key, out T data, [NotNull] Func<DataCorruptedException, bool> dataCorruptedHandler) where T : class {
         if (dataCorruptedHandler == null) {
            throw new ArgumentNullException(nameof(dataCorruptedHandler));
         }

         data = Load<T>(key, dataCorruptedHandler);
         return data != null;
      }

      /// <summary>
      /// Save the data to <see cref="IDataProvider"/> with associated key.
      /// </summary>
      /// <param name="key">Key value to associate with data.</param>
      /// <param name="data">Whatever data you want to save.</param>
      public void Save<T>(string key, T data) where T : class {
         Save(key, data, exception => true);
      }

      /// <summary>
      /// Load the data by key from <see cref="IDataProvider"/>.
      /// </summary>
      /// <param name="key">Key value associated with data</param>
      public T Load<T>(string key) where T : class {
         return Load<T>(key, exception => true);
      }

      /// <summary>
      /// Load the data by key from <see cref="IDataProvider"/>.
      /// </summary>
      /// <param name="key">Key value associated with data</param>
      /// <param name="data">Whatever data you want to load.</param>
      public bool TryLoad<T>(string key, out T data) where T : class {
         return TryLoad(key, out data, exception => true);
      }
   }
}
