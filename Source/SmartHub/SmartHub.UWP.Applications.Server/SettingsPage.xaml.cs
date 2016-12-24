using SmartHub.UWP.Core;
using SmartHub.UWP.Core.StringResources;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartHub.UWP.Applications.Server
{
    public class LanguageItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public sealed partial class SettingsPage : Page
    {
        #region Constructor
        public SettingsPage()
        {
            InitializeComponent();
            InitLanguageList();
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
            for (int i = 0; i < 10; i++)
                (Application.Current.Resources["LabelsManager"] as LabelsManager).RefreshResources();

            AppShell.Current.SetNavigationInfo("Settings", "menuSettings");
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
        #endregion
    }
}
