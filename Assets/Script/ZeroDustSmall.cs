using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroDustSmall : MonoBehaviour
{
    private Transform player;

    private float activeTime = 0.15f;//显示时间
    private float activeStart = 0f;//开始现实的时间点

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
            Pool.instance.ReturnPoolDustSmall(this.gameObject);//返回对象池
        }
    }
}
