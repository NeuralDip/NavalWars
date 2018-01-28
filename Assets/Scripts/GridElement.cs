using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridElement : MonoBehaviour
{
    public enum GridElementState
    {
        Invalid = 0,
        Empty = 1,
        Occupied = 2,
        Hit = 3,
        Hovering = 4
    }

    public Color SeaColor;
    public Color HoveringColor;
    public Color OccupiedColor;
    public Color HitColor;
    public Color InvalidColor;

    public bool UnderEnemyGrid = false;

    private MyGridController MyGridControllerScript;
    private EnemyGridController EnemyGridControllerScript;
    private Image MyImage;
    public GridElementState MyCurrState;
    public GridElementState MyPrevState;

    private int MyIndex;

    public void Start()
    {
        if (UnderEnemyGrid)
            EnemyGridControllerScript = transform.parent.GetComponent<EnemyGridController>();
        else
            MyGridControllerScript = transform.parent.GetComponent<MyGridController>();
        MyImage = GetComponent<Image>();
        MyIndex = transform.GetSiblingIndex();
        SetState(GridElementState.Empty);
    }

    public void SetState(GridElementState NewState, bool AllTheWay = false)
    {
        if (AllTheWay) MyPrevState = NewState;
        else MyPrevState = MyCurrState;
        MyCurrState = NewState;
        switch (MyCurrState)
        {
            case GridElementState.Empty:
                MyImage.color = SeaColor;
                break;
            case GridElementState.Occupied:
                MyImage.color = OccupiedColor;
                break;
            case GridElementState.Hit:
                MyImage.color = HitColor;
                break;
            case GridElementState.Invalid:
                MyImage.color = InvalidColor;
                break;
            case GridElementState.Hovering:
                MyImage.color = HoveringColor;
                break;
        }
    }
    public void IHovered()
    {
        if (UnderEnemyGrid)
            EnemyGridControllerScript.IBeenHovered(MyIndex);
        else
            MyGridControllerScript.IBeenHovered(MyIndex);
    }
    public void IUnHovered()
    {
        if (UnderEnemyGrid)
            EnemyGridControllerScript.IBeenUnhovered(MyIndex);
        else
            MyGridControllerScript.IBeenUnhovered(MyIndex);
    }
    public void IClicked()
    {
        if (UnderEnemyGrid)
            EnemyGridControllerScript.IBeenClicked(MyIndex);
        else
            MyGridControllerScript.IBeenClicked(MyIndex);
    }
    public void SetBack()
    {
        SetState(MyPrevState);
    }
}
