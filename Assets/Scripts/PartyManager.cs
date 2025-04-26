using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }

    [SerializeField]
    private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }

    public static PartyManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Character c in members)
        {
            c.charInit(VFXManager.instance, UIManager.instance);
        }

        SelectSingleHero(0);

        members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[0]));
        members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[1]));

        InventoryManager.instance.AddItem(members[0], 0); //Health Potion
        InventoryManager.instance.AddItem(members[0], 1); //Sword
        
        InventoryManager.instance.AddItem(members[1], 0); //Health Potion
        InventoryManager.instance.AddItem(members[1], 1); //Sword
        InventoryManager.instance.AddItem(members[1], 2); //Shield A
        InventoryManager.instance.AddItem(members[1], 3); //Shield B

        UIManager.instance.ShowMagicToggles();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(selectChars.Count > 0)
            {
                selectChars[0].IsMagicMode = true;
                selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
            }
        }
    }

    public void SelectSingleHero(int i)
    {
        foreach (Character c in selectChars)
            c.ToggleSelection(false);

        selectChars.Clear();

        selectChars.Add(members[i]);
        selectChars[0].ToggleSelection(true);
    }

    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0)
            return;

        selectChars[0].IsMagicMode = true;
        selectChars[0].CurMagicCast = selectChars[0].MagicSkills[i];
    }

    public int FindIndexFromClass(Character hero)
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] == hero)
            {
                return 1;
            }
        }

        return 0;
    }

    public void SelectSingleHeroByToggle(int i)
    {
        Debug.Log(($"Select {i}"));

        if (selectChars.Contains(members[i]))
        {
            members[i].ToggleSelection(true);
            UIManager.instance.ShowMagicToggles();
        }
        else
        {
            selectChars.Add(members[i]);
            members[i].ToggleSelection(true);
            UIManager.instance.ShowMagicToggles();
        }
    }

    public void UnSelectSingleHeroToggle(int i)
    {
        if (selectChars.Count <= 1)
        {
            UIManager.instance.ToggleAvatar[i].isOn = true;
            return;
        }

        if (selectChars.Contains(members[i]))
        {
            selectChars.Remove(members[i]);
            members[i].ToggleSelection(false);
        }
    }
}
