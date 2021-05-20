using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 被拖曳的物件
    public RectTransform item;

    // 所有可拖曳物件的容器
    private GameObject container;

    // 被拖曳的物件的位置
    private Vector3 pos;

    // 可拖曳物件個數
    private int n_child;

    private float threshold_distance = 10f;

    // 當滑鼠按下時
    public void OnPointerDown(PointerEventData eventData)
    {
        pos = item.position;

        // 所有可拖曳物件的容器('被拖曳的物件' 的父物件)
        container = item.parent.gameObject;

        n_child = container.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 只有 Y 軸方向會被移動位置
        item.position = new Vector3(item.position.x, eventData.position.y, item.position.z);

        for (int i = 0; i < n_child; i++)
        {
            // 當物件們共享著同一個父物件(container)，GetSiblingIndex 返回'被拖曳物件'在所有子物件中的索引值
            // 當 i 所指向的物件，並非'被拖曳物件'時
            if (i != item.GetSiblingIndex())
            {
                // 取得 i 所指向的 Transform
                Transform other = container.transform.GetChild(i);

                // 計算 '被拖曳物件' 和 'i 所指向的物件' 之間的距離
                float distance = Vector3.Distance(item.position, other.position);

                if (distance <= threshold_distance)
                {
                    #region 交換 '被拖曳物件' 和 'i 所指向的物件' 彼此的位置
                    Vector3 other_position = other.position;

                    other.position = new Vector3(other.position.x, pos.y, other.position.z);

                    item.position = new Vector3(item.position.x, other_position.y, item.position.z);
                    #endregion

                    // 修改 '被拖曳物件' 在所有子物件中的順位
                    item.SetSiblingIndex(other.GetSiblingIndex());

                    // 更新 current_position 為新的 '被拖曳物件' 的位置
                    pos = item.position;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 拖動中的 dragged_transform 的 Y 值是連續性的移動，但滑鼠放開時的 Y 值，是離散的，是原本在該處的子物件的位置，
        // 而原本的子物件則依序被被擠開到 dragged_transform 的前一個位置
        item.position = pos;
    }

    public void setThresholdDistance(float threshold)
    {
        threshold_distance = threshold;
    }
}
