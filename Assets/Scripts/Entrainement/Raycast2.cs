using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast2 : MonoBehaviour
{
    void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.up * 10, Color.red);

        if(Physics.Raycast(transform.position, transform.up, out hit, 10))
        {
            Debug.Log("Le raycast touche un objet !");
        }
    }

}

