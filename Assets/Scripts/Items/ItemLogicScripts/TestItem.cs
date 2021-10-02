using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : MonoBehaviour, Item
{
    public int _MaxStackSize;
    public int MaxStackSize => _MaxStackSize;

    private int _currentCount = 0;
    public int CurrentCount { get => _currentCount; set => _currentCount = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
