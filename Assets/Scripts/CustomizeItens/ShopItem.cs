using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] public bool isSelected;
    [SerializeField] private bool inInventory;
    [SerializeField] private bool inSlot;
    [SerializeField] private bool isCollidingWithSlot;
    [SerializeField] private Vector2 initialPosition;

    [SerializeField] private GameObject slotRef;

    void Start()
    {
        isSelected = false;
        inInventory = true;
        inSlot = false;
        initialPosition = transform.position;

        if (slotRef == null)
        {
            Debug.LogError("slotRef not assigned in the Inspector!");
        }
    }

    void Update()
    {
        HandleInput();
        followMouse();
        putInSlot();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isSelected = !isSelected; // Alternar seleção
            }
        }

        if (Input.GetMouseButtonUp(0) && isSelected)
        {
            if (!isCollidingWithSlot)
            {
                returnToOriginPosition();
            }
        }
    }

    void followMouse()
    {
        if (isSelected)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            transform.position = mousePos;

            inInventory = false;
        }
    }

    void putInSlot()
    {
        if (isSelected && isCollidingWithSlot)
        {
            transform.position = slotRef.transform.position;
            inSlot = true;
            isSelected = false;
            Destroy(slotRef.gameObject);
        }
    }

    void returnToOriginPosition()
    {
        transform.position = initialPosition;
        isSelected = false;
        inInventory = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == slotRef)
        {
            isCollidingWithSlot = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == slotRef)
        {
            isCollidingWithSlot = false;
        }
    }
}