using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
   public Animator anim;
   public bool canSelected;
   public bool isDead;

   private void Start() 
   {
        anim = GetComponent<Animator>();
   }

    void Update()
    {
        if (canSelected == true)
        {   
            anim.Play("Slot2CanSelected");
        }
    }

}
