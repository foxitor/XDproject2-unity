#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class GeometryTulevoAttributes : MainGameAttributes {
    [Header("UniqeSettings")]
    public int collectedBullets;
    public int bulletGoal;
    public int layersSearched;

    public override void TranslateDifficulty() {
        string product = "";
        switch (Difficulty) {
            case GameDifficulties.easy : product = "Плачущая тапка"; break;
            case GameDifficulties.normal : product = "Адекват"; break;
            case GameDifficulties.hard : product = "Пароклятие Альфредо."; break;
            case GameDifficulties.test : product = "dev test"; break;
            default : product = "unknown"; break;
        }
        TranslatedDifficulty = product;
    }
    public override void UpdateSituation() {
        SituationDisplay.text = $"Патронов найдено : {collectedBullets} / {bulletGoal};\n Слоёв обысканно : {layersSearched}";
    }
}
#endregion