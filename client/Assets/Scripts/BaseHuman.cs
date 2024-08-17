using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseHuman : MonoBehaviour
{
    //�Ƿ������ƶ�
    protected bool isMoving = false;
    //�ƶ�Ŀ���
    private Vector3 targetPosition;
    //�ƶ��ٶ�
    public float speed = 1.2f;
    //�������
    private Animator animator;
    //����
    public string desc = "";

    internal bool isAttacking = false;
    internal float attackTime = float.MinValue;

    //�ƶ���ĳ��
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    //�ƶ�Update
    public void MoveUpdate()
    {
        if (isMoving == false)
        {
            return;
        }
        float time = Time.deltaTime;
        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * time);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }

    //��������
    public void Attack()
    {
        isAttacking = true;
        attackTime = Time.time;
        animator.SetBool("isAttacking", true);
    }

    //����Update
    public void AttackUpdate()
    {
        if (!isAttacking) return;
        if (Time.time - attackTime < 1.2f) return;
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }


    // Use this for initialization
    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }
}