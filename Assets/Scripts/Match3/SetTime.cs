using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTime : MonoBehaviour
{ 
    public FloatSO timeData;

    public void SetterTime(float time)
    {
        timeData.value = time;
    }
}
