using UnityEngine; using UnityEngine.Video;
namespace XD.Games {
    public enum GameStates {
        preGame, //Wearable & Difficulty Config
        game, //Gameplay
        endGame //Ranking Screen
    }
    public enum GameDifficulties {
        test,
        easy,
        normal,
        hard
    }
    public enum WearableStates {
        locked,
        unlocked,
        selected
    }
    public enum DetalizationLevels {
        low,
        medium,
        high
    }
    [System.Serializable]
    public class RankVideo {
        public string NameKey;
        public VideoClip Clip;
    }
    #region localGameData
    //GT
    public enum GTObjTypes {
        spike, saw,
        orb, gravityOrb,
        teleportPortal, reversePortal, gravityPortal,
        interactive, item,
        wallpaintedTip
    }
    #endregion
}
