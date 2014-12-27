﻿using Microsoft.AspNet.SignalR;
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
        private IDisposable wsServer;
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
            wsServer = WebApp.Start<WSStartup>(url);
        }
        public override void StopPlugin()
        {
            if (wsServer != null)
            {
                wsServer.Dispose();
                wsServer = null;
            }
        }
        #endregion

        #region Public methods
        public void Broadcast(object data)
        {
            if (ChatConnection.CurrentChatConnection != null &&
                ChatConnection.CurrentChatConnection.Connection != null &&
                data != null)
                ChatConnection.CurrentChatConnection.Connection.Broadcast(data);
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

        //    // регистрируем обработчики для ресурсов
        //    foreach (var plugin in Context.GetAllPlugins())
        //    {
        //        Type type = plugin.GetType();
        //        var attributes = type.GetCustomAttributes<HttpResourceAttribute>();

        //        foreach (var attribute in attributes)
        //        {
        //            Logger.Info("Register HTTP resource handler: '{0}'", attribute.Url);

        //            var resHandler = new ResourceListenerHandler(type.Assembly, attribute.ResourcePath, attribute.ContentType);
        //            handlers.Register(attribute.Url, resHandler);
        //        }
        //    }

        //    return handlers;
        //}
        #endregion

        class WSStartup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseCors(CorsOptions.AllowAll);

                //app.MapSignalR(); // for hubs
                app.MapSignalR<ChatConnection>("/chat"); // for persistent connection
            }
        }

        class ChatConnection : PersistentConnection
        {
            public static ChatConnection CurrentChatConnection;

            public ChatConnection()
            {
                CurrentChatConnection = this;
            }

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

                return null;
            }

            //protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
            //{
            //    Data chatData = new Data() { Name = "Сообщение сервера", Message = "Пользователь " + connectionId + " покинул чат" };
            //    return Connection.Broadcast(chatData);
            //}
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
