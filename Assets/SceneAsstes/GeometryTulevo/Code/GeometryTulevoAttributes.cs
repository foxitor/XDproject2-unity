#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class GeometryTulevoAttributes : MainGameAttributes {
    [Header("UniqeSettings")]
    public GameObject bulletObject;
    public Transform[] spawns;
    [Space]
    public int collectedBullets;
    public int bulletGoal;
    public int layersSearched;
    [Space]
    public int bulletsPerLayer;
    public int dashGoal; 

    public override void TranslateDifficulty() {
        string product = "";
        switch (Difficulty) {
            case GameDifficulties.easy : 
                product = "Плачущая тапка"; 
                bulletGoal = 4;
                bulletsPerLayer = 2;
                break;
            case GameDifficulties.normal : product = "Адекват"; 
                bulletGoal = 7;
                bulletsPerLayer = 2;
                break;
            case GameDifficulties.hard : product = "Пароклятие Альфредо."; 
                bulletGoal = 14;
                bulletsPerLayer = 1;
                break;
            case GameDifficulties.test : product = "dev test"; 
                break;
            default : product = "unknown"; break;
        }
        TranslatedDifficulty = product;
    }
    public override void UpdateSituation() {
        SituationDisplay.text = $"Патронов найдено : {collectedBullets} / {bulletGoal}; Тяжкость : {TranslatedDifficulty}\nСлоёв обысканно : {layersSearched}";
    }
    public void OnNewLayer() {
        //Deleting previous bullets
        foreach (Transform spawn in spawns) {
            if (spawn.childCount > 0) {
                Destroy(spawn.GetChild(0).gameObject);
            }
        }
        //Spawning the bullets
        for (int i = 0; i < bulletsPerLayer; i++) {
            int pick = Random.Range(0, spawns.Length);
            Vector3 defPos = spawns[pick].position;
            Vector3 spawnPos = new Vector3(defPos.x + Random.Range(-0.25f, 0.25f), defPos.y + Random.Range(-0.25f, 0.25f), defPos.z);
            Instantiate(bulletObject, spawnPos, spawns[pick].rotation, spawns[pick]);
        }
    }
}
#endregion