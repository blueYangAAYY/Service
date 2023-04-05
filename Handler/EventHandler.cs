using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>
    /// 事件处理模块
    /// </summary>
    class EventHandler
    {
        /// <summary>
        /// 客户端下线
        /// </summary>
        /// <param name="c"></param>
        public static void OnDisconnect(ClientState c)
        {
            Console.WriteLine("OnDisconnect");
        }
    }
}
