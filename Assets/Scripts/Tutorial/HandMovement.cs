using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    public float amplitude = 0.5f; 
    public float speed = 1f; 

    void Update()
    {
        Vector3 currentLocalPosition = transform.localPosition;

        float newY = currentLocalPosition.y + Mathf.Sin(Time.time * speed) * amplitude;

        transform.localPosition = new Vector3(currentLocalPosition.x, newY, currentLocalPosition.z);
    }
}