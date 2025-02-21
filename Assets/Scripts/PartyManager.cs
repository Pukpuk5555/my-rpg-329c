using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars = new List<Character>();

    public static PartyManager instance;

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
}
