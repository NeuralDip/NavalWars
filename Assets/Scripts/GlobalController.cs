using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GlobalController : MonoBehaviour
{
    // The Player
    public string MyPlayerName;
    public GameObject ThePlayerObject;
    // PLayers data
    public List<string> PlayersNames;
    public List<GridElement.GridElementState[]> PlayersShips;
    public List<NetworkPlayer> PlayersNetworks;
    //Panels
    public RectTransform WelcomeGiveNamePanel;
    public RectTransform GameOverPanel;
    public RectTransform MessagesPanel;
    public RectTransform MyShipsPanel;
    public RectTransform EnemyShipsPanel;

    //Welcome Objects
    public Text WelcomeInputName;
    public Text WelcomeInputHost;
    public Text WelcomeMessage;
    public Toggle WelcomeHostToggle;
    //MessagePanel Objects
    public Text Console;
    public Text ConsoleMessage;
    public string ConsoleString = "";
    public InputField ConsoleInput;
    public Text ConsoleInputText;
    //My ships objects
    public Text MyShipsMessage;
    //EnemyShips
    public Dropdown EnemiesDropDown;
    // Scripts
    public MyGridController MyGridControllerScript;
    public EnemyGridController EnemyGridControllerScript;

    public enum GuiGlobalState
    {
        None = 0,
        Welcome = 1,
        SetMyShips = 2,
        Play = 3,
        GameOver = 4,

    }
    public GuiGlobalState MyGuiGlobalState;

    //variables
    public float SlidingSpeed = 10;
    // The inpu is here, te surfaces are in the surfaceobject scripts
    //public MnesicSurfaceInput MyMnesicSurfaceInput;

    private Vector2 AnchorMinOnWelcomeGiveNamePanel = new Vector2(0.3626615f, 0.5775377f);
    private Vector2 AnchorMinOnGameOverPanel = new Vector2(0.3547171f, 0.7724615f);
    private Vector2 AnchorMinOnMessagesPanel = new Vector2(0, 0);
    private Vector2 AnchorMinOnMyShipsPanel = new Vector2(0.2375041f, 0.03352055f);
    private Vector2 AnchorMinOnEnemyShipsPanel = new Vector2(0.618578f, 0.03352048f);
    private Vector2 AnchorMaxOnWelcomeGiveNamePanel = new Vector2(0.6435212f, 0.9806794f);
    private Vector2 AnchorMaxOnGameOverPanel = new Vector2(0.6319271f, 0.985145f);
    private Vector2 AnchorMaxOnMessagesPanel = new Vector2(0.2375041f, 1);
    private Vector2 AnchorMaxOnMyShipsPanel = new Vector2(0.6108076f, 0.980074f);
    private Vector2 AnchorMaxOnEnemyShipsPanel = new Vector2(0.9918814f, 0.980074f);
    private Vector2 AnchorMinOffWelcomeGiveNamePanel = new Vector2(0.359306f, 1.054692f);
    private Vector2 AnchorMinOffGameOverPanel = new Vector2(0.3629556f, 1.058018f);
    private Vector2 AnchorMinOffMessagesPanel = new Vector2(-0.2553952f, -0.007321954f);
    private Vector2 AnchorMinOffMyShipsPanel = new Vector2(1.020167f, 0.03352061f);
    private Vector2 AnchorMinOffEnemyShipsPanel = new Vector2(1.022267f, 0.03352053f);
    private Vector2 AnchorMaxOffWelcomeGiveNamePanel = new Vector2(0.6401657f, 1.457834f);
    private Vector2 AnchorMaxOffGameOverPanel = new Vector2(0.6401657f, 1.270702f);
    private Vector2 AnchorMaxOffMessagesPanel = new Vector2(-0.01789106f, 0.992678f);
    private Vector2 AnchorMaxOffMyShipsPanel = new Vector2(1.39347f, 0.9800741f);
    private Vector2 AnchorMaxOffEnemyShipsPanel = new Vector2(1.395571f, 0.9800741f);

    private bool WelcomeGiveNamePanelOn;
    private bool GameOverPanelOn;
    private bool MessagesPanelOn;
    private bool MyShipsPanelOn;
    private bool EnemyShipsPanelOn;

    public string LogPath;
    // Use this for initialization
    void Start()
    {
        LogPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DebugLogs.txt";
        File.Delete(LogPath);
        LogDebug("Starting Unity");

        SetState(GuiGlobalState.Welcome);

    }

    // Update is called once per frame
    void Update()
    {
        MovePanels();
    }

    public void SetState(GuiGlobalState NewGuiState)
    {
        if (MyGuiGlobalState == NewGuiState) return;
        MyGuiGlobalState = NewGuiState;

        switch (MyGuiGlobalState)
        {
            case GuiGlobalState.Welcome:
                WelcomeGiveNamePanelOn = true;
                GameOverPanelOn = false;
                MessagesPanelOn = false;
                MyShipsPanelOn = false;
                EnemyShipsPanelOn = false;
                break;
            case GuiGlobalState.SetMyShips:
                WelcomeGiveNamePanelOn = false;
                GameOverPanelOn = false;
                MessagesPanelOn = true;
                MyShipsPanelOn = true;
                EnemyShipsPanelOn = false;
                ConsoleMessage.text = "";
                ConsoleInput.interactable = false;
                ThePlayerObject.GetComponent<PlaceShips>().SetState(PlaceShips.PlaceShipsStates.Place2x1);
                break;
            case GuiGlobalState.Play:
                WelcomeGiveNamePanelOn = false;
                GameOverPanelOn = false;
                MessagesPanelOn = true;
                MyShipsPanelOn = true;
                EnemyShipsPanelOn = true;
                ConsoleMessage.text = "<color=green>You Can Fire A missile Now.</color>";
                ConsoleInput.interactable = true;
                break;
            case GuiGlobalState.GameOver:
                WelcomeGiveNamePanelOn = false;
                GameOverPanelOn = true;
                MessagesPanelOn = false;
                MyShipsPanelOn = false;
                EnemyShipsPanelOn = false;
                break;
        }
    }
    private void MovePanels()
    {
        if (WelcomeGiveNamePanelOn)
        {
            WelcomeGiveNamePanel.anchorMin = Vector2.Lerp(WelcomeGiveNamePanel.anchorMin, AnchorMinOnWelcomeGiveNamePanel, Time.deltaTime * SlidingSpeed);
            WelcomeGiveNamePanel.anchorMax = Vector2.Lerp(WelcomeGiveNamePanel.anchorMax, AnchorMaxOnWelcomeGiveNamePanel, Time.deltaTime * SlidingSpeed);
        }
        else
        {
            WelcomeGiveNamePanel.anchorMin = Vector2.Lerp(WelcomeGiveNamePanel.anchorMin, AnchorMinOffWelcomeGiveNamePanel, Time.deltaTime * SlidingSpeed);
            WelcomeGiveNamePanel.anchorMax = Vector2.Lerp(WelcomeGiveNamePanel.anchorMax, AnchorMaxOffWelcomeGiveNamePanel, Time.deltaTime * SlidingSpeed);
        }
        if (GameOverPanelOn)
        {
            GameOverPanel.anchorMin = Vector2.Lerp(GameOverPanel.anchorMin, AnchorMinOnGameOverPanel, Time.deltaTime * SlidingSpeed);
            GameOverPanel.anchorMax = Vector2.Lerp(GameOverPanel.anchorMax, AnchorMaxOnGameOverPanel, Time.deltaTime * SlidingSpeed);
        }
        else
        {
            GameOverPanel.anchorMin = Vector2.Lerp(GameOverPanel.anchorMin, AnchorMinOffGameOverPanel, Time.deltaTime * SlidingSpeed);
            GameOverPanel.anchorMax = Vector2.Lerp(GameOverPanel.anchorMax, AnchorMaxOffGameOverPanel, Time.deltaTime * SlidingSpeed);
        }
        if (MessagesPanelOn)
        {
            MessagesPanel.anchorMin = Vector2.Lerp(MessagesPanel.anchorMin, AnchorMinOnMessagesPanel, Time.deltaTime * SlidingSpeed);
            MessagesPanel.anchorMax = Vector2.Lerp(MessagesPanel.anchorMax, AnchorMaxOnMessagesPanel, Time.deltaTime * SlidingSpeed);
        }
        else
        {
            MessagesPanel.anchorMin = Vector2.Lerp(MessagesPanel.anchorMin, AnchorMinOffMessagesPanel, Time.deltaTime * SlidingSpeed);
            MessagesPanel.anchorMax = Vector2.Lerp(MessagesPanel.anchorMax, AnchorMaxOffMessagesPanel, Time.deltaTime * SlidingSpeed);
        }
        if (MyShipsPanelOn)
        {
            MyShipsPanel.anchorMin = Vector2.Lerp(MyShipsPanel.anchorMin, AnchorMinOnMyShipsPanel, Time.deltaTime * SlidingSpeed);
            MyShipsPanel.anchorMax = Vector2.Lerp(MyShipsPanel.anchorMax, AnchorMaxOnMyShipsPanel, Time.deltaTime * SlidingSpeed);
        }
        else
        {
            MyShipsPanel.anchorMin = Vector2.Lerp(MyShipsPanel.anchorMin, AnchorMinOffMyShipsPanel, Time.deltaTime * SlidingSpeed);
            MyShipsPanel.anchorMax = Vector2.Lerp(MyShipsPanel.anchorMax, AnchorMaxOffMyShipsPanel, Time.deltaTime * SlidingSpeed);
        }
        if (EnemyShipsPanelOn)
        {
            EnemyShipsPanel.anchorMin = Vector2.Lerp(EnemyShipsPanel.anchorMin, AnchorMinOnEnemyShipsPanel, Time.deltaTime * SlidingSpeed);
            EnemyShipsPanel.anchorMax = Vector2.Lerp(EnemyShipsPanel.anchorMax, AnchorMaxOnEnemyShipsPanel, Time.deltaTime * SlidingSpeed);
        }
        else
        {
            EnemyShipsPanel.anchorMin = Vector2.Lerp(EnemyShipsPanel.anchorMin, AnchorMinOffEnemyShipsPanel, Time.deltaTime * SlidingSpeed);
            EnemyShipsPanel.anchorMax = Vector2.Lerp(EnemyShipsPanel.anchorMax, AnchorMaxOffEnemyShipsPanel, Time.deltaTime * SlidingSpeed);
        }


        WelcomeGiveNamePanel.offsetMax = Vector2.zero;
        GameOverPanel.offsetMax = Vector2.zero;
        MessagesPanel.offsetMax = Vector2.zero;
        MyShipsPanel.offsetMax = Vector2.zero;
        EnemyShipsPanel.offsetMax = Vector2.zero;
    }
    public void LogDebug(string Message)
    {
        File.AppendAllText(LogPath, Message + "\n");
    }
}
