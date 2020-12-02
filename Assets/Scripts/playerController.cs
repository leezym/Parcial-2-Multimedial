using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public int masa;
    public float movementSpeed, runSpeed, jumpSpeed, fallSpeed;
    public bool idle, run, climb, nearWall, fall, jump, throwA, duck, pickup, die, restart;

    float verticalInput;
    Animator anim;
    Rigidbody rigidbody;
    GameObject camera;


    void Start()
    {
        anim = GetComponent<Animator>();        
        rigidbody = GetComponent<Rigidbody>();

        idle = true;
        nearWall = false;
        climb = false;
        fall = false;
        run = false;
        jump = false;
        throwA = false;
        duck = false;
        pickup = false;
        die = false;
        restart = false;
        rigidbody.mass = masa;
    }

    void Update()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        movePlayer();
    }

    void movePlayer()
    {
        // Movimiento camara
        verticalInput = Input.GetAxis("Vertical");
        //limitCamera();
        transform.localEulerAngles = new Vector3(0, camera.transform.localEulerAngles.y, 0);

        anim.SetFloat("inputV", verticalInput);
        anim.SetBool("idle", idle);
        anim.SetBool("runV", run);
        anim.SetBool("climb", climb);
        anim.SetBool("fall", fall);
        anim.SetBool("jump", jump);
        anim.SetBool("throw", throwA);    
        anim.SetBool("duck", duck);
        anim.SetBool("pickup", pickup);
        anim.SetBool("die", die);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }       

        // Escalar
        if (nearWall && Input.GetKey(KeyCode.E))
        {
            rigidbody.mass = masa * 3;
            climb = true;
        }
        else
        {
            rigidbody.mass = masa;
            climb = false;
        }

        // Agacharse
        if (Input.GetKey(KeyCode.S))
        {
            duck = true;
        }
        else
        {
            duck = false;
        }

        // Recoger objeto
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            pickup = true;
        } 

        // Morir
        if (Input.GetKeyDown(KeyCode.F))
        {
            die = true;
        }

        // Revivir
        if (Input.GetKeyDown(KeyCode.R) && die && restart)
        {
            die = false;
            restart = false;
        }

    }

    void LateUpdate()
    {
        if (idle && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            idle = false;
            throwA = true;
        }

        if (jump && anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            jump = false;
        }

        if (throwA && anim.GetCurrentAnimatorStateInfo(0).IsName("Throw") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            throwA = false;
            idle = true;
        }

        if (fall && anim.GetCurrentAnimatorStateInfo(0).IsName("Falling") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            fall = false;
        }

        if (pickup && anim.GetCurrentAnimatorStateInfo(0).IsName("Picking up") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            pickup = false;
        }

        if (die && anim.GetCurrentAnimatorStateInfo(0).IsName("Die") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            restart = true;
        }
    }

    void FixedUpdate()
    {
        bool floor = touchGround();
        bool wall = touchWall();
        bool sJump = smallJump();

        // Adelate o subir
        if (Input.GetKey(KeyCode.W) && !climb)
        {
            rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * movementSpeed);
        }
        else if (Input.GetKey(KeyCode.W) && climb)
        {
            rigidbody.AddForce(Vector3.up * movementSpeed, ForceMode.Impulse);
        }
        
        // Bajar
        if (Input.GetKey(KeyCode.S) && climb)
        {
            rigidbody.AddForce(-Vector3.up * movementSpeed, ForceMode.Impulse);
        }

        // Correr
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
            rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * runSpeed);
        }
        else
        {
            run = false;
        }

        // Caer
        if (fall)
        {
            rigidbody.MovePosition(transform.position - transform.up * Time.fixedDeltaTime * fallSpeed);
        }

        // Salto
        if (jump)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
            {
                rigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                if (Input.GetKey(KeyCode.W))
                {
                    rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * movementSpeed);
                }
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                rigidbody.AddForce(-Vector3.up * movementSpeed, ForceMode.Impulse);
                if (Input.GetKey(KeyCode.W))
                {
                    rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * movementSpeed);
                }
            }
        }

        // Escalar y Caer
        if (wall)
        {
            nearWall = true;
        }
        else
        {
            nearWall = false;
        }

        if (floor)
        {
            fall = false;
        }
        else
        {
            if (sJump)
            {
                fall = false;
            }
            else
            {
                fall = true;
            }
        }
    }

    bool touchWall()
    {
        int mask = 1 << 9;
        if (Physics.Raycast(transform.position, transform.forward, 2f, mask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool touchGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, 1f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool smallJump()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 8f) && !fall) // Condicion de muerte
        {
            Debug.DrawRay(transform.position, -transform.up * hit.distance, Color.blue);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.up, Color.red);
            return false;
        }
    }
}
