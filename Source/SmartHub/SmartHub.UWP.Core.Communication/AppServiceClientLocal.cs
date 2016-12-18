using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace SmartHub.UWP.Core.Communication
{
    public class AppServiceClientLocal
    {
        private AppServiceConnection connection;
        private string appServiceName;
        private string packageFamilyName;

        public AppServiceClientLocal(string appServiceName, string packageFamilyName)
        {
            this.appServiceName = appServiceName;
            this.packageFamilyName = packageFamilyName;
        }

        public async Task<bool> Connect()
        {
            if (connection == null)
            {
                connection = new AppServiceConnection();
                connection.AppServiceName = appServiceName;
                connection.PackageFamilyName = packageFamilyName;
                connection.ServiceClosed += (s, e) => Disconnect();

                return await connection.OpenAsync() == AppServiceConnectionStatus.Success;
            }

            return false;
        }
        public void Disconnect()
        {
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
        public async Task<ValueSet> Send(ValueSet request)
        {
            if (connection == null)
            {
                var response = await connection.SendMessageAsync(request);
                if (response.Status == AppServiceResponseStatus.Success)
                    return response.Message;
            }
            return null;
        }


        // single request/response
        public static async Task<ValueSet> SendOnceAsync(string appServiceName, string packageFamilyName, ValueSet request)
        {
            using (var connection = new AppServiceConnection() { AppServiceName = appServiceName, PackageFamilyName = packageFamilyName })
            {
                var status = await connection.OpenAsync();
                if (status == AppServiceConnectionStatus.Success)
                {
                    var response = await connection.SendMessageAsync(request);
                    if (response.Status == AppServiceResponseStatus.Success)
                        return response.Message;
                }
            }

            return null;
        }
    }
}
