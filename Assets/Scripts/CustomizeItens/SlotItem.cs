using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItem : MonoBehaviour
{
    [SerializeField] private List<GameObject> shopItens;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        foreach (GameObject obj in shopItens)
        {
            if (obj != null)
            {
                ShopItem shopComponent = obj.GetComponent<ShopItem>();
                if (shopComponent != null)
                {
                    
                    if (shopComponent.isSelected) 
                    {
                        animator.Play("CanPut");
                    }
                }
            }
        }
    }

}
