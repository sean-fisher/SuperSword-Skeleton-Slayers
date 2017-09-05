using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScreenTransition : MonoBehaviour
{
    public abstract bool TransitionIn(float widthRatio = 0, float speed = 1f);
    public abstract bool TransitionOut(float widthRatio = 0, float speed = 1f);
    public TransitionType tt;
    public bool transitioning = false;
    public bool waitOnBlack = false;
}
public enum TransitionType
{
    FADE,
    FOURSIDE
}
