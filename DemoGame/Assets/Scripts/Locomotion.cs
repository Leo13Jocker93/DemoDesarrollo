using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    Rigidbody2D rb;

    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    public LayerMask jumpableGround;
    private float dirX = 0.0f;
    public float vel_horizontal = 2.5f;
    public float vel_runing = 5.0f;
    private bool isSpacebarPressedOnce = false;
    private float lastSpacePressTime = 0f;
    public float superJumpDuration = 1.0f; // Tiempo límite entre dos presiones de espacio para considerar como "doble salto"
    public float normalJumpForce = 14.0f;
    public float superJumpForce = 22.0f;
    float standingColl_SizeY;
    private bool isCrouching = false;

    private AttackSystem attackSystem;

    private enum MovementState {idle, walk, runing, jumping, falling, superJumping, superFalling, crounching, interaction, punch, kick, die }
        
    // Start is called before the first frame update
    void Start()
    {        
        Debug.Log("Hello, World");
        rb = GetComponent<Rigidbody2D>(); 
        coll = GetComponent<BoxCollider2D>();
        standingColl_SizeY = coll.size.y;
        sprite = GetComponent<SpriteRenderer>();
        anim =  GetComponent<Animator>();
        attackSystem = GetComponent<AttackSystem>();
        if(attackSystem == null)
        {
            Debug.LogError("No hay attacksystem en el player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxis("Horizontal");
        //agacharse
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {            
            coll.size = new Vector2(coll.size.x, coll.size.y / 2f);
            coll.offset -= new Vector2(0f, standingColl_SizeY / 4f);
            isCrouching = true;
        } 
        else if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            coll.size = new Vector2(coll.size.x, standingColl_SizeY);

            // Restaurar la posición original del collider
            coll.offset += new Vector2(0f, standingColl_SizeY / 4f);

            // Actualizar el estado de agachado
            isCrouching = false;
        }

        if(isCrouching)
        {
            rb.velocity = Vector2.zero;
            //return;
        } 

        rb.velocity = new Vector2(dirX * vel_horizontal, rb.velocity.y);

        //Identificar si el jugador hacer una unica presión o sostiene la presión de la tecla espacio
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(!Input.GetKey(KeyCode.LeftShift))
                {
                    rb.velocity = new Vector2(rb.velocity.x, normalJumpForce);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, superJumpForce);
                }
            }
        }
        //Si se presiona shift corre
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            rb.velocity = new Vector2(dirX *vel_runing, rb.velocity.y);
        }

        if(Input.GetMouseButton(0))
        {
            attackSystem.BasicAttack(transform);
        }
        if(Input.GetMouseButton(1))
        {
            attackSystem.StrongAttack(transform);
        }  
        UpdateAnimationState(); 
    }   

    private void UpdateAnimationState()
    {
        MovementState state = MovementState.idle;

        bool isMovingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool isMovingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

        if (isMovingRight)
        {
            sprite.flipX = false; // Personaje mira hacia la derecha
        }
        else if (isMovingLeft)
        {
            sprite.flipX = true; // Personaje mira hacia la izquierda
        }
        // Ahora maneja el estado de la animación para que no quede atascado el player en elas intersecciones del terrain
        bool isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(rb.velocity.x), 0.1f, LayerMask.GetMask("Ground")) ||
                            Physics2D.Raycast(transform.position, Vector2.left * Mathf.Sign(rb.velocity.x), 0.1f, LayerMask.GetMask("Ground"));

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }        
        else if (Mathf.Abs(rb.velocity.x) > 0.1f && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            if (state == MovementState.runing)
            {
                state = MovementState.jumping; // Cambia a la animación de salto si el jugador está corriendo y salta
            }
            else
            {
                state = MovementState.runing;
                sprite.flipX = rb.velocity.x < 0; // Ajusta el flipX según la dirección del movimiento
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            state = MovementState.walk;
            sprite.flipX = rb.velocity.x < 0; // Ajusta el flipX según la dirección del movimiento
        }
        else
        {
            state = MovementState.idle;
        }

        if(rb.velocity.y > normalJumpForce)
        {
            state = MovementState.superJumping;
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            state = MovementState.crounching;
        }

        if (Input.GetKey(KeyCode.E))
        {
            state = MovementState.interaction;
        }

        if (Input.GetMouseButton(0))
        {
            state = MovementState.punch;
        }

        if (Input.GetMouseButton(1))
        {
            state = MovementState.kick;
        }
        anim.SetInteger ("State", (int)state);  
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    //sistema de vida

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);   
    }
}
