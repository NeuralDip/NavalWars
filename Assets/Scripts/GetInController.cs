using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GetInController : NetworkBehaviour
{
    private ExtendedNetworkManager MyNetworkManager;
    private GlobalController MyGlobalController;
    private NetworkController MyNetworkController;

    public override void OnStartClient()
    {
        if (!isLocalPlayer) return;
        MyNetworkManager = GameObject.Find("NetworkManager").GetComponent<ExtendedNetworkManager>();
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        MyNetworkController = GetComponent<NetworkController>();
        MyGlobalController.ThePlayerObject = gameObject;
    }
    public override void OnStartServer()
    {
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        MyNetworkController = GetComponent<NetworkController>();
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        OnStartClient();
        string LanIP = GetComputer_LanIP();
        MyGlobalController.LogDebug("Client Started");
        
        MyGlobalController.LogDebug(MyGlobalController.MyPlayerName + " Launching CmdConnectPlayer");
        if (isServer)
        {
            MyGlobalController.PlayersNames = new List<string>();
            MyGlobalController.PlayersShips = new List<GridElement.GridElementState[]>();
            MyGlobalController.PlayersShips.Add(new GridElement.GridElementState[48]);
            MyGlobalController.PlayersNames.Add(MyGlobalController.WelcomeInputName.text);
            MyGlobalController.MyPlayerName = MyGlobalController.WelcomeInputName.text;
            MyNetworkController.CmdAppendToConsole(MyGlobalController.WelcomeInputName.text + " Connected as a Host.", NetworkController.Severities.Info);
            if (LanIP.Split('\n').Length > 2)
                MyNetworkController.CmdAppendToConsole("Clients connecting to this Host should enter one of these HostIPs : " + LanIP, NetworkController.Severities.Warning);
            else
                MyNetworkController.CmdAppendToConsole("Clients connecting to this Host should enter this HostIP: " + LanIP, NetworkController.Severities.Warning);

            MyGlobalController.SetState(GlobalController.GuiGlobalState.SetMyShips);
        }
        else CmdConnectPlayer(MyGlobalController.WelcomeInputName.text, gameObject);
    }

    [Command]
    public void CmdConnectPlayer(string PlayerName, GameObject PlayerObject)
    {
        MyGlobalController.LogDebug(PlayerName + " Launched CmdConnectPlayer");
        if (MyGlobalController.PlayersNames.Contains(PlayerName))
        {
            MyGlobalController.LogDebug(PlayerName + " calling RpcPlayerExists");
            RpcPlayerExists(PlayerName);
        }
        else
        {
            MyGlobalController.PlayersShips.Add(new GridElement.GridElementState[48]);
            MyGlobalController.PlayersNames.Add(PlayerName);
            MyNetworkController.CmdAppendToConsole(PlayerName + " Connected as a Client.", NetworkController.Severities.Info);
            MyGlobalController.LogDebug(PlayerName + " calling RpcClientConnectedCorrectly");
            RpcClientConnectedCorrectly(PlayerName);
            MemoryStream MSS = new MemoryStream();
            MemoryStream MSN = new MemoryStream();
            BinaryFormatter BF = new BinaryFormatter();
            BF.Serialize(MSS, MyGlobalController.PlayersShips);
            BF.Serialize(MSN, MyGlobalController.PlayersNames);
            GetComponent<PlaceShips>().RpcSendShipsToClients(Convert.ToBase64String(MSS.GetBuffer()), Convert.ToBase64String(MSN.GetBuffer()));
        }
    }
    [ClientRpc]
    public void RpcClientConnectedCorrectly(string PlayerName)
    {
        MyGlobalController.MyPlayerName = MyGlobalController.WelcomeInputName.text;
        if (PlayerName != MyGlobalController.MyPlayerName) return;
        MyGlobalController.LogDebug(PlayerName + " called RpcClientConnectedCorrectly");
        MyGlobalController.SetState(GlobalController.GuiGlobalState.SetMyShips);
    }
    [ClientRpc]
    public void RpcPlayerExists(string PlayerName)
    {
        if (PlayerName != MyGlobalController.MyPlayerName) return;
        MyGlobalController.LogDebug(PlayerName + " called RpcPlayerExists");
        MyGlobalController.WelcomeMessage.text = "Player " + MyGlobalController.WelcomeInputName.text + " Is already connected.\nPlease tell us your name : ";
        ExtendedNetworkManager.singleton.StopClient();
    }

    private string GetComputer_LanIP()
    {
        string strHostName = Dns.GetHostName();
        string Toreturn = "";
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

        foreach (IPAddress ipAddress in ipEntry.AddressList)
        {
            if (ipAddress.AddressFamily.ToString() == "InterNetwork")
            {
                Toreturn += ipAddress.ToString() + "\n";
            }
        }

        return Toreturn;
    }

}
