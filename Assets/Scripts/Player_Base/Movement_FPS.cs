/********************************************************************************************************
 * 
 * ***NAME***
 * FPSCharacterMovement.cs
 * 
 * ***DESCRIPTION***
 * A simple all-in-one First-Person controller.
 * 
 * ***USAGE***
 * Place this script onto a gameobject with rigidbody and capsule OR box collider components, with the main camera childed to this object.
 * 
 * ***AUTHOR***
 * Natalie Soltis
 * 
 * 
 * ***GENERAL TODO***
 * 
 * 
 * * fix strafing speed issue
 * 
 * * Create prefab with all settings optimally set
 * 
 * 
 * 
 *******************************************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Movement_FPS : MonoBehaviour
{
    private static float SPEED = 10f;

    //SERIALIZED

    [Header("Ground movement options")]
    [SerializeField]
    [Tooltip("The player's speed")]
    private float playerSpeed = SPEED;

    //IMPLIMENT IN NEXT IT
    //[SerializeField]
    //[Tooltip("Curve of the player's accel")]
    //private AnimationCurve acelerationCurve;

    //[SerializeField]
    //[Tooltip("Curve of the player's decel")]
    //private AnimationCurve decelerationCurve;

    [SerializeField]
    [Tooltip("The angle the player can look up/down")]
    private float maxYAngle = 90f;

    [SerializeField]
    [Tooltip("Height the player can jump")]
    private float jumpHeight = 10f;


    [Header("Air control parameters")]
    [SerializeField]
    [Tooltip("How much air control the player has\n0: none\n 1: same control as on ground")]
    [Range(0.0f, 1.0f)]
    private float airControl = SPEED;

    [SerializeField]
    [Tooltip("How fast the player can go while not grounded.")]
    private float AirSpeed;

    [Header("First/TP")]
    [SerializeField]
    [Tooltip("Third-Person offset")]
    private Vector3 TPPos;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Wether to display debug info and renders")]
    private bool displayDebugInfo;

    [SerializeField]
    [Tooltip("length for raycast for jump, default value 1.5f")]
    private float groundedCheckdist = 1.5f;

    [Header("Load settings")]
    [SerializeField]
    [Tooltip("Wether to destroy or not destroy when loading the next level \n true: do destroy\nfalse: do not destroy")]
    private bool destroyOnLoad;

    [Header("DEPRICATED")]
    [SerializeField]
    [Tooltip("The Sensitivity of the camera\nDEPRICATED, USE NEW INPUT SENSITIVITY")]
    private float sensitivity = 1f;



    //NONSERIALIZED

    /**
     * Used to calculate the x rotation of the body
     */
    private Vector2 currentRotation;

    /**
     * Used to calculate the y rotation of the camera, also used to set the x value to be equal to the body
     */
    private Vector2 cameraRot;

    /**
     * used to get the value of the movement input from the new input script
     **/
    private Vector2 moveInputForce;

    /**
     * the camera attatched to this object
     */
    private GameObject cameraObject;

    /**
     * the Input Action asset generated script
     */
    private MasterInput inputController;

    /**
     * the float used to curve movement inputs
     */
    private float curveFloat;

    private bool isTP;

    private Vector3 FPPos;

    // NOT USED IN CURRENT IT
    //private AnimationCurve currentCurve;


    private void Awake()
    {
        transform.GetComponent<Rigidbody>().mass = 1;

        cameraObject = transform.GetChild(0).gameObject;

        FPPos = cameraObject.transform.localPosition;

        currentRotation = cameraObject.transform.rotation.eulerAngles;


        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        //initializes the input script tied to the New Input Manager.
        inputController = new MasterInput();

        //vectors
        inputController.Player.Move.performed += ctx /**grabs context for input**/ => moveInputForce = ctx.ReadValue<Vector2>();
        inputController.Player.Move.canceled += _ => moveInputForce = Vector2.zero;

        inputController.Player.Look.performed += ctx => MoveCamera(ctx.ReadValue<Vector2>());

        inputController.Player.SwitchPerspective.performed += _ => SwitchCameraPos();

        //bools 
        inputController.Player.Jump.performed += _ => Jump();

    }
    private void OnDestroy()
    {
        //unbind all actions to prevent memory leak
        inputController.Player.Move.performed -= ctx /**grabs context for input**/ => movePlayer(ctx.ReadValue<Vector2>());
        inputController.Player.Move.canceled -= _ => moveInputForce = Vector2.zero;
        inputController.Player.Look.performed -= ctx => MoveCamera(ctx.ReadValue<Vector2>());
        inputController.Player.Jump.performed -= _ => Jump();
        inputController.Player.SwitchPerspective.performed -= _ => SwitchCameraPos();
    }

    private void OnEnable()
    {
        inputController.Enable();

        //vectors
        inputController.Player.Move.performed += ctx /**grabs context for input**/ => moveInputForce = ctx.ReadValue<Vector2>();
        inputController.Player.Move.canceled += _ => moveInputForce = Vector2.zero;

        inputController.Player.Look.performed += ctx => MoveCamera(ctx.ReadValue<Vector2>());

        inputController.Player.SwitchPerspective.performed += _ => SwitchCameraPos();

        //bools 
        inputController.Player.Jump.performed += _ => Jump();
    }

    private void OnDisable()
    {
        inputController.Disable();

        inputController.Player.Move.performed -= ctx /**grabs context for input**/ => movePlayer(ctx.ReadValue<Vector2>());
        inputController.Player.Move.canceled -= _ => moveInputForce = Vector2.zero;
        inputController.Player.Look.performed -= ctx => MoveCamera(ctx.ReadValue<Vector2>());
        inputController.Player.Jump.performed -= _ => Jump();
        inputController.Player.SwitchPerspective.performed -= _ => SwitchCameraPos();
    }

    private void FixedUpdate()
    {
        //TODO: find a way to make it so this code does not have to be run every frame if possible
         movePlayer(moveInputForce);
    }

    /**
     * does everything related to non-camera movement such as handling wasd movement.
     **/
    void movePlayer(Vector2 movementForce)
    {
        print(isGrounded());
        if (isGrounded())
        {
            if (Mathf.Abs(moveInputForce.x) == 0 && Mathf.Abs(moveInputForce.y) == 0)
            {
                //there's a better way to do this. Find if you have the time.
                transform.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.MoveTowards(transform.GetComponent<Rigidbody>().velocity.x, 0,
                Time.deltaTime * 50), transform.GetComponent<Rigidbody>().velocity.y, Mathf.MoveTowards(transform.GetComponent<Rigidbody>().velocity.z, 0, Time.deltaTime * 50));
                curveFloat = 0;
                return;
            }

            //does player wasd/stick movement
            curveFloat = Mathf.MoveTowards(curveFloat, playerSpeed, Time.deltaTime * 10);
            Vector3 transformForce = transform.forward * movementForce.y + transform.right * movementForce.x;
            //this could be better, but not important rn
            transformForce = new Vector3(transformForce.x * curveFloat, transformForce.y, transformForce.z * curveFloat);
            transformForce.y = transform.GetComponent<Rigidbody>().velocity.y; 
            transform.GetComponent<Rigidbody>().velocity = transformForce;


            //DEPRICATED ADDFORCE
            //transform.GetComponent<Rigidbody>().drag = 5;
            //transform.GetComponent<Rigidbody>().AddForce(transformForce * 10 * playerSpeed);
            //Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.x, -playerSpeed * movementForce.x, playerSpeed * movementForce.x);
            //Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.y, -playerSpeed * movementForce.y, playerSpeed * movementForce.y);
        }
        else
        {
            //does airborne wasd/stick movement
            transform.GetComponent<Rigidbody>().AddForce((transform.forward * movementForce.y * playerSpeed * airControl) + (transform.right * movementForce.x * playerSpeed));
            float newx, newz;
            newx = Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.x, -AirSpeed, AirSpeed);
            newz = Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.z, -AirSpeed, AirSpeed);
            transform.GetComponent<Rigidbody>().AddForce(new Vector3(newx, transform.GetComponent<Rigidbody>().velocity.y, newz));

            //backwards movement modifier. Makes sure if you hit -Y on controller while moving in +Y the player instantly stops moving in +Y
            //if (movementForce.y < -.1f && transform.InverseTransformPoint(transform.GetComponent<Rigidbody>().velocity).y < .01f)
            //{
            //    //calculated through worldtolocalmatrix so the local z value can be modified, instead of the global z value.
            //    Vector3 tempVec = transform.worldToLocalMatrix * transform.GetComponent<Rigidbody>().velocity;
            //    tempVec.z /= 1.1f;
            //    transform.GetComponent<Rigidbody>().velocity = transform.localToWorldMatrix * tempVec;
            //}

            if (movementForce.y < -.1f && transform.InverseTransformPoint(transform.GetComponent<Rigidbody>().velocity).y < .01f)
            {
                //calculated through worldtolocalmatrix so the local z value can be modified, instead of the global z value.
                Vector3 tempVec = transform.worldToLocalMatrix * transform.GetComponent<Rigidbody>().velocity;
                tempVec.z /= 1.1f;
                transform.GetComponent<Rigidbody>().velocity = transform.localToWorldMatrix * tempVec;
            }
        }
    }


    /**
     * two-stage check to see if the player is grounded
     **/
    public bool isGrounded()
    {
        float rad = transform.GetComponent<Collider>().bounds.extents.y;
        Vector3 modUp = new Vector3(rad / 2, 0, 0), modDown = new Vector3(-rad / 2, 0, 0), modLeft = new Vector3(0, 0, -rad / 2), modRight = new Vector3(0, 0, rad / 2);
        if (transform.GetComponent<Rigidbody>().velocity.y < -.1)
        {
            return false;
        }
        //below is to visualize the raws drayn for raycast test.
        if (displayDebugInfo)
        {
            UnityEngine.Debug.DrawRay(transform.localPosition, -transform.up, Color.green, groundedCheckdist);
            UnityEngine.Debug.DrawRay(transform.localPosition + modLeft, -transform.up, Color.green, groundedCheckdist);
            UnityEngine.Debug.DrawRay(transform.localPosition + modUp, -transform.up, Color.green, groundedCheckdist);
            UnityEngine.Debug.DrawRay(transform.localPosition + modDown, -transform.up, Color.green, groundedCheckdist);
            UnityEngine.Debug.DrawRay(transform.localPosition + modRight, -transform.up, Color.green, groundedCheckdist);
        }
        //checks if any of the player is close enough to the ground
        if (Physics.Raycast(transform.localPosition, -transform.up, groundedCheckdist) || Physics.Raycast(transform.localPosition + modLeft, -transform.up, groundedCheckdist) ||
            Physics.Raycast(transform.localPosition + modUp, -transform.up, groundedCheckdist) || Physics.Raycast(transform.localPosition + modDown, -transform.up, groundedCheckdist) ||
            Physics.Raycast(transform.localPosition + modRight, -transform.up, groundedCheckdist)) //this might be overkill, might want to scale back to 1-2?
        {
            return (true);
        }
        return (false);
    }

    /**
    * handles the camera's Y and the parent's X (therefore also handling the camera's X)
    **/
    void MoveCamera(Vector2 rotation)
    {
        //does playerobject rotation
        currentRotation.y += rotation.x * sensitivity;
        currentRotation.y = Mathf.Repeat(currentRotation.y, 360);
        transform.rotation = Quaternion.Euler(0, currentRotation.y, 0);

        //manipulates the child camera to sync with the y value of the parent.
        cameraRot.x -= rotation.y * sensitivity;

        //clamps the y rotation of the camera from -maxYangle to maxYangle to prevent the camera inverting itself
        cameraRot.x = Mathf.Clamp(cameraRot.x, -maxYAngle, maxYAngle);

        cameraObject.transform.localRotation = Quaternion.Euler(cameraRot.x, 0, 0);
    }


    void Jump()
    {
        if (isGrounded())
        {
            transform.GetComponent<Rigidbody>().AddForce(0, jumpHeight * 20, 0);
        }
    }


    void SwitchCameraPos()
    {
        if (isTP)
        {
            cameraObject.transform.localPosition = FPPos;
            isTP = !isTP;
        } else
        {
            cameraObject.transform.localPosition += TPPos;
            isTP = !isTP;
        }
    }
}