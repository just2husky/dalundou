﻿using System;
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

    public static void MsgMove(ClientState c, string msgArgs)
    {
        //解析参数
        string[] split = msgArgs.Split(new string[] { ", " }, StringSplitOptions.None);
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        //赋值
        c.x = x;
        c.y = y;
        c.z = z;
        //广播
        string sendStr = "Move|" + msgArgs;
        foreach (ClientState cs in MainClass.clients.Values)
        {
            MainClass.Send(cs, sendStr);
        }
    }

    public static void MsgAttack(ClientState c, string msgArgs)
    {
        //广播
        string sendStr = "Attack|" + msgArgs;
        foreach (ClientState cs in MainClass.clients.Values)
        {
            MainClass.Send(cs, sendStr);
        }
    }

    public static void MsgHit(ClientState c, string msgArgs)
    {
        //解析参数
        string[] split = msgArgs.Split(new string[] { ", " }, StringSplitOptions.None);
        string attDesc = split[0];
        string hitDesc = split[1];
        //找出被攻击的角色
        ClientState hitCS = null;
        foreach (ClientState cs in MainClass.clients.Values)
        {
            if (cs.socket.RemoteEndPoint.ToString() == hitDesc)
                hitCS = cs;
        }
        Console.WriteLine("MsgHit " + attDesc + ", " + hitDesc + ", hitCS:"+ hitCS);

        if (hitCS == null)
            return;
        //扣血
        hitCS.hp -= 25;
        Console.WriteLine("MsgHit2 " +" hitCS:" + hitCS + "hp:" + hitCS.hp);

        //死亡
        if (hitCS.hp <= 0)
        {
            string sendStr = "Die|" + hitCS.socket.RemoteEndPoint.ToString();
            foreach (ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }
    }
}