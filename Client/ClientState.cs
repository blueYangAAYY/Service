using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ClientState
    {
        //客户端socket
        public Socket socket;

        //缓存
        public byte[] readBuff = new byte[1024];

        //hp
        public int hp = -100;

        //角色位置
        public float x = 0;
        public float y = 0;
        public float z = 0;

        //角色旋转
        public float euly = 0;
    }

}
