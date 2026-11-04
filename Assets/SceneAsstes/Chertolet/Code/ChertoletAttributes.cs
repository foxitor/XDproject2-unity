#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class ChertoletAttributes : MainGameAttributes {
    [Header("UniqeSettings")]
    public ChertoletEvents CurrentEvent;
    public Slider ProgressBar;
    public float FlightTime = 0.0f;
    public float WholeFlightTime = 0.0f;
    public float EventDelay = 15.0f;
    public int CurrentLap = 0;
    public int MaxLaps = 5;

    public static event System.Action NewLayer;

    public override void TranslateDifficulty() {
        string product = "";
        switch (Difficulty) {
            case GameDifficulties.easy : 
                product = "Попутный ветер"; 
                EventDelay = 10.0f;
                MaxLaps = 4;
                break;
            case GameDifficulties.normal : 
                product = "Канон"; 
                EventDelay = 15.0f;
                MaxLaps = 5;
                break;
            case GameDifficulties.hard : 
                product = "Буря."; 
                EventDelay = 20.0f;
                MaxLaps = 4;
                break;
            case GameDifficulties.test : 
                product = "dev test"; 
                EventDelay = 3.0f;
                MaxLaps = 1;
                break;
            default : product = "unknown"; break;
        }
        TranslatedDifficulty = product;
    }
    public override void UpdateSituation() {
        FlightTime += Time.deltaTime;
        WholeFlightTime += Time.deltaTime;
        if (FlightTime >= EventDelay) {
            FlightTime = 0.0f;
            CurrentLap++;
        }
        ProgressBar.value = (WholeFlightTime/(EventDelay * MaxLaps));
        SituationDisplay.text = $"Вертолётик брбрбрбрбр; Тяжкость : {TranslatedDifficulty}\nДо нового События - {(EventDelay - FlightTime).ToString("F1")} сек, Событий {CurrentLap}/{MaxLaps}";
    }
    public void OnNewLayer() {
        NewLayer?.Invoke();
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
                    if (compiledChallanges == "") { compiledChallanges = "Нету..."; }
                    if (challangeRankAmount > 1) {
                        RankingText.text += $"\n\n‣Выполненные Испытания - \n{compiledChallanges}"; 
                    } else {
                        RankingText.text += $"\n\n‣Выполненное Испытание - \n{compiledChallanges}"; 
                    }
                    break;
                case 8 : 
                    string compiledObtains = "";
                    for (int step = 0; step <= challangeRankAmount-1; step++) { 
                        compiledObtains += (challangeRankByte[step].justObtained ? "• Да!" : "∘ Нет"); 
                        if (step <= challangeRankAmount-2) {
                            compiledObtains += ", ";
                        }
                    }
                    if (challangeRankAmount != 0) {
                        RankingText.text += $"\nВпервые выполненно(-ы)? : {compiledObtains};"; 
                    } else {
                        RankingText.text += " Увы.!";
                    }
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
            case 0 : endingRankByte.textReturn = "Одним Шигимой меньше!"; break;
            case 1 : endingRankByte.textReturn = "Он Подорвал Чертолёт"; break;
            case 2 : endingRankByte.textReturn = "Умер от Последнего Патрону..."; break;
            case 3 : endingRankByte.textReturn = "Чертолётчик из меня плохой."; break;
            case 4 : endingRankByte.textReturn = "Перехал Шигиму В доль и поперёк!"; break;
            case 5 : endingRankByte.textReturn = "Пчела Стилит Мой Килл..."; break;
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
        if (21 >= 25) {
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
[System.Serializable]
public enum ChertoletEvents {
    Rocks,
    Chicken,
    Liquid
}