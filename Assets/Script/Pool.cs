using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool instance;

    public GameObject dustBigPrefab;
    public GameObject dustSmallPrefab;

    public GameObject shadowPrefab;

    
    private Queue<GameObject> availableObjectsDustSmall = new Queue<GameObject>();
    private Queue<GameObject> availableObjectsDustBig = new Queue<GameObject>();

    private Queue<GameObject> availableObjectsShadow = new Queue<GameObject>();

    private void Awake()
    {
        instance = this;

        FillPoolDustBig();

        FillPoolDustSmall();

        FillPoolShadow();


    }

    public void FillPoolDustBig()
    {
        for(int i = 0; i < 2; i++)
        {
            var newDustBig = Instantiate(dustBigPrefab);
            newDustBig.transform.SetParent(transform);
     
            ReturnPoolDustBig(newDustBig);//取消启用，返回对象池
        }
    }

    public void FillPoolDustSmall()
    {
        for (int j = 0; j < 2; j++)
        {
            var newDustSmall = Instantiate(dustSmallPrefab);
            newDustSmall.transform.SetParent(transform);

            ReturnPoolDustSmall(newDustSmall);
        }

    }

    
    public void FillPoolShadow()
    {
        for (int k = 0; k < 2; k++)
        {
            var newShadow = Instantiate(shadowPrefab);
            newShadow.transform.SetParent(transform);

            ReturnPoolShadow(newShadow);
        }
    }


    public void ReturnPoolDustBig(GameObject gameObject)
    {
        gameObject.SetActive(false);

        availableObjectsDustBig.Enqueue(gameObject);
    }

    public void ReturnPoolDustSmall(GameObject gameObject)
    {
        gameObject.SetActive(false);

        availableObjectsDustSmall.Enqueue(gameObject);
    }

    public void ReturnPoolShadow(GameObject gameObject)
    {
        gameObject.SetActive(false);

        availableObjectsShadow.Enqueue(gameObject);
    }



    public GameObject GetFormPoolDustBig()
    {
        if(availableObjectsDustBig.Count == 0)
        {
            FillPoolDustBig();
        }
        
        var outDustBig = availableObjectsDustBig.Dequeue();

        outDustBig.SetActive(true);

        return outDustBig;

    }

    public GameObject GetFormPoolDustSmall()
    {
        if(availableObjectsDustSmall.Count == 0)
        {
            FillPoolDustSmall();
        }

        var outDustSmall = availableObjectsDustSmall.Dequeue();

        outDustSmall.SetActive(true);

        return outDustSmall;
    }

    public GameObject GetFormPoolShadow()
    {
        if (availableObjectsShadow.Count == 0)
        {
            FillPoolShadow();
        }
        
        var outShadow = availableObjectsShadow.Dequeue();

        outShadow.SetActive(true);

        return outShadow;
    }
}
