using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class playercontrol : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    private float gravity = 9.81f;

    private CharacterController jones;
    [SerializeField]
    private Vector2 input = Vector2.zero;
    private Vector3 velocity;
    private Vector3 velocityXZ;
    
    // player's top speed when moving
    [SerializeField]
    private float movespeed = 5;
    

    // rate of acceleration and deceleration when
    // beginning and ending movement is based on this value
    private float accel = 20;

    // Start is called before the first frame update
    void Start()
    {
        jones = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Gravity();
        Jump();

        var camTrans = cam.transform;
        
        velocityXZ = Vector3.Lerp(velocityXZ, 
            (camTrans.forward * input.y + camTrans.right * input.x) * movespeed,
                                                                                        accel*Time.deltaTime);

        velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);
        jones.Move(velocity * Time.deltaTime);
    }
    
    void GetInput()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (!jones.isGrounded) input.x /= 2;
        
        input = Vector2.ClampMagnitude(input, 1);
    }
    
    void Gravity()
    {
        if (jones.isGrounded)
        {
            velocity.y = -0.5f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        velocity.y = Mathf.Clamp(velocity.y, -10, 10);
    }

    void Jump()
    {
        if(jones.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = 6;
                //14 degrees is highest angle we can still jump on slopes
                //issue with raycast length in CalculateGround
                //doesnt work when descending slopes
            }
        }
    }
}