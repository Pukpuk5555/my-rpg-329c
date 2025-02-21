using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    public RectTransform SelectionBox { get { return selectionBox; } }

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }
}
