using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;

namespace SmartHub.UWP.Core.Communication.AppService
{
    public class AppServiceClient
    {
        public static async Task<ValueSet> SendReceiveAsync(string appServiceName, string packageFamilyName, ValueSet request, RemoteSystem remoteDevice = null)
        {
            using (var connection = new AppServiceConnection() { AppServiceName = appServiceName, PackageFamilyName = packageFamilyName })
            {
                AppServiceConnectionStatus status;

                if (remoteDevice != null)
                    status = await connection.OpenRemoteAsync(new RemoteSystemConnectionRequest(remoteDevice));
                else
                    status = await connection.OpenAsync();

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
