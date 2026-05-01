using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RoutePoint
{
    public Transform destination;

    [Min(0)]
    public float waitTime = 0f;

    public Color gizmoColor = Color.yellow;
}

public class CarAI : MonoBehaviour
{
    [Header("Car Wheels - WheelCollider")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;

    [Header("Car Wheels - Visual Transform")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("Car Front")]
    public Transform carFront;

    [Header("Movement")]
    public int MaxSteeringAngle = 45;
    public int MaxRPM = 150;
    public float motorTorque = 400f;
    public float brakeTorque = 5000f;
    public float reachDistance = 2f;

    [Header("Route")]
    public List<RoutePoint> routePoints = new List<RoutePoint>();
    public bool loopRoute = true;

    [Header("NavMesh")]
    public List<string> NavMeshLayers = new List<string>() { "AllAreas" };

    [Header("Debug")]
    public bool ShowGizmos = true;
    public bool Debugger = false;

    [Header("Legacy Compatibility")]
    [HideInInspector] public bool Patrol = false;

    public bool move = true;

    private readonly List<Vector3> pathPoints = new List<Vector3>();

    private int currentRouteIndex = 0;
    private int currentPathIndex = 0;

    private int navMeshMask = NavMesh.AllAreas;
    private bool waiting = false;
    private bool routeFinished = false;

    private Vector3 positionToFollow;
    private float localMaxSpeed;

    private void Awake()
    {
        if (carFront == null)
            carFront = transform;

        positionToFollow = carFront.position;
        CalculateNavMeshMask();
    }

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.centerOfMass = Vector3.zero;

        StartRoute();
    }

    private void FixedUpdate()
    {
        UpdateWheels();

        if (!move || waiting || routeFinished)
        {
            ApplyBrakes();
            return;
        }

        FollowRoute();
        ApplySteering();
        Movement();
    }

    private void StartRoute()
    {
        CalculateNavMeshMask();

        currentRouteIndex = 0;
        currentPathIndex = 0;
        routeFinished = false;
        waiting = false;

        if (!HasValidRoute())
        {
            DebugMessage("No hay puntos de ruta válidos.", true);
            routeFinished = true;
            return;
        }

        GeneratePathToCurrentRoutePoint();
    }

    private bool HasValidRoute()
    {
        if (routePoints == null || routePoints.Count == 0)
            return false;

        foreach (RoutePoint point in routePoints)
        {
            if (point != null && point.destination != null)
                return true;
        }

        return false;
    }

    private void FollowRoute()
    {
        if (pathPoints.Count == 0)
        {
            ArriveToRoutePoint();
            return;
        }

        if (currentPathIndex >= pathPoints.Count)
        {
            ArriveToRoutePoint();
            return;
        }

        positionToFollow = pathPoints[currentPathIndex];

        float distance = Vector3.Distance(carFront.position, positionToFollow);

        if (distance <= reachDistance)
            currentPathIndex++;

        Transform finalDestination = routePoints[currentRouteIndex].destination;

        if (finalDestination != null)
        {
            float finalDistance = Vector3.Distance(carFront.position, finalDestination.position);

            if (finalDistance <= reachDistance)
                ArriveToRoutePoint();
        }
    }

    private void ArriveToRoutePoint()
    {
        if (!waiting)
            StartCoroutine(WaitAndGoNextPoint());
    }

    private IEnumerator WaitAndGoNextPoint()
    {
        waiting = true;
        ApplyBrakes();

        RoutePoint point = routePoints[currentRouteIndex];

        if (point.waitTime > 0f)
            yield return new WaitForSeconds(point.waitTime);

        currentRouteIndex++;

        if (currentRouteIndex >= routePoints.Count)
        {
            if (loopRoute)
            {
                currentRouteIndex = 0;
            }
            else
            {
                routeFinished = true;
                waiting = false;
                ApplyBrakes();
                yield break;
            }
        }

        GeneratePathToCurrentRoutePoint();
        waiting = false;
    }

    private void GeneratePathToCurrentRoutePoint()
    {
        pathPoints.Clear();
        currentPathIndex = 0;

        if (currentRouteIndex < 0 || currentRouteIndex >= routePoints.Count)
        {
            move = false;
            return;
        }

        Transform destination = routePoints[currentRouteIndex].destination;

        if (destination == null)
        {
            DebugMessage("Hay un punto de ruta sin destination asignado.", true);
            routeFinished = true;
            move = false;
            return;
        }

        NavMeshPath navPath = new NavMeshPath();

        bool sourceFound = NavMesh.SamplePosition(
            carFront.position,
            out NavMeshHit sourceHit,
            10f,
            navMeshMask
        );

        bool destinationFound = NavMesh.SamplePosition(
            destination.position,
            out NavMeshHit destinationHit,
            10f,
            navMeshMask
        );

        if (!sourceFound || !destinationFound)
        {
            DebugMessage("No se pudo encontrar posición válida en el NavMesh.", true);
            routeFinished = true;
            return;
        }

        bool pathFound = NavMesh.CalculatePath(
            sourceHit.position,
            destinationHit.position,
            navMeshMask,
            navPath
        );

        if (!pathFound || navPath.status != NavMeshPathStatus.PathComplete || navPath.corners.Length == 0)
        {
            DebugMessage("No se pudo calcular una ruta completa.", true);
            routeFinished = true;
            return;
        }

        pathPoints.Clear();
        pathPoints.Add(destination.position);
        positionToFollow = destination.position;

        Debug.Log("Ruta generada hacia punto " + currentRouteIndex);
    }

