using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftClick : MonoBehaviour
{
    public static LeftClick instance;

    private Camera cam;

    [SerializeField] private LayerMask layerMask;

    [SerializeField]
    private RectTransform boxSelection;
    private Vector2 oldAnchoredPos; //old Anchored position
    private Vector2 startPos; //pos where mouse is down

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");

        boxSelection = UIManager.instance.SelectionBox;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;

            //if click UI, don't clear
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            ClearEverything();
        }

        //mouse hold down
        if(Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }

        //mouse up
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox(Input.mousePosition);
            TrySelect(Input.mousePosition);
        }
    }

    private int SelectCharacter(RaycastHit hit)
    {
        ClearEverything();
        
        Character hero = hit.collider.GetComponent<Character>();
        Debug.Log("Select Char: " + hit.collider.gameObject);

        int i = PartyManager.instance.FindIndexFromClass(hero);
        Debug.Log($"Click Release: {i}");
        UIManager.instance.ToggleAvatar[i].isOn = true;
        return i;
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        int i = 0;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    i = SelectCharacter(hit);
                    break;
            }
        }

        if (PartyManager.instance.SelectChars.Count == 0)
            UIManager.instance.ToggleAvatar[i].isOn = true;
    }

    private void ClearRingSelection()
    {
        foreach (Character h in PartyManager.instance.SelectChars)
            h.ToggleSelection(false);
    }

    private void ClearEverything()
    {
        foreach (Toggle t in UIManager.instance.ToggleAvatar)
        {
            t.isOn = false;
        }
        
        ClearRingSelection();
        PartyManager.instance.SelectChars.Clear();
    }

    private void UpdateSelectionBox(Vector2 mousePos)
    {
        //set active to selection box if it isn't active
        Debug.Log("Mouse Pos - " + mousePos);
        if (!boxSelection.gameObject.activeInHierarchy)
            boxSelection.gameObject.SetActive(true);

        //find wigth and height box value
        float width = mousePos.x - startPos.x;
        float height = mousePos.y - startPos.y;

        //find mid point of box
        boxSelection.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

        //turn width n height to ansolute value
        width = Mathf.Abs(width);
        height = Mathf.Abs(height);

        //check width and height
        boxSelection.sizeDelta = new Vector2(width, height);

        //store old position for real unit selection
        oldAnchoredPos = boxSelection.anchoredPosition;
    }

    private void ReleaseSelectionBox(Vector2 mousePos)
    {
        Vector2 corner1; //down-left corner
        Vector2 corner2; //tor-right corner

        boxSelection.gameObject.SetActive(false);

        //check both corner value
        corner1 = oldAnchoredPos - (boxSelection.sizeDelta / 2);
        corner2 = oldAnchoredPos + (boxSelection.sizeDelta / 2);

        //loop every chars in party
        foreach (Character member in PartyManager.instance.Members)
        {
            //transform stand pos from Vector3 to Vector2
            Vector2 unitPos = cam.WorldToScreenPoint(member.transform.position);

            //check if unit is stay in box, yes-> show selection ring
            if((unitPos.x > corner1.x && unitPos.x < corner2.x)
                && (unitPos.y > corner1.y && unitPos.y < corner2.y))
            {
                int i = PartyManager.instance.FindIndexFromClass(member);
                Debug.Log($"Drag: {i}");
                UIManager.instance.ToggleAvatar[i].isOn = true;
            }
        }
        //clear selection Box's size
        boxSelection.sizeDelta = new Vector2(0, 0);
    }
}
