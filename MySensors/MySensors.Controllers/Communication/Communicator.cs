using Griffin.WebServer;
using Griffin.WebServer.Files;
using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace MySensors.Controllers.Communication
{
    public class Communicator
    {
        private HttpServer webServer;
        private WebSocketServer wsServer;
        private bool isWebServerStarted = false;
        private bool isWSServerStarted = false;
        private NetworkMessageReceiver nmr = new NetworkMessageReceiver();

        public bool IsStarted
        {
            get { return isWSServerStarted && isWebServerStarted; }
        }

        public event NetworkMessageEventProcessor NetworkMessageProcessor;

        public void Start(int httpPort, int wsPort)
        {
            StartWebServer(httpPort);
            StartWSServer(wsPort);
        }
        public void Stop()
        {
            webServer.Stop();
            webServer = null;
            isWebServerStarted = false;
            
            wsServer.Stop();
            wsServer.Dispose();
            wsServer = null;
            isWSServerStarted = false;
        }
        public void Broadcast(NetworkMessage msg)
        {
            if (IsStarted && msg != null)
                foreach (WebSocketSession session in wsServer.GetAllSessions())
                    Send(session, msg);
        }

        private void StartWebServer(int httpPort)
        {
            if (!isWebServerStarted)
            {
                //if (Log != null)
                //    Log(this, "Starting web server... ", false, LogLevel.Normal);

                try
                {
                    var moduleManager = new ModuleManager();

                    //string root = Path.GetPathRoot(Assembly.GetExecutingAssembly().Location);
                    string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\WebSite\";

                    var fileService = new DiskFileService("/", root);
                    var module = new FileModule(fileService) { ListFiles = false };

                    // Add the module
                    moduleManager.Add(new RootModule());
                    moduleManager.Add(module);
                    //moduleManager.Add(new BodyDecodingModule(new UrlFormattedDecoder()));

                    // And start the server.
                    webServer = new HttpServer(moduleManager);
                    webServer.Start(IPAddress.Any, httpPort);

                    isWebServerStarted = true;
                }
                catch (Exception)
                {
                    isWebServerStarted = false;
                }

                //if (Log != null)
                //    Log(this, isWebServerStarted ? "Success." : "Failed.", true, isWebServerStarted ? LogLevel.Success : LogLevel.Error);
            }
        }
        private void StartWSServer(int wsPort)
        {
            if (!isWSServerStarted)
            {
                //if (Log != null)
                //    Log(this, "Starting websocket server... ", false, LogLevel.Normal);

                wsServer = new WebSocketServer();

                if (wsServer.Setup(wsPort))
                {
                    wsServer.NewSessionConnected += new SessionHandler<WebSocketSession>(wsServer_newConnection);
                    wsServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(wsServer_newMessage);

                    wsServer.Start();

                    isWSServerStarted = true;
                }

                //if (Log != null)
                //    Log(this, isWSServerStarted ? "Success." : "Failed.", true, isWSServerStarted ? LogLevel.Success : LogLevel.Error);
            }
        }
        private void Send(WebSocketSession session, NetworkMessage msg)
        {
            if (IsStarted && msg != null)
                session.Send(msg.PackToString());
        }

        private void wsServer_newConnection(WebSocketSession session)
        {
            Console.WriteLine("New web-socket connection from " + session.RemoteEndPoint);
            //if (Log != null)
            //    Log(this, "New web-socket connection from " + session.RemoteEndPoint, true, LogLevel.Normal);
        }
        private void wsServer_newMessage(WebSocketSession session, string txt)
        {
            Console.WriteLine(session.RemoteEndPoint + ": " + txt);

            if (NetworkMessageProcessor != null)
            {
                List<NetworkMessage> msgs = nmr.Process(txt);
                foreach (NetworkMessage msg in msgs)
                {
                    NetworkMessage response = NetworkMessageProcessor(msg);
                    Send(session, response);
                }
            }
        }
    }
}
