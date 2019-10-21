using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class MovementManager : MonoBehaviour
{
    public float velocity;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public float desiredRotationSpeed = 0.1f;
    public float speed;
    public float allowPlayerMovement = 0.1f;

    public Camera cam;
    public Animator anim;
    public CharacterController charControl;

    public bool rifle;


    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        charControl = this.GetComponent<CharacterController>();
        cam = Camera.main;
        anim.SetBool("Rifle", true);
        rifle = anim.GetBool("Rifle");
    }

    // Update is called once per frame
    void Update()
    {
        GetInputMagnitude(); 
        GetWeapon();
    }

    private void GetInputMagnitude()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if (speed > allowPlayerMovement)
        {
            PlayerMoveAndRotation();
        }
    }

    private void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        desiredMoveDirection = forward * InputZ + right * InputX;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        charControl.Move(desiredMoveDirection * Time.deltaTime * velocity);
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }
    private void GetWeapon()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            rifle = !rifle;
            if (rifle != anim.GetBool("Rifle"))
            {
                anim.SetBool("Rifle", rifle);
            }
        }
    }

    
}
