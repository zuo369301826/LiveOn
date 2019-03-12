using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginWnd : UIBaseWnd {

    LoginWndPrefab m_prefab;

    GameObject btn_land;
    GameObject btn_register;

    protected override void OnOpen()
    {
        m_prefab = root.GetComponent<LoginWndPrefab>();
        btn_land = m_prefab.btn_land;
        btn_register = m_prefab.btn_register;
    }

    protected override void AddEventListener()
    {
        if (btn_land != null)
        {
            UIButton land = btn_land.GetComponent<UIButton>();
            if (land != null)
            {
                EventDelegate.Add(land.onClick, delegate () { this.OnClick(land.name); });
            }
        }
        if (btn_register != null)
        {
            UIButton register = btn_register.GetComponent<UIButton>();
            if (register != null)
            {
                EventDelegate.Add(register.onClick, delegate () { this.OnClick(register.name); });
            }
        }
    }

    void Start()
    {

    }

    public override void Update()
    {

    }

    public override void RefreshUI()
    {
   
    }

    void OnClick(string name)
    {
        switch (name)
        {
            case "btn_land":
                Debug.Log("land");
                break;
            case "btn_register":
                Debug.Log("register");
                break;
        }
    }
}
