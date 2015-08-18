using System;

namespace textsmile.net.Model.Shortcut {
   public interface IShortcutCreator {
      bool CheckShortcut(Environment.SpecialFolder specialFolder);
      bool Create(string shortcutPath, string targetPath, string arguments = "");
      bool Delete(string shortcutPath);
      bool SetShortcut(bool create, Environment.SpecialFolder specialFolder, string arguments = "");
   }
}