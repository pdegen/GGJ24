using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class JumpingEgg : Egg
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void InitEggMovement()
        {
            transform.DOMoveY(transform.position.y + 6f, 4f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);
        }
    }
}