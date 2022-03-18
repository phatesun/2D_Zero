using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaberController : MonoBehaviour
{
    public Animator animZero;
    public Animator animShadow;

    public AudioSource lightSaberAudioAttack;    //光剑声音1号
    public AudioSource lightSaberAudioEffect;    //光剑声音2号

    //-------------------动画事件-------------------
    void SetLayerWeight_RunAttackLayer1()
    {
        animZero.SetLayerWeight(2, 1f);             //本体动画权重
        //animShadow.SetLayerWeight(2, 1f);           //残影动画权重
    }

    void SetLayerWeight_RunAttackLayer2()
    {
        animZero.SetLayerWeight(3, 1f);             //本体动画权重
        //animShadow.SetLayerWeight(3, 1f);           //残影动画权重
    }

    void SetLayerWeight_RunAttackLayerOff()
    {
        animZero.SetLayerWeight(2, 0f);             //本体动画权重
        animZero.SetLayerWeight(3, 0f);             //本体动画权重
        //animShadow.SetLayerWeight(2, 0f);           //残影动画权重
        //animShadow.SetLayerWeight(3, 0f);           //残影动画权重
    }

    void SetLayerWeight_JumpAttackLayerOn()
    {
        animZero.SetLayerWeight(4, 1f);             //本体动画权重
        animShadow.SetLayerWeight(4, 1f);           //残影动画权重
    }

    void SetLayerWeight_JumpAttackLayerOff()
    {
        animZero.SetLayerWeight(4, 0f);             //本体动画权重
        animShadow.SetLayerWeight(4, 0f);           //残影动画权重
    }

    void LightSaberAudioAttack()     //光剑声音1
    {
        lightSaberAudioAttack.Play();
    }

    void LightSaberAudioEffect()     //光剑声音2
    {

        lightSaberAudioEffect.Play();

    }

}
