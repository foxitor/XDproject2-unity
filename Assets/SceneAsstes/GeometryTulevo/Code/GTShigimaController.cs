using System.Collections; using System.Collections.Generic;
using UnityEngine; using UnityEngine.InputSystem;
using XD.Games;

public class GTShigimaController : MonoBehaviour {
    GeometryTulevoAttributes Attributes;
    Rigidbody2D Phy;
    bool canMove = true;

    public LayerMask obsticaleLayer;
    public GameObject messengeGroup;
    public GameObject cameraMessengeGroup;

    float currentSpeed, defaultSpeed = 5.2f, superSpeed = 12.96f, jumpForce = 10f, airRotationSpeed = -225f; 
    float groundTestRadius = 0.05f, dashRestore;
    bool dashAbility, onGround, frontObsticale;
    bool gravity, direction;

    Color[] powerColors = new Color[] {
        new Color(0.75f, 0.45f, 0.45f, 1f), //Activated
        new Color(0.45f, 0.45f, 0.45f, 1f) //Dully
    };

    GameObject visual; SpriteRenderer visualRenderer;
    Camera localCamera;
    AudioSource mySource;
    [Header("-sounds-")]
    public AudioClip[] orbHopSounds;
    public AudioClip[] dashSounds;
    public AudioClip[] coinSounds;
    public AudioClip[] radioPlaylist;
    public AudioClip deathCall;
    public AudioClip teleporing;
    bool isRadioMuted = false;
    public AudioSource RadioPlayer;
    int radioChannel;

    bool hasOrb, hasGravityOrb, hasBox;
    GameObject curBox; 
    List<GameObject> boxMemory = new List<GameObject>();
    bool deathPulsed;

    //Controlls
    GlobalControlls Controls;
    Gamepad gamepad;

