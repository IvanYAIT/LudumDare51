using UnityEngine;
using AFPC;

/// <summary>
/// Example of setup AFPC with Lifecycle, Movement and Overview classes.
/// </summary>
public class Hero : MonoBehaviour {

    /* UI Reference */
    public HUD HUD;

    /* Lifecycle class. Damage, Heal, Death, Respawn... */
    public Lifecycle lifecycle;

    /* Movement class. Move, Jump, Run... */
    public Movement movement;

    /* Overview class. Look, Aim, Shake... */
    public Overview overview;

    private float timer;
    public int item = 0;
    [SerializeField] private GameObject bed;

    /* Optional assign the HUD */
    private void Awake () {
        if (HUD) {
            HUD.hero = this;
        }
    }

    /* Some classes need to initizlize */
    private void Start () {

        /* a few apllication settings for more smooth. This is Optional. */
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;

        /* Initialize lifecycle and add Damage FX */
        lifecycle.Initialize();
        lifecycle.AssignDamageAction (DamageFX);

        /* Initialize movement and add camera shake when landing */
        movement.Initialize();
        movement.AssignLandingAction (()=> overview.Shake(0.5f));
    }

    private void Update () {

        /* Read player input before check availability */
        ReadInput();

        /* Block controller when unavailable */
        if (!lifecycle.Availability()) return;

        /* Mouse look state */
        overview.Looking();

        /* Change camera FOV state */
        //overview.Aiming();

        /* Shake camera state. Required "physical camera" mode on */
        overview.Shaking();

        /* Control the speed */
        movement.Running();

        /* Control the jumping, ground search... */
        movement.Jumping();

        /* Control the health and shield recovery */
        //lifecycle.Runtime();
    }

    private void FixedUpdate () {

        /* Block controller when unavailable */
        if (!lifecycle.Availability()) return;

        /* Physical movement */
        movement.Accelerate();

        /* Physical rotation with camera */
        overview.RotateRigigbodyToLookDirection (movement.rb);
    }

    private void LateUpdate () {

        /* Block controller when unavailable */
        if (!lifecycle.Availability()) return;

        /* Camera following */
        overview.Follow (transform.position);
    }

    private void ReadInput () {
        overview.lookingInputValues.x = Input.GetAxis("Mouse X");
        overview.lookingInputValues.y = Input.GetAxis("Mouse Y");
        overview.aimingInputValue = Input.GetMouseButton(1);
        movement.movementInputValues.x = Input.GetAxis("Horizontal");
        movement.movementInputValues.y = Input.GetAxis("Vertical");
        movement.jumpingInputValue = Input.GetButtonDown("Jump");
        movement.runningInputValue = Input.GetKey(KeyCode.LeftShift);
    }

    private void DamageFX () {
        if (HUD) HUD.DamageFX();
        overview.Shake(0.75f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RadiationZone1"))
        {
            if (timer >=  500)
            {
                lifecycle.DamageFromRadiation(1);
                timer = 0;
            }
            else
            {
                timer += 1;
            }
        }

        if(other.CompareTag("Item") && Input.GetKey(KeyCode.E))
        {
            Destroy(other.gameObject);
            item += 1;
            ProjectManager.instance.item = item;
        }

        if (other.CompareTag("NPC") && Input.GetKey(KeyCode.E))
        {
            Time.timeScale = 0;
            bed.SetActive(true);
            ProjectManager.instance.isDialog = true;
        }

        if (other.CompareTag("Bed") && Input.GetKey(KeyCode.E))
        {
            Time.timeScale = 0;
            lifecycle.Heal(lifecycle.referenceHealth - lifecycle.GetHealthValue());
            other.gameObject.SetActive(false);
            ProjectManager.instance.isRest = true;
        }

        if (other.CompareTag("Elevator") && Input.GetKey(KeyCode.E) && !ProjectManager.instance.isElevatorActive)
        {
            ProjectManager.instance.isElevatorActive = true;
        }
    }
}
