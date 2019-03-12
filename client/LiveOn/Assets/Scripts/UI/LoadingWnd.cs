using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWnd : UIBaseWnd {

    private LoadingPrefab m_prefab;

    private GameObject text;

    private GameObject scrollbar;

    private float value;

    private const float scrollbarMax = 740f;

    public override void RefreshUI()
    {

    }

    protected override void AddEventListener()
    {

    }

    protected override void OnOpen()
    {
        m_prefab = root.GetComponent<LoadingPrefab>();
        if(m_prefab != null)
        {
            scrollbar = m_prefab.srollbar;
            text = m_prefab.text;
        }
        
    }

    public void SetProcessPercent(float value)
    {
        this.value = value;
        if(text != null)
        {
            UILabel label = text.GetComponent<UILabel>();
            if(label != null)
            {
                label.text = value.ToString();
            }
        }
        if(scrollbar != null)
        {
            UIWidget widget = scrollbar.GetComponent<UIWidget>();
            if(widget != null)
            {
                widget.width = (int)(scrollbarMax * value);
            }
        }
    }

    public float GetProcessPercent()
    {
        return value;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	public override void Update () {
		
	}
}
