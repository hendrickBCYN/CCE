using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Select(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select(int p_index)
    {
        if(p_index >= 0 && p_index < transform.childCount)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == p_index);
            }
        }
    }
}
