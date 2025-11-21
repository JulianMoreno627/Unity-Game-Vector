using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Configuración General")]
    public int score = 0;
    public TextMeshProUGUI uiText; 

    [Header("Configuración Nivel 2 -> 3")]
    public int scoreToPassLevel2 = 1000; 
    public string level2Name = "Nivel2"; 
    public string level3Name = "Nivel3"; 

    [Header("Configuración Nivel 3 -> Final")]
    public int enemiesLeftInLevel3 = 0;
    // CAMBIO 1: Asegúrate de poner "final" en el inspector o cámbialo aquí
    public string victorySceneName = "FINAL"; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Esto MANTIENE el puntaje entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. BUSCAR EL TEXTO AUTOMÁTICAMENTE
        // Esto funcionará en Nivel 2, Nivel 3 Y en la escena "final"
        GameObject textObj = GameObject.FindGameObjectWithTag("ScoreText");
        
        if (textObj != null) 
        {
            uiText = textObj.GetComponent<TextMeshProUGUI>();
            UpdateUI(); // Actualiza el texto inmediatamente con el puntaje acumulado
        }

        // 2. LÓGICA NIVEL 3
        if (scene.name == level3Name)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemiesLeftInLevel3 = enemies.Length;
            Debug.Log("Enemigos en Nivel 3: " + enemiesLeftInLevel3);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();

        // LÓGICA PARA PASAR DE NIVEL 2 A 3
        if (SceneManager.GetActiveScene().name == level2Name && score >= scoreToPassLevel2)
        {
            SceneManager.LoadScene(level3Name);
        }
    }

    public void RegisterEnemyKill()
    {
        if (SceneManager.GetActiveScene().name == level3Name)
        {
            enemiesLeftInLevel3--; 

            // CAMBIO 2: Transición a la escena final
            if (enemiesLeftInLevel3 <= 0)
            {
                Debug.Log("¡Nivel 3 Completado! Cargando escena final...");
                SceneManager.LoadScene(victorySceneName); 
            }
        }
    }

    void UpdateUI()
    {
        // Puedes personalizar el mensaje final aquí si quieres
        if (uiText != null) 
        {
             if (SceneManager.GetActiveScene().name == victorySceneName)
             {
                 uiText.text = "Puntaje Final: " + score.ToString();
             }
             else
             {
                 uiText.text = "Score: " + score.ToString();
             }
        }
    }
}