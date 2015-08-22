using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace textsmile.net {
   public static class UrlHelper {
      public const string MAIN = "https://github.com/baguwka/textsmile.net";
      public const string RELEASES = "https://github.com/baguwka/textsmile.net/releases";
      public const string README = "https://github.com/baguwka/textsmile.net#readme";

      public static bool IsUrlValid(string url) {
         return Regex.IsMatch(url, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
      }

      public static void OpenUrl(string url) {
         if (!IsUrlValid(url)) {
            MessageBox.Show($"App trying to open incorrect Url ({url})", "Incorrect url", MessageBoxButton.OK,
               MessageBoxImage.Error);
            return;
         }

         var result = MessageBox.Show("This will be opened in your default borwser, proceed?", "Open in browser request",
            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

         if (result == MessageBoxResult.Yes) {
            Process.Start(url);
         }
      }
   }
}