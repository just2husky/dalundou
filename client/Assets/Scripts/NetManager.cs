using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;

public static class NetManager
{
    //�����׽���
    static Socket socket;
    //���ջ�����
    static byte[] readBuff = new byte[1024];
    //ί������
    public delegate void MsgListener(String str);
    //�����б�
    private static Dictionary<string, MsgListener> listeners =
        new Dictionary<string, MsgListener>();
    //��Ϣ�б�
    static List<String> msgList = new List<string>();

    //��Ӽ���
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //��ȡ����
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    //����
    public static void Connect(string ip, int port)
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //Connect����ͬ����ʽ�򻯴��룩
        socket.Connect(ip, port);
        //BeginReceive
        socket.BeginReceive(readBuff, 0, 1024, 0,
            ReceiveCallback, socket);
    }
    //Receive�ص�
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr =
                System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            Debug.Log("ReceiveCallback" + recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0,
                ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //����
    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;

        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //Update
    public static void Update()
    {
        if (msgList.Count <= 0)
            return;
        String msgStr = msgList[0];
        Debug.Log("Update process msgList" + msgStr);
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        Debug.Log("Update process msgList exec  listen callback111: " + msgName + ", " + msgArgs);
        //�����ص���
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
            Debug.Log("Update process msgList exec  listen callback" + msgStr);
        }
    }
}