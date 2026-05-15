#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEngine.SceneManagement; using UnityEngine.InputSystem;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class MainGameAttributes : MonoBehaviour {
    [Header("GameBasics")]
    public GameStates State;
    public GameDifficulties Difficulty;

    [Header("Locations")]
    public GameObject preGameSetup;
    public GameObject GameplaySetup;
    public GameObject RankingSetup;
    [Space]
    public Text difficultyDisplayText;
    public string menuDifficultyPrefix;
    
    [Header("Display")]
    public Text SituationDisplay;
    public string TranslatedDifficulty;

    [Header("Gamepad support")]
    public GameObject menuGamePadGroup;

    [Header("Wearable")]
    public int WearableValue;
    public Sprite[] GameWearables;
    public SpriteRenderer PlayerRenderer;
    [HideInInspector]public MainWeariableMechanism oneShotWearable;

    [Header("Ranks")]
    public RankVideo[] VideoClips;

    //Controlls
    GlobalControlls Controls; Gamepad gamepad;

    public static event System.Action OnGameStart;

    public static event System.Action OnLowGraphDraw;
    public static event System.Action OnMediumGraphDraw;
    public static event System.Action OnHighGraphDraw;

    public static event System.Action OnGamepadWearableScroll;
    public static event System.Action SelectDefaultWearable;

    void Awake() { 
        Controls = new GlobalControlls();
    }
    void OnEnable() { 
        Controls.Joystick.Enable(); 
        Controls.Joystick.Left.performed += ctx => AddDifficulty();
        Controls.Joystick.Right.performed += ctx => AddWearable();
        Controls.Joystick.Up.performed += ctx => RunGame();
        Controls.Joystick.Down.performed += ctx => ReloadOrExit();
    } 
    void OnDisable() { 
        Controls.Joystick.Disable(); 
        Controls.Joystick.Left.performed -= ctx => AddDifficulty();
        Controls.Joystick.Right.performed -= ctx => AddWearable();
        Controls.Joystick.Up.performed -= ctx => RunGame();
        Controls.Joystick.Down.performed -= ctx => ReloadOrExit();
    }

    void Start() {
        gamepad = Gamepad.current;
        if (gamepad == null) { menuGamePadGroup.SetActive(false); }
        AssignDifficulty(1);
        preGameSetup.SetActive(true);
        GameplaySetup.SetActive(false);
        RankingSetup.SetActive(false);
    }
    //preGame
    public void AssignDifficulty(int value) {
        switch (value) {
            case 0: Difficulty = GameDifficulties.easy; break;
            case 1: Difficulty = GameDifficulties.normal; break;
            case 2: Difficulty = GameDifficulties.hard; break;
            default : Difficulty = GameDifficulties.test; break;
        }
        
        difficultyDisplayText.text = menuDifficultyPrefix + translateMenuDifficulty();
    }
    public void AssignWearable(int value) {
        WearableValue = value;
    }
    public void AddWearable() {
        oneShotWearable = null;
        OnGamepadWearableScroll?.Invoke();
        if (oneShotWearable == null) {
            SelectDefaultWearable?.Invoke();
        } else {
            oneShotWearable.OnPress();
        }
    }
    void AddDifficulty() {
        switch (Difficulty) {
            case GameDifficulties.easy : AssignDifficulty(1); break;
            case GameDifficulties.normal : AssignDifficulty(2); break;
            case GameDifficulties.hard : AssignDifficulty(0); break;
        }
    }
    public void RunGame() {
        OnGameStart?.Invoke();
        State = GameStates.game;
        TranslateDifficulty();
        preGameSetup.SetActive(false);
        GameplaySetup.SetActive(true);
        gameObject.GetComponent<Camera>().enabled = false;

        OnDetailDraw(DetalizationLevels.low);
        OnDetailDraw(DetalizationLevels.medium);
        OnDetailDraw(DetalizationLevels.high);

        PlayerRenderer.sprite = GameWearables[WearableValue - 1];
    }
    //Game
    void Update() {
        if (State == GameStates.preGame) {
            if (gamepad != null) {
            } 
        }
        else if (State == GameStates.endGame) { return; }
        else {
            UpdateSituation();
        }
    }
    void OnDetailDraw(DetalizationLevels level) {
        switch (level) {
            case DetalizationLevels.low : 
                if (PlayerPrefs.HasKey("DetailLevel") && PlayerPrefs.GetInt("DetailLevel") >= 0) { 
                    OnLowGraphDraw?.Invoke(); 
                } 
                break;
            case DetalizationLevels.medium : 
                if (PlayerPrefs.HasKey("DetailLevel") && PlayerPrefs.GetInt("DetailLevel") >= 1) { 
                    OnMediumGraphDraw?.Invoke(); 
                } 
                break;
            case DetalizationLevels.high : 
                if (PlayerPrefs.HasKey("DetailLevel") && PlayerPrefs.GetInt("DetailLevel") >= 2) { 
                    OnHighGraphDraw?.Invoke(); 
                } 
                break;
        }
    }
    string translateMenuDifficulty() {
        string product = "";
        switch (Difficulty) {
            case GameDifficulties.easy : product = "Лёгкая"; break;
            case GameDifficulties.normal : product = "Средняя"; break;
            case GameDifficulties.hard : product = "Тяжёлая"; break;
            case GameDifficulties.test : product = "dev test"; break;
            default : product = "unknown"; break;
        }
        return product;
    }
    public virtual void TranslateDifficulty() {
        string product = "";
        switch (Difficulty) {
            case GameDifficulties.easy : product = "easy"; break;
            case GameDifficulties.normal : product = "norm"; break;
            case GameDifficulties.hard : product = "hard"; break;
            case GameDifficulties.test : product = "dev test"; break;
            default : product = "unknown"; break;
        }
        TranslatedDifficulty = product;
    }
    public virtual void UpdateSituation() {
        SituationDisplay.text = $"MainGameAttribute is runs the process. difficulty : {TranslatedDifficulty}";
    }
    public void ReloadOrExit() {
        if (State == GameStates.preGame) {
            if (gamepad != null) {
                gamepad.SetMotorSpeeds(0, 0); 
            }
            SceneManager.LoadScene("MainMenu");
        } else {
            if (gamepad != null) {
                gamepad.SetMotorSpeeds(0, 0); 
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
#endregion
