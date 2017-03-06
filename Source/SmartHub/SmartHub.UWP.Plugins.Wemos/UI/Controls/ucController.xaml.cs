using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Controllers;
using SmartHub.UWP.Plugins.Wemos.Controllers.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Wemos.UI.Controls
{
    public sealed partial class ucController : UserControl
    {
        #region Properties
        public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register("Controller", typeof(WemosController), typeof(ucController), new PropertyMetadata(null, new PropertyChangedCallback(OnControllerChanged)));
        public WemosController Controller
        {
            get { return (WemosController) GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }
        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue as WemosController != null)
                (d as ucController).LoadEditor();
        }
        #endregion

        #region Constructor
        public ucController()
        {
            InitializeComponent();
            Utils.FindFirstVisualChild<Grid>(this).DataContext = this;
        }
        #endregion

        #region Private methods
        private void LoadEditor()
        {
            var ctrl = WemosControllerBase.FromController(Controller);
            switch (Controller.Type)
            {
                case WemosControllerType.ScheduledSwitch:
                    ctrlPresenter.Content = new ucControllerScheduledSwitch();
                    break;


            }
        }
        #endregion
    }
}




//var item = context.CellInfo.Item as WemosController;
//    if (!string.IsNullOrEmpty(item.Name))
//    {
//        var res = await Utils.RequestAsync<bool>("/api/wemos/controllers/setname", item.ID, item.Name);
//        if (res)
//            Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);
//    }
