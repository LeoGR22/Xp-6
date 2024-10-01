using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeocorationManager : MonoBehaviour
{
    public ObjectToDecoration object1;
    public Slot slot1;

    void Update()
    {
        SetPosition();

        if(slot1.isDead)
        {
            object1.isSelected = false;
        }
    }

    void SetPosition()
    {
        if (object1.isSelected == true)
        {
            slot1.canSelected = true;
        }
    }

}
