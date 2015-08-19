using System.Windows;
using Microsoft.Practices.Unity;
using textsmile.net.Model;

namespace textsmile.net {
   public static class MainAppSaveLoader {
      public static void Save(AppSaveData data) {
         App.Container.Resolve<SaveLoadController>().Save("textsmile.net main", data, onSaveCorrupted);
      }

      public static bool TryLoad(out AppSaveData data) {
         return App.Container.Resolve<SaveLoadController>().TryLoad("textsmile.net main", out data, onLoadCorrupted);
      }

      private static bool onLoadCorrupted(DataCorruptedException exception) {
         var result = MessageBox.Show("SaveData corrupted and cannot be loaded. " +
                                      "\nWipe all save data to prevent this error next time? " +
                                      "(Yes is recomended, but if you can restore it somehow manually, then select No)" +
                                      $"\n\n\n Details:\n{exception.Message}" +
                                      $"\n\n StackTrace:\n{exception.StackTrace}",
            "Error", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);

         switch (result) {
            case MessageBoxResult.Yes:
               return true;
            case MessageBoxResult.No:
               return false;
            default:
               return true;
         }
      }

      private static bool onSaveCorrupted(DataCorruptedException exception) {
         var result = MessageBox.Show("SaveData corrupted and cannot be saved. " +
                                         "\nBlock writing attempt to not to corrupt the save file?? " +
                                         "(Yes is recomended, but if you can restore it somehow manually, then select No)" +
                                         $"\n\n\n Details:\n{exception.Message}" +
                                         $"\n\n StackTrace:\n{exception.StackTrace}",
               "Error", MessageBoxButton.YesNo,
               MessageBoxImage.Error, MessageBoxResult.Yes);

         switch (result) {
            case MessageBoxResult.Yes:
               return true;
            case MessageBoxResult.No:
               return false;
            default:
               return true;
         }
      }
   }
}