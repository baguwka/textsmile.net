using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace textsmile.net.Model {
   public class JsonIOSerializer : ISerializer {
      public string Serialize<T>(T data) where T : class{
         string serialized;
         try {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            serialized = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
         }
         catch (JsonSerializationException ex) {
            Debug.WriteLine(ex);
            return string.Empty;
         }
         catch (JsonWriterException ex) {
            Debug.WriteLine(ex);
            return string.Empty;
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
               data = null;
               Debug.WriteLine(ex);
            }
            catch (JsonSerializationException ex) {
               data = null;
               Debug.WriteLine(ex);
            }
            catch (JsonWriterException ex) {
               data = null;
               Debug.WriteLine(ex);
            }
            catch (InvalidCastException ex) {
               data = null;
               Debug.WriteLine(ex);
            }
            catch (TypeLoadException ex) {
               data = null;
               Debug.WriteLine(ex);
            }
            finally {
               reader.Close();
            }
         }

         return data;
      }
   }
}