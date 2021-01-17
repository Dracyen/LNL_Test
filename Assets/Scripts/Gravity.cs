using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class Gravity : MonoBehaviour
{
    float gravity;
    Vector3 newPosition;

    public bool gone = false;

    void Start()
    {
        gravity = 5;
    }

    private void Update()
    {
        if(transform.position.y <= -10)
        {
            gone = true;
        }
    }

    public void PosUpdate()
    {
        newPosition.y -= gravity * Time.deltaTime;

        gameObject.transform.position = newPosition;
    }
}
