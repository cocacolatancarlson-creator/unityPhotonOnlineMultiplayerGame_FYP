using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;

    [SerializeField]
    private float _gravity = 1f;
    [SerializeField]
    private float _jumpSpeed = 20f;
    [SerializeField]
    private float _doubleJumpMultiplier = 5.0f;

    private float _directionY;
    public float speed = 6f;
    public float stop = 0f;
    public float turnSpeed = 90f;
    // FYP report 5.2.5: moving the mouse turns the character (and its camera) left or right.
    public float mouseTurnSpeed = 3f;
    private bool _canDoubleJump = false;
    Vector3 playerMovement;
    PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();

        //playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;



        if (Input.GetKey(KeyCode.W))
        {
            playerMovement = transform.forward * Input.GetAxis("Vertical") * speed;

        }
        else if(!Input.GetKey(KeyCode.W))
        {

            playerMovement = transform.forward * Input.GetAxis("Vertical") * stop;
        }



        if (characterController.isGrounded)
        {
            _canDoubleJump = true;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _directionY = _jumpSpeed;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && _canDoubleJump)
            {
                _directionY = _jumpSpeed * _doubleJumpMultiplier;
                _canDoubleJump = false;
            }
        }


        _directionY -= _gravity * Time.deltaTime;

        playerMovement.y = _directionY;

        characterController.Move(playerMovement * Time.deltaTime);





        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.RotateAround(transform.position, transform.up, 180f);

        }

        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime);

        // Mouse turning, as the report describes. A click locks the pointer to the game so the mouse can keep
        // turning without leaving the window (Esc releases it in the browser).
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseTurnSpeed);
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;




    }
}