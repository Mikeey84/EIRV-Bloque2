using UnityEngine;
using TMPro;
using System.Collections;

public class GestorDeEntregasFlow : MonoBehaviour
{
    [Header("Puntuación")]
    public int puntos = 0;
    
    [Header("Referencias UI")]
    // Cambia a TextMeshPro si usas texto 3D puro, mantén UGUI si usas Canvas
    public TextMeshProUGUI textoPuntuacion; 
    
    [Header("Configuración de Animación (Flow)")]
    // Cuánto crecerá (1.0 = normal, 1.3 = 30% más grande)
    public float multiplicadorEscalaMax = 1.3f; 
    // Cuánto tardará la animación completa (subir y bajar)
    public float duracionAnimacion = 0.25f; 

    // CRUCIAL: Definiremos la forma del movimiento en el Inspector
    [Tooltip("Dibuja una curva que suba y baje. Eje X=Tiempo(0-1), Eje Y=Intensidad(0-1)")]
    public AnimationCurve curvaDePunch;

    private Vector3 escalaOriginal;
    private Vector3 escalaMaxima;
    private Coroutine corrutinaAnimacion;

    private void Start()
    {
        if (textoPuntuacion != null)
        {
            escalaOriginal = textoPuntuacion.transform.localScale;
            // Pre-calculamos la escala máxima para ahorrar CPU
            escalaMaxima = escalaOriginal * multiplicadorEscalaMax;
            ActualizarTextoInterfaz(false);
        }
    }

    private void OnTriggerEnter(Collider otroObjeto)
    {
        // Asegúrate de que el paquete tiene el Tag exacto "paquetes"
        if (otroObjeto.CompareTag("paquetes"))
        {
            SumarPuntos(100); // Ejemplo arcade: 100 puntos
            Destroy(otroObjeto.gameObject);
        }
    }

    public void SumarPuntos(int cantidad)
    {
        puntos += cantidad;
        ActualizarTextoInterfaz(true);
    }

    void ActualizarTextoInterfaz(bool animar)
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = puntos.ToString();

            if (animar)
            {
                // Si ya se estaba animando, la paramos para empezar de nuevo
                if (corrutinaAnimacion != null) StopCoroutine(corrutinaAnimacion);
                corrutinaAnimacion = StartCoroutine(EfectoPunchSmooth());
            }
        }
    }

    IEnumerator EfectoPunchSmooth()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionAnimacion)
        {
            tiempoTranscurrido += Time.deltaTime;
            
            // Calculamos el progreso de 0 a 1
            float progresoFormateado = tiempoTranscurrido / duracionAnimacion;

            // EVALUAMOS LA CURVA: Esta es la magia del "Flow"
            float valorCurva = curvaDePunch.Evaluate(progresoFormateado);

            // APLICAMOS LERP: Interpolamos entre la escala original y la max usando el valor de la curva
            textoPuntuacion.transform.localScale = Vector3.LerpUnclamped(escalaOriginal, escalaMaxima, valorCurva);

            yield return null; // Esperamos al siguiente frame
        }

        // Aseguramos que termine exactamente en la escala original
        textoPuntuacion.transform.localScale = escalaOriginal;
    }
}