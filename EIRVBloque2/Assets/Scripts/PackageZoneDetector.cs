using UnityEngine;

public class PackageZoneDetector : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] private PackageSpawner packageSpawner;

    [Header("Tag del paquete")]
    [SerializeField] private string packageTag = "Package";

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(packageTag)) return;

        packageSpawner.RequestSpawnPackage();
    }
}