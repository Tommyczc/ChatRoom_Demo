
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;
namespace myServer
{
    class Program
    {
        static List<ForClient> clientlist = new List<ForClient>();
        static void Main(string[] args)
        {

            Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            String ip = GetLocalIPv4();
            //String ip = "127.0.0.1";
            int port = 7788;
            if (ip!=null) {
                TcpServer.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                Console.WriteLine("服务器开启，本机IP： " + ip + "  端口号：" + port + "\n" + "请告知客户端。" + "\n");
                TcpServer.Listen(100);
            }
            
            

            while (true)
            {
                Socket clientSocket = TcpServer.Accept();
                Console.WriteLine("新接入一个客户端，IP地址："+ clientSocket.RemoteEndPoint);
                ForClient cc = new ForClient(clientSocket);//每个客户端通信逻辑放到client
                clientlist.Add(cc);
            }

        }

        public static string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName(); //得到主机名
            IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < iPEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    return iPEntry.AddressList[i].ToString();
            }
            return null;
        }

        public static bool nameIsExit(String userName) {
            bool exit = false;
            foreach (var clients in clientlist)
            {
                if (clients.nameIsMacth(userName))
                    exit = true;

            }
            return exit;
        }

        //广播信息，同时清除断连客户
        public static void BroadcastMessage(byte[] pck)
        {
            var notConnectedList = new List<ForClient>();

            foreach (var clients in clientlist)
            {
                if (clients.Connected)
                    clients.SendMessage(pck);
                else
                {
                    notConnectedList.Add(clients);
                }
            }
            foreach (var temp in notConnectedList)
            {
                clientlist.Remove(temp);
            }
        }
    }
}

