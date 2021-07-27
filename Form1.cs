using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleHTTPServer
{
    public partial class Form1 : Form
    {
        private Socket httpServer;
        private int serverPort = 80;
        private Thread thread;
        public Form1()
        {
            InitializeComponent();
        }

        private void startServerBTN_Click(object sender, EventArgs e)
        {
            serverLogsTextBox.Text = "";

            try
            {
                try
                {
                    httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    serverPort = int.Parse(serverPortTextBox.Text.ToString());

                    if (serverPort > 65535 || serverPort <= 0)
                    {
                        throw new Exception("Server Port not within the range");
                    }
                }
                catch (Exception ex)
                {
                    serverPort = 80;
                    serverLogsTextBox.Text = "Server Failed to Start on Specified Port \n";

                }
                thread = new Thread(new ThreadStart(this.connectionThreadMethod));
                thread.Start();

                startServerBTN.Enabled = false;
                stopServerBTN.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while starting the server");
                serverLogsTextBox.Text = "Server Starting Failed \n";
            }



            serverLogsTextBox.Text = "Server Started";
        }

        private void stopServerBTN_Click(object sender, EventArgs e)
        {
            try
            {
                httpServer.Close();

                thread.Abort();

                startServerBTN.Enabled = true;
                stopServerBTN.Enabled = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Stopping Failed");

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stopServerBTN.Enabled = false;
        }

        private void connectionThreadMethod()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                httpServer.Bind(endPoint);
                httpServer.Listen(1);
                startListeningForConnection();

            }
            catch (Exception ex)
            {
                Console.WriteLine("I could nt start");
            }
        }

        private void startListeningForConnection()
        {
            while (true)
            {
                DateTime time = DateTime.Now;

                string data = "";
                byte[] bytes = new byte[2048];

                Socket client = httpServer.Accept();

                while (true)
                {
                    int numBytes = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                    if (data.IndexOf("\r\n") > -1)
                        break;
                }

                serverLogsTextBox.Invoke((MethodInvoker)delegate
                {
                    serverLogsTextBox.Text += "\r\n\r\n";
                    serverLogsTextBox.Text += data;
                    serverLogsTextBox.Text += "\n\n------- End of Request -----------";
                });

                string resHeader = "HTTP/1.1 200 Everything is Fine\nServer: my_csharp_server\nContent-type: text/html; charset: UTF-8\n\n";
                string resBody = "<!DOCTYPE html>" +
                    "<html>" +
                    "<head><title>My Server</title></head>" +
                    "<body>" +
                    "<h4>Server Time is: " + time.ToString() + "</h4>" +
                    "</body></html>";

                string resStr = resHeader + resBody;

                byte[] resData = Encoding.ASCII.GetBytes(resStr);

                client.SendTo(resData, client.RemoteEndPoint);

                client.Close();

            }
        }
    }
}
