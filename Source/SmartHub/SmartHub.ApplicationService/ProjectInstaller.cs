using System.ComponentModel;
using System.Configuration.Install;

namespace SmartHub.ApplicationService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
