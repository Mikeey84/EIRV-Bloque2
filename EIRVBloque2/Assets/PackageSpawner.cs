using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PackageSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject packagePrefab;     
    public Transform spawnPoint;         

    [Header("Animación de spawn")]
    public float scaleUpDuration = 0.3f; 

    private GameObject currentPackage;

    void Start()
    {
        SpawnPackage();
    }

    void SpawnPackage()
    {
        if (currentPackage != null) return;

        currentPackage = Instantiate(packagePrefab, spawnPoint.position, spawnPoint.rotation);

        XRGrabInteractable grab = currentPackage.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnPackageGrabbed);
            grab.selectExited.AddListener(OnPackageReleased);
        }

        StartCoroutine(ScaleUp(currentPackage));
    }

    void OnPackageGrabbed(SelectEnterEventArgs args)
    {
        XRGrabInteractable grab = args.interactableObject.transform.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            // Activar trigger en TODOS los colliders
            foreach (Collider col in grab.GetComponentsInChildren<Collider>())
            {
                col.isTrigger = true;
            }

            grab.selectEntered.RemoveListener(OnPackageGrabbed);
        }

        currentPackage = null;

        StartCoroutine(SpawnAfterDelay(2f));
    }

    void OnPackageReleased(SelectExitEventArgs args)
    {
        XRGrabInteractable grab = args.interactableObject.transform.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            // Volver a collider normal
            foreach (Collider col in grab.GetComponentsInChildren<Collider>())
            {
                col.isTrigger = false;
            }

            grab.selectExited.RemoveListener(OnPackageReleased);
        }
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnPackage();
    }

    IEnumerator ScaleUp(GameObject obj)
    {
        float elapsed = 0f;
        obj.transform.localScale = Vector3.zero;

        while (elapsed < scaleUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleUpDuration;
            float scale = Mathf.SmoothStep(0f, 1f, t);
            obj.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        obj.transform.localScale = Vector3.one;
    }
}