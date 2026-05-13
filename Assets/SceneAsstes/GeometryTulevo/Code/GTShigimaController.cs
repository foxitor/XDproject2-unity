using System.Collections;
using UnityEngine;
using XD.Games;

public class GTShigimaController : MonoBehaviour {
    GeometryTulevoAttributes Attributes;
    Rigidbody2D Phy;
    bool canMove = true;

    public LayerMask obsticaleLayer;
    public GameObject messengeGroup;

    float currentSpeed, defaultSpeed = 5.2f, superSpeed = 12.96f, jumpForce = 10f, airRotationSpeed = -225f; 
    float groundTestRadius = 0.05f, dashRestore;
    bool speedBoosted, onGround, frontObsticale;
    bool gravity, direction;

    Color[] powerColors = new Color[] {
        new Color(0.75f, 0.45f, 0.45f, 1f), //Activated
        new Color(0.45f, 0.45f, 0.45f, 1f) //Dully
    };

    GameObject visual; SpriteRenderer visualRenderer;
    Camera localCamera;
    AudioSource mySource;
    [Header("-sounds-")]
    public AudioClip orbHopSound;
    public AudioClip deathCall;
    public AudioClip[] dashSounds;

    bool hasOrb, hasGravityOrb, hasBox;
    GameObject curBox;
    bool deathPulsed;

    void Start() {
        Attributes = GameObject.Find("Main Camera").GetComponent<GeometryTulevoAttributes>();
        mySource = gameObject.GetComponent<AudioSource>();
        visual = transform.GetChild(0).gameObject;
        visualRenderer = visual.GetComponent<SpriteRenderer>();
        localCamera = transform.GetChild(1).GetComponent<Camera>();
        Phy = this.gameObject.GetComponent<Rigidbody2D>();
        currentSpeed = defaultSpeed;
    }
    void Update() {
        //ManageSpeed(); 
        ManageJumpos();
        if (speedBoosted) { 
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
    } 
    void FixedUpdate() {
        if(Phy.velocity.y < -24.2f) { Phy.velocity = new Vector2(Phy.velocity.x, -24.2f); }

        onGround = Physics2D.OverlapBox(transform.position + Vector3.down * (gravity ? -1 : 1) * 0.5f, Vector2.right * 1.1f + Vector2.up * groundTestRadius, 
            0, obsticaleLayer);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, (direction ? -Vector2.right : Vector2.right), 0.55f, obsticaleLayer);
        frontObsticale = hit.collider != null;
        
        Vector3 startPoint = transform.position;
        Vector2 direction2D = direction ? -Vector2.right : Vector2.right;
        Vector3 direction3D = new Vector3(direction2D.x, direction2D.y, 0);
        Vector3 endPoint = startPoint + direction3D * 0.55f;
        Debug.DrawLine(startPoint, endPoint, Color.red);

        if (frontObsticale && !deathPulsed) {
            deathPulsed = true;
            StartCoroutine(Dying());
        }
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
            if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))) {
                if (!frontObsticale && curBox == null) {
                    impulseJump(true);
                    if (hasOrb) { //VibrateController(0.025f, 0.025f, 0.1f); 
                        impulseJump(false); hasOrb = false; }
                    if (hasGravityOrb) {
                        //VibrateController(0.025f, 0.025f, 0.1f);
                        Phy.velocity = Vector2.zero;
                        gravity = !gravity; Phy.gravityScale = Phy.gravityScale * -1;
                        hasGravityOrb = false;
                    }
                } else if (curBox != null) {
            //        curBox.GetComponent<TulevoObjectModifier>().OpenBox();
                    curBox = null;
                }
            }
            if (speedBoosted) {
                if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Z)) {
                    if (dashRestore <= 0) {
                        StartCoroutine(Dash());
                    }
                }
            }
        }
    }
    public void impulseJump(bool checkGround = false) {
        if (checkGround && onGround) {
            Phy.velocity = Vector2.zero; Phy.AddForce(Vector2.up * (jumpForce) * (gravity ? -1 : 1), ForceMode2D.Impulse);
        } else if (!checkGround) {
            Phy.velocity = Vector2.zero; Phy.AddForce(Vector2.up * (jumpForce) * (gravity ? -1 : 1), ForceMode2D.Impulse);
        }
    }
    //#interact
    void OnTriggerEnter2D(Collider2D collision) {
        GeometryTulevoTaggedObject Modifier = collision.gameObject.GetComponent<GeometryTulevoTaggedObject>();
        if (Modifier != null) {
            switch (Modifier.Type) {
                case GTObjTypes.teleportPortal : 
                    transform.position = Modifier.teleportLink.position;
                    if (Modifier.layerChanger) { 
                        Attributes.layersSearched++; 
                        StartCoroutine(Messenge(0, 0.75f));
                    }
                break;
                case GTObjTypes.spike :
                    if (!deathPulsed) {
                        StartCoroutine(Dying()); 
                        deathPulsed = true;
                    }
                break;
                case GTObjTypes.saw :
                    if (!deathPulsed) {
                        StartCoroutine(Dying());
                        deathPulsed = true;
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
                    if (Modifier.objectSubtype == "Bullet") {
                        Attributes.collectedBullets++;
                        Destroy(Modifier.gameObject);
                    }
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
        //VibrateController(0.25f, 0.25f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        currentSpeed = defaultSpeed;
    }
    IEnumerator Messenge(int order, float liveTime) {
        messengeGroup.transform.GetChild(order).gameObject.SetActive(true);
        //Add Joystick vibration later
        yield return new WaitForSeconds(liveTime);
        messengeGroup.transform.GetChild(order).gameObject.SetActive(false);
    }
    IEnumerator Dying() {
        Phy.bodyType = RigidbodyType2D.Static; 
        //Game.Music.Stop();
        canMove = false; 
        //Game.DeadMessange.SetActive(true);
        mySource.PlayOneShot(deathCall);
        //VibrateController(0.2f, 0.2f, 0.5f);
        yield return new WaitForSeconds(2.5f);
        Attributes.ReloadOrExit();
    } 
}
