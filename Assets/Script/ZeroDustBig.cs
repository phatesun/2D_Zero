using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroDustBig : MonoBehaviour
{
    private Transform player;

    private float activeTime = 0.25f;//��ʾʱ��
    private float activeStart = 0f;//��ʼ��ʵ��ʱ���

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        activeStart = Time.time;
    }



    void Update()
    {
        if (Time.time >= activeStart + activeTime)
        {
            Pool.instance.ReturnPoolDustBig(this.gameObject);//���ض����
        }
    }
}
