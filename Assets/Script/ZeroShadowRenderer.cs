using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroShadowRenderer : MonoBehaviour
{
    
    private Transform zeroShadow;

    private SpriteRenderer thisSprite;
    private SpriteRenderer zeroShadowSprite;



    void Start()
    {
        
        zeroShadow = GameObject.FindGameObjectWithTag("ZeroShadow").transform;

        thisSprite = GetComponent<SpriteRenderer>();
        zeroShadowSprite = zeroShadow.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        thisSprite.sprite = zeroShadowSprite.sprite;
    }
}
