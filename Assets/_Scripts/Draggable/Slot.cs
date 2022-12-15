﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public GameObject item
    {
        get
        {
            if(transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            else
            {
                return null;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!item)
        {
            DragHandler.start_dragged_obj.transform.SetParent(transform);
            ExecuteEvents.ExecuteHierarchy<IHasChanged>(
                gameObject, 
                null, 
                (x, y) => {
                    x.HasChanged();
                }
            );
        }
    }
}
