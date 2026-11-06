using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{
    //body
    public float curSpeedX;
    public float curSpeedY; 
    public float walkingSpeed;
    public float gravity;
    public CharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;
    public bool canMove;
    public AudioSource AS;

    //Camera
    public float lookSpeed;
    public float lookXLimit;
    public Transform Spina;
    float rotationX = 0;
    public float Outputi;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        CameraLog();
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        curSpeedX = canMove ? walkingSpeed * Input.GetAxis("Vertical") : 0;
        curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        if(curSpeedX != 0 || curSpeedY != 0)
        {
            if(AS.isPlaying == false)
            {
                AS.Play();
            }
        }
        else if(curSpeedX == 0 || curSpeedY == 0)
        {
            AS.Stop();
        }

        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }
    public void CameraLog()
    {
        if (canMove == true)
        { 
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX + Outputi, -lookXLimit, lookXLimit);
            Spina.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
