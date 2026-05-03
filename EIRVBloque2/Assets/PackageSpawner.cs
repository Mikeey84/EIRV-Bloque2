using System.Collections;
using UnityEngine;

public class PackageSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject packagePrefab;
    public Transform spawnPoint;

    public GameObject truck;

    [Header("Animación de spawn")]
    public float scaleUpDuration = 0.3f;

    [Header("Spawn delay")]
    public float spawnDelay = 2f;

    private GameObject currentPackage;
    private bool isSpawning;

    void Start()
    {
        SpawnPackage();
    }

    public void RequestSpawnPackage()
    {
        if (isSpawning) return;

        currentPackage = null;
        StartCoroutine(SpawnAfterDelay(spawnDelay));
    }

    void SpawnPackage()
    {
        if (currentPackage != null) return;

        currentPackage = Instantiate(packagePrefab, spawnPoint.position, spawnPoint.rotation);
        currentPackage.transform.parent = truck.transform;
        StartCoroutine(ScaleUp(currentPackage));
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        isSpawning = true;

        yield return new WaitForSeconds(delay);

        SpawnPackage();

        isSpawning = false;
    }

    IEnumerator ScaleUp(GameObject obj)
    {
        float elapsed = 0f;
        obj.transform.localScale = Vector3.zero;

        while (elapsed < scaleUpDuration)
        {
            if (obj == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / scaleUpDuration;
            float scale = Mathf.SmoothStep(0f, 1f, t);
            obj.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        if (obj != null)
            obj.transform.localScale = Vector3.one;
    }
}