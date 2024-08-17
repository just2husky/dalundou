using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CtrlHuman : BaseHuman
{
    void OnDrawGizmosSelected()
    {

        //攻击判定
        Vector3 lineEnd = transform.position + 2f * Vector3.up;
        Vector3 lineStart = lineEnd + 5 * transform.forward;

        // Draws a blue line from this transform to the target
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(lineStart, lineEnd);
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                //发送协议
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ", ";
                sendStr += hit.point.x + ", ";
                sendStr += hit.point.y + ", ";
                sendStr += hit.point.z + ", ";
                NetManager.Send(sendStr);
            }
        }

        //攻击
        if (Input.GetMouseButtonDown(1))
        {
            if (isAttacking) return;
/*            if (isMoving) return;*/

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            transform.LookAt(hit.point);
            Attack();

            //发送协议
            string sendStr = "Attack|";
            sendStr += NetManager.GetDesc() + ", ";
            sendStr += transform.eulerAngles.y + ", ";
            NetManager.Send(sendStr);

            //攻击判定
            Vector3 lineEnd = transform.position + 0.5f*Vector3.up;
            Vector3 lineStart = lineEnd + 5*transform.forward;
            Debug.DrawLine(lineStart, lineEnd, Color.red, 5f);
/*            Debug.DrawLine(new Vector3(20, 1, 75), new Vector3(28, 1, 76), Color.red, 5f);
*/            Debug.DrawRay(transform.position, transform.forward * 3, Color.green, 10f);
            Debug.Log("Attack>>> " + lineEnd + ", " + lineStart + ", " + transform.position);
            if (Physics.Linecast(lineEnd, lineStart, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                Debug.Log("Attack>>> hit " + hitObj);

                if (hitObj == gameObject)
                    return;
                SyncHuman h = hitObj.GetComponent<SyncHuman>();
                Debug.Log("Attack>>> hit SyncHuman" + h);
                if (h == null)
                    return;
                sendStr = "Hit|";
                sendStr += NetManager.GetDesc() + ", ";
                sendStr += h.desc + ", ";
                NetManager.Send(sendStr);
                Debug.Log("Attack>>> hit send hit msg" + sendStr);
            }
        }
    }



}