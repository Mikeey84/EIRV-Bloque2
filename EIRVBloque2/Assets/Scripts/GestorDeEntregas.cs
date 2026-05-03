using UnityEngine;
using TMPro; // Necesario para controlar el texto LED
using System.Collections;

public class GestorDeEntregas : MonoBehaviour
{
    [SerializeField]
    private int puntos = 0;
    
    [SerializeField]
    private TextMeshPro textoPuntuacion; // Arrastra aquí el texto de tu pantalla LED
    
    [SerializeField]
    private float escalaImpacto = 1.2f; // Tamaño al que crece
    [SerializeField]
    private float duracionAnimacion = 0.1f;
    private Vector3 escalaOriginal;

    private void Start()
    {
        // Guardamos la escala inicial del objeto de texto
        if (textoPuntuacion != null)
        {
            escalaOriginal = textoPuntuacion.transform.localScale;
            ActualizarInterfaz();
        }
    }

    private void OnTriggerEnter(Collider otroObjeto)
    {
        if (otroObjeto.CompareTag("paquetes"))
        {
            // Sumamos 1 punto
            puntos += 1;

            // Feedback en consola
            Debug.Log("¡Paquete entregado! Puntos totales: " + puntos);

            // Actualizamos la pantalla y lanzamos la animación
            ActualizarInterfaz();
            
            // Destruimos el paquete
            Destroy(otroObjeto.gameObject);
        }
    }

    void ActualizarInterfaz()
    {
        if (textoPuntuacion != null)
        {
            // Cambiamos el texto
            textoPuntuacion.text = puntos.ToString();
            
            // Lanzamos el efecto visual de "golpe"
            StopAllCoroutines(); // Por si entran dos paquetes muy rápido
            StartCoroutine(EfectoPunch());
        }
    }

    IEnumerator EfectoPunch()
    {
        // Crece de golpe
        textoPuntuacion.transform.localScale = escalaOriginal * escalaImpacto;
        
        // Espera un instante
        yield return new WaitForSeconds(duracionAnimacion);
        
        // Vuelve a su tamaño normal
        textoPuntuacion.transform.localScale = escalaOriginal;
    }
}