using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Enemigo : MonoBehaviour
{
    public Transform Objetivo;
    public float Velocidad;
    public NavMeshAgent IA;
    public Animator Anim;

    [Header("Configuracion de IA")]
    public float RangoDeVision = 15f; // Distancia a la que el enemigo te "ve"
    public float DañoPorAtaque = 10f; // Cuánta vida quita
    public float TiempoEntreAtaques = 1.5f; // Segundos entre cada golpe

    private float _timerAtaque; // Temporizador interno para el cooldown
    private Codigo_Salud _saludDelJugador; // Referencia al script de salud del jugador

    void Start()
    {
        if (Anim == null)
        {
            Anim = GetComponent<Animator>();
        }

        // Buscamos y guardamos el script de salud del jugador al empezar
        if (Objetivo != null)
        {
            _saludDelJugador = Objetivo.GetComponent<Codigo_Salud>();
        }

        // Aseguramos que el agente pueda rotar
        IA.updateRotation = true;
        // Reiniciamos el timer de ataque
        _timerAtaque = 0f;
    }

    void Update()
    {
        // Si no hay jugador o no tiene script de salud, no hacemos nada
        if (Objetivo == null || _saludDelJugador == null)
        {
            // Entramos en modo Idle por si acaso
            if(IA.isOnNavMesh) IA.isStopped = true;
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
            return;
        }

        IA.speed = Velocidad;

        // Calculamos la distancia al jugador
        float distancia = Vector3.Distance(transform.position, Objetivo.position);

        // --- 1. LOGICA DE RANGO DE VISION ---
        if (distancia > RangoDeVision)
        {
            // ESTAMOS FUERA DE RANGO: MODO IDLE
            if(IA.isOnNavMesh) IA.isStopped = true; // El agente se detiene
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
            _timerAtaque = 0f; // Reiniciamos el timer de ataque
        }
        // --- 2. LOGICA DE PERSECUCIÓN ---
        // (Si estamos en rango de visión PERO fuera de rango de ataque)
        else if (distancia > IA.stoppingDistance)
        {
            // ESTAMOS EN RANGO DE VISION: MODO PERSEGUIR
            if(IA.isOnNavMesh)
            {
                IA.isStopped = false;
                IA.updateRotation = true; // El NavMesh controla la rotación
                IA.SetDestination(Objetivo.position);
            }
            Anim.SetBool("isWalking", true);
            Anim.SetBool("isAttacking", false);
            _timerAtaque = 0f; // Reiniciamos el timer de ataque
        }
        // --- 3. LOGICA DE ATAQUE ---
        // (Si estamos en rango de visión Y en rango de ataque)
        else
        {
            // ESTAMOS EN RANGO DE ATAQUE: MODO ATACAR
            if(IA.isOnNavMesh) IA.isStopped = true; // El agente se detiene para atacar
            IA.updateRotation = false; // Nosotros controlamos la rotación
            MirarAlJugador();

            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", true); // ¡Disparamos la animación de ataque!

            // --- Logica de Quitar Vida con Cooldown ---
            _timerAtaque += Time.deltaTime; // Sumamos tiempo al timer

            if (_timerAtaque >= TiempoEntreAtaques)
            {
                _timerAtaque = 0f; // Reiniciamos el timer
                
                // ¡Hacemos daño al jugador!
                if (_saludDelJugador != null)
                {
                    _saludDelJugador.RecibirDaño(DañoPorAtaque);
                    Debug.Log("Enemigo golpea al jugador por " + DañoPorAtaque + " de daño.");
                }
            }
        }
    }

    void MirarAlJugador()
    {
        // Calcula la dirección hacia el jugador, ignorando la altura (eje Y)
        Vector3 lookDirection = (Objetivo.position - transform.position).normalized;
        lookDirection.y = 0;
        
        // Rota al enemigo para que mire en esa dirección
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Slerp para rotación suave
        }
    }
}