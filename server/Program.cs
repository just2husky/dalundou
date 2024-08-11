using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

class ClientState
{
    public Socket socket;
    public byte[] readBuff = new byte[1024];
}

/*class MainClass
{
    //监听Socket
    static Socket listenfd;
    //客户端Socket及状态信息
    static Dictionary<Socket, ClientState> clients =
        new Dictionary<Socket, ClientState>();

    //Accept回调
    public static void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            Console.WriteLine("[服务器]Accept");
            Socket listenfd = (Socket)ar.AsyncState;
            Socket clientfd = listenfd.EndAccept(ar);
            //clients列表
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
            //接收数据BeginReceive
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0,
                ReceiveCallback, state);
            //继续Accept
            listenfd.BeginAccept(AcceptCallback, listenfd);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Accept fail" + ex.ToString());
        }
    }

    //Receive回调
    public static void ReceiveCallback(IAsyncResult ar)
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

            string recvStr =
                System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            byte[] sendBytes =
                System.Text.Encoding.Default.GetBytes("echo" + recvStr);
            foreach (ClientState s in clients.Values)
            {
                s.socket.Send(sendBytes);
            }
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0,
                ReceiveCallback, state);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Receive fail" + ex.ToString());
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World! ");
        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
        listenfd.Bind(ipEp);
        //Listen
        listenfd.Listen(0);
        Console.WriteLine("[服务器]启动成功");
        //Accept
        listenfd.BeginAccept(AcceptCallback, listenfd);
        //等待
        Console.ReadLine();
    }
}*/

class MainClass
{
    //监听Socket
    static Socket listenfd;
    //客户端Socket及状态信息
    static Dictionary<Socket, ClientState> clients =
        new Dictionary<Socket, ClientState>();


    //读取Listenfd
    public static void ReadListenfd(Socket listenfd)
    {
        Console.WriteLine("Accept");
        Socket clientfd = listenfd.Accept();
        ClientState state = new ClientState();
        state.socket = clientfd;
        clients.Add(clientfd, state);
    }

    //读取Clientfd
    public static bool ReadClientfd(Socket clientfd)
    {
        ClientState state = clients[clientfd];
        //接收
        int count = 0;
        try
        {
            count = clientfd.Receive(state.readBuff);
        }
        catch (SocketException ex)
        {
            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Receive SocketException " + ex.ToString());
            return false;
        }
        //客户端关闭
        if (count == 0)
        {
            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Socket Close");
            return false;
        }
        //广播
        string recvStr =
            System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
        Console.WriteLine("Receive" + recvStr);
        string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        foreach (ClientState cs in clients.Values)
        {
            cs.socket.Send(sendBytes);
        }
        return true;
    }

    public static void Main(string[] args)
    {
        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
        listenfd.Bind(ipEp);
        //Listen
        listenfd.Listen(0);
        Console.WriteLine("[服务器]启动成功");
        //checkRead
        List<Socket> checkRead = new List<Socket>();
        //主循环
        while (true)
        {
            //填充checkRead列表
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach (ClientState s in clients.Values)
            {
                checkRead.Add(s.socket);
            }
            //select
            Socket.Select(checkRead, null, null, 1000);
            //检查可读对象
            foreach (Socket s in checkRead)
            {
                if (s == listenfd)
                {
                    ReadListenfd(s);
                }
                else
                {
                    ReadClientfd(s);
                }
            }
        }
    }
}