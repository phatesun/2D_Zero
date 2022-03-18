using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroControler : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animZero;
    private bool isOnGround;

    private AnimatorStateInfo curAnimInfo, lastAnimInfo;
    //private bool bIsOnWall;
    //按键设置（监测是否按下了某个按键）
    private bool jumpPressed;
    private bool releasedJump = false;
    private bool jumping = false;
    private bool faceRight = true;
    private bool dash = false;
    private bool dashing = false;//用于设置冲出平台时的相关设定。

    private float nextJumpAttackTime = 0;   //跳跃攻击的CD用
    private float dashTime = 0;             //按住冲刺多久以后冲刺停止用

    [SerializeField]
    private int AttackNumber = 0;           //该数字用于三连击的控制

    public AudioSource runAudio;            //跑动的声音
    public AudioSource dashAudio;           //冲刺的声音
    public AudioSource lightSaberAudioAttack;    //光剑声音1号
    public AudioSource lightSaberAudioEffect;    //光剑声音2号

    public Animator animSaber;              //获取光剑的动画器
    public Animator animShadow;             //获取残影的动画控制器

    public Transform wallCheck;             //用于检测面向侧是否有墙壁的点
    public LayerMask isOnWall;              //使用LayerMask检测面向侧是否有墙壁的点

    //public Collider2D groundCheck;          //地面监测用的检测器(本来以为要声明，但是发现不在这里声明一样可以 OnTriggerEnter2D 会检测所有的2D触发)

    static public float speed = 12;                //移动速度设置
    public float jumpForce = 20;            //跳跃力的大小
    
    [Header("--------跳跃中的重力参数--------")]
    #region

    public float gravityScaleA = 0.2f;
    public float gravityScaleB = 3f;
    public float gravityScaleC = 5f;
    public float gravityScaleD = 100f;

    public float plusA = 0.2f;
    public float plusB = 0.7f;
    public float plusC = 1f;
    public float plusD = 0.3f;

    public float gravityScaleFinal;
    #endregion

    [Header("环境检测")]
    public LayerMask groundLayer;

    void Start()
    {      
        rb = GetComponent<Rigidbody2D>();
        animZero = GetComponent<Animator>();
    }

    void Update()               //需要逐帧运行的函数方法 , 放在update里面
    {
        UpdateAni02();          //Debug

        JumpCheck();            //监测按键输入-是否按下了跳跃键

        AttackCheckAnimSet();   //监测按键输入-是否按下了攻击键，并设置相关动画

        Dash();                 //监测按键输入-是否按下了冲刺键，并设置相关动画

        SwitchAnim();           //监测动画参数的变化，并设置相攻击以外的关动画    
    }

    private void FixedUpdate()  //和物理模拟有关的（比如力的施加），都放在FixedUpdate里运行
    {
        GroundMovement();       //移动监测

        Jump();                 //跳跃监测

        Pool.instance.GetFormPoolShadow();  //生成幻影    
    }



    private void JumpCheck()            //放在update中, 监测跳跃键是否按下,并设置相应的布尔值
    {
        if (animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3B"))
        {
            jumpPressed = false;//监测正在播放的动画是否为攻击动画，如果是，则跳跃为false（无法跳跃）
        }
        else    //否则
        {
            if (Input.GetButtonDown("Jump"))//按下跳跃键的时候
            {
                jumpPressed = true;

            }
            if (Input.GetButton("Jump"))    //按住跳跃键的时候
            {
                jumping = true;
            }
            if (Input.GetButtonUp("Jump"))  //放开跳跃键的时候
            {
                releasedJump = true;
            }

            if (rb.gravityScale < 0.2f && jumping == false)
            {
                rb.gravityScale = gravityScaleFinal;
            }
        }
    }

    private void AttackCheckAnimSet()   //放在update中, 用于监测攻击键是否按下,设定相应的动画参数
    {
        if (Input.GetKeyDown("j") && isOnGround && dash == false && (Input.GetKey("a")|| Input.GetKey("d")))    //在地面移动状态按下攻击键
        {
            animSaber.SetTrigger("RunAttackBladeTrigger");
        }

        if (Input.GetKeyDown("j") && isOnGround && dash ==true)    //在地面冲刺状态按下攻击键
        {
            animZero.SetTrigger("DashAttackTrigger");
            animSaber.SetTrigger("DashAttackTrigger");
            animShadow.SetTrigger("DashAttackTrigger");
        }

        if (Input.GetKeyDown("j") && isOnGround && !Input.GetKey("a") && !Input.GetKey("d") && !Input.GetKey("l"))    //在地面静止状态按下攻击键  原先用是否有位移判定，有点问题，还是判定是否按下某个按键更好System.Math.Abs(rb.velocity.x) < 0.3
        {
            AttackNumber++;
            animZero.SetInteger("AttackNum", AttackNumber);
        }

        if (Input.GetKeyDown("j") && !isOnGround && Time.time > nextJumpAttackTime)         //在空中的时候按下攻击键
        {
            animZero.SetTrigger("JumpAttackTrigger");                       //本体的动画设置
            animShadow.SetTrigger("JumpAttackTrigger");                       //残影的动画设置
            animSaber.SetTrigger("JumpAttackBladeTrigger");
            nextJumpAttackTime = Time.time + 0.3f;//攻击间隔设置
        }
    }

    void SwitchAnim()                   //放在update中,设置跳跃与落下的动画
    {
        if (isOnGround)
        {          
            animZero.SetBool("idle", true);
            animZero.SetBool("falling", false);
            animZero.SetBool("jumping", false);               //本体的动画设置

            animShadow.SetBool("idle", true);
            animShadow.SetBool("falling", false);
            animShadow.SetBool("jumping", false);             //残影的动画设置
        }
        else if (rb.velocity.y > 0)
        {
            animZero.SetBool("jumping", true);
            animZero.SetBool("falling", false);
            animZero.SetBool("idle", false);                  //本体的动画设置

            animShadow.SetBool("jumping", true);
            animShadow.SetBool("falling", false);
            animShadow.SetBool("idle", false);                //残影的动画设置
        }
        else if ( !isOnGround && rb.velocity.y <= 0)        
        {
            animZero.SetBool("jumping", false);
            animZero.SetBool("falling", true);
            animZero.SetBool("idle", false);                   //本体的动画设置

            animShadow.SetBool("jumping", false);
            animShadow.SetBool("falling", true);
            animShadow.SetBool("idle", false);                 //本体的动画设置
        }
    }



    void GroundMovement()               //放在FixedUpdate中， 因为涉及到力的施加
    {

        float horizontalMove = Input.GetAxisRaw("Horizontal");  //设置horizontalMove = 左摇杆的输入
        if (animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3B")) //监测是否在三连击动画中,如果是
        {
            rb.velocity = new Vector2(0, rb.velocity.y);                //则无法移动
        }
        else
        {
            //以下函数方法使用摇杆的时候, 不会有力度感应
            if (horizontalMove > 0.4)
            {
                faceRight = true;
                transform.localScale = new Vector3(1, 1, 1);
                rb.velocity = new Vector2(speed, rb.velocity.y);
                animZero.SetFloat("running", speed);    //设置成speed，可以用于切换冲刺与跑步的动画
                animShadow.SetFloat("running", speed);
            }
            if (horizontalMove < -0.4)
            {
                faceRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                animZero.SetFloat("running", speed);    //设置成speed，可以用于切换冲刺与跑步的动画
                animShadow.SetFloat("running", speed);
            }
            if (horizontalMove >= -0.4 && horizontalMove <= 0.4)        //当没有方向输入的时候
            {
                if (!isOnGround)        //没有方向输入的大前提下， 不在地面上
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animZero.SetFloat("running", 0);        //那么移动就是0
                    animShadow.SetFloat("running", 0);
                }
                if(dashing == false)                   //dashing 状态为false（设置了两个布尔值，以方便控制冲刺时的各种状态变化） 一个布尔值可以表示两种状态，两个布尔值则可以表示四种状态，其实可以考虑使用int来切换也是可以的
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animZero.SetFloat("running", 0);
                    animShadow.SetFloat("running", 0);
                }
                if(dash == false)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animZero.SetFloat("running", 0);        //切换到待机动画
                    animShadow.SetFloat("running", 0);
                }
                
                if (dash == true && faceRight == true && isOnGround && dashing == true)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                    animZero.SetFloat("running", speed);
                    animShadow.SetFloat("running", speed);
                }

                if (dash == true && faceRight == false && isOnGround && dashing == true)
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                    animZero.SetFloat("running", speed);
                    animShadow.SetFloat("running", speed);
                }

            }

            //以下方法使用摇杆时，有力度感应。
            /*
            rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

            if (horizontalMove != 0)
            {
                transform.localScale = new Vector3(horizontalMove, 1, 1);
            }
            */
        }

    }              

    
    void Jump()                         //放在FixedUpdate中，因为涉及到力的施加
    {

        if (jumpPressed && isOnGround)                                      //如果跳跃键按下了，并且在地面上
        {
            rb.gravityScale = 0;                             //重力改为0
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);    //施加一个 瞬发力
            jumpPressed = false;                                            //将jumpPressd改为false (因为已经按下了jump了,功能已经执行完)
        }

        if (jumping && rb.velocity.y > 0)           //如果正在向上运动,则进行一系列的重力调整设置
        {
            if (rb.gravityScale < gravityScaleA)
            {
                rb.gravityScale += plusA;
            }
            if (rb.gravityScale >= gravityScaleA && rb.gravityScale < gravityScaleB)
            {
                rb.gravityScale += plusB;
            }

            if (rb.gravityScale >= gravityScaleB && rb.gravityScale < gravityScaleC)
            {
                rb.gravityScale += plusC;
            }

            if (rb.gravityScale >= gravityScaleC && rb.gravityScale < gravityScaleD)
            {
                rb.gravityScale += plusD;
            }
        }

        if (rb.velocity.y <= 0 )                     //如果正在下落,则设置重力为最终重力-2, 这样可以确保按住跳跃键的时候,比放开跳跃键,跳跃距离微妙的长一点点
        {
            rb.gravityScale = gravityScaleFinal - 2f;
        }

        if (rb.velocity.y <= -25)                  //为了不让物体掉落的时候持续加速， 当达到某个速度之后，希望他速度恒定，否则太快了。  
        {
            rb.gravityScale = 0;
        }


        if (releasedJump)                           //如果跳跃键被放开了,releasedJump就会为true
        {
            rb.gravityScale = gravityScaleFinal;    //此时的重力为最终重力
            releasedJump = false;                   //将releasedJump改为false (因为已放开了jump了,功能已经执行完)
            jumping = false;                        //此时jumping自然也为false,因为不在跳跃中了
        }

    }

    private void Dash()//冲刺的整体思路:在地面上冲刺,不按方向键,角色也可以移动. 但若在空中,则必须靠方向键控制移动方向.  
        //有一个难点在于, 原本在地面上,然后离开地面又回到地面, 这个部分的逻辑比较难以整理.
    {
        if (animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3B")) //监测是否在三连击动画中,如果是
        {
                         //则不会检测是否按下冲刺键
        }
        else
        {
            if (Input.GetKeyDown("l") && isOnGround)
            {
                speed = 18;
                dash = true;    //启动的时候也许只设置dash为true, 在持续按下冲刺时,再设置dashing为ture会比较好.这样也比较容易理解.
                animShadow.SetBool("dash", true);
                dashTime = Time.time + 0.5f;
                Pool.instance.GetFormPoolDustBig();     //生成大灰尘
            }

            if (Input.GetKeyDown("l") && Time.time <= dashTime && !isOnGround)//按下冲刺键,并且冲刺CD中, 不在地面上了, dash数值还是设置成true,但是dashing要设置成false,以防止冲出平台边缘,再落回地面的时候,角色仍能够移动.
            {
                dash = true;
                dashing = false;
            }

            if (Input.GetKeyDown("l") && Time.time <= dashTime && isOnGround)//按下冲刺键,并且冲刺CD中, 但仍然在地面上,那么dash和dashing都是ture,并且动画布尔值dash也要为true.
            {
                dash = true;                    //此处的布尔值用于控制左右方向键移动的时候，和冲刺之间的关系
                dashing = true;                 //此处的布尔值用于控制左右方向键移动的时候，和冲刺之间的关系
                animZero.SetBool("dash", true);
                animShadow.SetBool("dash", true);
            }

            if (Time.time > dashTime && isOnGround)         //冲刺CD到了,结束冲刺(与其说CD ,不如说是单次冲刺的持续时长, 但是CD方便写一些)
            {
                speed = 12;
                dash = false;
                animZero.SetBool("dash", false);
                animShadow.SetBool("dash", false);
            }

            if (Input.GetKeyUp("l") && isOnGround)          //如果在地面上放开了冲刺键
            {
                speed = 12;                 //设置他的速度为12
                dash = false;
                dashing = false;
                animZero.SetBool("dash", false);
                animShadow.SetBool("dash", false);
            }
        }
       
    }
    
    
    
    
    //----------------------------------动画切换判定---------------------------------//

    private void UpdateAni02()          //用于判定动画是否由跑动切换到了待机,  如果是则播放音效
    {
        curAnimInfo = animZero.GetCurrentAnimatorStateInfo(0);
        if(lastAnimInfo.shortNameHash != curAnimInfo.shortNameHash)
        {
            if (lastAnimInfo.IsName("Zero_run")&& curAnimInfo.IsName("Zero_idle"))         
                RunAudioPlay();


            lastAnimInfo = curAnimInfo;
        }
    }



    //------------------------------------墙壁判定-----------------------------------//
    #region
    /*
        public void IsOnWall()
        {
            if (Physics2D.OverlapCircle(wallCheck.position, 0.1f, isOnWall))
            {

                bIsOnWall = true;
                Debug.Log("isOnWall");
            }
            else
            {
                bIsOnWall = false;
            }
        }
    */
    #endregion
    //------------------------------------地面判定-----------------------------------//
    #region
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;
            speed = 12;//进入地面判定的时候,速度也要切换回12
            animZero.SetFloat("running", speed);    //设置成speed，可以用于切换冲刺与跑步的动画
            animShadow.SetFloat("running", speed);

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;//站在地面上的时候， 速度是可以变化的
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = false;
            dashing = false;        //每次离开地面,都要把dashing 设置成false, 这样即便出平台后,再回到地面,也不会发生位移了.
        }
    }
    #endregion

    //------------------------------------动画事件------------------------------------//
    #region
    void SetLayerWeight_RunAttackLayerOff()   //设置动画权重，让跳跃的时候，跑动攻击的动画为off       //冲刺需要用到
    {
        animZero.SetLayerWeight(2, 0f);
        animZero.SetLayerWeight(3, 0f);
    }
    void SetAttackNum()     //设置AttackNum  用以控制播放三段攻击
    {
        AttackNumber = 0;
        animZero.SetInteger("AttackNum", AttackNumber);
    }

    void SetJumpAttackLayerOff()
    {
        if (isOnGround)     //设置跳跃攻击动画的权重，让跳跃攻击动画在落地的一瞬间直接消失，这样会比较流畅
        {
            animZero.SetLayerWeight(4, 0f);
        }
    }
    

    void GetDustSmall()     //生成小灰尘
    {
        Pool.instance.GetFormPoolDustSmall();
    }

    void SetSpeed()         //设置移动速度
    {
        speed = 12;
    }

    void RunAudioPlay()     //跑动声音
    {
        runAudio.Play();
    }
    
    void DashAudioPlay()    //冲刺声音
    {
        dashAudio.Play();
    }

    void LightSaberAudioAttack()     //光剑声音1
    {
        lightSaberAudioAttack.Play();
    }

    void LightSaberAudioEffect()     //光剑声音2
    {
        
        lightSaberAudioEffect.Play();
        
    }
    #endregion
}



