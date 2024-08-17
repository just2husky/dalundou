using System;
using System.Collections.Generic;

class MsgHandler
{
    public static void MsgEnter(ClientState c, string msgArgs)
    {
        //解析参数
        Console.WriteLine("MsgEnter " + c + ", " + msgArgs) ;
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1].Trim());
        float y = float.Parse(split[2].Trim());
        float z = float.Parse(split[3].Trim());
/*        float eulY = float.Parse(split[4]);
*/        float eulY = 0;
        //赋值
        c.hp = 100;
        c.x = x;
        c.y = y;
        c.z = z;
        c.eulY = eulY;
        //广播
        string sendStr = "Enter|" + msgArgs;
        foreach (ClientState cs in MainClass.clients.Values)
        {
            MainClass.Send(cs, sendStr);
        }
    }

    public static void MsgList(ClientState c, string msgArgs)
    {
        Console.WriteLine("MsgList " + c + ", " + msgArgs);
        string sendStr = "List|";
        foreach (ClientState cs in MainClass.clients.Values)
        {
            sendStr += cs.socket.RemoteEndPoint.ToString() + ", ";
            sendStr += cs.x.ToString() + ", ";
            sendStr += cs.y.ToString() + ", ";
            sendStr += cs.z.ToString() + ", ";
            sendStr += cs.eulY.ToString() + ", ";
            sendStr += cs.hp.ToString() + ", ";
        }
        MainClass.Send(c, sendStr);
    }
}