using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    public event Action onAnimTrigger = delegate { };

    public void _AnimTrigger()
    {
        onAnimTrigger();
    }
}
