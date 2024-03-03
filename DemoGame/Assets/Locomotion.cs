using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    Rigidbody2D rb;
    public float vel_horizontal = 5.0f;
    public float fuerza_salto = 14.0f;
        
    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log("Hello, World");
        rb = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * vel_horizontal, rb.velocity.y);
        
        if (Input.GetButton("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerza_salto);
        }
            
        
    }


}
