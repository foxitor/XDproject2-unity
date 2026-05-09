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
        State = GameStates.game;
        TranslateDifficulty();
    }

    //Update
    void Update() {
        if (State == GameStates.preGame) return;
        if (State == GameStates.endGame) return;
        UpdateSituation();
    }
    //ReadableDifficulty
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
    //DisplaysTopText
    public virtual void UpdateSituation() {
        SituationDisplay.text = $"MainGameAttribute is runs the process. difficulty : {TranslatedDifficulty}";
    }
}
#endregion
