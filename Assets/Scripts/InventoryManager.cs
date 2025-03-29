using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs;
    public GameObject[] ItemPrefabs
    {
        get { return itemPrefabs; }
        set { itemPrefabs = value; }
    }

    [SerializeField] private ItemData[] itemDatas;
    public ItemData[] ItemDatas
    {
        get { return itemDatas; }
        set { itemDatas = value; }
    }

    public const int MAXSLOT = 16;

    public static InventoryManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddItem(Character character, int id)
    {
        Item item = new Item(itemDatas[id]);

        for (int i = 0; i < character.InventoryItems.Length; i++)
        {
            if(character.InventoryItems[i] == null)
            {
                character.InventoryItems[i] = item;
                return true;
            }
        }
        Debug.Log("Inventory Full");
        return false;
    }
}