    bool pressingEnter => (Input.GetKey(KeyCode.Return) || Controls.Joystick.Y.ReadValue<float>() > 0);
    bool pressingJump => (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || Controls.Joystick.A.ReadValue<float>() > 0);
    bool pressingDash => (Input.GetKey(KeyCode.Z) || Input.GetMouseButton(1) || Controls.Joystick.B.ReadValue<float>() > 0);

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
        gamepad = Gamepad.current;
        Attributes = GameObject.Find("Main Camera").GetComponent<GeometryTulevoAttributes>();
        mySource = gameObject.GetComponent<AudioSource>();
        visual = transform.GetChild(0).gameObject;
        visualRenderer = visual.GetComponent<SpriteRenderer>();
        localCamera = transform.GetChild(1).GetComponent<Camera>();
        Phy = this.gameObject.GetComponent<Rigidbody2D>();
        currentSpeed = defaultSpeed;
    }
    void Update() {
        ManageJumpos();
        if (dashAbility) { 
            if (dashRestore > 0) {
                dashRestore -= Time.deltaTime;
                float t = 1 - (dashRestore / 2.5f);
                visualRenderer.color = Color.Lerp(powerColors[1], powerColors[0], t);
            } else { visualRenderer.color = powerColors[0]; } 
        }
        if (dashRestore > 0) {
            dashRestore -= 1 * Time.deltaTime;
        }
        if (canMove && !frontObsticale) { 
            transform.Translate(transform.right * (direction ? -currentSpeed : currentSpeed) * Time.deltaTime); 
            Vector3 camPos = localCamera.transform.localPosition;
            localCamera.transform.localPosition = new Vector3(direction ? -0.5f : 0.5f, camPos.y, camPos.z);
        } if (!onGround) {
            float compiledRotation = airRotationSpeed * (gravity == direction ? 1 : -1);
            visual.transform.Rotate(0, 0, compiledRotation * Time.deltaTime);
        } else { SnapToNearestAngle(); }

        if (pressingEnter) {
            if (Attributes.collectedBullets >= Attributes.bulletGoal && !deathPulsed) {
                localCamera.gameObject.SetActive(false);
                Attributes.EndGame();
            }
        }
        if (canInteractWithRadio) {
            if (pressingRadioPlaylist) {
                radioChannel++;
                if (radioChannel > radioPlaylist.Length-1) { radioChannel = 0; }
                RadioPlayer.Stop();
                RadioPlayer.clip = radioPlaylist[radioChannel];
                RadioPlayer.Play();
            }
            if (pressingRadioMute) {
                isRadioMuted = !isRadioMuted;
                RadioPlayer.volume = isRadioMuted ? 0f : 1f;
            }
        }
    } 
    void FixedUpdate() {
        //Velosity Limits
        if (Phy.velocity.y < -24.2f) { Phy.velocity = new Vector2(Phy.velocity.x, -24.2f); }

        //Grounded
        onGround = Physics2D.OverlapBox(transform.position + Vector3.down * (gravity ? -1 : 1) * 0.5f, 
            Vector2.right * 1.1f + Vector2.up * groundTestRadius, 0, obsticaleLayer);
        //FrontObsticale
        Vector3 start_point = transform.position;
        Vector2 direction2D = direction ? -Vector2.right : Vector2.right;
        Vector3 direction3D = new Vector3(direction2D.x, direction2D.y, 0);
        float RayLength = 0.55f;
        Vector3 endPoint = start_point + direction3D * RayLength;

        RaycastHit2D hit = Physics2D.Raycast(start_point, direction2D, RayLength, obsticaleLayer);
        Debug.DrawLine(start_point, endPoint, Color.red);

        frontObsticale = hit.collider != null;

        if (frontObsticale && !deathPulsed) { StartCoroutine(Dying()); }
    }
    void SnapToNearestAngle() {
        if (visual != null) {
            Vector3 Rotation = visual.transform.rotation.eulerAngles;
            Rotation.z = Mathf.Round(Rotation.z / 90) * 90;
            visual.transform.rotation = Quaternion.Euler(Rotation);
        }
    }
    void ManageJumpos() {
        if (canMove) {
            if (pressingJump) {
                JumpControllPressed();
            }
            if (dashAbility) {
                if (pressingDash && dashRestore <= 0) {
                    StartCoroutine(Dash());
                }
            }
        }
    }
    void JumpControllPressed() {
        if (canMove) {
            if (!frontObsticale && curBox == null) {
                impulseJump(true);
                if (hasOrb) {
                    mySource.PlayOneShot(orbHopSounds[Random.Range(0, orbHopSounds.Length)]);
                    impulseJump(false);
                    Attributes.StartGamepadVibration(0.1f, 0.1f, 0.1f);
                    hasOrb = false;
                } if (hasGravityOrb) {
                    mySource.PlayOneShot(orbHopSounds[Random.Range(0, orbHopSounds.Length)]);
                    Attributes.StartGamepadVibration(0.1f, 0.1f, 0.1f);
                    Phy.velocity = Vector2.zero;
                    gravity = !gravity;
                    Phy.gravityScale = Phy.gravityScale * -1;
                    hasGravityOrb = false;
                }
            }
            else if (curBox != null) {
                curBox.GetComponent<GeometryTulevoTaggedObject>().OpenBox();
                boxMemory.Add(curBox);
                curBox = null;
            }
        }
    }
    public void impulseJump(bool checkSituation = false) {
        if (checkSituation && onGround) {
            Phy.velocity = Vector2.zero; Phy.AddForce(Vector2.up * (jumpForce) * (gravity ? -1 : 1), ForceMode2D.Impulse);
        } else if (!checkSituation) {
            Phy.velocity = Vector2.zero; Phy.AddForce(Vector2.up * (jumpForce) * (gravity ? -1 : 1), ForceMode2D.Impulse);
        }
    }
    //interact
    void OnTriggerEnter2D(Collider2D collision) {
        GeometryTulevoTaggedObject Modifier = collision.gameObject.GetComponent<GeometryTulevoTaggedObject>();
        if (Modifier != null) {
            switch (Modifier.Type) {
                case GTObjTypes.teleportPortal : 
                    transform.position = Modifier.teleportLink.position;
                    if (Modifier.layerChanger) { 
                        Attributes.layersSearched++; 
                        Attributes.OnNewLayer();
                        foreach (GameObject box in boxMemory) {
                            box.GetComponent<GeometryTulevoTaggedObject>().RestoreBox();
                        }
                        mySource.PlayOneShot(teleporing);
                        boxMemory.Clear();
                        if (Attributes.layersSearched >= Attributes.dashGoal) { dashAbility = true; }
                    }
                break;
                case GTObjTypes.spike :
                    if (!deathPulsed) {
                        StartCoroutine(Dying());
                    }
                break;
                case GTObjTypes.saw :
                    if (!deathPulsed) {
                        StartCoroutine(Dying());
                    } 
                break;
                case GTObjTypes.orb :
                    hasOrb = true;
                break;
                case GTObjTypes.interactive :
                    if (Modifier.objectSubtype == "Box") {
                        curBox = collision.gameObject;
                    }
                break;
                case GTObjTypes.reversePortal :
                    direction = !direction;
                break;
                case GTObjTypes.gravityOrb :
                    hasGravityOrb = true;
                break;
                case GTObjTypes.gravityPortal :
                    Phy.velocity = Vector2.zero;
                    gravity = !gravity; 
                    Phy.gravityScale = Phy.gravityScale * -1;
                break;
                case GTObjTypes.item :
                    string itemId = Modifier.objectSubtype.ToLower();
                    if (itemId == "bullet") {
                        Attributes.collectedBullets++;
                        mySource.PlayOneShot(coinSounds[Random.Range(0, coinSounds.Length)]);
                        Destroy(Modifier.gameObject);
                    }
                    Attributes.onItemPickup(itemId);
                break;
            }
        }
    } 
    void OnTriggerExit2D(Collider2D collision) {
        GeometryTulevoTaggedObject Modifier = collision.gameObject.GetComponent<GeometryTulevoTaggedObject>();
        if (Modifier != null) {
            switch (Modifier.Type) {
                case GTObjTypes.orb :
                    hasOrb = false;
                break;
                case GTObjTypes.gravityOrb :
                    hasGravityOrb = false;
                break;
                case GTObjTypes.interactive :
                    curBox = null;
                break;
            }
        }
    }
    IEnumerator Dash() {
        mySource.PlayOneShot(dashSounds[Random.Range(0, dashSounds.Length)]);
        currentSpeed = superSpeed;
        dashRestore = 2.5f;
        Attributes.StartGamepadVibration(0.25f, 0.25f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        currentSpeed = defaultSpeed;
    }
    IEnumerator Dying() {
        deathPulsed = true;
        canMove = false;
        RadioPlayer.Stop();
        Phy.bodyType = RigidbodyType2D.Static;

        Attributes.ManualMessageCamera("start", false);
        Attributes.ManualMessageCamera("win", false);
        Attributes.ManualMessageLocal("cheer", false);
        Attributes.ManualMessageCamera("loose", true);

        mySource.PlayOneShot(deathCall);
        
        Attributes.StartGamepadVibration(0f, 0.75f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        Attributes.StartGamepadVibration(0.75f, 0f, 0.2f);
        yield return new WaitForSeconds(4f - 0.2f);

        Attributes.ReloadOrExit();
    } 
}
