using UnityEngine;

public class GestorDeEntregas : MonoBehaviour
{
   
    public int puntos = 0;

    private void OnTriggerEnter(Collider otroObjeto)
    {
        if (otroObjeto.CompareTag("paquetes"))
        {
            // Sumamos 1 punto
            puntos += 1;

            // Mostramos el mensaje en la consola de Unity
            Debug.Log("¡Paquete entregado! Puntos totales: " + puntos);

            Destroy(otroObjeto.gameObject);
        }
    }
}