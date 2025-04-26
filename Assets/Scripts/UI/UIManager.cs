using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    public RectTransform SelectionBox { get { return selectionBox; } }

    [SerializeField] private Toggle togglePauseUnpause;

    [SerializeField]
    private Toggle[] toggleMagic;
    public Toggle[] ToggleMagic { get { return toggleMagic; } }

    [SerializeField]
    private int curToggleMagicID = -1;

    [SerializeField] private GameObject blackImage;

    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private GameObject itemUIPrefabs;

    [SerializeField] private GameObject[] slots;

    [SerializeField] private GameObject downPanel;

    [SerializeField] private GameObject npcDialoguePanel;

    [SerializeField] private Image npcImage;

    [SerializeField] private TMP_Text npcNameText;

    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private int index; //dialogue step

    [SerializeField] private GameObject btnNext;

    [SerializeField] private TMP_Text btnNextText;

    [SerializeField] private GameObject btnAccept;

    [SerializeField] private TMP_Text btnAcceptText;

    [SerializeField] private GameObject btnReject;

    [SerializeField] private TMP_Text btnRejectText;

    [SerializeField] private GameObject btnFinish;

    [SerializeField] private TMP_Text btnFinishText;

    [SerializeField] private GameObject btnNotFinish;

    [SerializeField] private TMP_Text btnNotFinishText;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            togglePauseUnpause.isOn = !togglePauseUnpause.isOn;

        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventoryPanel();
    }

    public void ToogleAI(bool isOn)
    {
        foreach (Character member in PartyManager.instance.Members)
        {
            AttackAI ai = member.gameObject.GetComponent<AttackAI>();

            if (ai != null)
                ai.enabled = isOn;
        }
    }

    public void SelectAll()
    {
        foreach (Character member in PartyManager.instance.Members)
        {
            if (member.CurHP > 0)
            {
                member.ToggleSelection(true);
                PartyManager.instance.SelectChars.Add(member);
            }
        }
    }

    public void PauseUnpause(bool isOn)
    {
        Time.timeScale = isOn ? 0 : 1;
    }

    public void ShowMagicToggles()
    {
        if (PartyManager.instance.SelectChars.Count <= 0)
            return;

        //Show Magic skill only the single selected hero
        Character hero = PartyManager.instance.SelectChars[0];

        for(int i = 0; i < hero.MagicSkills.Count; i++)
        {
            toggleMagic[i].interactable = true;
            toggleMagic[i].isOn = false;
            toggleMagic[i].GetComponentInChildren<Text>().text = hero.MagicSkills[i].Name;
            toggleMagic[i].targetGraphic.GetComponent<Image>().sprite = hero.MagicSkills[i].Icon;
        }
    }

    public void SelectMagicSkill(int i)
    {
        curToggleMagicID = i;
        PartyManager.instance.HeroSelectMagicSkill(i);
    }

    public void IsOnCurToggleMagic(bool flag)
    {
        toggleMagic[curToggleMagicID].isOn = flag;
    }

    public void ToggleInventoryPanel()
    {
        if (!inventoryPanel.activeInHierarchy)
        {
            inventoryPanel.SetActive(true);
            blackImage.SetActive(true);
            ShowInventory();
        }
        else
        {
            inventoryPanel.SetActive(false);
            blackImage.SetActive(false);
            ClearInventory();
        }
    }

    public void ShowInventory()
    {
        if(PartyManager.instance.SelectChars.Count <= 0)
            return;
        
        //show inventory only the single selected hero
        Character hero = PartyManager.instance.SelectChars[0];
        
        //show items
        for (int i = 0; i < hero.InventoryItems.Length; i++)
        {
            GameObject itemObj = Instantiate(itemUIPrefabs, slots[i].transform);
            itemObj.GetComponent<Image>().sprite = hero.InventoryItems[i].Icon;
        }
    }

    public void ClearInventory()
    {
        //clear slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Transform child = slots[i].transform.GetChild(0);
                Destroy(child.gameObject);
            }
        }
    }
}
