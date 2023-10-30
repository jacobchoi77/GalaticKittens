using UnityEngine;
using System;

public enum ButtonActions : byte{
    LobbyReady,
    LobbyNotReady
}

/*
    This class works for UI buttons that needs to be press by specific client instance
*/

public class OnButtonPress : MonoBehaviour{
    public static Action<ButtonActions> a_OnButtonPress;
    [SerializeField] private ButtonActions _buttonAction;

    public void OnPress(){
        a_OnButtonPress?.Invoke(_buttonAction);
    }
}