using UnityEngine;

public class FaderPersistente : MonoBehaviour
{
    private void Awake()
    {
        // Esto hace que este objeto NO se destruya al cambiar de escena o reiniciar
        DontDestroyOnLoad(gameObject);
        
        // Pero cuidado: si reinicias, habrá dos. Este código borra el duplicado:
        if (FindObjectsOfType<FaderPersistente>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}