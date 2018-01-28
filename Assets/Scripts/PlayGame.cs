using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlayGame : NetworkBehaviour
{
    private GlobalController MyGlobalController;
    private NetworkController MyNetworkController;

    public bool HasLoadedMissile = true;
    // Use this for initialization
    void Start()
    {
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        MyGlobalController.EnemyGridControllerScript.StartMe();
        MyNetworkController = GetComponent<NetworkController>();
    }

    public void IBeenClicked(string FromPlayer, string ToPlayer, int Index)
    {
        int EnemyIndex = MyGlobalController.PlayersNames.IndexOf(ToPlayer);
        if (MyGlobalController.PlayersShips[EnemyIndex][Index] == GridElement.GridElementState.Invalid) return;
        if (true)//HasLoadedMissile)
        {
            HasLoadedMissile = false;
            MyGlobalController.ConsoleMessage.text = "<color=red>You Have No Missile Loaded. Type 'reload' to Load One Now.</color>";
            CmdSendMissile(FromPlayer, ToPlayer, Index);
        }
    }
    [Command]
    public void CmdSendMissile(string FromPlayer, string ToPlayer, int TileIndex)
    {
        int MyPosY = (int)Mathf.Floor(TileIndex / 6);
        int MyPosX = TileIndex - (MyPosY * 6);
        int PlayerIndex = MyGlobalController.PlayersNames.IndexOf(ToPlayer);
        if (MyGlobalController.PlayersShips[PlayerIndex][TileIndex] == GridElement.GridElementState.Occupied)
        {
            MyGlobalController.PlayersShips[PlayerIndex][TileIndex] = GridElement.GridElementState.Hit;
            MyNetworkController.CmdAppendToConsole(FromPlayer + " launched a missile on " + ToPlayer + " at (" + MyPosX + "," + MyPosY + ") and Hit...", NetworkController.Severities.Warning);

            if (CheckLossCondition(MyGlobalController.PlayersShips[PlayerIndex]))
            {
                RpcYouJustDied(ToPlayer);

                MyGlobalController.PlayersNames.RemoveAt(PlayerIndex);
                MyGlobalController.PlayersShips.RemoveAt(PlayerIndex);

                MyNetworkController.CmdAppendToConsole(ToPlayer + " Just lost all the ships... Goodbye...", NetworkController.Severities.Exception);
            }
        }
        else
        {
            MyNetworkController.CmdAppendToConsole(FromPlayer + " launched a missile on " + ToPlayer + " at (" + MyPosX + "," + MyPosY + ") and missed...", NetworkController.Severities.Info);
        }

        MemoryStream MSS = new MemoryStream();
        MemoryStream MSN = new MemoryStream();
        BinaryFormatter BF = new BinaryFormatter();
        BF.Serialize(MSS, MyGlobalController.PlayersShips);
        BF.Serialize(MSN, MyGlobalController.PlayersNames);
        RpcSendShipsToClients(Convert.ToBase64String(MSS.GetBuffer()), Convert.ToBase64String(MSN.GetBuffer()));
    }
    [ClientRpc]
    public void RpcSendShipsToClients(string AllShips, string AllNames)
    {
        BinaryFormatter BF = new BinaryFormatter();
        MemoryStream MS = new MemoryStream(Convert.FromBase64String(AllShips));
        List<GridElement.GridElementState[]> PlayersShips = (List<GridElement.GridElementState[]>)BF.Deserialize(MS);
        MyGlobalController.PlayersShips = PlayersShips;
        MS = new MemoryStream(Convert.FromBase64String(AllNames));
        List<string> PlayersNames = (List<string>)BF.Deserialize(MS);
        MyGlobalController.PlayersNames = PlayersNames;
        MyGlobalController.MyGridControllerScript.UpdateShips();
        if (MyGlobalController.MyGuiGlobalState == GlobalController.GuiGlobalState.Play)
            MyGlobalController.EnemyGridControllerScript.UpdateShips();
    }
    [ClientRpc]
    public void RpcYouJustDied(string WhoDied)
    {
        if (MyGlobalController.MyPlayerName != WhoDied) return;
        MyGlobalController.SetState(GlobalController.GuiGlobalState.GameOver);
        GameObject.Find("NetworkManager").GetComponent<ExtendedNetworkManager>().client.Disconnect();
    }
    private bool CheckLossCondition(GridElement.GridElementState[] Ships)
    {
        for (int CntX = 0; CntX < 6; CntX++)
            for (int CntY = 0; CntY < 8; CntY++)
                if (Ships[CntY * 6 + CntX] == GridElement.GridElementState.Occupied) return false;
        return true;
    }
}
