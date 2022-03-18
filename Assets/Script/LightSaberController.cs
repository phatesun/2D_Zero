using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaberController : MonoBehaviour
{
    public Animator animZero;
    public Animator animShadow;

    public AudioSource lightSaberAudioAttack;    //�⽣����1��
    public AudioSource lightSaberAudioEffect;    //�⽣����2��

    //-------------------�����¼�-------------------
    void SetLayerWeight_RunAttackLayer1()
    {
        animZero.SetLayerWeight(2, 1f);             //���嶯��Ȩ��
        //animShadow.SetLayerWeight(2, 1f);           //��Ӱ����Ȩ��
    }

    void SetLayerWeight_RunAttackLayer2()
    {
        animZero.SetLayerWeight(3, 1f);             //���嶯��Ȩ��
        //animShadow.SetLayerWeight(3, 1f);           //��Ӱ����Ȩ��
    }

    void SetLayerWeight_RunAttackLayerOff()
    {
        animZero.SetLayerWeight(2, 0f);             //���嶯��Ȩ��
        animZero.SetLayerWeight(3, 0f);             //���嶯��Ȩ��
        //animShadow.SetLayerWeight(2, 0f);           //��Ӱ����Ȩ��
        //animShadow.SetLayerWeight(3, 0f);           //��Ӱ����Ȩ��
    }

    void SetLayerWeight_JumpAttackLayerOn()
    {
        animZero.SetLayerWeight(4, 1f);             //���嶯��Ȩ��
        animShadow.SetLayerWeight(4, 1f);           //��Ӱ����Ȩ��
    }

    void SetLayerWeight_JumpAttackLayerOff()
    {
        animZero.SetLayerWeight(4, 0f);             //���嶯��Ȩ��
        animShadow.SetLayerWeight(4, 0f);           //��Ӱ����Ȩ��
    }

    void LightSaberAudioAttack()     //�⽣����1
    {
        lightSaberAudioAttack.Play();
    }

    void LightSaberAudioEffect()     //�⽣����2
    {

        lightSaberAudioEffect.Play();

    }

}
