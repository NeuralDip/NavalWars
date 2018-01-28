using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkController : NetworkBehaviour
{
    private GlobalController MyGlobalController;

    public enum Severities
    {
        Info = 0,
        Warning = 1,
        Hit = 2,
        Exception = 3,
        ClearAll = 4
    }

    private string PrevMessage;

    private int ConsoleUpdated = 5;

    public override void OnStartClient()
    {
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        if (!isLocalPlayer) return;
        MyGlobalController.ThePlayerObject = gameObject;
    }
    public void OnPlayerConnected(NetworkPlayer player)
    {
        if(MyGlobalController.PlayersNetworks==null) MyGlobalController.PlayersNetworks = new List<NetworkPlayer>();
        MyGlobalController.PlayersNetworks.Add(player);
    }
    public void OnPlayerDisconnected(NetworkPlayer player)
    {
        int PlayerIndex = MyGlobalController.PlayersNetworks.IndexOf(player);
        CmdAppendToConsole(MyGlobalController.PlayersNames[PlayerIndex]+ " has disconnected", Severities.Exception);
        MyGlobalController.PlayersNames.RemoveAt(PlayerIndex);
        MyGlobalController.PlayersShips.RemoveAt(PlayerIndex);
        MemoryStream MSS = new MemoryStream();
        MemoryStream MSN = new MemoryStream();
        BinaryFormatter BF = new BinaryFormatter();
        BF.Serialize(MSS, MyGlobalController.PlayersShips);
        BF.Serialize(MSN, MyGlobalController.PlayersNames);
        GetComponent<PlaceShips>().RpcSendShipsToClients(Convert.ToBase64String(MSS.GetBuffer()), Convert.ToBase64String(MSN.GetBuffer()));
    }
    public void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        CmdAppendToConsole("You have been disconnected from the host...", Severities.Exception);
    }

    void Update()
    {
        FixConsoleAlignment();
    }
    //[Command]
    public void CmdAppendToConsole(string Message, Severities Criticality)
    {
        if (!isServer) return;
        if (MyGlobalController == null) MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        switch (Criticality)
        {
            case Severities.Info:
                MyGlobalController.ConsoleString += "<color=white>" + Message + "</color>\n";
                break;
            case Severities.Warning:
                MyGlobalController.ConsoleString += "<color=yellow>" + Message + "</color>\n";
                break;
            case Severities.Hit:
                MyGlobalController.ConsoleString += "<color=magenta>" + Message + "</color>\n";
                break;
            case Severities.Exception:
                MyGlobalController.ConsoleString += "<color=red> " + Message + "</color>\n";
                break;
            case Severities.ClearAll:
            default:
                PrevMessage = "";
                MyGlobalController.ConsoleString = "";
                return;
        }
        PrevMessage = Message;
        ConsoleUpdated = 5;
        MyGlobalController.Console.text = MyGlobalController.ConsoleString;
        RpcOnConsoleEdit(MyGlobalController.ConsoleString);
    }
    [ClientRpc]
    public void RpcOnConsoleEdit(string ConsoleString)
    {
        if (MyGlobalController == null) OnStartClient();
        MyGlobalController.ConsoleString = ConsoleString;
        MyGlobalController.Console.text = ConsoleString;
    }
    private void FixConsoleAlignment()
    {
        if (!isServer || MyGlobalController == null) return;
        if (ConsoleUpdated == 1)
        {
            int MaxLines = 100;
            string[] AllLines = MyGlobalController.Console.text.Split('\n');

            if (AllLines.Length > MaxLines)
            {
                string NewText = "";
                for (int Cnt = 0; Cnt < MaxLines - 1; Cnt++)
                    NewText += AllLines[AllLines.Length - MaxLines + Cnt] + '\n';
                MyGlobalController.Console.text = NewText;
                MyGlobalController.ConsoleString = NewText;
                ConsoleUpdated = 5;
            }
            else
            {
                ConsoleUpdated = 0;
                MyGlobalController.Console.transform.parent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
            }

        }
        else if (ConsoleUpdated > 1) ConsoleUpdated--;
    }
    public void MessageFromClient(string FromPlayer, string MyMessage)
    {
        CmdMessageFromClient(FromPlayer, MyMessage);
        MyGlobalController.ConsoleInput.text = "";
    }
    [Command]
    public void CmdMessageFromClient(string FromPlayer, string MyMessage)
    {
        bool ContainsReload = MyMessage.IndexOf("reload", StringComparison.OrdinalIgnoreCase) >= 0;
        if (ContainsReload)
        {
            CmdAppendToConsole(FromPlayer + " Just reloaded...", Severities.Warning);
            RpcClientHasReloaded(FromPlayer);
        }
        else
        {
            CmdAppendToConsole(FromPlayer + " says : " + MyMessage, Severities.Info);
        }
    }
    [ClientRpc]
    public void RpcClientHasReloaded(string PlayerName)
    {
        if (PlayerName != MyGlobalController.MyPlayerName) return;
        MyGlobalController.ConsoleMessage.text = "<color=green>You Can Fire A missile Now.</color>";
        GetComponent<PlayGame>().HasLoadedMissile = true;
    }
}
