using SmartHub.UWP.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SmartHub.UWP.Applications.Server.Common
{
    public static class LocalUtils
    {
        public static void SetAppTheme()
        {
            var theme = (ElementTheme) AppManager.AppData.Theme;

            AppShell.Current.RequestedTheme = theme;

            // set title bar colors:
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                //(Color) Application.Current.Resources["SystemChromeMediumColor"];
                var colorLight = Color.FromArgb(255, 230, 230, 230); // #FFE6E6E6
                var colorDark = Color.FromArgb(255, 31, 31, 31); // #FF1F1F1F

                Color color;
                if (theme == ElementTheme.Light)
                    color = colorLight;
                else if (theme == ElementTheme.Dark)
                    color = colorDark;
                else if (theme == ElementTheme.Default)
                    color = Application.Current.RequestedTheme == ApplicationTheme.Light ? colorLight : colorDark;

                titleBar.BackgroundColor = color;
                titleBar.ButtonBackgroundColor = color;
            }
        }
    }
}
