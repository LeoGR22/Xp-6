using System.Collections;
using UnityEngine;

[System.Serializable]

public class ArrayLayout
{
    [System.Serializable]
     
     public struct rowData
     {
        public bool[] row;
     }

    public rowData[] rows = new rowData[8]; 
 } 
