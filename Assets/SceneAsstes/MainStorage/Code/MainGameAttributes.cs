#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEngine.SceneManagement; using UnityEngine.InputSystem; using UnityEngine.Video;
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
    [Space]
    public VideoPlayer VideoScreen;
    public Text RankingText;
    public GameObject RankingScreenClosed;
    
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
    public VideoClip[] VideoClips;
    public AudioClip[] RankingSounds;
    public string[] endings;

    [HideInInspector]public RankScreenAttribute endingRankByte;
    [HideInInspector]public RankScreenAttribute seasonRankByte;
    [HideInInspector]public RankScreenAttribute[] challangeRankByte = new RankScreenAttribute[10];
    [HideInInspector]public RankScreenAttribute difficultyRankByte;
    [HideInInspector]public int challangeRankAmount; 

    //Controlls
    GlobalControlls Controls; Gamepad gamepad;

    public static event System.Action OnGameStart;
    public static event System.Action OnLateGameStart;

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
        if (State == GameStates.preGame) {
            oneShotWearable = null;
            OnGamepadWearableScroll?.Invoke();
            if (oneShotWearable == null) {
                SelectDefaultWearable?.Invoke();
            } else {
                oneShotWearable.OnPress();
            }
        }
    }
    void AddDifficulty() {
        if (State == GameStates.preGame) {
            switch (Difficulty) {
                case GameDifficulties.easy : AssignDifficulty(1); break;
                case GameDifficulties.normal : AssignDifficulty(2); break;
                case GameDifficulties.hard : AssignDifficulty(0); break;
            }
        }
    }
    public void RunGame() {
        if (State == GameStates.preGame) {
            OnGameStart?.Invoke();
            State = GameStates.game;
            TranslateDifficulty();
            preGameSetup.SetActive(false);
            GameplaySetup.SetActive(true);
            OnLateGameStart?.Invoke();
            gameObject.GetComponent<Camera>().enabled = false;

            OnDetailDraw(DetalizationLevels.low);
            OnDetailDraw(DetalizationLevels.medium);
            OnDetailDraw(DetalizationLevels.high);
            if (WearableValue <= 0) { WearableValue = 1; } 
            PlayerRenderer.sprite = GameWearables[WearableValue - 1]; 
        }
    }
    //Game
    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            ReloadOrExit();
        }
        if (State == GameStates.preGame) {
            return;
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
    public virtual void EndGame() {
        if (State == GameStates.game) {
            State = GameStates.endGame;

            GameplaySetup.SetActive(false);
            RankingSetup.SetActive(true);
            
            VideoScreen.clip = VideoClips[WearableValue - 1];
            StartCoroutine(RunEnding());
        }
    }
    public virtual IEnumerator RunEnding() {
        RankingScreenClosed.SetActive(true);
        VideoScreen.enabled = false;
        VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[0]);
        yield return new WaitForSeconds(1.25f);
        RankingScreenClosed.SetActive(false);
        VideoScreen.enabled = true;
        VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[1]);
        SaveSessionProgress();
        int slides = 6;
        for (int i = 0; i < slides; i++) {
            switch(i) {
                case 0 : RankingText.text = ""; break;
                case 1 : RankingText.text += "Hey There!"; break;
                case 2 : RankingText.text += "\nThis scene is running by MainAttributes."; break;
                case 3 : RankingText.text += "\nOverride it in new Attributes."; break;
                case 4 : RankingText.text += "\nRewards saved;"; break;
                case 5 : RankingText.text += "\nEscape/Down(Gamepad) to escape."; break;
            }
            if (i == slides-1) {
                VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[3]);
            } else {
                VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[2]);
            }
            yield return new WaitForSeconds(0.75f);
        }
    }
    public virtual void SaveSessionProgress() {
        //saving alghorytm in override.
    }
    public void ReloadOrExit() {
        if (State == GameStates.preGame || State == GameStates.endGame) {
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
