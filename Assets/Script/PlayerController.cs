using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    public Collider2D groundCheckColl;

    

    public float gravityScaleJump = 0;

    public float gravityScaleA = 1;
    public float gravityScaleB = 2;
    public float gravityScaleC = 3;
    public float gravityScaleD = 4;

    public float plusA = 0.3f;
    public float plusB = 0.5f;
    public float plusC = 0.8f;
    public float plusD = 1.0f;

    public float gravityPrint;
    public float gravityScaleFinal;
    public float speed, jumpForce;
    //public Transform groundCheck;         //ԭ�����ڵ�����ķ���
    
    public LayerMask ground;

    public bool isGround, isJump;

    public bool releasedJump = false;

    public bool Jumping = false;

    

    bool jumpPressed;
    int jumpCount;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Jump") /*&& jumpCount > 0*/)
        {
            jumpPressed = true;
        }
        if (Input.GetButton("Jump"))
        {
            Jumping = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            releasedJump = true;
        }
    }

    private void FixedUpdate()
    {
        //isGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);      //ԭ�����ڵ�����ķ���
        GroundMovement();
        Jump();

        if (Jumping && rb.velocity.y > 0)
        {
            JumpingUp();
            gravityPrint = rb.gravityScale;
            print(gravityPrint);
        }

        if(rb.velocity.y <= 0)
        {
            rb.gravityScale = gravityScaleFinal;
        }

        SwitchAnim();

        if (releasedJump)
        {
            StopJump();
        }
    }



    void GroundMovement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
        anim.SetFloat("running", Mathf.Abs(horizontalMove));

        if (horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        }

    }

    void Jump()
    {
        /*
        if (isGround)
        {
            jumpCount = 1;

        }
        */
        if (jumpPressed && isGround)
        {
            isJump = true;
            rb.gravityScale = gravityScaleJump;   //������Ϊ0
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);  //ԭ������Ծ����
            //jumpCount++;
            jumpPressed = false;
        }
        /*
        else if (jumpPressed && jumpCount > 0 && isJump)
        {
            rb.gravityScale = -4;   //������Ϊ0
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce); //ԭ������Ծ����
            jumpCount--;
            jumpCount--;
            jumpPressed = false;
        }
        */

    }

    void JumpingUp()
    {
        if(rb.gravityScale < gravityScaleA)
        {
            rb.gravityScale += plusA;
        }
        if(rb.gravityScale >= gravityScaleA && rb.gravityScale < gravityScaleB)
        {
            rb.gravityScale += plusB;
        }

        if(rb.gravityScale >= gravityScaleB && rb.gravityScale < gravityScaleC)
        {
            rb.gravityScale += plusC;
        }

        if (rb.gravityScale >= gravityScaleC && rb.gravityScale <= gravityScaleD)
        {
            rb.gravityScale += plusD;
        }


    }

    private void StopJump()
    {

        rb.gravityScale = gravityScaleFinal;
        releasedJump = false;
        Jumping = false;

    }

    void SwitchAnim()//ע�⶯�����ٵ��л����ܵ�ʱ��Ҫ��飬���˳�ʱ�� ȡ���˹�ѡ���Լ��̶�����ʱ������Ϊ0
                     //���Ҹ�����߼�û�������⣬����������ֵͼ����⣩
    {


        if (isGround)
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
            anim.SetBool("jumping", false);
            anim.SetFloat("running", Mathf.Abs(rb.velocity.x));
        }
        else if (rb.velocity.y > 0)
        {
            anim.SetBool("jumping", true);
            anim.SetBool("falling", false);
            anim.SetBool("idle", false);
        }
        else if (!isGround && rb.velocity.y < 0)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
            anim.SetBool("idle", false);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag =="Ground")
        {
            isGround = false;
        }
    }
}

