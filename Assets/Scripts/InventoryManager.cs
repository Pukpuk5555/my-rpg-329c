using System;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs;
    public GameObject[] ItemPrefabs
    {
        get { return itemPrefabs; }
        set { itemPrefabs = value; }
    }

    [SerializeField] private ItemData[] itemData;
    public ItemData[] ItemDatas
    {
        get { return itemData; }
        set { itemData = value; }
    }

    public const int MAXSLOT = 17;

    public static InventoryManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddItemShopToNpc(1, 0);
        AddItemShopToNpc(1, 3);
        AddItemShopToNpc(1, 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddItem(Character character, int id)
    {
        Item item = new Item(itemData[id]);

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

    public void SaveItemInBag(int index, Item item)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = item;

        switch (index)
        {
            case 16 :
                PartyManager.instance.SelectChars[0].EquipShield(item);
                break;
        }
    }

    public void RemoveItemInBag(int index)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = null;
        
        switch (index)
        {
            case 16 :
                PartyManager.instance.SelectChars[0].UnEquipShield();
                break;
        }
    }

    public bool CheckPartyForItem(int id)
    {
        Item item = new Item(itemData[id]);
        Debug.Log(item.ItemName);

        List<Character> party = PartyManager.instance.Members;

        foreach (Character hero in party)
        {
            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                Debug.Log(hero.InventoryItems[i].ItemName);
                if (hero.InventoryItems[i].ID == item.ID)
                    return true;
            }
        }
        return false;
    }

    private void AddItemShopToNpc(int npcId, int itemId)
    {
        Item item = new Item(itemData[itemId]);
        QuestManager.instance.NpcPerson[npcId].ShopItems.Add(item);
    }
}
