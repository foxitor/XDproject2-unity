#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
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
    
    [Header("Display")]
    public Text SituationDisplay;
    public string TranslatedDifficulty;

    [Header("Wearable")]
    public int WearableValue;
    public Sprite[] GameWearables;

    [Header("Ranks")]
    public RankVideo[] VideoClips;

    public static event System.Action OnGameStart;

    public static event System.Action OnLowGraphDraw;
    public static event System.Action OnMediumGraphDraw;
    public static event System.Action OnHighGraphDraw;

    void Start() {
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
    }
    public void AssignWearable(int value) {
        WearableValue = value;
    }
    public void RunGame() {
        OnGameStart?.Invoke();
        State = GameStates.game;
        TranslateDifficulty();
        preGameSetup.SetActive(false);
        GameplaySetup.SetActive(true);

        OnDetailDraw(DetalizationLevels.low);
        OnDetailDraw(DetalizationLevels.medium);
        OnDetailDraw(DetalizationLevels.high);
    }
    //Game
    void Update() {
        if (State == GameStates.preGame) return;
        if (State == GameStates.endGame) return;
        UpdateSituation();
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
}
#endregion
