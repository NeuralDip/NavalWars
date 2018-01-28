using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGridController : MonoBehaviour
{
    private GlobalController MyGlobalController;
    private PlayGame PlayGameScript;

    public void StartMe()
    {
        MyGlobalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        PlayGameScript = MyGlobalController.ThePlayerObject.GetComponent<PlayGame>();
    }

    public void IBeenHovered(int Index)
    {
        if (MyGlobalController.EnemiesDropDown.options.Count == 0) return;
        if (Check(Index))
            transform.GetChild(Index).GetComponent<GridElement>().SetState(GridElement.GridElementState.Hovering);
        else
            transform.GetChild(Index).GetComponent<GridElement>().SetState(GridElement.GridElementState.Invalid);
    }
    public void IBeenUnhovered(int Index)
    {
        if (MyGlobalController.EnemiesDropDown.options.Count == 0) return;
        transform.GetChild(Index).GetComponent<GridElement>().SetBack();
    }
    public void IBeenClicked(int Index)
    {
        if (MyGlobalController.EnemiesDropDown.options.Count == 0) return;
        PlayGameScript.IBeenClicked(MyGlobalController.MyPlayerName, MyGlobalController.EnemiesDropDown.options[MyGlobalController.EnemiesDropDown.value].text, Index);
    }
    public void UpdateShips()
    {
        if (MyGlobalController.MyGuiGlobalState != GlobalController.GuiGlobalState.Play) return;
        Dropdown.OptionData OldOption;
        if (MyGlobalController.EnemiesDropDown.options.Count == 0) OldOption = new Dropdown.OptionData();
        else OldOption = MyGlobalController.EnemiesDropDown.options[MyGlobalController.EnemiesDropDown.value];
        List<Dropdown.OptionData> NewOptions = new List<Dropdown.OptionData>();
        MyGlobalController.EnemiesDropDown.ClearOptions();
        int NewValue = 0;
        for (int Cnt = 0; Cnt < MyGlobalController.PlayersNames.Count; Cnt++)
            if (MyGlobalController.PlayersNames[Cnt] != MyGlobalController.MyPlayerName)
            {
                Dropdown.OptionData NewOption = new Dropdown.OptionData();
                NewOption.text = MyGlobalController.PlayersNames[Cnt];
                if (MyGlobalController.PlayersNames[Cnt] == OldOption.text) NewValue = Cnt;
                NewOptions.Add(NewOption);
            }

        MyGlobalController.EnemiesDropDown.AddOptions(NewOptions);
        MyGlobalController.EnemiesDropDown.value = NewValue;
        OnDropDownChanged();
    }
    public void OnDropDownChanged()
    {
        if (MyGlobalController.EnemiesDropDown.options.Count == 0) return;
        bool Updated = false;
        for (int Cnt = 0; Cnt < MyGlobalController.PlayersNames.Count; Cnt++)
            if (MyGlobalController.PlayersNames[Cnt] == MyGlobalController.EnemiesDropDown.options[MyGlobalController.EnemiesDropDown.value].text)
            {
                Updated = true;
                for (int CntX = 0; CntX < 6; CntX++)
                    for (int CntY = 0; CntY < 8; CntY++)
                    {
                        if (MyGlobalController.PlayersShips[Cnt][CntY * 6 + CntX] == GridElement.GridElementState.Hit)
                            transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().SetState(GridElement.GridElementState.Hit, true);
                        else if (MyGlobalController.PlayersShips[Cnt][CntY * 6 + CntX] == GridElement.GridElementState.Invalid)
                            transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().SetState(GridElement.GridElementState.Invalid, true);
                        else transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().SetState(GridElement.GridElementState.Empty, true);
                    }
            }
        if (!Updated)
        {
            for (int CntX = 0; CntX < 6; CntX++)
                for (int CntY = 0; CntY < 8; CntY++)
                {
                    transform.GetChild(CntY * 6 + CntX).GetComponent<GridElement>().SetState(GridElement.GridElementState.Invalid);
                }
        }
    }
    private bool Check(int Index)
    {
        if (transform.GetChild(Index).GetComponent<GridElement>().MyCurrState == GridElement.GridElementState.Hit ||
            transform.GetChild(Index).GetComponent<GridElement>().MyCurrState == GridElement.GridElementState.Invalid) return false;
        else return true;
    }
}
