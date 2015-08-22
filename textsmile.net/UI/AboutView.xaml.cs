using System.Windows;

namespace textsmile.net.UI {
   public partial class AboutView : Window {
      public AboutView() {
         InitializeComponent();
      }

      private void onLoaded(object sender, RoutedEventArgs e) {
         VersionTextBlock.Text = $"Version {VersionHelper.Version}";
      }

      private void onOkButtonClicked(object sender, RoutedEventArgs e) {
         Close();
      }

      private void onViewOnGithubButtonClicked(object sender, RoutedEventArgs e) {
         UrlHelper.OpenUrl(UrlHelper.MAIN);
      }

      private void onLicenseButtonClicked(object sender, RoutedEventArgs e) {
         UrlHelper.OpenUrl("https://www.gnu.org/licenses/gpl-3.0.en.html");
      }

      private void onReleasesClicked(object sender, RoutedEventArgs e) {
         UrlHelper.OpenUrl(UrlHelper.RELEASES);
      }
   }
}
