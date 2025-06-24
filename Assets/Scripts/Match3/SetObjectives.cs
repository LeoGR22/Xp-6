using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjectives : MonoBehaviour
{
   public ObjectiveBoardData redObjective;
   public ObjectiveBoardData orangeObjective;
   public ObjectiveBoardData greenObjective;
   public ObjectiveBoardData violetObjective;
   public ObjectiveBoardData blueObjective;

    public void SetRed(int count)
   {
        redObjective.count = count;
   }
   public void SetOrange(int count)
   {
        orangeObjective.count = count;
   }
    public void SetGreen(int count)
    {
        greenObjective.count = count;
    }
    public void SetViolet(int count)
    {
        violetObjective.count = count;
    }
    public void SetBlue(int count)
    {
        blueObjective.count = count;
    }
}
