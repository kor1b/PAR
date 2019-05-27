using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PetrifiedEffectBehaviour : MonoBehaviour
    {
        public Animator PetrifiedAnimator;
        public AudioSource PetrifiedSound;
        public void PetrifiedFadeIn()
        {
            PetrifiedAnimator.SetTrigger("StartPetrifiedFade");
           // PetrifiedSound.Play();
        }
        public void PetrifiedFadeOut()
        {
            PetrifiedAnimator.SetTrigger("EndPetrifiedFade");
        }
    }
}