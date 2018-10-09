using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    CharacterController charController;
    Vector3 moveDir;
    public float speed;
    public float jumpPower;
    public GameObject target;

    float usedSpeed;

    Animator myAnim;

    float rotX;
    float rotZ;

    bool running;

    public static bool canMove = true;

    bool canAttack = true;

    bool inCombat = false;

    Camera cam;
    float yVel;

    GameObject targetEnemy;

	// Use this for initialization
	void Start ()
    {
        charController = GetComponent<CharacterController>();
        myAnim = GetComponent<Animator>();
        cam = Camera.main;
        usedSpeed = speed;
	}

    public void ToggleInCombat()
    {
        inCombat = !inCombat;
    }

    public void SetTarget(GameObject enemy)
    {
        targetEnemy = enemy;
    }

    public void SetSpeed(int newSpeed)
    {
        usedSpeed = newSpeed;
    }

    public void ResetSpeed()
    {
        usedSpeed = speed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!canMove)
        {
            return;
        }

        //move dir effected by camera direction.
        moveDir = (cam.transform.forward * Input.GetAxis("Vertical")) + (cam.transform.right * Input.GetAxis("Horizontal"));

        //When not running
        if(moveDir == Vector3.zero)
        {
            running = false;
        }
        //When running.
        else
        {
            running = true;


            if (!inCombat) { target.transform.position = transform.position + moveDir; }
            else { target.transform.position = targetEnemy.transform.position; }
            
        }

        myAnim.SetBool("run", running);

        
        //Change y velocity of the player if they are not grounded.
        if(!charController.isGrounded)
        {
            //Should be in line with real world gravity - ish.
            yVel -= 9.8f * Time.deltaTime;
        }
        //If the player presses space and the player is on the ground then jump
        if (Input.GetKeyDown(KeyCode.Space) && charController.isGrounded){yVel = jumpPower;}


        moveDir.y = yVel;
        

        charController.Move(moveDir * usedSpeed * Time.deltaTime);

        //Look in direction the player has input based on the placement of the gameobject.
        rotX = transform.eulerAngles.x;
        rotZ = transform.eulerAngles.z;

        transform.LookAt(target.transform);

        transform.eulerAngles = new Vector3(rotX, transform.eulerAngles.y, rotZ);

        
        if(inCombat)
        {

            AnimatorClipInfo[] clipInfo = myAnim.GetCurrentAnimatorClipInfo(0);
            Debug.Log(clipInfo[0].clip.name);
            //Check for a case to break away from being in combat state.
            if (CheckForBreakFromCombat()) { BreakFromCombat(); }

            if(Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
            {
                myAnim.SetTrigger("attack");
                canAttack = false;
            }
                
        }
	}

    public void ResetCanAttack()
    {
        canAttack = true;
    }

    bool CheckForBreakFromCombat()
    {
        float dist = Vector3.Distance(transform.position, targetEnemy.transform.position);
        if (dist > 10) { return true; }
        else { return false; }
            
    }

    public void BreakFromCombat()
    {
        inCombat = false;

        targetEnemy.GetComponent<CrabController>().ToggleCombat();
        targetEnemy = null;
    }

    void CheckInput()
    {
        if (Input.GetAxis("Vertical") == 0f)
        {
            if (Input.GetAxis("Horizontal") < 0f)
            {
                // look left
                target.transform.position = new Vector3(transform.position.x - 3, transform.position.y, transform.position.z);
            }
            if (Input.GetAxis("Horizontal") > 0f)
            {
                // look right
                target.transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z);
            }
        }
        else
        {
            if (Input.GetAxis("Horizontal") == 0f)
            {
                if (Input.GetAxis("Vertical") < 0f)
                {
                    // look back
                    target.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 3);
                }
                if (Input.GetAxis("Vertical") > 0f)
                {
                    // look forward
                    target.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3);
                }
            }
            else
            {
                if(Input.GetAxis("Vertical") < 0f && Input.GetAxis("Horizontal") < 0f)
                {
                    // look back + left
                    target.transform.position = new Vector3(transform.position.x - 3, transform.position.y, transform.position.z - 3);
                }
                if (Input.GetAxis("Vertical") > 0f && Input.GetAxis("Horizontal") < 0f)
                {
                    // look forward + left
                    target.transform.position = new Vector3(transform.position.x - 3, transform.position.y, transform.position.z + 3);
                }
                if (Input.GetAxis("Vertical") > 0f && Input.GetAxis("Horizontal") > 0f)
                {
                    // look forward + right
                    target.transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z + 3);
                }
                if (Input.GetAxis("Vertical") < 0f && Input.GetAxis("Horizontal") > 0f)
                {
                    // look back + right
                    target.transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z - 3);
                }
            }
        }


    }
}
