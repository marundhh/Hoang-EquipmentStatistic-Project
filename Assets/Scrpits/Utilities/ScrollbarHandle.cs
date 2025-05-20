using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarHandle : MonoBehaviour
{
    [Header("Scrollbar Handle")]
    [SerializeField] protected Scrollbar scrollbar;
    [SerializeField] protected float scrollbarSize = 0f;

    protected void Start()
    {
        Invoke("ChangeScrollbarSize", 0.01f);
    }

    public void ChangeScrollbarSize()
    {
        scrollbar.size = scrollbarSize;
    }
}