using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 moveDirection;
    Rigidbody rgbd;

    [Header(" Public Parameter for multi-scrip")]
    public float timer = 0;

    [Header("Movement Parameters")]
    [SerializeField] float speed = 5f;
    [SerializeField] float rotationSpeed = 90f;

    float axisX;
    float axisY;

    [Header("Jump & Fall parameter")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isJumping = false;
    [SerializeField] float jumpDuration = 0.03f;

    [SerializeField] float downForce = 2.5f;
    [SerializeField] bool isFalling = false;
    [SerializeField] bool fallToGround = false;


    float lastJump;



    [Header("Floor Detection")]
    [SerializeField] bool isGrounded = false;
    [SerializeField] float maxDistance = 20f;
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


    [Header("Animation && Animator")]
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
        timer += Time.deltaTime;
        JumpAndFall();
        AnimationStateActivation();
        TurnToCamForwardDirection();
        GroundMovement();
        Grounded();

    }
    void AnimationStateActivation()
    {
        //Axis values to know if walk or run if playing with gamepad (values are 0/1 if on keyboard so run only)
        animator.SetFloat("X", axisX);
        animator.SetFloat("Y", axisY);

        //Pre existing bools to reuse for animator
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("FallToGround", fallToGround);


        // Axis state for Idle/Moving when not jumping
        if (isGrounded && !isJumping && !isFalling && !fallToGround && animator.GetBool("StealthMode")==false)
        {
            animator.SetBool("isMoving", true);
        }

        // to prevent  Idle/Moving when Falling
        if (isFalling)
        {
            animator.SetBool("isMoving", false);
        }
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("isJumping");
        }

        //Stealth activation button down if up stealth mode desactivated
        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("StealthMode",true);
            animator.SetBool("isMoving", false);
        } else animator.SetBool("StealthMode", false);

    }
    private void FixedUpdate()
    {
        // As long as nothing impact velocity the rigidbodystays on ground
        moveDirection.y = rgbd.velocity.y;

        rgbd.velocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);

    }

    void GroundMovement()
    {
        axisY = Input.GetAxis("Vertical");
        axisX = Input.GetAxis("Horizontal");

        //Movement relative to camera view
        moveDirection = Camera.main.transform.forward * axisY + Camera.main.transform.right * axisX;

    }
    public void Grounded()
    {
        // Rays to detect Ground
        rayToGround1 = new Ray(frontRightRay.position, Vector3.down);
        rayToGround2 = new Ray(frontLeftRay.position, Vector3.down);
        rayToGround3 = new Ray(backRightRay.position, Vector3.down);
        rayToGround4 = new Ray(backLeftRay.position, Vector3.down);

        //Bool to  know that a collider has been hit 
        HitGround1 = Physics.Raycast(rayToGround1, out touchGround1, maxDistance, LayerMask.GetMask("Ground"));
        HitGround2 = Physics.Raycast(rayToGround2, out touchGround2, maxDistance, LayerMask.GetMask("Ground"));
        HitGround3 = Physics.Raycast(rayToGround3, out touchGround3, maxDistance, LayerMask.GetMask("Ground"));
        HitGround4 = Physics.Raycast(rayToGround4, out touchGround4, maxDistance, LayerMask.GetMask("Ground"));

        //If  hit get the information of the center of the 4 points : it will be our comparison point to know if we are grounded
        if (HitGround1 && HitGround2 && HitGround3 && HitGround4)
        {
            onTheGround = (touchGround1.point + touchGround2.point + touchGround3.point + touchGround4.point) / 4;
        }

        // Rigid body Y position comparison to know if grounded /falling /jumping
        if (rgbd.position.y <= onTheGround.y + 0.1f && rgbd.position.y >= onTheGround.y - 0.1f)
        {
            isGrounded = true;
            isJumping = false;
            isFalling = false;
            fallToGround = false;
        }
        else
        {
            isGrounded = false;
            rgbd.AddForce(Vector3.down * downForce);
        }

        //Detection of ground in stealth mode , ray need to be rotated
        if (animator.GetBool("StealthMode") == true)
        {
            rayToGround1 = new Ray(frontRightRay.position, Vector3.forward);
            rayToGround2 = new Ray(frontLeftRay.position, Vector3.forward);
            rayToGround3 = new Ray(backRightRay.position, Vector3.forward);
            rayToGround4 = new Ray(backLeftRay.position, Vector3.forward);

            //Bool to  know that a collider has been hit 
            HitGround1 = Physics.Raycast(rayToGround1, out touchGround1, maxDistance, LayerMask.GetMask("Ground"));
            HitGround2 = Physics.Raycast(rayToGround2, out touchGround2, maxDistance, LayerMask.GetMask("Ground"));
            HitGround3 = Physics.Raycast(rayToGround3, out touchGround3, maxDistance, LayerMask.GetMask("Ground"));
            HitGround4 = Physics.Raycast(rayToGround4, out touchGround4, maxDistance, LayerMask.GetMask("Ground"));

            //If  hit get the information of the center of the 4 points : it will be our comparison point to know if we are grounded
            if (HitGround1 && HitGround2 && HitGround3 && HitGround4)
            {
                onTheGround = (touchGround1.point + touchGround2.point + touchGround3.point + touchGround4.point) / 4;
            }

            // Rigid body X position comparison to know if grounded /falling /jumping
            if (rgbd.position.y <= onTheGround.y + 0.1f && rgbd.position.y >= onTheGround.y - 0.1f)
            {
                isGrounded = true;
                isJumping = false;
                isFalling = false;
                fallToGround = false;
            }
            else
            {
                isGrounded = false;
                rgbd.AddForce(Vector3.down * downForce);
            }
        }
    }
    void JumpAndFall()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded )
        {
            isJumping = true;
            rgbd.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            lastJump = Time.time;
            isGrounded = false;
            isFalling = false;
        }

        //to prevent bug of non-exit of is jumping
        if (animator.GetCurrentAnimatorStateInfo(LayerMask.NameToLayer("Player")).IsName("Jump") == true && isGrounded)
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(LayerMask.NameToLayer("Player")).IsName("Jump"));
            rgbd.AddForce(Vector3.down * downForce);
            isFalling = true;
            fallToGround = true;
        }

        //Jump +land
        if (rgbd.position.y > onTheGround.y + 0.5f && timer > lastJump && timer < lastJump + jumpDuration && rgbd.velocity.y != 0)
        {
            rgbd.AddForce(Vector3.down * downForce);
            isFalling = true;
            fallToGround = true;
            isGrounded = false;
        }

        // Fall
        if (rgbd.position.y > onTheGround.y + 0.5f && timer > lastJump + jumpDuration && rgbd.velocity.y != 0)
        {
            rgbd.AddForce(Vector3.down * downForce);
            isJumping = false;
            isFalling = true;
            fallToGround = false;
            isGrounded = false;

        }

        //Fall+land
        if (rgbd.position.y <= onTheGround.y + 0.4f && rgbd.position.y >= onTheGround.y + 0.3f && !isJumping)
        {
            rgbd.AddForce(Vector3.down * downForce);
            isFalling = true;
            fallToGround = true;
            isGrounded = false;
        }

        //to prevent bug of non-exit of is falling state
        if(animator.GetCurrentAnimatorStateInfo(LayerMask.NameToLayer("Player")).IsName("Fall") == true && isGrounded)
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(LayerMask.NameToLayer("Player")).IsName("Fall"));
            isFalling = true;
            fallToGround = true;
        }

      


    }
    void TurnToCamForwardDirection()
    {

        Vector3 CamForward = Camera.main.transform.forward;

        // We do not want the  player to fall on ground nor fly so Vertical axis = 0
        CamForward.y = 0;

        Quaternion rgbdLookAt = Quaternion.LookRotation(CamForward);
        Quaternion rgbdRotationSpeed = Quaternion.RotateTowards(rgbd.rotation, rgbdLookAt, rotationSpeed * Time.fixedDeltaTime);

        //Forces the rgbd to rotate to the value wanted
        rgbd.MoveRotation(rgbdLookAt);

        //Applies speed
        rgbd.rotation = rgbdRotationSpeed;


    }

    

  

    private void OnTriggerEnter(Collider other)
    {
        //Trigger collider added to avoid sticking to walls
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
            rgbd.AddForce(Vector3.down * downForce);
        }
    }



}
