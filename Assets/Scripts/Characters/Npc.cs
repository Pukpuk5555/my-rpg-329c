using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class Npc : Character
{
    [SerializeField] private List<Quest> questToGive = new List<Quest>();
    public List<Quest> QuestToGive { get { return questToGive; } set { questToGive = value; } }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Quest CheckQuestList(QuestStatus status)
    {
        foreach (Quest quest in questToGive)
        {
            if (quest.Status == status)
                return quest;
        }

        return null;
    }
}
