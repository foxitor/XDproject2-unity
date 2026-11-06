using System.Collections; using System.Collections.Generic;
using UnityEngine; using UnityEngine.InputSystem;
using XD.Games;

public class ChertoletController : MonoBehaviour {
    ChertoletAttributes Attributes;
    Rigidbody2D Phy;
    bool canMove = true;

    GameObject visual; SpriteRenderer visualRenderer;
    AudioSource mySource;

    public float FlapStrength = 3.5f;
    float currentRotation;
    public float flapRotationLimit = 30f;

    public GameObject feedMessengeGroup;
    public GameObject cameraMessengeGroup;

    public LayerMask obsticaleLayer;
    bool deathPulsed;
    public AudioClip[] deathCalls;

    //Controlls
    GlobalControlls Controls;
    Gamepad gamepad;

    bool pressingEnter => (Input.GetKey(KeyCode.Return) || Controls.Joystick.Y.ReadValue<float>() > 0);
    bool pressingJump => (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Controls.Joystick.A.triggered);

    bool pressingRadioPlaylist => (Input.GetKeyDown(KeyCode.M) || Controls.Joystick.Left.triggered);
    bool pressingRadioMute => (Input.GetKeyDown(KeyCode.N) || Controls.Joystick.Up.triggered);

    bool canInteractWithRadio => (!deathPulsed);

    void Awake() { 
        Controls = new GlobalControlls();
    }
    void OnEnable() { 
        Controls.Joystick.Enable();
    } 
    void OnDisable() { 
        Controls.Joystick.Disable(); 
    }

    void Start() {
        Attributes = GameObject.Find("Main Camera").GetComponent<ChertoletAttributes>();
        mySource = gameObject.GetComponent<AudioSource>();
        visual = transform.GetChild(0).gameObject;
        visualRenderer = visual.GetComponent<SpriteRenderer>();
        Phy = gameObject.GetComponent<Rigidbody2D>();
        obsticaleLayer = LayerMask.GetMask("Obsticale");
    }
    void Update() {
        if (pressingJump) { 
            Flap(); 
        }

        float targetRotation = Mathf.Lerp(-flapRotationLimit, flapRotationLimit, (Phy.velocity.y + 5) / 10);
        targetRotation = Mathf.Clamp(targetRotation, -flapRotationLimit, flapRotationLimit);
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, 2 * Time.deltaTime);

        visual.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        if (!canMove) { 
            Phy.simulated = false; 
        } else { 
            Phy.simulated = true; 
        }
        if (Attributes.CurrentLap >= Attributes.MaxLaps) {
            cameraMessengeGroup.transform.GetChild(2).gameObject.SetActive(true);
            if (pressingEnter) {
                Attributes.EndGame();
            }
        }
    } 
    void Flap() {
        if (canMove) {
            Phy.velocity = Vector2.up * FlapStrength;
        }
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if ((obsticaleLayer.value & (1 << collision.gameObject.layer)) > 0) {
            if (!deathPulsed) {
                StartCoroutine(Dying());
            }
        }
        ChertoletTaggedObject Modifier = collision.gameObject.GetComponent<ChertoletTaggedObject>();
        if (Modifier != null) {
            switch (Modifier.Type) {
                case CLObjTypes.rock :
                case CLObjTypes.egg :
                case CLObjTypes.liqud :
                    if (!deathPulsed) {
                        StartCoroutine(Dying());
                    }
                break;
            }
        }
    }
    IEnumerator Dying() {
        deathPulsed = true;
        canMove = false;
        Phy.bodyType = RigidbodyType2D.Static;

        Attributes.ManualMessageCamera("start", false);
        Attributes.ManualMessageCamera("win", false);
        Attributes.ManualMessageCamera("loose", true);

        mySource.PlayOneShot(deathCalls[Random.Range(0, deathCalls.Length)]);
        
        Attributes.StartGamepadVibration(1f, 1f, 1f);
        yield return new WaitForSeconds(1.5f);

        Attributes.ReloadOrExit();
    }
}
