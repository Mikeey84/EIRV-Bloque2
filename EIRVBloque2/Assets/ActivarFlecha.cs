using UnityEngine;

public class ActivarFlecha : MonoBehaviour
{
    [SerializeField]
    private GameObject flechaObjetivo;
    

    [SerializeField]
    private bool destruirFlecha = true;

    void Awake()
    {
        flechaObjetivo.SetActive(false);
    }
    // Esta función se ejecuta automáticamente cuando algo entra en el Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si lo que ha entrado es el camión (usando su Tag)
        if (other.CompareTag("Player")) 
        {
            // Si hay una flecha asignada, la encendemos
            if (flechaObjetivo != null)
            {
                flechaObjetivo.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Al salir del trigger
        if (other.CompareTag("Player") && flechaObjetivo != null)
        {
            if (destruirFlecha)
            {
                // La borramos de la escena definitivamente
                Destroy(flechaObjetivo);
            }
            else
            {
                // Solo la apagamos por si acaso
                flechaObjetivo.SetActive(false);
            }
        }
    }
}