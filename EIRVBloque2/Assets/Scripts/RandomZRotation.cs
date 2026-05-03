using UnityEngine;

public class RandomZRotation : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Rango mínimo de rotación en grados")]
    public float minRotation = 0f;

    [Tooltip("Rango máximo de rotación en grados")]
    public float maxRotation = 360f;

    void Start()
    {
        ApplyRandomRotation();
    }

    public void ApplyRandomRotation()
    {
        float randomAngle = Random.Range(minRotation, maxRotation);

        // Mantiene la rotación actual en X e Y, solo cambia Z
        Vector3 currentRotation = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, randomAngle);
    }
}