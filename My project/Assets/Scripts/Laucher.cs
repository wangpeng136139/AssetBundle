using GameAsset;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Laucher : MonoBehaviour
{
    TextMeshProUGUI process = null;
    TextMeshProUGUI state = null;
    // Start is called before the first frame update
    void Start()
    {
        process = gameObject.FindComponent<TextMeshProUGUI>("process");
        state = gameObject.FindComponent<TextMeshProUGUI>("state");
        Updater.GetInstance().Initialization();
        Map.StartUpdate(onState, onProcess, onFinish);
    }
    
    private void onAsset( object obj)
    {
        
    }
    private void onFinish(int code, object obj)
    {
        Bundle.InitializationCallBack();
        Asset.AsyncLoad<GameObject>("Assets/Data/Model/Canvas.prefab", true,onAsset);
    }

    private void onProcess(float process,ulong bandwidth, ulong bytes, ulong length)
    {
        this.process.text = "process:" + process + " bandwidth:" + bandwidth + " bytes:" + bytes + " length:" + length;
    }

    private void onState(UpdateState state)
    {
        this.state.text = state.ToString();
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
