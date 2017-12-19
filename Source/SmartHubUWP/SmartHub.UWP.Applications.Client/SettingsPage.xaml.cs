using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Xaml;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Client
{
    public class LanguageItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class ThemeItem
    {
        public ElementTheme Value { get; set; }
        public string Text { get; set; }
    }

    public sealed partial class SettingsPage : Page
    {
        #region Constructor
        public SettingsPage()
        {
            InitializeComponent();
            InitLanguageList();
            InitThemeList();
        }
        #endregion

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetLabelsText();
        }
        #endregion

        #region Private methods
        private void SetLabelsText()
        {
            AppShell.Current.SetNavigationInfo("Settings", "menuSettings");
            InitThemeList();
        }
        private void InitLanguageList()
        {
            cbLanguage.ItemsSource = new List<LanguageItem>() {
                new LanguageItem() { Text = "English", Value = "en-US" },
                new LanguageItem() { Text = "Русский", Value = "ru-RU" }
            };

            foreach (LanguageItem li in cbLanguage.Items)
                if (li.Value == AppManager.AppData.Language)
                {
                    cbLanguage.SelectedItem = li;
                    break;
                }
        }
        private void InitThemeList()
        {
            cbTheme.ItemsSource = new List<ThemeItem>() {
                new ThemeItem() { Text = XamlUtils.GetLocalizedString(AppManager.AppData.Language, "Default"), Value = ElementTheme.Default },
                new ThemeItem() { Text = XamlUtils.GetLocalizedString(AppManager.AppData.Language, "Light"), Value = ElementTheme.Light },
                new ThemeItem() { Text = XamlUtils.GetLocalizedString(AppManager.AppData.Language, "Dark"), Value = ElementTheme.Dark }
            };

            foreach (ThemeItem li in cbTheme.Items)
                if (li.Value == (ElementTheme) AppManager.AppData.Theme)
                {
                    cbTheme.SelectedItem = li;
                    break;
                }
        }
        #endregion

        #region Event handlers
        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selLang = (cbLanguage.SelectedItem as LanguageItem).Value;
            if (AppManager.AppData.Language != selLang)
            {
                AppManager.AppData.Language = selLang;
                SetLabelsText();
            }
        }
        private void cbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTheme.SelectedItem != null)
                AppManager.AppData.Theme = (int)(cbTheme.SelectedItem as ThemeItem).Value;
        }
        #endregion
    }
}
