using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    private bool isAttacking = false;
    public float basicAttackDamage = 10f;
    public float strongAttackDamage = 20f; 
    public float attackRange = 10.0f;
    public LayerMask enemyLayer;
    
    void Die()
    {
        // Agregar aquí lógica para la muerte del enemigo, como animación de muerte, sonido, etc.
        Destroy(gameObject);
    }

    public void BasicAttack(Transform transform)
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack(basicAttackDamage, transform));
        }
    }

    public void StrongAttack(Transform transform)
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack(strongAttackDamage, transform));
        }
    }

    private IEnumerator PerformAttack(float damage, Transform transform)
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // Intenta aplicar daño al enemigo
            if (enemyCollider.TryGetComponent(out AttackableEnemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        isAttacking = false;
    }
}
