using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToDecoration : MonoBehaviour
{
   public bool isSelected;
   
   
    private void Update() 
    {
        if (isSelected == true)
        {
            Active();
        }
    }

    private void OnMouseDown() 
    {
        isSelected = true;
    }

    private void Active()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        transform.position = mousePosition;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Slot"))
        {
            Debug.Log("Encostou");
            isSelected = false;
            gameObject.transform.position = other.transform.position;
            Destroy(other.gameObject);
        }
    }

}
