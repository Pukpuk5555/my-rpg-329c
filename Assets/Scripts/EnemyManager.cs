using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> monster;
    public List<Enemy> Monster { get { return monster; } }

    public static EnemyManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Character m in monster)
        {
            m.CharInit(VFXManager.instance, UIManager.instance, InventoryManager.instance);
        }

        InventoryManager.instance.AddItem(monster[0], 0);//Health Potion
        InventoryManager.instance.AddItem(monster[0], 1);
        InventoryManager.instance.AddItem(monster[0], 2);
    }
}
