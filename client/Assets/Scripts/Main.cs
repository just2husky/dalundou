using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Main : MonoBehaviour
{

    //人物模型预设
    public GameObject humanPrefab;
    //人物列表
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans;

    protected void Update()
    {
        NetManager.Update();
    }

    void Start()
    {

        otherHumans = new Dictionary<string, BaseHuman>();

        //网络模块
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8888);
        //添加一个角色
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(24 + x, 0, 80 + z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        //发送协议
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ", ";
        sendStr += pos.x + ", ";
        sendStr += pos.y + ", ";
        sendStr += pos.z + ", ";
        sendStr += eul.y;
        NetManager.Send(sendStr);

        Debug.Log("Main Start" + sendStr);
    }

    void OnEnter(string msgArgs)
    {
        Debug.Log("Main OnEnter" + msgArgs);
        NetManager.Send("List|");
        //解析参数
        string[] split = msgArgs.Split(", ");
        if (split.Length <= 1) {
            return;
        }
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        //是自己
        if (desc == NetManager.GetDesc() || otherHumans.ContainsKey(desc))
            return;
        //添加一个角色
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        Debug.Log("create other human " + obj.transform.position);

        obj.transform.eulerAngles = new Vector3(0, eulY, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
  
    }


    void OnList(string msgArgs)
    {
        Debug.Log("OnList" + msgArgs);

        if (humanPrefab == null)
        {
            Debug.LogError("humanPrefab is null!");
            return;
        }

        if (NetManager.GetDesc() == null)
        {
            Debug.LogError("NetManager.GetDesc() returned null!");
            return;
        }

        if (otherHumans == null)
        {
            Debug.LogError("otherHumans is not initialized!");
            return;
        }

        //解析参数
        string[] split = msgArgs.Split(", ");
        int count = (split.Length - 1) / 6;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            //是自己
            if (desc == NetManager.GetDesc() || otherHumans.ContainsKey(desc))
                continue;
            //添加一个角色
            GameObject obj = (GameObject)Instantiate(humanPrefab);
            if (obj == null)
            {
                Debug.LogError("Instantiate failed to create an object!");
                continue;
            }

            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    void OnMove(string msgArgs)
    {
        Debug.Log("OnMove" + msgArgs);
    }

    void OnLeave(string msgArgs)
    {
        Debug.Log("OnLeave" + msgArgs);
    }
}