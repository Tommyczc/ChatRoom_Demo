
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace myServer
{

    class ForClient
    {

        private byte[] data = new byte[1024];//这个是一个数据容器
        private Socket socketclient;
        private Thread tt;

        public ForClient(Socket s)
        {
            socketclient = s;
            //启动一个线程，处理客户端数据接收

            tt = new Thread(ReceiveMessage);
            tt.Start();
        }

        void ReceiveMessage()
        {
            //一直接收客户端的数据
            while (true)
            {
                //在接收数据之前  判断一下socket连接是否断开
                if (socketclient.Poll(10, SelectMode.SelectRead))
                {
                    socketclient.Close();
                    break;//跳出循环 终止线程的执行
                }
                else
                {

                    try
                    {
                        int length = socketclient.Receive(data);
                        string message = Encoding.UTF8.GetString(data, 0, length);

                        //接收到数据的时候 要把这个数据 分发到客户端
                        //广播这个消息
                        Program.BroadcastMessage(socketclient.RemoteEndPoint +":  "+ message);
                        Console.WriteLine("收到了客户端：" + socketclient.RemoteEndPoint + "  "+"消息: " + message);
                    }

                    catch
                    {
                        Console.WriteLine("出错了");
                    }
                }
            }
        }

        public void SendMessage(string message)
        {

            byte[] data = Encoding.UTF8.GetBytes(message);
            socketclient.Send(data);
        }

        public bool Connected
        {
            get { return socketclient.Connected; }
        }
    }
}
