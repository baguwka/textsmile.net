using System;
using System.IO;

namespace textsmile.net.Model {
   public class AppDataFolderProvider : IDataProvider {
      protected string _dataPath;

      protected string DataPath {
         get {
            if (string.IsNullOrEmpty(_dataPath)) {
               _dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Baguwk\\textsmile.net\\";
               if (!Directory.Exists(_dataPath)) {
                  Directory.CreateDirectory(_dataPath);
               }
            }
            return _dataPath;
         }
      }

      private string getFilePath(string key) {
         return $"{DataPath}/{fileMask(key)}.json";
      }

      protected static string fileMask(string filename) {
         return $"{filename}.bgw";
      }

      public string Read(string key) {
         var filepath = getFilePath(key);

         if (!File.Exists(filepath)) {
            return null;
         }

         using (var content = File.OpenText(filepath)) {
            return content.ReadToEnd();
         }
      }

      public void Write(string key, string data) {
         using (var stream = new StreamWriter(getFilePath(key))) {
            stream.Write(data);
         }
      }
   }
}