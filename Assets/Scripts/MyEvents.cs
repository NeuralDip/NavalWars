using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MyEvents : MonoBehaviour
{
    private ExtendedNetworkManager MyNetworkManager;
    private GlobalController MyGlobalController;

    public void Start()
    {
        MyNetworkManager = GameObject.Find("NetworkManager").GetComponent<ExtendedNetworkManager>();
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
    }
    public void InputGiven()
    {
        if (!MyGlobalController.WelcomeHostToggle.isOn)// You are a client
        {
            if (String.Compare(MyGlobalController.WelcomeInputName.text, "") == 0 || String.Compare(MyGlobalController.WelcomeInputHost.text, "") == 0) return;
            MyNetworkManager.networkAddress = MyGlobalController.WelcomeInputHost.text;
            MyNetworkManager.MyStartClient();
        }
        else
        {
            if (String.Compare(MyGlobalController.WelcomeInputName.text, "") == 0) return;
            MyNetworkManager.MyStartHost();
        }
        MyGlobalController.WelcomeMessage.text = "Connecting...";
    }
    public void HostTicked()
    {
        if (MyGlobalController.WelcomeHostToggle.isOn) MyGlobalController.WelcomeInputHost.transform.parent.GetComponent<InputField>().interactable = false;
        else MyGlobalController.WelcomeInputHost.transform.parent.GetComponent<InputField>().interactable = true;
    }
    public void SentMessage()
    {
        MyGlobalController.ThePlayerObject.GetComponent<NetworkController>().MessageFromClient(MyGlobalController.MyPlayerName, MyGlobalController.ConsoleInputText.text);
    }
    public void ReconnectAfterGameover()
    {

    }
    public void CloseAfterGameover()
    {
        Application.Quit();
    }
}
