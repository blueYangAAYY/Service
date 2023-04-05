using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Service
{
    class MainClass
    {
        //监听Socket
        static Socket listenfd;

        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket,ClientState>();

        static void Main(string[] args)
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);

            listenfd.Bind(ipEp);
            listenfd.Listen(0);

            Console.WriteLine("服务器启动成功");

            List<Socket> checkRead = new List<Socket>();

            //主循环
            while (true)
	        {
                checkRead.Clear();
                checkRead.Add(listenfd);

                foreach (ClientState clientState in clients.Values)
	            {
                    checkRead.Add(clientState.socket);
	            }

                Socket.Select(checkRead,null,null,1000);

                //检查可读对象
                foreach (var item in checkRead)
	            {
                    if (item == listenfd)
	                {
                        ReadListenfd(item);
	                }
                    else
	                {
                        ReadClientfd(item);
	                }
	            }
	        }

        }

        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine ("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState(); 
            state.socket = clientfd; 
            clients.Add(clientfd, state);
        }

        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            int count = 0;

            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch(SocketException ex)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close(); 
                clients.Remove(clientfd); 
                Console.WriteLine("Receive SocketException " + ex.ToString()); 
                return false; 
            } 
                        
             //客户端关闭
            if (count == 0)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close(); 
                clients.Remove(clientfd); 
                Console.WriteLine("Socket Close");
                return false; 
            }

            //消息处理
            string recvStr = Encoding.Default.GetString(state.readBuff, 0, count);

            string[] msg = recvStr.Split('|');

            string msgName = msg[0];
            string msgArgs = msg[1];

            string funName = "Msg" + msgName;

            //利用反射，根据函数名 直接调用 MsgHandler 中的 函数
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = { state, msgArgs };

            //第一个参数 null代表this指针， 由千消息处理方法都是静态方法， 因此此处要埴null
            mi.Invoke(null, o);
            return true;
        }

        public static void Send(ClientState clientState,string str)
        {
            clientState.socket.Send(Encoding.Default.GetBytes(str));
        }
    }
}
