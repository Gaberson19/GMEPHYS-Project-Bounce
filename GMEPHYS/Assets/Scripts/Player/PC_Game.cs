using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Game : MonoBehaviour
{


    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float playerAcceleration;
    [SerializeField] Vector3 playerCurrentVelocity;

    float VerticalInput;
    float HorizontalInput;
    [SerializeField] Vector3 controllerDirection;

    [SerializeField] float JumpVelocity;
    [SerializeField] float GRAVITYMULTIPLIER;
    [SerializeField] float LOWJUMPMULTIPLIER;

    [SerializeField] float DASHSPEED;

    [SerializeField] float mouseSensitivity;

    [SerializeField] float waitTime;

    PlayerController._CurrentController[] functions;

    [SerializeField] Rigidbody playerRB;

    [SerializeField] int playerState = 0;

    enum PlayerState{
        IDLE,
        MOVE,
        JUMP,
        DASH
    };

    private void Start()
    {
        functions = new PlayerController._CurrentController[5];

        functions[0] = Jump;
        functions[1] = Gravity;
        functions[2] = Movement;
        functions[3] = Dash;
        functions[4] = SetControllerDirection;

        AddActions();
    }

    public void AddActions()
    {
        for (int currentFunction = 0; currentFunction < functions.Length; currentFunction++)
        {
            PlayerController.Instance.CurrentController += functions[currentFunction];
        }
    }

    /*set movement rotation:
       Defining Vertical and Horizontal Input
       Define Rotation Controller

       Set rotation Controller's Y Axis to Sin(Vertical*360) + Cos(Horizontal *360)
       Transform.lookat(Vertical Input,0,horizontalInput)
         */

    public void SetControllerDirection()
    {
        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");

        controllerDirection = (
            Vector3.forward * VerticalInput +
            Vector3.right * HorizontalInput
            );
    }

    public void Movement()
    {
        float currentSpeed;
        float decelerationSpeed;

        playerCurrentVelocity =
            (Vector3.forward * playerRB.velocity.z) +
            Vector3.right * playerRB.velocity.x;

        currentSpeed = Mathf.Abs(playerCurrentVelocity.x) + Mathf.Abs(playerCurrentVelocity.z);

        //Move forward in the direction of Direction Controller

        playerRB.velocity += controllerDirection * speed;

        //if player velocity X + player velocity z > max velocity, decelerate to max speed
        //if player velocity X + player velocity z < - max velocity, decelerate to - max speed
        if (currentSpeed > maxSpeed)
        {
            decelerationSpeed = currentSpeed - maxSpeed;
            playerRB.velocity =
                (controllerDirection.normalized * (currentSpeed - decelerationSpeed/1.2f)) +
                (Vector3.up * playerRB.velocity.y);
               
        }


    }

    //increase acceleration
    //Acceleration value
    //Every frame the movement buttons are held, Acceleration value is added to player velocity
    //once the player lets go of the button, the velocity will be decreased by acceleration value
    //note to self: Add a stopping mechanic (Ground pounds?)

    public void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerRB.velocity += Vector3.up * JumpVelocity;
        }
    }

    public void Dash()
    {

        if (Input.GetButtonDown("Dash"))
        {
            StartCoroutine(_Dash());
        }
    }

    IEnumerator _Dash()
    {
        PlayerController.Instance.CurrentController -= Dash;
        PlayerController.Instance.CurrentController -= Movement;
        //playerRB.velocity = controllerDirection.normalized * DASHSPEED + Vector3.up * playerRB.velocity.y;
        playerRB.AddForce(Vector3.zero + controllerDirection * DASHSPEED, ForceMode.Impulse);
        yield return new WaitForSeconds(waitTime);
        PlayerController.Instance.CurrentController += Dash;
        PlayerController.Instance.CurrentController += Movement;
    }

    public void Gravity()
    {
        if(playerRB.velocity.y < 0)
        {
            playerRB.velocity += Vector3.up * Physics.gravity.y * (GRAVITYMULTIPLIER - 1) * Time.deltaTime;
        }
        else if(playerRB.velocity.y < 0 && Input.GetButton("Jump"))
        {
            playerRB.velocity += Vector3.up * Physics.gravity.y * (LOWJUMPMULTIPLIER - 1) * Time.deltaTime;
        }
    }

}
