using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using SmartHub.Core.Plugins;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartHub.Plugins.SignalR
{
    [Plugin]
    public class SignalRPlugin : PluginBase
    {
        #region Fields
        // This will *ONLY* bind to localhost, if you want to bind to all addresses use http://*:8080 to bind to all addresses. 
        // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx for more information.
        private const string url = "http://localhost:55556";

        private IDisposable server;
        //private MyHub wsHub;
        #endregion

        #region Import
        [ImportMany("ADD7FADD-D706-4D5A-9103-09B26FD2FD9C")]
        public Action<string, string>[] OnClientSignal { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            //var handlers = RegisterHandlers();
        }
        public override void StartPlugin()
        {
            server = WebApp.Start(url, ConfigureModules);
        }
        public override void StopPlugin()
        {
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }
        #endregion

        #region Public methods
        public void Broadcast(object data)
        {
            if (data != null)
            {
                var context = GlobalHost.ConnectionManager.GetConnectionContext<ChatConnection>();
                if (context != null && context.Connection != null)
                    context.Connection.Broadcast(data);
            }
        }
        #endregion

        #region Private methods
        //private InternalDictionary<ListenerHandlerBase> RegisterHandlers()
        //{
        //    var handlers = new InternalDictionary<ListenerHandlerBase>();

        //    // регистрируем обработчики для методов плагинов
        //    foreach (var action in HttpCommandHandlers)
        //    {
        //        Logger.Info("Register WebApi command handler '{0}'", action.Metadata.Url);

        //        var handler = new ApiListenerHandler(action.Value);
        //        handlers.Register(action.Metadata.Url, handler);
        //    }

        //    return handlers;
        //}
        private void ConfigureModules(IAppBuilder appBuilder)
        {
            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(5); // 10 = default
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(60);
            //GlobalHost.Configuration.DefaultMessageBufferSize = 1;

            appBuilder
                .UseCors(CorsOptions.AllowAll)
                //.Use<HttpListenerModule>(registeredHandlers, Logger)
                //.UseErrorPage()

                //.MapSignalR(); // for hubs
                .MapSignalR<ChatConnection>("/chat"); // for persistent connection
        }
        #endregion

        class ChatConnection : PersistentConnection
        {
            protected override Task OnConnected(IRequest request, string connectionId)
            {
                //Data chatData = new Data() { Name = "Сообщение сервера", Message = "Пользователь " + connectionId + " присоединился к чату" };
                //return Connection.Broadcast(chatData);

                return null;
            }

            protected override Task OnReceived(IRequest request, string connectionId, string data)
            {
                //Data chatData = JsonConvert.DeserializeObject<Data>(data);
                //return Connection.Broadcast(chatData);

                //Run(OnClientSignal, x => x(connectionId, data));

                return null;
            }

            protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
            {
                //Data chatData = new Data() { Name = "Сообщение сервера", Message = "Пользователь " + connectionId + " покинул чат" };
                //return Connection.Broadcast(chatData);

                return null;
            }
        }
    }

    #region Hubs
    //public class MyHub : Hub
    //{
    //    public static MyHub Instance;
    //    public void onClientMessage(string name, string message)
    //    {
    //        Clients.All.onServerMessage("server 1", "success");
    //        //Run(OnClientSignal, x => x(name, message));
    //    }
    //}
    #endregion


    //public class Data
    //{
    //    public string Name { get; set; } // Имя пользователя
    //    public string Message { get; set; } // Сообщение пользователя
    //}
}
