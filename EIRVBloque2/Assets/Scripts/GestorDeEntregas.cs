using UnityEngine;

public class GestorDeEntregas : MonoBehaviour
{
    [Tooltip("¿Cuántos puntos da entregar aquí?")]
    public int puntosPorEntrega = 100;
    [Header("Audio")]
    public AudioSource reproductorAudio;
    private void OnTriggerEnter(Collider otroObjeto)
    {
        if (otroObjeto.CompareTag("paquetes"))
        {
            // 1. Llamamos al cerebro central usando el Singleton
            ScoreManager.instancia.SumarPuntos(puntosPorEntrega);
            reproductorAudio.Play();
            // 2. Destruimos el paquete
            Destroy(otroObjeto.gameObject);
            
            // Opcional: Podrías añadir un efecto de partículas o sonido aquí
        }
    }
}