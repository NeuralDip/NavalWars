using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlaceShips : NetworkBehaviour
{
    private GlobalController MyGlobalController;
    public enum PlaceShipsStates
    {
        Waiting = 0,
        Place2x1 = 1,
        Place1X3 = 2,
        Place4X1 = 3,
        Place1X5 = 4,
        SendData = 5
    }
    public PlaceShipsStates CurrState;
    // Use this for initialization
    void Start()
    {
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        MyGlobalController.MyGridControllerScript.StartMe();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetState(PlaceShipsStates NewState)
    {
        if (MyGlobalController == null) Start();
        CurrState = NewState;
        switch (CurrState)
        {
            case PlaceShipsStates.Place2x1:
                MyGlobalController.MyShipsMessage.text = "Place your 2x1 ship.";
                break;
            case PlaceShipsStates.Place1X3:
                MyGlobalController.MyShipsMessage.text = "Place your 1x3 ship.";
                break;
            case PlaceShipsStates.Place4X1:
                MyGlobalController.MyShipsMessage.text = "Place your 4x1 ship.";
                break;
            case PlaceShipsStates.Place1X5:
                MyGlobalController.MyShipsMessage.text = "Place your 1x5 ship.";
                break;
            case PlaceShipsStates.SendData:
                GridElement.GridElementState[] MyShips = new GridElement.GridElementState[48];
                for (int CntX = 0; CntX < 6; CntX++)
                    for (int CntY = 0; CntY < 8; CntY++)
                        if (MyGlobalController.MyGridControllerScript.gameObject.transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().MyCurrState != GridElement.GridElementState.Empty)
                            MyShips[CntY * 6 + CntX] = GridElement.GridElementState.Occupied;
                        else MyShips[CntY * 6 + CntX] = GridElement.GridElementState.Empty;
                if (isClient)
                {
                    CmdSendShips(MyShips, MyGlobalController.WelcomeInputName.text);
                }
                break;
            default:
                MyGlobalController.MyShipsMessage.text = "This is your sea.";
                break;
        }
    }
    [Command]
    public void CmdSendShips(GridElement.GridElementState[] Ships, string MyName)
    {
        int Index = MyGlobalController.PlayersNames.IndexOf(MyName);
        MyGlobalController.PlayersShips[Index] = Ships;
        RpcMovetoPlayState(MyName);
        MemoryStream MSS = new MemoryStream();
        MemoryStream MSN = new MemoryStream();
        BinaryFormatter BF = new BinaryFormatter();
        BF.Serialize(MSS, MyGlobalController.PlayersShips);
        BF.Serialize(MSN, MyGlobalController.PlayersNames);
        RpcSendShipsToClients(Convert.ToBase64String(MSS.GetBuffer()), Convert.ToBase64String(MSN.GetBuffer()));
    }
    [ClientRpc]
    public void RpcMovetoPlayState(string PlayerSentShips)
    {
        if (MyGlobalController.MyPlayerName != PlayerSentShips) return;
        MyGlobalController.SetState(GlobalController.GuiGlobalState.Play);
        SetState(PlaceShipsStates.Waiting);
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
}
