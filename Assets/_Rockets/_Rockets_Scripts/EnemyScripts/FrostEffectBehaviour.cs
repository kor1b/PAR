using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class FrostEffectBehaviour : MonoBehaviour
    {
        public Animator FrostAnimator;
        public AudioSource FrostSound;
        public void FrostFadeIn()
        {
            FrostAnimator.SetTrigger("StartFrostEffect");
            FrostSound.Play();
        }
        public void FrostFadeOut()
        {
            FrostAnimator.SetTrigger("EndFrostEffect");
        }

    }
}