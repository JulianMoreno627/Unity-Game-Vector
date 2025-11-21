using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public int enemyHealth = 20;
    public bool enemyDead = false;
    public GameObject enemyAI; 
    private Animator anim;

    // Puntos que da este enemigo AL MORIR
    public int pointsToGive = 100; 

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // ---------------------------------------------------------
    // FUNCIÓN 1: RECIBIR DISPARO
    // (Se ejecuta cada vez que una bala le pega)
    // ---------------------------------------------------------
    public void DamageEnemy(int damageAmount)
    {
        // Si ya está muerto, ignoramos el disparo
        if (enemyDead) return;

        // Solo restamos vida
        enemyHealth -= damageAmount;

        // ¡IMPORTANTE! AQUÍ NO DEBE HABER NADA DE SCOREMANAGER
        // Si pones ScoreManager aquí, te dará puntos con cada bala.
        
        Debug.Log("¡AY! Vida restante: " + enemyHealth);
    }

    // ---------------------------------------------------------
    // FUNCIÓN 2: COMPROBAR SI MURIÓ
    // (Se ejecuta en cada frame)
    // ---------------------------------------------------------
    void Update()
    {
        // Solo entramos aquí si la vida llegó a 0 Y todavía no hemos ejecutado la muerte
        if (enemyHealth <= 0 && enemyDead == false)
        {
            // 1. Lo marcamos como muerto inmediatamente
            enemyDead = true;

            // 2. AQUÍ, Y SOLO AQUÍ, DAMOS LOS PUNTOS
            if (ScoreManager.Instance != null)
            {
                Debug.Log("¡Enemigo abatido! Sumando " + pointsToGive + " puntos.");
                ScoreManager.Instance.AddScore(pointsToGive);
                ScoreManager.Instance.RegisterEnemyKill();
            }

            // 3. Animación de muerte
            if (anim != null)
            {
                anim.Play("Death");
            }

            // 4. Desactivar IA
            if (enemyAI != null)
            {
                enemyAI.SetActive(false);
            }

            // 5. Desactivar colisiones
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            // 6. Destruir el cuerpo a los 4 segundos
            Destroy(gameObject, 4f); 
        }
    }
}