using GHI.Premium.SQLite;
using MFE.Net.Http;
using MFE.Net.WebSocket;
using Microsoft.SPOT;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace SmartNetwork.HttpFileServerTest
{
    public class Program
    {
        private static string root = @"\WINFS";// \\NAND
        //private static string root = @"\SD";
        private static HttpServer httpServer;

        public static void Main()
        {
            httpServer = new HttpServer();
            httpServer.OnGetRequest += new GETRequestEventHandler(httpServer_OnGetRequest);
            //httpServer.Start("http", -1);
            httpServer.Start("http", 81);

            WSServer wss = new WSServer(12000);//, "http://localhost:81", "ws://localhost:2013");
            wss.Start();

            //NameService ns = new NameService(); // Declare as global, name service will stop once the object gets disposed
            //ns.AddName("Typhoon", NameService.NameType.Unique, NameService.MsSuffix.Default); // register your fez as FEZCOBRA on the local network

            //DbTest();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void httpServer_OnGetRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            if (path.ToLower() == "\\admin") // There is one particular URL that we process differently
            {
                //httpServer.ProcessPasswordProtectedArea(request, response);
            }
            else if (path.ToLower().IndexOf("json") != -1)
                ProcessJSONRequest(path, parameters, response);
            else
                httpServer.SendFile(root + path, response);
        }

        private static void ProcessJSONRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            path = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            if (path.ToLower() == @"\content\decoders")
                DecodersRead(root + path, response);
            else if (path.ToLower() == @"\content\consists")
                LayoutRead(root + path, response);
            
            else if (path.ToLower() == @"\content\layout")
                LayoutRead(root + path, response);
            else if (path.ToLower() == @"\content\layout\create")
                LayoutCreate(root + path, response);


        }
        private static void DecodersRead(string path, HttpListenerResponse response)
        {
            string[] files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                response.ContentType = "application/json";

                byte[] b = Encoding.UTF8.GetBytes("[");
                response.OutputStream.Write(b, 0, b.Length);

                foreach (string fileName in files)
                {
                    int bufferSize = 1024 * 1024; // 4
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long fileLength = fs.Length;
                        //response.ContentLength64 = fileLength;
                        //Debug.Print("File: " + strFilePath + "; length = " + fileLength);

                        //byte[] buf = new byte[bufferSize];
                        for (long bytesSent = 0; bytesSent < fileLength; )
                        {
                            long bytesToRead = fileLength - bytesSent;
                            bytesToRead = bytesToRead < bufferSize ? bytesToRead : bufferSize;

                            byte[] buf = new byte[bytesToRead];

                            int bytesRead = fs.Read(buf, 0, (int)bytesToRead);
                            response.OutputStream.Write(buf, 0, bytesRead);
                            bytesSent += bytesRead;

                            //Thread.Sleep(1);
                        }

                        fs.Close();
                    }
                }

                b = Encoding.UTF8.GetBytes("]");
                response.OutputStream.Write(b, 0, b.Length);
            }
        }
        private static void LayoutRead(string path, HttpListenerResponse response)
        {
            response.ContentType = "application/json";

            int bufferSize = 1024 * 1024; // 4
            using (FileStream fs = new FileStream(path + ".json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long fileLength = fs.Length;
                response.ContentLength64 = fileLength;
                //Debug.Print("File: " + strFilePath + "; length = " + fileLength);

                //byte[] buf = new byte[bufferSize];
                for (long bytesSent = 0; bytesSent < fileLength; )
                {
                    long bytesToRead = fileLength - bytesSent;
                    bytesToRead = bytesToRead < bufferSize ? bytesToRead : bufferSize;

                    byte[] buf = new byte[bytesToRead];

                    int bytesRead = fs.Read(buf, 0, (int)bytesToRead);
                    response.OutputStream.Write(buf, 0, bytesRead);
                    bytesSent += bytesRead;

                    //Thread.Sleep(1);
                }

                fs.Close();
            }
        }
        private static void LayoutCreate(string path, HttpListenerResponse response)
        {

        }




        private static void DbTest()
        {
            try
            {
                // Mount NAND Flash File Media
                //PersistentStorage ps = new PersistentStorage("SD");
                //ps.MountFileSystem();

                //// Format the media if it was not.
                //VolumeInfo nand = new VolumeInfo("SD");
                //if (!nand.IsFormatted)
                //{
                //    nand.Format("FAT", 0);
                //}
                // Create new database file
                Database myDatabase = new Database();
                // Open a new Database in NAND Flash
                myDatabase.Open(root + "\\myDatabase.dbs");
                //add a table
                myDatabase.ExecuteNonQuery(
                  "CREATE Table Temperature" +
                  "(Room TEXT, Time INTEGER, Value DOUBLE)");

                //add rows to table
                myDatabase.ExecuteNonQuery(
                  "INSERT INTO Temperature (Room, Time,Value) " +
                  "VALUES ('Kitchen',010000,4423)");

                myDatabase.ExecuteNonQuery(
                 "INSERT INTO Temperature (Room, Time,Value) " +
                 "VALUES ('living room',053000,9300)");

                myDatabase.ExecuteNonQuery(
                 "INSERT INTO Temperature (Room, Time,Value) " +
                 "VALUES ('bed room',060701,7200)");

                // Process SQL query and save returned records in SQLiteDataTable
                SQLiteDataTable table = myDatabase.ExecuteQuery("SELECT * FROM Temperature");

                // Get a copy of columns orign names example
                String[] origin_names = table.ColumnOriginNames;

                // Get a copy of table data example
                ArrayList[] tabledata = table.ColumnData;


                String temp = "Fields: ";
                for (int i = 0; i < table.Columns; i++)
                {
                    temp += table.ColumnOriginNames[i] + " |";
                }
                Debug.Print(temp);
                object obj;
                for (int j = 0; j < table.Rows; j++)
                {
                    temp = j.ToString() + " ";
                    for (int i = 0; i < table.Columns; i++)
                    {
                        obj = table.ReadRecord(i, j);
                        if (obj == null)
                            temp += "N/A";
                        else
                            temp += obj.ToString();
                        temp += " |";
                    }
                    Debug.Print(temp);

                }
                myDatabase.Close();
                //ps.UnmountFileSystem();
                //ps.Dispose();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(Database.GetLastError());
            }
        }
    }
}
