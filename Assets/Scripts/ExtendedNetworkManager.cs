using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ExtendedNetworkManager : NetworkManager
{
    public Text ConnectionMessage;

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);
        ConnectionError(null);
    }

    public void MyStartClient()
    {
        client = NetworkManager.singleton.StartClient();
        client.RegisterHandler(MsgType.Error, ConnectionError);
        client.RegisterHandler(MsgType.Disconnect, ClientDisconnected);
    }
    public void MyStartHost()
    {
        client = NetworkManager.singleton.StartHost();
        if (client == null) ConnectionError(null);// Host creation hailed for some reason
        else
        {
            client.RegisterHandler(MsgType.Error, ConnectionError);
            client.RegisterHandler(MsgType.Disconnect, ClientDisconnected);
        }
    }

    public void ConnectionError(NetworkMessage netMsg)
    {
        ConnectionMessage.text = "Connection failed... You are trying to create a host on a busy IP or a client on a wrong IP.";
    }

    public void ClientDisconnected(NetworkMessage netMsg)
    {
        string NewConsoleText = GameObject.Find("GlobalController").GetComponent<GlobalController>().ConsoleString + "\n<color=red>  You have been disconnected from Host. </color>\n";
        GameObject.Find("GlobalController").GetComponent<GlobalController>().Console.text = NewConsoleText;
    }
}
