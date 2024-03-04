using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Agregar aquí lógica para la muerte del enemigo, como animación de muerte, sonido, etc.
        Destroy(gameObject);
    }
}
