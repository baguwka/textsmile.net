using System;
using JetBrains.Annotations;

namespace textsmile.net.Model {
   [UsedImplicitly]
   public class SaveLoadController {
      public const string WRITE_IF_CORRUPTED = "";

      private readonly ISerializer _serializer;
      private readonly IDataProvider _dataProvider;

      public SaveLoadController([NotNull] ISerializer serializer, [NotNull] IDataProvider dataProvider) {
         if (serializer == null) {
            throw new ArgumentNullException(nameof(serializer));
         }
         if (dataProvider == null) {
            throw new ArgumentNullException(nameof(dataProvider));
         }
         _serializer = serializer;
         _dataProvider = dataProvider;
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
            serialized = _serializer.Serialize(data);
         }
         catch (DataCorruptedException ex) {
            var result = dataCorruptedHandler(ex);
            if (result) {
               return;
            }
         }

         _dataProvider.Write(key, serialized);
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

         var readResult = _dataProvider.Read(key);

         if (string.IsNullOrEmpty(readResult)) {
            return null;
         }

         try {
            data = _serializer.Deserialize<T>(readResult);
         }
         catch (DataCorruptedException ex) {
            var result = dataCorruptedHandler(ex);
            if (result) {
               _dataProvider.Write(key, WRITE_IF_CORRUPTED);
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
