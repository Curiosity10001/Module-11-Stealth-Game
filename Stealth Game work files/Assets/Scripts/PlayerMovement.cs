using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 moveDirection;
    Rigidbody rgbd;
    public float timer = 0;

    [SerializeField] float speed = 5f; 
    [SerializeField] float jumpForce= 5f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] Transform groundCheck;

    
    public Animator animator;
    float lastJump;
    float deltaTimeJump =1f;



    private void Awake()
    {
        rgbd = GetComponent<Rigidbody>();
        timer += Time.deltaTime;
     
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection =  Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal") ;
        timer += Time.deltaTime;
        if (timer >= lastJump + deltaTimeJump)
        {

            if (Input.GetButtonDown("Jump") && onGround() )
            {
                Jump();
                lastJump = Time.time;
              
            }
            
        }


    }
    private void FixedUpdate()
    {
        moveDirection.y = rgbd.velocity.y;
        rgbd.velocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);
        TurnToCamForwardDirection();
        
    }
    bool onGround()
    {
        return Physics.CheckSphere(groundCheck.position, 0.01f, 3);
    }

    void Jump()
    {
       rgbd.AddForce(Vector2.up * jumpForce, ForceMode.Impulse) ;
    }
    void TurnToCamForwardDirection()
    {
        
        Vector3 CamForward = Camera.main.transform.forward;

        CamForward.y = 0;

        Quaternion rgbdLookAt= Quaternion.LookRotation(CamForward);
        Quaternion rgbdRotationSpeed = Quaternion.RotateTowards(rgbd.rotation, rgbdLookAt, rotationSpeed * Time.fixedDeltaTime);
        rgbd.MoveRotation(rgbdLookAt);
        

    }
}
