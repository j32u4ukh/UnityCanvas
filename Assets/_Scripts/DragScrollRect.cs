using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragScrollRect : MonoBehaviour
{
    public GameObject container_obj;
    GridLayoutGroup glg;
    public GameObject prefab_obj;

    [SerializeField] private float cell_x = 180f;
    [SerializeField] private float cell_y = 50f;
    [SerializeField] private float spacing_x = 0f;
    [SerializeField] private float spacing_y = 5f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        glg = container_obj.GetComponent<GridLayoutGroup>();
        glg.cellSize = new Vector2(cell_x, cell_y);
        glg.spacing = new Vector2(spacing_x, spacing_y);

        for (int i = 0; i < 20; i++)
        {
            addItem($"Button #{i}");
        }

    }

    // 新增 ListView 元素
    public void addItem(string name)
    {
        GameObject temp = Instantiate(prefab_obj, container_obj.transform, false);
        temp.name = name;

        modifyContainerSize();

        // TODO: return GameObject for more setting
    }

    public void modifyContainerSize()
    {
        int n_child = container_obj.transform.childCount;
        RectTransform rect_transform = container_obj.GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(rect_transform.rect.width, (cell_y + spacing_y) * n_child);
    }

    public void setCellSize(float x = -1f, float y = -1f)
    {
        if(x != -1f)
        {
            cell_x = x;
        }

        if(y != -1f)
        {
            cell_y = y;
        }

        glg.cellSize = new Vector2(cell_x, cell_y);
    }

    public Tuple<float, float> getCellSize()
    {
        return new Tuple<float, float>(cell_x, cell_y);
    }

    public void setSpacingSize(float x = -1f, float y = -1f)
    {
        if (x != -1f)
        {
            spacing_x = x;
        }

        if (y != -1f)
        {
            spacing_y = y;
        }

        glg.spacing = new Vector2(spacing_x, spacing_y);
    }

    public Tuple<float, float> getSpacingSize()
    {
        return new Tuple<float, float>(spacing_x, spacing_y);
    }
}
