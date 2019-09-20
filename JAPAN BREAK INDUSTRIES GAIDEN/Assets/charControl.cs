﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charControl : MonoBehaviour
{
    public GameObject protag;
    public float speed;
    public float maxSpeed;
    public float groundAccel;
    public float climbAccel;
    public float accel;
    public float dashMax;
    public float decel;
    public float dashDecel;
    public float jogMax;
    public float climbMax;
    public float speedY;
    public bool jumping;
    public bool grounded;
    public bool squat;
    public bool m_FacingRight;
    public bool climbing;
    public bool canClimb;
    public int squatFrames;
    private int squatCounter;
    public float jumpAccel;
    public float jumpSpeed;
    public float gravityAccel;
    public float termVel;
    public float airAccel;
    private int regrabTimer;
    //public GameObject groundBox;
    public Collider2D groundBox;
    public Collider2D wallBox;
    public GameObject[] buildings;
    // Start is called before the first frame update
    void Start()
    {
        /*
        protag = GameObject.Find("protag");
        maxSpeed = .05f;
        accel = .02f;
        dashMax = .1f;
        decel = .01f;
        jogMax = .05f;
        */
        
        buildings = GameObject.FindGameObjectsWithTag("building");
        //print(buildings.Length);
    }
    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void jump(bool shortHop)
    {
        if (shortHop || climbing)
            jumpSpeed = jumpAccel;
        else
            jumpSpeed = jumpAccel * 1.5f;
        climbing = false;
    }
    /*this is handled in seperate script in the groundbox now
    void OnCollisionEnter2D(Collision2D groundBox)
    {
        //print(groundBox.otherCollider);
        grounded = true;
        jumping = false;
        accel = groundAccel;
    }
    
    void OnCollisionExit2D(Collision2D groundBox)
    {
        grounded = false;
        jumping = true;
        accel = airAccel;
        //print(groundBox.collider);
        //print(groundBox.otherCollider);
    }
    */
    void OnTriggerEnter2D(Collider2D wallBox)
    {
        if(wallBox.tag == "building")
        {
            canClimb = true;
        }
        /*
        foreach (GameObject building in buildings)
        {
            //print(building);
            if(wallBox.GetComponent<Collider>() == building)
            {
                print("lol");
            }
        }
        */
    }
    void OnTriggerExit2D(Collider2D wallBox)
    {
        if (wallBox.tag == "building")
        {
            canClimb = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (squat)
        {
            squatCounter += 1;
            if(squatCounter > squatFrames)
            {
                squat = false;
                jumping = true;
                grounded = false;
                if (Input.GetAxisRaw("Jump") > 0)
                {
                    jump(false);
                }
                else
                {
                    jump(true);
                }
            }
            return;
        }
        else
        {
            squatCounter = 0;
        }
        if(Input.GetAxisRaw("climb") == 0)
            regrabTimer += 1;
        if(!grounded && !climbing)
        {
            jumping = true;
        }
        if (canClimb && Input.GetAxisRaw("climb") > 0 && !climbing && regrabTimer > 5)
        {
            //print("climbing");
            climbing = true;
            jumping = false;
            jumpSpeed = 0f;
            regrabTimer = 0;
            maxSpeed = climbMax;
            accel = climbAccel;
        }
        else if (!canClimb)
        {
            climbing = false;
            regrabTimer = 0;
        }
        else if(climbing && Input.GetAxisRaw("climb") > 0 && regrabTimer > 5)
        {
            climbing = false;
            regrabTimer = 0;
        }
        if (!climbing)
        {
            speedY = 0f;
            if (!grounded)
            {
                jumping = true;
                accel = airAccel;
            }
            else
            {
                jumping = false;
                accel = groundAccel;
            }
        }
        if (climbing)
        {
            if (Input.GetAxisRaw("Vertical") > 0)// && speed < maxSpeed)
            {
                speedY += accel;
                if (speedY > maxSpeed)
                {
                    speedY = maxSpeed;
                }
            }
            else if (Input.GetAxisRaw("Vertical") < 0)// && Mathf.Abs(speed) < maxSpeed)
            {
                speedY -= accel;
                if (Mathf.Abs(speedY) > maxSpeed)
                {
                    speedY = -maxSpeed;
                }
            }
            else
            {
                if (jumping == false && squat == false)
                {
                    if (Mathf.Abs(speedY) < decel)
                    {
                        speedY = 0;
                    }
                    if (speedY > 0) speedY -= decel;
                    else if (speedY < 0) speedY += decel;
                }
            }
        }
        if (Input.GetAxisRaw("Horizontal") > 0)// && speed < maxSpeed)
        {
            speed += accel;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)// && Mathf.Abs(speed) < maxSpeed)
        {
            speed -= accel;
            if (Mathf.Abs(speed) > maxSpeed)
            {
                speed = -maxSpeed;
            }
        }
        else
        {
            if (jumping == false && squat == false)
            {
                if (Mathf.Abs(speed) < decel)
                {
                    speed = 0;
                }
                if (speed > 0) speed -= decel;
                else if (speed < 0) speed += decel;
            }
        }
        if(speed == 0 && !climbing)
        {
            maxSpeed = dashMax;//not moving resets dashing
        }
        else
        {
            if(maxSpeed > jogMax && !climbing)
            {
                maxSpeed -= dashDecel;//maybe this shouldn't be the same variable
            }
        }
        if (maxSpeed < jogMax && !climbing)
        {
            maxSpeed = jogMax;//jogMax is the slowest maxSpeed
        }
        else if (maxSpeed < climbMax && climbing)
        {
            maxSpeed = climbMax;//jogMax is the slowest maxSpeed
        }
        /*
        if(speed > maxSpeed)
        {
            speed -= .01f;
        }
        else if(speed < -maxSpeed)
        {
            speed += .01f;
        }
        */
        //print(speed);

        if (jumping)
        {
            protag.transform.position = new Vector3(protag.transform.position.x + (speed), protag.transform.position.y + jumpSpeed, protag.transform.position.z);
            jumpSpeed -= gravityAccel;
            if(jumpSpeed < termVel)
            {
                jumpSpeed = termVel;
            }
        }
        else
        {
            protag.transform.position = new Vector3(protag.transform.position.x + speed, protag.transform.position.y + speedY, protag.transform.position.z);
            if (speed < 0 && m_FacingRight)
            {
                protag.transform.position = new Vector3(protag.transform.position.x - .45f, protag.transform.position.y + jumpSpeed, protag.transform.position.z);
                Flip();

            }
            else if (speed > 0 && !m_FacingRight)
            {
                protag.transform.position = new Vector3(protag.transform.position.x + .45f, protag.transform.position.y + jumpSpeed, protag.transform.position.z);
                Flip();
            }
        }
        if (Input.GetAxisRaw("Jump") > 0 && (grounded == true || climbing == true) && squat == false)
        {
            squat = true;
        }
    }
}
