using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PackageSpawner : MonoBehaviour
{
    [Header("Configuraci�n")]
    public GameObject packagePrefab;        // Tu prefab del paquete
    public Transform spawnPoint;            // Donde aparece el nuevo paquete

    [Header("Animaci�n de spawn")]
    public float scaleUpDuration = 0.3f;    // Tiempo de animaci�n de aparici�n

    private GameObject currentPackage;

    void Start()
    {
        //SpawnPackage();
    }

    void Update()
    {
        SpawnPackage();
    }

    void SpawnPackage()
    {
        if (currentPackage != null) return;

        currentPackage = Instantiate(packagePrefab, spawnPoint.position, spawnPoint.rotation);

        // Suscribirse al evento de cuando el jugador lo agarra
        XRGrabInteractable grab = currentPackage.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnPackageGrabbed);
        }

        // Animaci�n de aparici�n
        StartCoroutine(ScaleUp(currentPackage));
    }

    void OnPackageGrabbed(SelectEnterEventArgs args)
    {
        // Desuscribirse para no llamarlo dos veces
        XRGrabInteractable grab = currentPackage.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnPackageGrabbed);
        }

        // El paquete actual ya no es "el de la furgoneta"
        currentPackage = null;

        // Instanciar uno nuevo tras un peque�o delay
        StartCoroutine(SpawnAfterDelay(0.5f));
    }

    System.Collections.IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnPackage();
    }

    System.Collections.IEnumerator ScaleUp(GameObject obj)
    {
        float elapsed = 0f;
        obj.transform.localScale = Vector3.zero;

        while (elapsed < scaleUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleUpDuration;
            // Curva de rebote suave
            float scale = Mathf.SmoothStep(0f, 1f, t);
            obj.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        obj.transform.localScale = Vector3.one;
    }
}