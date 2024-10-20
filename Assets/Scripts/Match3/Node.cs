using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isUsable;
    public GameObject potion;

    public Node(bool usable, GameObject potion)
    {
        isUsable = usable;
        this.potion = potion;
    }
}
