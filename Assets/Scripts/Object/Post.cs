using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post : MonoBehaviour
{
    [SerializeField] GameObject _post;
    [SerializeField] GameObject _post2D;
    private float _length;
    private float _width;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePost(float p_postWidth, float p_postLength, float p_roomWidth, float p_roomLength, float p_thickness, float p_height)
    {
        _length = p_postLength;
        _width = p_postWidth;

        if(_post != null)
        {
            _post.transform.localPosition = new Vector3(-(_length/2f - p_thickness), p_height/2f, -p_thickness/2f);
            _post.transform.localScale = new Vector3(_length, p_height, _width);
        }
        if(_post2D != null)
        {
            _post2D.transform.localPosition = new Vector3(-(_length/2f - p_thickness), 0.0005f, -p_thickness / 2f);
        }

        transform.localPosition = new Vector3(p_roomWidth / 2f, 0, p_roomLength / 2f);
        transform.localRotation = Quaternion.Euler(0, -90, 0);
    }

    public float Width => _width;
    public float Length => _length;
}
