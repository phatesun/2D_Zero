using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroShadowZZZ : MonoBehaviour
{
    private Transform player;
    private GameObject zeroShadow1;
    private GameObject zeroShadow2;
    private GameObject zeroShadow3;



    

    public float speedZero = ZeroControler.speed;

    private float timeLater1;   //控制第一个残影的延迟
    private float timeLater2;   //控制第二个残影的延迟
    private float timeLater3;   //控制第三个残影的延迟

    private float timeLaterDash = -0.7f; //控制放开冲刺后残影是否依次排开

    private float timeLaterSprite1 = -0.1f;  //控制第一个残影的SpriteRenderer的存在时间
    private float timeLaterSprite2 = -0.1f;  //控制第二个残影的SpriteRenderer的存在时间
    private float timeLaterSprite3 = -0.1f;  //控制第三残影的SpriteRenderer的存在时间


    //private bool isDash;
    //private bool isDashKeyDown;

    //private float timeLaterFinal;


    //private SpriteRenderer thisSprite;
    //private SpriteRenderer zeroShadowSprite;



    [Header("事件控制参数")]
    public float activeTime;//显示事件
    public float activeStart;//开始显示的事件点

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        zeroShadow1 = GameObject.FindGameObjectWithTag("ZeroShadow1");
        zeroShadow2 = GameObject.FindGameObjectWithTag("ZeroShadow2");
        zeroShadow3 = GameObject.FindGameObjectWithTag("ZeroShadow3");

        

        //thisSprite = GetComponent<SpriteRenderer>();
        //zeroShadowSprite = zeroShadow1.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        ShadowControl();

        if (Time.time >= activeStart + activeTime)
        {
            Pool.instance.ReturnPoolShadow(this.gameObject);//返回对象池++++++
        }
       
    }

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //zeroShadow1 = GameObject.FindGameObjectWithTag("ZeroShadow1").transform;
        //zeroShadow2 = GameObject.FindGameObjectWithTag("ZeroShadow2").transform;
        //zeroShadow3 = GameObject.FindGameObjectWithTag("ZeroShadow3").transform;

        //thisSprite = GetComponent<SpriteRenderer>();
        //zeroShadowSprite = zeroShadow1.GetComponent<SpriteRenderer>();
        transform.position = player.position;

        timeLater1 = Time.time + 0.02f;

        timeLater2 = Time.time + 0.06f;

        timeLater3 = Time.time + 0.10f;

        //timeLaterFinal = Time.time + 0.5f;

        activeStart = Time.time;
    }
   
    private void TimeDelay()
    {


        if (Time.time > timeLater3)
        {
            zeroShadow3.transform.position = transform.position;
            //zeroShadow3.transform.rotation = player.transform.rotation;
            //zeroShadow3.transform.localScale = player.transform.localScale;
        }

        else if (Time.time > timeLater2)
        {
            zeroShadow2.transform.position = transform.position;
            //zeroShadow2.transform.rotation = player.transform.rotation;
            //zeroShadow2.transform.localScale = player.transform.localScale;
        }
        else if (Time.time > timeLater1)
        {
            zeroShadow1.transform.position = transform.position;
            //zeroShadow1.transform.rotation = player.transform.rotation;
            //zeroShadow1.transform.localScale = player.transform.localScale;
        }


    }

    private void NoDelay()
    {
        zeroShadow3.transform.position = transform.position;
        zeroShadow2.transform.position = transform.position;
        zeroShadow1.transform.position = transform.position;
    }

    private void ShadowControl()   //控制残影的存留时间， 以及让残影跟随主体
    {
        speedZero = ZeroControler.speed;


        if (speedZero > 15f)
        {
            timeLaterDash = Time.time + 0.15f;
            timeLaterSprite1 = Time.time + 0.08f;
            timeLaterSprite2 = Time.time + 0.16f;
            timeLaterSprite3 = Time.time + 0.2f;

        }

        if (Time.time > timeLaterSprite1)
        {
            zeroShadow1.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Time.time > timeLaterSprite2)
        {
            zeroShadow2.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Time.time > timeLaterSprite3)
        {
            zeroShadow3.GetComponent<SpriteRenderer>().enabled = false;
        }


        if (Time.time > timeLaterDash)
        {
            NoDelay();

        }
        else
        {
            TimeDelay();
            zeroShadow1.GetComponent<SpriteRenderer>().enabled = true;
            zeroShadow2.GetComponent<SpriteRenderer>().enabled = true;
            zeroShadow3.GetComponent<SpriteRenderer>().enabled = true;
        }

    }
}
