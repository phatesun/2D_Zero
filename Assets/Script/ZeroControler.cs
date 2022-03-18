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
    //�������ã�����Ƿ�����ĳ��������
    private bool jumpPressed;
    private bool releasedJump = false;
    private bool jumping = false;
    private bool faceRight = true;
    private bool dash = false;
    private bool dashing = false;//�������ó��ƽ̨ʱ������趨��

    private float nextJumpAttackTime = 0;   //��Ծ������CD��
    private float dashTime = 0;             //��ס��̶���Ժ���ֹͣ��

    [SerializeField]
    private int AttackNumber = 0;           //�����������������Ŀ���

    public AudioSource runAudio;            //�ܶ�������
    public AudioSource dashAudio;           //��̵�����
    public AudioSource lightSaberAudioAttack;    //�⽣����1��
    public AudioSource lightSaberAudioEffect;    //�⽣����2��

    public Animator animSaber;              //��ȡ�⽣�Ķ�����
    public Animator animShadow;             //��ȡ��Ӱ�Ķ���������

    public Transform wallCheck;             //���ڼ��������Ƿ���ǽ�ڵĵ�
    public LayerMask isOnWall;              //ʹ��LayerMask���������Ƿ���ǽ�ڵĵ�

    //public Collider2D groundCheck;          //�������õļ����(������ΪҪ���������Ƿ��ֲ�����������һ������ OnTriggerEnter2D �������е�2D����)

    static public float speed = 12;                //�ƶ��ٶ�����
    public float jumpForce = 20;            //��Ծ���Ĵ�С
    
    [Header("--------��Ծ�е���������--------")]
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

    [Header("�������")]
    public LayerMask groundLayer;

    void Start()
    {      
        rb = GetComponent<Rigidbody2D>();
        animZero = GetComponent<Animator>();
    }

    void Update()               //��Ҫ��֡���еĺ������� , ����update����
    {
        UpdateAni02();          //Debug

        JumpCheck();            //��ⰴ������-�Ƿ�������Ծ��

        AttackCheckAnimSet();   //��ⰴ������-�Ƿ����˹���������������ض���

        Dash();                 //��ⰴ������-�Ƿ����˳�̼�����������ض���

        SwitchAnim();           //��⶯�������ı仯���������๥������Ĺض���    
    }

    private void FixedUpdate()  //������ģ���йصģ���������ʩ�ӣ���������FixedUpdate������
    {
        GroundMovement();       //�ƶ����

        Jump();                 //��Ծ���

        Pool.instance.GetFormPoolShadow();  //���ɻ�Ӱ    
    }



    private void JumpCheck()            //����update��, �����Ծ���Ƿ���,��������Ӧ�Ĳ���ֵ
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
            jumpPressed = false;//������ڲ��ŵĶ����Ƿ�Ϊ��������������ǣ�����ԾΪfalse���޷���Ծ��
        }
        else    //����
        {
            if (Input.GetButtonDown("Jump"))//������Ծ����ʱ��
            {
                jumpPressed = true;

            }
            if (Input.GetButton("Jump"))    //��ס��Ծ����ʱ��
            {
                jumping = true;
            }
            if (Input.GetButtonUp("Jump"))  //�ſ���Ծ����ʱ��
            {
                releasedJump = true;
            }

            if (rb.gravityScale < 0.2f && jumping == false)
            {
                rb.gravityScale = gravityScaleFinal;
            }
        }
    }

    private void AttackCheckAnimSet()   //����update��, ���ڼ�⹥�����Ƿ���,�趨��Ӧ�Ķ�������
    {
        if (Input.GetKeyDown("j") && isOnGround && dash == false && (Input.GetKey("a")|| Input.GetKey("d")))    //�ڵ����ƶ�״̬���¹�����
        {
            animSaber.SetTrigger("RunAttackBladeTrigger");
        }

        if (Input.GetKeyDown("j") && isOnGround && dash ==true)    //�ڵ�����״̬���¹�����
        {
            animZero.SetTrigger("DashAttackTrigger");
            animSaber.SetTrigger("DashAttackTrigger");
            animShadow.SetTrigger("DashAttackTrigger");
        }

        if (Input.GetKeyDown("j") && isOnGround && !Input.GetKey("a") && !Input.GetKey("d") && !Input.GetKey("l"))    //�ڵ��澲ֹ״̬���¹�����  ԭ�����Ƿ���λ���ж����е����⣬�����ж��Ƿ���ĳ����������System.Math.Abs(rb.velocity.x) < 0.3
        {
            AttackNumber++;
            animZero.SetInteger("AttackNum", AttackNumber);
        }

        if (Input.GetKeyDown("j") && !isOnGround && Time.time > nextJumpAttackTime)         //�ڿ��е�ʱ���¹�����
        {
            animZero.SetTrigger("JumpAttackTrigger");                       //����Ķ�������
            animShadow.SetTrigger("JumpAttackTrigger");                       //��Ӱ�Ķ�������
            animSaber.SetTrigger("JumpAttackBladeTrigger");
            nextJumpAttackTime = Time.time + 0.3f;//�����������
        }
    }

    void SwitchAnim()                   //����update��,������Ծ�����µĶ���
    {
        if (isOnGround)
        {          
            animZero.SetBool("idle", true);
            animZero.SetBool("falling", false);
            animZero.SetBool("jumping", false);               //����Ķ�������

            animShadow.SetBool("idle", true);
            animShadow.SetBool("falling", false);
            animShadow.SetBool("jumping", false);             //��Ӱ�Ķ�������
        }
        else if (rb.velocity.y > 0)
        {
            animZero.SetBool("jumping", true);
            animZero.SetBool("falling", false);
            animZero.SetBool("idle", false);                  //����Ķ�������

            animShadow.SetBool("jumping", true);
            animShadow.SetBool("falling", false);
            animShadow.SetBool("idle", false);                //��Ӱ�Ķ�������
        }
        else if ( !isOnGround && rb.velocity.y <= 0)        
        {
            animZero.SetBool("jumping", false);
            animZero.SetBool("falling", true);
            animZero.SetBool("idle", false);                   //����Ķ�������

            animShadow.SetBool("jumping", false);
            animShadow.SetBool("falling", true);
            animShadow.SetBool("idle", false);                 //����Ķ�������
        }
    }



    void GroundMovement()               //����FixedUpdate�У� ��Ϊ�漰������ʩ��
    {

        float horizontalMove = Input.GetAxisRaw("Horizontal");  //����horizontalMove = ��ҡ�˵�����
        if (animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3B")) //����Ƿ���������������,�����
        {
            rb.velocity = new Vector2(0, rb.velocity.y);                //���޷��ƶ�
        }
        else
        {
            //���º�������ʹ��ҡ�˵�ʱ��, ���������ȸ�Ӧ
            if (horizontalMove > 0.4)
            {
                faceRight = true;
                transform.localScale = new Vector3(1, 1, 1);
                rb.velocity = new Vector2(speed, rb.velocity.y);
                animZero.SetFloat("running", speed);    //���ó�speed�����������л�������ܲ��Ķ���
                animShadow.SetFloat("running", speed);
            }
            if (horizontalMove < -0.4)
            {
                faceRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                animZero.SetFloat("running", speed);    //���ó�speed�����������л�������ܲ��Ķ���
                animShadow.SetFloat("running", speed);
            }
            if (horizontalMove >= -0.4 && horizontalMove <= 0.4)        //��û�з��������ʱ��
            {
                if (!isOnGround)        //û�з�������Ĵ�ǰ���£� ���ڵ�����
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animZero.SetFloat("running", 0);        //��ô�ƶ�����0
                    animShadow.SetFloat("running", 0);
                }
                if(dashing == false)                   //dashing ״̬Ϊfalse����������������ֵ���Է�����Ƴ��ʱ�ĸ���״̬�仯�� һ������ֵ���Ա�ʾ����״̬����������ֵ����Ա�ʾ����״̬����ʵ���Կ���ʹ��int���л�Ҳ�ǿ��Ե�
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animZero.SetFloat("running", 0);
                    animShadow.SetFloat("running", 0);
                }
                if(dash == false)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animZero.SetFloat("running", 0);        //�л�����������
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

            //���·���ʹ��ҡ��ʱ�������ȸ�Ӧ��
            /*
            rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

            if (horizontalMove != 0)
            {
                transform.localScale = new Vector3(horizontalMove, 1, 1);
            }
            */
        }

    }              

    
    void Jump()                         //����FixedUpdate�У���Ϊ�漰������ʩ��
    {

        if (jumpPressed && isOnGround)                                      //�����Ծ�������ˣ������ڵ�����
        {
            rb.gravityScale = 0;                             //������Ϊ0
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);    //ʩ��һ�� ˲����
            jumpPressed = false;                                            //��jumpPressd��Ϊfalse (��Ϊ�Ѿ�������jump��,�����Ѿ�ִ����)
        }

        if (jumping && rb.velocity.y > 0)           //������������˶�,�����һϵ�е�������������
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

        if (rb.velocity.y <= 0 )                     //�����������,����������Ϊ��������-2, ��������ȷ����ס��Ծ����ʱ��,�ȷſ���Ծ��,��Ծ����΢��ĳ�һ���
        {
            rb.gravityScale = gravityScaleFinal - 2f;
        }

        if (rb.velocity.y <= -25)                  //Ϊ�˲�����������ʱ��������٣� ���ﵽĳ���ٶ�֮��ϣ�����ٶȺ㶨������̫���ˡ�  
        {
            rb.gravityScale = 0;
        }


        if (releasedJump)                           //�����Ծ�����ſ���,releasedJump�ͻ�Ϊtrue
        {
            rb.gravityScale = gravityScaleFinal;    //��ʱ������Ϊ��������
            releasedJump = false;                   //��releasedJump��Ϊfalse (��Ϊ�ѷſ���jump��,�����Ѿ�ִ����)
            jumping = false;                        //��ʱjumping��ȻҲΪfalse,��Ϊ������Ծ����
        }

    }

    private void Dash()//��̵�����˼·:�ڵ����ϳ��,���������,��ɫҲ�����ƶ�. �����ڿ���,����뿿����������ƶ�����.  
        //��һ���ѵ�����, ԭ���ڵ�����,Ȼ���뿪�����ֻص�����, ������ֵ��߼��Ƚ���������.
    {
        if (animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber1C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2B") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber2C") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3A") ||
            animZero.GetCurrentAnimatorStateInfo(1).IsName("Zero_Saber3B")) //����Ƿ���������������,�����
        {
                         //�򲻻����Ƿ��³�̼�
        }
        else
        {
            if (Input.GetKeyDown("l") && isOnGround)
            {
                speed = 18;
                dash = true;    //������ʱ��Ҳ��ֻ����dashΪtrue, �ڳ������³��ʱ,������dashingΪture��ȽϺ�.����Ҳ�Ƚ��������.
                animShadow.SetBool("dash", true);
                dashTime = Time.time + 0.5f;
                Pool.instance.GetFormPoolDustBig();     //���ɴ�ҳ�
            }

            if (Input.GetKeyDown("l") && Time.time <= dashTime && !isOnGround)//���³�̼�,���ҳ��CD��, ���ڵ�������, dash��ֵ�������ó�true,����dashingҪ���ó�false,�Է�ֹ���ƽ̨��Ե,����ص����ʱ��,��ɫ���ܹ��ƶ�.
            {
                dash = true;
                dashing = false;
            }

            if (Input.GetKeyDown("l") && Time.time <= dashTime && isOnGround)//���³�̼�,���ҳ��CD��, ����Ȼ�ڵ�����,��ôdash��dashing����ture,���Ҷ�������ֵdashҲҪΪtrue.
            {
                dash = true;                    //�˴��Ĳ���ֵ���ڿ������ҷ�����ƶ���ʱ�򣬺ͳ��֮��Ĺ�ϵ
                dashing = true;                 //�˴��Ĳ���ֵ���ڿ������ҷ�����ƶ���ʱ�򣬺ͳ��֮��Ĺ�ϵ
                animZero.SetBool("dash", true);
                animShadow.SetBool("dash", true);
            }

            if (Time.time > dashTime && isOnGround)         //���CD����,�������(����˵CD ,����˵�ǵ��γ�̵ĳ���ʱ��, ����CD����дһЩ)
            {
                speed = 12;
                dash = false;
                animZero.SetBool("dash", false);
                animShadow.SetBool("dash", false);
            }

            if (Input.GetKeyUp("l") && isOnGround)          //����ڵ����Ϸſ��˳�̼�
            {
                speed = 12;                 //���������ٶ�Ϊ12
                dash = false;
                dashing = false;
                animZero.SetBool("dash", false);
                animShadow.SetBool("dash", false);
            }
        }
       
    }
    
    
    
    
    //----------------------------------�����л��ж�---------------------------------//

    private void UpdateAni02()          //�����ж������Ƿ����ܶ��л����˴���,  ������򲥷���Ч
    {
        curAnimInfo = animZero.GetCurrentAnimatorStateInfo(0);
        if(lastAnimInfo.shortNameHash != curAnimInfo.shortNameHash)
        {
            if (lastAnimInfo.IsName("Zero_run")&& curAnimInfo.IsName("Zero_idle"))         
                RunAudioPlay();


            lastAnimInfo = curAnimInfo;
        }
    }



    //------------------------------------ǽ���ж�-----------------------------------//
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
    //------------------------------------�����ж�-----------------------------------//
    #region
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;
            speed = 12;//��������ж���ʱ��,�ٶ�ҲҪ�л���12
            animZero.SetFloat("running", speed);    //���ó�speed�����������л�������ܲ��Ķ���
            animShadow.SetFloat("running", speed);

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;//վ�ڵ����ϵ�ʱ�� �ٶ��ǿ��Ա仯��
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = false;
            dashing = false;        //ÿ���뿪����,��Ҫ��dashing ���ó�false, ���������ƽ̨��,�ٻص�����,Ҳ���ᷢ��λ����.
        }
    }
    #endregion

    //------------------------------------�����¼�------------------------------------//
    #region
    void SetLayerWeight_RunAttackLayerOff()   //���ö���Ȩ�أ�����Ծ��ʱ���ܶ������Ķ���Ϊoff       //�����Ҫ�õ�
    {
        animZero.SetLayerWeight(2, 0f);
        animZero.SetLayerWeight(3, 0f);
    }
    void SetAttackNum()     //����AttackNum  ���Կ��Ʋ������ι���
    {
        AttackNumber = 0;
        animZero.SetInteger("AttackNum", AttackNumber);
    }

    void SetJumpAttackLayerOff()
    {
        if (isOnGround)     //������Ծ����������Ȩ�أ�����Ծ������������ص�һ˲��ֱ����ʧ��������Ƚ�����
        {
            animZero.SetLayerWeight(4, 0f);
        }
    }
    

    void GetDustSmall()     //����С�ҳ�
    {
        Pool.instance.GetFormPoolDustSmall();
    }

    void SetSpeed()         //�����ƶ��ٶ�
    {
        speed = 12;
    }

    void RunAudioPlay()     //�ܶ�����
    {
        runAudio.Play();
    }
    
    void DashAudioPlay()    //�������
    {
        dashAudio.Play();
    }

    void LightSaberAudioAttack()     //�⽣����1
    {
        lightSaberAudioAttack.Play();
    }

    void LightSaberAudioEffect()     //�⽣����2
    {
        
        lightSaberAudioEffect.Play();
        
    }
    #endregion
}



