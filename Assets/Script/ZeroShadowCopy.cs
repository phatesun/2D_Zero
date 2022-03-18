using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroShadowCopy : MonoBehaviour
{
    public Transform LastPosition;

    public SpriteRenderer ZeroShadowSprite;

    private SpriteRenderer thisSprite;


    private void Start()
    {
        thisSprite = GetComponent<SpriteRenderer>();
        thisSprite.sprite = ZeroShadowSprite.sprite;
    }

    private void FixedUpdate()
    {
        SetSprite(); 

        SetPosition();
        
    }



    void SetSprite()
    {
        thisSprite.sprite = ZeroShadowSprite.sprite;
        transform.localScale = LastPosition.localScale;
        transform.rotation = LastPosition.rotation;    
        
    }


    void SetPosition()
    {
        transform.position = LastPosition.position;
    }


}



