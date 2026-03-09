using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPart : MonoBehaviour
{
    [SerializeField] protected GameObject _animatedObject;
    [SerializeField] protected bool _animatedObjectZUp = false;
    [SerializeField] protected float _animationDuration = 5f;
    [SerializeField] protected bool _animationInverted = false;
    [SerializeField] protected float _elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void StartStopAnimation()
    {

    }

    public Vector3 GetAnimatedObjectDimensions()
    {
        Vector3 l_result = _animatedObject.transform.localScale;
        Vector3 l_bbsize = _animatedObject.GetComponent<BoxCollider>().size;

        l_result.x *= l_bbsize.x;
        l_result.y *= l_bbsize.y;
        l_result.z *= l_bbsize.z;

        if (_animatedObjectZUp)
            l_result = new Vector3(l_result.x, l_result.z, l_result.y);

        return l_result;
    }
}
