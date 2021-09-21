using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject start_dragged_obj;
    Transform start_parent;
    Vector3 start_position;

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_dragged_obj = gameObject;
        start_parent = transform.parent;
        start_position = transform.position;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        start_dragged_obj = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (transform.parent == start_parent)
        {
            transform.position = start_position;
        }
    }
}
