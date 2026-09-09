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

    public static event System.Action NewLayer;

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
        NewLayer?.Invoke();
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
    public override void EndGame() {
        if (State == GameStates.game) {
            State = GameStates.endGame;

            GameplaySetup.SetActive(false);
            RankingSetup.SetActive(true);
            
            VideoScreen.clip = VideoClips[WearableValue - 1];
            StartCoroutine(RunEnding());
        }
    }
    public override IEnumerator RunEnding() {
        RankingScreenClosed.SetActive(true);
        VideoScreen.enabled = false;
        VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[0]);
        yield return new WaitForSeconds(1.25f);
        RankingScreenClosed.SetActive(false);
        VideoScreen.enabled = true;
        VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[1]);
        SaveSessionProgress();
        int slides = 10;
        for (int i = 0; i < slides; i++) {
            switch(i) {
                case 0 : RankingText.text = ""; break;
                case 1 : RankingText.text += $"‣Рандомная концовка - ''{endingRankByte.textReturn}''"; break;
                case 2 : RankingText.text += $"\nНовая концовка? : {(endingRankByte.justObtained ? "• Да!" : "∘ Нет")};"; break;

                case 3 : RankingText.text += $"\n\n‣Сложность Забега - ''{difficultyRankByte.textReturn}''"; break;
                case 4 : RankingText.text += $"\nНовая сложность? : {(difficultyRankByte.justObtained ? "• Да!" : "∘ Нет")};"; break;

                case 5 : RankingText.text += $"\n\n‣Сезон Забега - ''{seasonRankByte.textReturn}''"; break;
                case 6 : RankingText.text += $"\nНовый сезон? : {(seasonRankByte.justObtained ? "• Да!" : "∘ Нет")};"; break;

                case 7 : 
                    string compiledChallanges = "";
                    for (int step = 0; step <= challangeRankAmount-1; step++) { 
                        compiledChallanges += challangeRankByte[step].textReturn; 
                        if (step <= challangeRankAmount-2) {
                            compiledChallanges += ", ";
                        }
                    }
                    if (compiledChallanges == "") { compiledChallanges = "Нету"; }
                    RankingText.text += $"\n\n‣Выполненное(-ые) Испытание(-я) - \n{compiledChallanges}"; 
                    break;
                case 8 : 
                    string compiledObtains = "";
                    for (int step = 0; step <= challangeRankAmount-1; step++) { 
                        compiledObtains += (challangeRankByte[step].justObtained ? "• Да!" : "∘ Нет"); 
                        if (step <= challangeRankAmount-2) {
                            compiledObtains += ", ";
                        }
                    }
                    RankingText.text += $"\nВпервые выполненно(-ы)? : {compiledObtains};"; 
                break;

                case 9 : 
                    RankingText.text += "\n\n>‣Заработанное сохраненно;"; 
                    RankingText.text += "\nEscape / Down(геймпад) что бы выйти."; 
                break;
            }
            if (i == slides-1) {
                VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[3]);
            } else {
                VideoScreen.GetComponent<AudioSource>().PlayOneShot(RankingSounds[2]);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public override void SaveSessionProgress() {
        //RandomEndingRank
        int randomSeed = Random.Range(0, endings.Length);
        string endVariant = endings[randomSeed];
        //>Translate
        switch (randomSeed) {
            case 0 : endingRankByte.textReturn = "Алё пушкин, я дантес"; break;
            case 1 : endingRankByte.textReturn = "Я затулил Скулика."; break;
            case 2 : endingRankByte.textReturn = "У меня патроны закончились..."; break;
            case 3 : endingRankByte.textReturn = "Кинул распальцовку"; break;
            case 4 : endingRankByte.textReturn = "ЭУ МА БОЙЯ!!1!"; break;
            case 5 : endingRankByte.textReturn = "Скулик сдался!"; break;
            default : endingRankByte.textReturn = "Unknown error happened, contact the dev."; break;
        }
        //>isNew?
        if (AdvancedPPfs.Core.isNull(endVariant)) { endingRankByte.justObtained = true; } else { endingRankByte.justObtained = false; }
        PlayerPrefs.SetInt(endVariant, 1);

        //DifficultyRank
        string difficultyVariant = "";
        difficultyRankByte.textReturn = TranslatedDifficulty;
        //>TranslateToPreff
        switch (Difficulty) {
            case GameDifficulties.easy : difficultyVariant = "GTD-easy"; break;
            case GameDifficulties.normal : difficultyVariant = "GTD-normal"; break;
            case GameDifficulties.hard : difficultyVariant = "GTD-hard"; break;
            case GameDifficulties.test : difficultyVariant = "kill"; break;
        }
        //>isNew?
        if (AdvancedPPfs.Core.isNull(difficultyVariant)) { difficultyRankByte.justObtained = true; } else { difficultyRankByte.justObtained = false; }
        PlayerPrefs.SetInt(difficultyVariant, 1);

        //SeasonRank
        string seasonVariant = "";
        //>Translates
        switch (PlayerPrefs.GetInt("Season")) {
            case 0 : seasonRankByte.textReturn = "Обыденность"; seasonVariant = "GTS-neutral"; break;
            case 1 : seasonRankByte.textReturn = "Снежность"; seasonVariant = "GTS-cold"; break;
            case 2 : seasonRankByte.textReturn = "Жаркость"; seasonVariant = "GTS-warm"; break;
        }
        //>isNew?
        if (AdvancedPPfs.Core.isNull(seasonVariant)) { seasonRankByte.justObtained = true; } else { seasonRankByte.justObtained = false; }
        PlayerPrefs.SetInt(seasonVariant, 1);

        //ChallangeRank
        challangeRankAmount = 0;
        if (collectedBullets >= 25) {
            if (AdvancedPPfs.Core.isNull("GTC-LeftAlone")) { 
                challangeRankByte[challangeRankAmount].justObtained = true; 
            } else { 
                challangeRankByte[challangeRankAmount].justObtained = false; 
            }
            challangeRankByte[challangeRankAmount].textReturn = "''Но никто не пришёл''";
            PlayerPrefs.SetInt(("GTC-LeftAlone"), 1);
            challangeRankAmount++;
        }
    }
}
#endregion