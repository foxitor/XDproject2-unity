using UnityEngine; using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class FunkinProgressBar : MonoBehaviour {
    public Animator MaxIconAnimator;
    public Animator MinIconAnimator;
    public bool canBeSad = true;
    public bool canBeHappy = true;
    Slider slider;
    [Header("Tresholds")]
    [Range(0.0f, 0.5f)] public float EndTreshold = 0.25f;
    [Range(0.0f, 0.5f)] public float StartTreshold = 0.25f;

    enum ProgressBarStates { Neutral, NearMin, NearMax };
    ProgressBarStates BarState = ProgressBarStates.Neutral;

    bool hasMaxIcon => MaxIconAnimator != null;
    bool hasMinIcon => MinIconAnimator != null;

    void Start() {
        slider = gameObject.GetComponent<Slider>();
        CheckState(true);
    }
    void Update() {
        CheckState(false);
    }
    public virtual void CheckState(bool forcePulse) {
        float progress = slider.value;

        float range = slider.maxValue - slider.minValue;
        float max_bound = slider.maxValue - (range * EndTreshold);
        float min_bound = slider.minValue + (range * StartTreshold);

        if (progress >= max_bound) {
            WhileNearMax();
            if (BarState != ProgressBarStates.NearMax || forcePulse) {
                OnPulseNearMax();
                BarState = ProgressBarStates.NearMax;
            }
        } else if (progress <= min_bound) {
            WhileNearMin();
            if (BarState != ProgressBarStates.NearMin || forcePulse) {
                OnPulseNearMin();
                BarState = ProgressBarStates.NearMin;
            }
        } else {
            WhileNeutral();
            if (BarState != ProgressBarStates.Neutral || forcePulse) {
                OnPulseNeutral();
                BarState = ProgressBarStates.Neutral;
            }
        }
    }
    public virtual void WhileNearMax() { /*override if needed.*/ }
    public virtual void OnPulseNearMax() {
        ResetTriggers();
        if (hasMaxIcon && canBeHappy) MaxIconAnimator.SetTrigger("GoHappy");
        if (hasMinIcon && canBeSad) MinIconAnimator.SetTrigger("GoSad");
    }
    public virtual void WhileNearMin() { /*override if needed.*/ }
    public virtual void OnPulseNearMin() {
        ResetTriggers();
        if (hasMaxIcon && canBeSad) MaxIconAnimator.SetTrigger("GoSad");
        if (hasMinIcon && canBeHappy) MinIconAnimator.SetTrigger("GoHappy");
    }
    public virtual void WhileNeutral() { /*override if needed.*/ }
    public virtual void OnPulseNeutral() {
        ResetTriggers();
        if (hasMaxIcon) MaxIconAnimator.SetTrigger("GoStill");
        if (hasMinIcon) MinIconAnimator.SetTrigger("GoStill");
    }
    public virtual void ResetTriggers() {
        if (hasMaxIcon) {
            MaxIconAnimator.ResetTrigger("GoStill");
            MaxIconAnimator.ResetTrigger("GoHappy");
            MaxIconAnimator.ResetTrigger("GoSad");
        }
        if (hasMinIcon) {
            MinIconAnimator.ResetTrigger("GoStill");
            MinIconAnimator.ResetTrigger("GoHappy");
            MinIconAnimator.ResetTrigger("GoSad");
        }
    }
}