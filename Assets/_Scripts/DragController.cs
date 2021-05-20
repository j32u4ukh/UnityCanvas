using UnityEngine;
using UnityEngine.EventSystems;

/* 參考自 https://github.com/dipen-apptrait/Vertical-drag-drop-listview-unity */
public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 被拖曳的物件
    public RectTransform dragged_transform;

    // 所有可拖曳物件的容器
    private GameObject container;

    // 被拖曳的物件的位置
    private Vector3 current_position;

    // 可拖曳物件個數
    private int n_child;

    // 當滑鼠按下時
    public void OnPointerDown(PointerEventData eventData)
    {
        current_position = dragged_transform.position;
        container = dragged_transform.parent.gameObject;
        n_child = container.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 只有 Y 軸方向會被移動位置
        dragged_transform.position =
            new Vector3(dragged_transform.position.x, eventData.position.y, dragged_transform.position.z);

        for (int i = 0; i < n_child; i++)
        {
            // 當物件們共享著同一個父物件(container)，GetSiblingIndex 返回'被拖曳物件'在所有子物件中的索引值
            // 當 i 所指向的物件，並非'被拖曳物件'時
            if (i != dragged_transform.GetSiblingIndex())
            {
                // 取得 i 所指向的 Transform
                Transform other = container.transform.GetChild(i);

                // 計算 '被拖曳物件' 和 'i 所指向的物件' 之間的距離
                int distance = (int)Vector3.Distance(dragged_transform.position, other.position);

                if (distance <= 10)
                {
                    Vector3 other_position = other.position;

                    #region 交換 '被拖曳物件' 和 'i 所指向的物件' 彼此的位置
                    other.position = new Vector3(other.position.x, current_position.y, other.position.z);

                    dragged_transform.position = new Vector3(dragged_transform.position.x, other_position.y,
                        dragged_transform.position.z); 
                    #endregion

                    // 修改 '被拖曳物件' 在所有子物件中的順位
                    dragged_transform.SetSiblingIndex(other.GetSiblingIndex());

                    // 更新 current_position 為新的 '被拖曳物件' 的位置
                    current_position = dragged_transform.position;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 拖動中的 dragged_transform 的 Y 值是連續性的移動，但滑鼠放開時的 Y 值，是離散的，是原本在該處的子物件的位置，
        // 而原本的子物件則依序被被擠開到 dragged_transform 的前一個位置
        dragged_transform.position = current_position;
    }
}