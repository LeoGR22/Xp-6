using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InvokeAfter : MonoBehaviour
{
    [SerializeField] private UnityEvent action;
    [SerializeField] private UnityEvent subAction;

    public Action onActionCall;
    public Action onSubActionCall;

    public void CallAction()
    {
        action.Invoke();
        if (onActionCall != null && gameObject.activeSelf)
        {
            onActionCall.Invoke();
        }
    }

    public void CallSubAction()
    {
        subAction.Invoke();
        if (onSubActionCall != null && gameObject.activeSelf)
        {
            onSubActionCall.Invoke();
        }
    }
}
