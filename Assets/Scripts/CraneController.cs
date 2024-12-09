using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CraneController : MonoBehaviour
{
    public float moveSpeed = 5f;        
    public float rotateSpeed = 50f;     

    private Rigidbody rb;

    void Start()
    {
        // Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

// Controller updatering varje frame
    void Update()
    {
        
        float moveHorizontal = Input.GetAxis("Horizontal"); 
        
        
        float moveVertical = Input.GetAxis("Vertical"); 

        
        Vector3 moveDirection = transform.right * moveVertical * moveSpeed * Time.deltaTime;

        
        Vector3 strafeDirection = transform.forward * moveHorizontal * moveSpeed * Time.deltaTime;

 
        rb.MovePosition(rb.position + moveDirection + strafeDirection);

       
        if (moveHorizontal != 0)
        {
            float rotationAmount = moveHorizontal * rotateSpeed * Time.deltaTime;
            Quaternion turnOffset = Quaternion.Euler(0, rotationAmount, 0);
            rb.MoveRotation(rb.rotation * turnOffset);
        }
    }
}
