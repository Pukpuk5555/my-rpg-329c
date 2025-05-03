using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInShop : MonoBehaviour
{
    [SerializeField] private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    [SerializeField] private Item item;
    public Item Item
    {
        get { return item; }
        set { item = value; }
    }

    [SerializeField] private Toggle iconToggle;
    public Toggle IconToggle
    {
        get { return iconToggle; }
    }

    [SerializeField] private TMP_Text itemText;

    [SerializeField] private TMP_Text priceText;

    [SerializeField] private UIManager uiMgr;

    public void SetupItemInShop(UIManager uiManager, float discount)
    {
        uiMgr = uiManager;
        iconToggle.targetGraphic.GetComponent<Image>().sprite = Item.Icon;
        itemText.text = item.ItemName;
        priceText.text = ((int)(item.NormalPrice * discount)).ToString();
    }
}
