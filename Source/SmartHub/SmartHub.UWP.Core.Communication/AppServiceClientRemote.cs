using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;

namespace SmartHub.UWP.Core.Communication
{
    public static class AppServiceClientRemote
    {
        // single request/response
        public static async Task<ValueSet> SendOnceAsync(RemoteSystem selectedDevice, string appServiceName, string packageFamilyName, ValueSet request)
        {
            if (selectedDevice != null)
            {
                using (AppServiceConnection connection = new AppServiceConnection { AppServiceName = appServiceName, PackageFamilyName = packageFamilyName })
                {
                    var status = await connection.OpenRemoteAsync(new RemoteSystemConnectionRequest(selectedDevice));
                    if (status == AppServiceConnectionStatus.Success)
                    {
                        var response = await connection.SendMessageAsync(request);
                        if (response.Status == AppServiceResponseStatus.Success)
                            return response.Message;
                    }
                }
            }

            return null;
        }
    }
}
