using UnityEngine;
using System.Collections;
using InfimaGames.LowPolyShooterPack; // Lo uso porque estaba en tu script original
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour {

	[Range(5, 100)]
	public float destroyAfter;
	public bool destroyOnImpact = true; 
	public float minDestroyTime;
	public float maxDestroyTime;

	[Header("Gameplay")]
	public int damage = 5; 

	[Header("Impact Effect Prefabs")]
	public Transform [] bloodImpactPrefabs;
	public Transform [] metalImpactPrefabs;
	public Transform [] dirtImpactPrefabs;
	public Transform []	concreteImpactPrefabs;

	private bool hasCollided = false; // Variable para evitar colisiones múltiples

	private void Start ()
	{
		// Inicia el temporizador de autodestrucción (por si no choca con nada)
		StartCoroutine (DestroyAfter ());
		
		// Intenta ignorar al jugador (Player)
		try
		{
			var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
			if (gameModeService != null && gameModeService.GetPlayerCharacter() != null)
			{
				Physics.IgnoreCollision(gameModeService.GetPlayerCharacter().GetComponent<Collider>(), GetComponent<Collider>());
			}
		}
		catch (System.Exception)
		{
			// Esto es normal si no usas el asset de Infima
		}
	}

	private void OnCollisionEnter (Collision collision)
	{
		// Si ya chocamos, no hacemos nada más.
		if (hasCollided) return;

		// --- PRUEBA 1: VER SI CHOCA CON ALGO ---
		Debug.Log("OnCollisionEnter: He chocado con -> " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

		// Ignorar otros proyectiles
		if (collision.gameObject.GetComponent<Projectile>() != null)
		{
			Debug.Log("Choque ignorado: Era otro proyectil.");
			return;
		}
		
		// Ignorar al jugador
		if (collision.gameObject.CompareTag("Player"))
		{
			Debug.Log("Choque ignorado: Era el jugador.");
			Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
			return;
		}

		// Marcamos que ya hemos chocado para evitar errores
		hasCollided = true;

		// --- 1. LÓGICA DE DAÑO AL ENEMIGO ---
		EnemyDeath enemy = collision.transform.GetComponentInParent<EnemyDeath>();

		if (enemy != null)
		{
			// --- PRUEBA 2: VER SI ENCUENTRA AL ENEMIGO ---
			Debug.Log("¡ÉXITO! Script 'EnemyDeath' encontrado en " + collision.gameObject.name + ". Haciendo daño.");
			enemy.DamageEnemy(damage);
		}
		else
		{
			// --- PRUEBA 3: SI CHOCA PERO NO ENCUENTRA EL SCRIPT ---
			Debug.LogError("¡FALLO! Choqué con " + collision.gameObject.name + " pero NO encontré el script 'EnemyDeath' en él o en sus padres.");
		}

		// --- 2. LÓGICA DE EFECTOS DE IMPACTO ---
		// NOTA: Si no tienes efectos, pon el 'Size' de estos arrays en 0 en el Inspector
		try
		{
			if (collision.transform.tag == "Blood" && bloodImpactPrefabs.Length > 0) 
			{
				Instantiate (bloodImpactPrefabs [Random.Range (0, bloodImpactPrefabs.Length)], transform.position, Quaternion.LookRotation (collision.contacts [0].normal));
			}
			else if (collision.transform.tag == "Metal" && metalImpactPrefabs.Length > 0) 
			{
				Instantiate (metalImpactPrefabs [Random.Range (0, metalImpactPrefabs.Length)], transform.position, Quaternion.LookRotation (collision.contacts [0].normal));
			}
			else if (collision.transform.tag == "Dirt" && dirtImpactPrefabs.Length > 0) 
			{
				Instantiate (dirtImpactPrefabs [Random.Range (0, dirtImpactPrefabs.Length)], transform.position, Quaternion.LookRotation (collision.contacts [0].normal));
			}
			else if (collision.transform.tag == "Concrete" && concreteImpactPrefabs.Length > 0) 
			{
				Instantiate (concreteImpactPrefabs [Random.Range (0, concreteImpactPrefabs.Length)], transform.position, Quaternion.LookRotation (collision.contacts [0].normal));
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError("Error al crear efecto de impacto. Asegúrate de que los prefabs estén asignados en el Inspector o pon el Size en 0. Error: " + e.Message);
		}
		
		// --- 3. LÓGICA DE OBJETOS INTERACTUABLES (¡RE-AÑADIDA!) ---
		
		//Si la bala colisiona con "Target" tag
		if (collision.transform.tag == "Target") 
		{
			TargetScript target = collision.transform.gameObject.GetComponent<TargetScript>();
            if (target != null)
            {
			    target.isHit = true;
            }
		}
			
		//Si la bala colisiona con "ExplosiveBarrel" tag
		if (collision.transform.tag == "ExplosiveBarrel") 
		{
			ExplosiveBarrelScript barrel = collision.transform.gameObject.GetComponent<ExplosiveBarrelScript>();
            if (barrel != null)
            {
			    barrel.explode = true;
            }
		}

		//Si la bala colisiona con "GasTank" tag
		if (collision.transform.tag == "GasTank") 
		{
			GasTankScript tank = collision.transform.gameObject.GetComponent<GasTankScript>();
            if (tank != null)
            {
			    tank.isHit = true;
            }
		}
		
		// --- 4. LÓGICA DE DESTRUCCIÓN DE LA BALA ---
		// (Esto ahora destruye la bala DESPUÉS de hacer todo lo demás,
		// por eso quitamos el 'Destroy' de cada 'if' de arriba)
		if (destroyOnImpact) 
		{
			Destroy (gameObject);
		}
		else 
		{
			StartCoroutine (DestroyTimer ());
		}
	}

	private IEnumerator DestroyTimer () 
	{
		yield return new WaitForSeconds(Random.Range(minDestroyTime, maxDestroyTime));
		Destroy(gameObject);
	}

	// --- ESTA ES LA ÚNICA VERSIÓN DE DestroyAfter ---
	private IEnumerator DestroyAfter () 
	{
		yield return new WaitForSeconds (destroyAfter);
		// Solo destruir si no ha chocado aún
		if (!hasCollided)
		{
			Destroy (gameObject);
		}
	}
}