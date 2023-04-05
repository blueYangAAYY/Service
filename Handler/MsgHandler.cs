using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{

    /// <summary>
    /// 消息处理模块
    /// </summary>
    static class MsgHandler
    {
        public static void MsgEnter(ClientState c, string msgArgs)
        {

            Console.WriteLine(string.Format("接收到 客户端【{0}】的消息，协议为：Enter,消息为：{1}", msgArgs[0], msgArgs));
            //解析消息参数
            string[] split = msgArgs.Split(',');

            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);

            //赋值
            c.hp = 100;
            c.x = x;
            c.y = y;
            c.z = z;
            c.euly = eulY;

            //广播
            string sendStr = "Enter|" + msgArgs;

            foreach (ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }

        public static void MsgList(ClientState c, string msgArgs)
        {
            Console.WriteLine(string.Format("接收到消息 协议为：List,消息为：{0}", msgArgs));

            string sendStr = "List|";

            //组装所有客户端信息
            foreach (ClientState cs in MainClass.clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.euly.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";

            }

            MainClass.Send(c, sendStr);
        }
    }
}
