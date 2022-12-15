using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
    public interface IHasChanged : IEventSystemHandler
    {
        void HasChanged();
    }
}

public class Inventory : MonoBehaviour, IHasChanged
{
    [SerializeField] Transform slots; 
    public Text inventory_text;

    // Start is called before the first frame update
    void Start()
    {
        HasChanged();
    }

    public void HasChanged()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(" - ");

        foreach(Transform slot in slots)
        {
            GameObject item = slot.GetComponent<Slot>().item;

            if (item)
            {
                sb.Append($"{item.name} - ");
            }
        }

        inventory_text.text = sb.ToString();
    }
}
