using UnityEngine;

public class EndZone : MonoBehaviour
{
    public CanvaManager canvaManager; // Referencia al script CanvaManager

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // Asegúrate de que el coche tenga la etiqueta "Player"
        {
            canvaManager.MostrarCanvaFinal(); // Llama al método para mostrar la pantalla final
        }
    }
}