    private void CalculateNavMeshMask()
    {
        navMeshMask = 0;

        if (NavMeshLayers == null || NavMeshLayers.Count == 0)
        {
            navMeshMask = NavMesh.AllAreas;
            return;
        }

        if (NavMeshLayers.Contains("AllAreas"))
        {
            navMeshMask = NavMesh.AllAreas;
            return;
        }

        foreach (string layer in NavMeshLayers)
        {
            int area = NavMesh.GetAreaFromName(layer);

            if (area >= 0)
                navMeshMask |= 1 << area;
        }

        if (navMeshMask == 0)
            navMeshMask = NavMesh.AllAreas;
    }

    private void ApplySteering()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(positionToFollow);

        if (relativeVector.magnitude < 0.1f)
            return;

        float steeringAngle = (relativeVector.x / relativeVector.magnitude) * MaxSteeringAngle;
        steeringAngle = Mathf.Clamp(steeringAngle, -MaxSteeringAngle, MaxSteeringAngle);

        localMaxSpeed = Mathf.Abs(steeringAngle) > 15f ? MaxRPM * 0.6f : MaxRPM;

        frontLeft.steerAngle = steeringAngle;
        frontRight.steerAngle = steeringAngle;
    }

    private void Movement()
    {
        ReleaseBrakes();

        int wheelSpeed = Mathf.Abs((int)(
            frontLeft.rpm +
            frontRight.rpm +
            backLeft.rpm +
            backRight.rpm
        ) / 4);

        if (wheelSpeed < localMaxSpeed)
            SetMotorTorque(motorTorque);
        else
            SetMotorTorque(0f);
    }

    private void SetMotorTorque(float torque)
    {
        frontLeft.motorTorque = torque;
        frontRight.motorTorque = torque;
        backLeft.motorTorque = torque;
        backRight.motorTorque = torque;
    }

    private void ReleaseBrakes()
    {
        frontLeft.brakeTorque = 0f;
        frontRight.brakeTorque = 0f;
        backLeft.brakeTorque = 0f;
        backRight.brakeTorque = 0f;
    }

    private void ApplyBrakes()
    {
        SetMotorTorque(0f);

        frontLeft.brakeTorque = brakeTorque;
        frontRight.brakeTorque = brakeTorque;
        backLeft.brakeTorque = brakeTorque;
        backRight.brakeTorque = brakeTorque;
    }

    private void UpdateWheels()
    {
        UpdateWheel(frontLeft, wheelFL);
        UpdateWheel(frontRight, wheelFR);
        UpdateWheel(backLeft, wheelBL);
        UpdateWheel(backRight, wheelBR);
    }

    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelCollider == null || wheelTransform == null)
            return;

        wheelCollider.ConfigureVehicleSubsteps(5, 12, 15);

        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    public void RestartRoute()
    {
        StopAllCoroutines();
        StartRoute();
    }

    public void RandomPath()
    {
        DebugMessage("RandomPath() llamado, pero esta IA ahora usa solo routePoints. Reiniciando ruta.", false);
        RestartRoute();
    }

    public void CustomPath(Transform destination)
    {
        if (destination == null)
        {
            DebugMessage("CustomPath() recibió un destino null.", true);
            return;
        }

        StopAllCoroutines();

        routePoints.Clear();

        RoutePoint newPoint = new RoutePoint();
        newPoint.destination = destination;
        newPoint.waitTime = 0f;
        newPoint.gizmoColor = Color.yellow;

        routePoints.Add(newPoint);

        loopRoute = false;
        RestartRoute();
    }

    private void DebugMessage(string message, bool error)
    {
        if (!Debugger)
            return;

        if (error)
            Debug.LogError(message, this);
        else
            Debug.Log(message, this);
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos)
            return;

        if (routePoints != null)
        {
            for (int i = 0; i < routePoints.Count; i++)
            {
                RoutePoint point = routePoints[i];

                if (point == null || point.destination == null)
                    continue;

                Gizmos.color = point.gizmoColor;
                Gizmos.DrawWireSphere(point.destination.position, 3f);

                if (i == currentRouteIndex)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(point.destination.position, 4f);
                }

                int nextIndex = i + 1;

                if (nextIndex >= routePoints.Count)
                {
                    if (!loopRoute)
                        continue;

                    nextIndex = 0;
                }

                if (routePoints[nextIndex] != null && routePoints[nextIndex].destination != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(
                        point.destination.position,
                        routePoints[nextIndex].destination.position
                    );
                }
            }
        }

        if (pathPoints != null)
        {
            for (int i = 0; i < pathPoints.Count; i++)
            {
                Gizmos.color = i == currentPathIndex ? Color.blue : Color.red;
                Gizmos.DrawWireSphere(pathPoints[i], 1.5f);

                if (i < pathPoints.Count - 1)
                    Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            }
        }
    }
}