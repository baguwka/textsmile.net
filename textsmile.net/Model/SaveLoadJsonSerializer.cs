using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using textsmile.net.Annotations;

namespace textsmile.net.Model {
   [UsedImplicitly]
   public class SaveLoadJsonSerializer : ISerializer {
      public string Serialize<T>(T data) where T : class{
         string serialized;
         try {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            serialized = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
         }
         catch (JsonSerializationException ex) {
            Debug.WriteLine(ex);
            throw new DataCorruptedException(ex.Message, ex);
         }
         catch (JsonWriterException ex) {
            Debug.WriteLine(ex);
            throw new DataCorruptedException(ex.Message, ex);
         }

         return serialized;
      }

      public T Deserialize<T>(string serialized) where T : class {
         T data;

         using (var reader = new StringReader(serialized)) {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
            try {
               data = (T)serializer.Deserialize(reader, typeof (T));
            }
            catch (JsonReaderException ex) {
               Debug.WriteLine(ex);
               throw new DataCorruptedException(ex.Message, ex);
            }
            catch (JsonSerializationException ex) {
               Debug.WriteLine(ex);
               throw new DataCorruptedException(ex.Message, ex);
            }
            catch (JsonWriterException ex) {
               Debug.WriteLine(ex);
               throw new DataCorruptedException(ex.Message, ex);
            }
            catch (InvalidCastException ex) {
               Debug.WriteLine(ex);
               throw new DataCorruptedException(ex.Message, ex);
            }
            catch (TypeLoadException ex) {
               Debug.WriteLine(ex);
               throw new DataCorruptedException(ex.Message, ex);
            }
            finally {
               reader.Close();
            }
         }

         return data;
      }
   }
}
