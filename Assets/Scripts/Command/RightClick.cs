using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RightClick : MonoBehaviour
{
    public static RightClick instance;

    private Camera cam;

    [SerializeField] private LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            TryCommand(Input.mousePosition);
        }
    }

    private void CommandToWalk(RaycastHit hit, List<Character> heroes)
    {
        foreach (Character h in heroes)
        {
            if (h != null)
                h.WalkToPosition(hit.point);
        }
        
        CreateVFX(hit.point, VFXManager.instance.DoubleRingMarker);
    }

    private void TryCommand(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                    CommandToWalk(hit, PartyManager.instance.SelectChars);
                    break;
                case "Enemy":
                    CommandToAttack(hit, PartyManager.instance.SelectChars);
                    break;
                case "NPC":
                    CommandTalkToNPC(hit, PartyManager.instance.SelectChars);
                    break;
            }
        }
    }

    private void CreateVFX(Vector3 pos, GameObject vfxPrefab)
    {
        if (vfxPrefab == null)
            return;

        Instantiate(vfxPrefab, pos + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
    }

    private void CommandToAttack(RaycastHit hit, List<Character> heroes)
    {
        Character target = hit.collider.GetComponent<Character>();
        Debug.Log("Attack" + target);
        
        foreach (Character h in heroes)
        {
            h.ToAttackCharacter(target);
        }
    }

    private void CommandTalkToNPC(RaycastHit hit, List<Character> heros)
    {
        Character npc = hit.collider.GetComponent<Character>();
        Debug.Log("Talk to NPC: " + npc);

        if (heros.Count <= 0)
            return;
        
        heros[0].ToTalkToNPC(npc);
    }
}
