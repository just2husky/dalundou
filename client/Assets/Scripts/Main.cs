using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Main : MonoBehaviour
{

    //����ģ��Ԥ��
    public GameObject humanPrefab;
    //�����б�
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans;

    protected void Update()
    {
        NetManager.Update();
    }

    void Start()
    {

        otherHumans = new Dictionary<string, BaseHuman>();

        //����ģ��
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("Attack", OnAttack);
        NetManager.AddListener("Die", OnDie);
        NetManager.Connect("127.0.0.1", 8888);
        //���һ����ɫ
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(24 + x, 0, 80 + z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        //����Э��
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
        //��������
        string[] split = msgArgs.Split(", ");
        if (split.Length <= 1) {
            return;
        }
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        //���Լ�
        if (desc == NetManager.GetDesc() || otherHumans.ContainsKey(desc))
            return;
        //���һ����ɫ
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

        //��������
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
            //���Լ�
            if (desc == NetManager.GetDesc() || otherHumans.ContainsKey(desc))
                continue;
            //���һ����ɫ
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
        //��������
        string[] split = msgArgs.Split(", ");
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        //�ƶ�
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        h.MoveTo(targetPos);
    }
    void OnLeave(string msgArgs)
    {
        Debug.Log("OnLeave" + msgArgs);
        //��������
        string[] split = msgArgs.Split(", ");
        string desc = split[0];
        //ɾ��
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);
        otherHumans.Remove(desc);
    }

    void OnAttack(string msgArgs)
    {

        Debug.Log("OnAttack" + msgArgs);
        //��������
        string[] split = msgArgs.Split(", ");
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        //��������
        if (!otherHumans.ContainsKey(desc))
            return;
        SyncHuman h = (SyncHuman)otherHumans[desc];
        h.SyncAttack(eulY);
    }

    void OnDie(string msgArgs)
    {
        Debug.Log("OnDie" + msgArgs);
        //��������
        string[] split = msgArgs.Split(", ");
        string attDesc = split[0];
        string hitDesc = split[0];
        //�Լ�����
        if (hitDesc == myHuman.desc)
        {
            Debug.Log("Game Over");
            return;
        }
        //����
        if (!otherHumans.ContainsKey(hitDesc))
            return;
        SyncHuman h = (SyncHuman)otherHumans[hitDesc];
        h.gameObject.SetActive(false);
    }
}