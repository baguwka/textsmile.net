using System;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using JetBrains.Annotations;
using File = System.IO.File;

namespace textsmile.net.Model.Shortcut {
   [UsedImplicitly]
   public class WSHShortcutCreator : IShortcutCreator {
      public bool Create(string shortcutPath, string targetPath, string arguments = "") {
         if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(targetPath)) {
            //try {
            var wsh = new WshShellClass();
            var shortcut = (IWshShortcut)wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Arguments = arguments;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.Save();

            return true;
            //}
            //catch (Exception e) {
            //   //DebugHelper.WriteException(e);
            //}
         }
         return false;
      }

      public bool Delete(string shortcutPath) {
         if (!string.IsNullOrEmpty(shortcutPath) && File.Exists(shortcutPath)) {
            File.Delete(shortcutPath);
            return true;
         }

         return false;
      }

      public bool SetShortcut(bool create, Environment.SpecialFolder specialFolder, string arguments = "") {
         string shortcutPath = GetShortcutPath(specialFolder);

         if (create) {
            return Create(shortcutPath, Application.ExecutablePath, arguments);
         }

         return Delete(shortcutPath);
      }

      public bool CheckShortcut(Environment.SpecialFolder specialFolder) {
         string shortcutPath = GetShortcutPath(specialFolder);
         return File.Exists(shortcutPath);
      }

      public static string GetShortcutPath(Environment.SpecialFolder specialFolder) {
         string folderPath = Environment.GetFolderPath(specialFolder);
         string shortcutPath = Path.Combine(folderPath, "textsmile.net");

         if (!Path.GetExtension(shortcutPath).Equals(".lnk", StringComparison.InvariantCultureIgnoreCase)) {
            shortcutPath = Path.ChangeExtension(shortcutPath, "lnk");
         }

         return shortcutPath;
      }
   }
}