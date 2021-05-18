using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject start_dragged_obj;
    Vector3 start_position;

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_dragged_obj = gameObject;
        start_position = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        start_dragged_obj = null;
        transform.position = start_position;
    }
}
