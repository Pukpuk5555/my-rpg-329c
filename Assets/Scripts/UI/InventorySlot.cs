using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0)
            return;

        GameObject objDrop = eventData.pointerDrag;
        ItemDrag item = objDrop.GetComponent<ItemDrag>();
        item.IconParent = transform;
    }
}
