using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Service
{
    public class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }

    class MainClass
    {
        //监听Socket
        static Socket listenfd;

        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket,ClientState>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);

            listenfd.Bind(ipEp);
            listenfd.Listen(0);

            Console.WriteLine("服务器启动成功");

            listenfd.BeginAccept(AcceptCallback, listenfd);

            //等待
            Console.ReadLine();

            //while(true)
            //{
            //    Socket connfd = socket.Accept();
            //    Console.WriteLine("服务器Accept");

            //    byte[] readBuff = new byte[1024];
            //    int count = connfd.Receive(readBuff);

            //    string readStr = Encoding.Default.GetString(readBuff, 0, count);
            //    Console.WriteLine("服务器接收:"+ readStr);

            //    byte[] sendBytes = System.Text.Encoding.Default.GetBytes(readStr);
            //    connfd.Send(sendBytes);
            //}
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listenfd = (Socket)ar.AsyncState;
                Socket clientfd = listenfd.EndAccept(ar);

                ClientState state = new ClientState();
                state.socket = clientfd;

                clients.Add(clientfd, state);

                //接收数据
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

                //继续响应客户端
                listenfd.BeginAccept(AcceptCallback, listenfd);


            }
            catch(SocketException ex)
            {
                Console.WriteLine("Socket Accept fail" + ex.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                int count = clientfd.EndReceive(ar);

                //客户端关闭
                if (count == 0)
                {
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    return;
                }

                string recvStr = Encoding.Default.GetString(state.readBuff,0,count);
                byte[] sendBytes = Encoding.Default.GetBytes("echo" + recvStr);

                clientfd.Send(sendBytes);

                //接收数据
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

            }
            catch(SocketException ex)
            {
                Console.WriteLine("Socket  Receive fail" + ex.ToString());
            }
        }
    }
}
