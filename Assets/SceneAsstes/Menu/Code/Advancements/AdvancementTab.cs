using UnityEngine;

public class AdvancementTab : MonoBehaviour {
    public GameObject[] Games;
    public int SelectedGame;

    void Start() {
        UpdateView();
    }
    public void Operate(int add) {
        SelectedGame += add;
        UpdateView();
    }
    void UpdateView() {
        if (SelectedGame < 0) { SelectedGame = Games.Length-1; }
        if (SelectedGame > Games.Length-1) { SelectedGame = 0; }
        foreach (GameObject gameTab in Games) { gameTab.SetActive(false); }
        Games[SelectedGame].SetActive(true);
    }
}
