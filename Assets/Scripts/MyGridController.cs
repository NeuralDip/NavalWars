using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGridController : MonoBehaviour
{
    private GlobalController MyGlobalController;
    private PlaceShips PlaceShipsScript;

    public void StartMe()
    {
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        PlaceShipsScript = MyGlobalController.ThePlayerObject.GetComponent<PlaceShips>();
    }

    public void IBeenHovered(int Index)
    {
        switch (PlaceShipsScript.CurrState)
        {
            case PlaceShips.PlaceShipsStates.Place2x1:
                if (Check(Index, 2, 1))
                    Set(Index, 2, 1, GridElement.GridElementState.Hovering);
                else
                    Set(Index, 2, 1, GridElement.GridElementState.Invalid);
                break;
            case PlaceShips.PlaceShipsStates.Place1X3:
                if (Check(Index, 1, 3))
                    Set(Index, 1, 3, GridElement.GridElementState.Hovering);
                else
                    Set(Index, 1, 3, GridElement.GridElementState.Invalid);
                break;
            case PlaceShips.PlaceShipsStates.Place4X1:
                if (Check(Index, 4, 1))
                    Set(Index, 4, 1, GridElement.GridElementState.Hovering);
                else
                    Set(Index, 4, 1, GridElement.GridElementState.Invalid);
                break;
            case PlaceShips.PlaceShipsStates.Place1X5:
                if (Check(Index, 1, 5))
                    Set(Index, 1, 5, GridElement.GridElementState.Hovering);
                else
                    Set(Index, 1, 5, GridElement.GridElementState.Invalid);
                break;
            case PlaceShips.PlaceShipsStates.Waiting:
            default:
                break;
        }
    }
    public void IBeenUnhovered(int Index)
    {
        switch (PlaceShipsScript.CurrState)
        {
            case PlaceShips.PlaceShipsStates.Place2x1:
                Set(Index, 2, 1, GridElement.GridElementState.Empty, true);
                break;
            case PlaceShips.PlaceShipsStates.Place1X3:
                Set(Index, 1, 3, GridElement.GridElementState.Empty, true);
                break;
            case PlaceShips.PlaceShipsStates.Place4X1:
                Set(Index, 4, 1, GridElement.GridElementState.Empty, true);
                break;
            case PlaceShips.PlaceShipsStates.Place1X5:
                Set(Index, 1, 5, GridElement.GridElementState.Empty, true);
                break;
            case PlaceShips.PlaceShipsStates.Waiting:
            default:
                break;
        }
    }
    public void IBeenClicked(int Index)
    {
        switch (PlaceShipsScript.CurrState)
        {
            case PlaceShips.PlaceShipsStates.Place2x1:
                if (Check(Index, 2, 1))
                {
                    Set(Index, 2, 1, GridElement.GridElementState.Occupied);
                    PlaceShipsScript.SetState(PlaceShips.PlaceShipsStates.Place1X3);
                    IBeenHovered(Index);
                }
                break;
            case PlaceShips.PlaceShipsStates.Place1X3:
                if (Check(Index, 1, 3))
                {
                    Set(Index, 1, 3, GridElement.GridElementState.Occupied);
                    PlaceShipsScript.SetState(PlaceShips.PlaceShipsStates.Place4X1);
                    IBeenHovered(Index);
                }
                break;
            case PlaceShips.PlaceShipsStates.Place4X1:
                if (Check(Index, 4, 1))
                {
                    Set(Index, 4, 1, GridElement.GridElementState.Occupied);
                    PlaceShipsScript.SetState(PlaceShips.PlaceShipsStates.Place1X5);
                    IBeenHovered(Index);
                }
                break;
            case PlaceShips.PlaceShipsStates.Place1X5:
                if (Check(Index, 1, 5))
                {
                    Set(Index, 1, 5, GridElement.GridElementState.Occupied);
                    PlaceShipsScript.SetState(PlaceShips.PlaceShipsStates.SendData);
                }
                break;
            case PlaceShips.PlaceShipsStates.Waiting:
            default:
                break;
        }
    }
    public void UpdateShips()
    {
        if (MyGlobalController.MyGuiGlobalState != GlobalController.GuiGlobalState.Play) return;
        for (int Cnt = 0; Cnt < MyGlobalController.PlayersNames.Count; Cnt++)
            if (MyGlobalController.PlayersNames[Cnt] == MyGlobalController.MyPlayerName)
                for (int CntX = 0; CntX < 6; CntX++)
                    for (int CntY = 0; CntY < 8; CntY++)
                        transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().SetState(MyGlobalController.PlayersShips[Cnt][CntY * 6 + CntX]);
    }
    public void ClearMyShip()
    {
        for (int CntX = 0; CntX < 6; CntX++)
            for (int CntY = 0; CntY < 8; CntY++)
                transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().SetState(GridElement.GridElementState.Empty);
    }
    private bool Check(int Index, int XDim, int YDim)
    {
        int MyPosY = (int)Mathf.Floor(Index / 6);
        int MyPosX = Index - (MyPosY * 6);

        if (MyPosX + XDim > 6 || MyPosY + YDim > 8) return false;

        for (int CntX = 0; CntX < XDim; CntX++)
            for (int CntY = 0; CntY < YDim; CntY++)
            {
                int NewIndex = IndexFromDims(Index, CntX, CntY);
                if (NewIndex == -1 ||
                    !(transform.GetChild(NewIndex).GetComponent<GridElement>().MyCurrState == GridElement.GridElementState.Empty ||
                    transform.GetChild(NewIndex).GetComponent<GridElement>().MyCurrState == GridElement.GridElementState.Hovering)) return false;
            }
        return true;
    }
    private void Set(int Index, int XDim, int YDim, GridElement.GridElementState GriElementType, bool SetBack = false)
    {
        int MyPosY = (int)Mathf.Floor(Index / 6);
        int MyPosX = Index - (MyPosY * 6);

        if (SetBack)
            for (int CntX = 0; CntX < XDim; CntX++)
                for (int CntY = 0; CntY < YDim; CntY++)
                {
                    int NewIndex = IndexFromDims(Index, CntX, CntY);
                    if (NewIndex != -1) transform.GetChild(NewIndex).GetComponent<GridElement>().SetBack();
                }
        else
            for (int CntX = 0; CntX < XDim; CntX++)
                for (int CntY = 0; CntY < YDim; CntY++)
                {
                    int NewIndex = IndexFromDims(Index, CntX, CntY);
                    if (NewIndex != -1)
                    {
                        transform.GetChild(NewIndex).GetComponent<GridElement>().SetState(GriElementType);
                    }
                }
    }
    private int IndexFromDims(int StartIndex, int XDim, int YDim)
    {
        int FinalIndex = StartIndex + YDim * 6 + XDim;
        if (FinalIndex > 47) FinalIndex = -1;
        return FinalIndex;
    }
}
