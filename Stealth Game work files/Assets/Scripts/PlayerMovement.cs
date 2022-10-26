using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 moveDirection;
    Rigidbody rgbd;

    [Header (" Public Parameter for multi-scrip")]
    public float timer = 0;

    [Header ("Movement parameters")]
    [SerializeField] float speed = 5f; 
    [SerializeField] float rotationSpeed = 60f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isJumping = false;
  
    float lastJump;
    float deltaTimeJump = 1f;

    [Header("Floor Detection")]
    [SerializeField] bool isGrounded = false;
    [SerializeField] float maxDistance = 1.9f;
    [SerializeField] Transform frontRightRay;
    [SerializeField] Transform frontLeftRay;
    [SerializeField] Transform backRightRay;
    [SerializeField] Transform backLeftRay;
    
    Vector3 onTheGround;   
    RaycastHit touchGround1;
    RaycastHit touchGround2;
    RaycastHit touchGround3;
    RaycastHit touchGround4;
    Ray rayToGround1;
    Ray rayToGround2;
    Ray rayToGround3;
    Ray rayToGround4;
    bool HitGround1;
    bool HitGround2;
    bool HitGround3;
    bool HitGround4;



    public Animator animator;
   



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

        Grounded();
        if (rgbd.position.y <= onTheGround.y + 0.1f && rgbd.position.y >= onTheGround.y - 0.1f)
        {
            isGrounded = true;
            isJumping = false;
        }
        else isGrounded = false;

        if (timer >= lastJump + deltaTimeJump)
        {

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                
                Jump();
                lastJump = Time.time;
              
            }
            
        }

        Debug.Log("rgbd y" + rgbd.position.y + " raycast Y " + onTheGround.y);
    }
    private void FixedUpdate()
    {
        moveDirection.y = rgbd.velocity.y;
        rgbd.velocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);
        TurnToCamForwardDirection();
        
    }

    void Jump()
    {
       rgbd.AddForce(Vector2.up * jumpForce) ;
       isJumping = true;

    }
    void TurnToCamForwardDirection()
    {
        
        Vector3 CamForward = Camera.main.transform.forward;

        CamForward.y = 0;

        Quaternion rgbdLookAt= Quaternion.LookRotation(CamForward);
        Quaternion rgbdRotationSpeed = Quaternion.RotateTowards(rgbd.rotation, rgbdLookAt, rotationSpeed * Time.fixedDeltaTime);
        rgbd.MoveRotation(rgbdLookAt);
        rgbd.rotation = rgbdRotationSpeed;
        

    }

    public void  Grounded()
    {
        
        rayToGround1 = new Ray(frontRightRay.position, Vector3.down);
        rayToGround2 = new Ray(frontLeftRay.position, Vector3.down);
        rayToGround3 = new Ray(backRightRay.position, Vector3.down);
        rayToGround4 = new Ray(backLeftRay.position, Vector3.down);

        HitGround1 = Physics.Raycast(rayToGround1, out touchGround1, maxDistance, LayerMask.GetMask("Ground"));
        HitGround2 = Physics.Raycast(rayToGround2, out touchGround2, maxDistance, LayerMask.GetMask("Ground"));
        HitGround3 = Physics.Raycast(rayToGround3, out touchGround3, maxDistance, LayerMask.GetMask("Ground"));
        HitGround4 = Physics.Raycast(rayToGround4, out touchGround4, maxDistance, LayerMask.GetMask("Ground"));
        
            if (HitGround1 && HitGround2 && HitGround3 && HitGround4)
            {
            onTheGround = (touchGround1.point + touchGround2.point+ touchGround3.point + touchGround4.point) / 4 ;
            }
        
       
        
    }
  



}
