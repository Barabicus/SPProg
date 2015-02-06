using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TabGUI : UIBehaviour
{

    public Text tabText;

    public string Text
    {
        get { return tabText.text; }
        set { tabText.text = value; }
    }

    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    [Serializable]
    public class TestEvent : UnityEvent { }


    // Event delegates triggered on click.
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
    
    [SerializeField]
    private TestEvent m_tedd = new TestEvent();



}
