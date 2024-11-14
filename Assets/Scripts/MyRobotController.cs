using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class MyRobotController : MonoBehaviour
{
    public Transform Joint0;
    public Transform Joint1;
    public Transform Joint2;
    public Transform endFactor;
    public Transform Stud_target;
    public Transform Workbench_destination;

    public float minAngle;
    public float maxAngle;

    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;

    private float l1;
    private float l2;
    private float l3;


    // Start is called before the first frame update
    void Start()
    {
        l1 = Vector3.Distance(Joint0.position, Joint1.position);
        l2 = Vector3.Distance(Joint1.position, Joint2.position);
        l3 = Vector3.Distance(Joint2.position, endFactor.position);

        InitializeLineRenderer(lineRenderer1);
        InitializeLineRenderer(lineRenderer2);
        InitializeLineRenderer(lineRenderer3);
    }

    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        // Set up the LineRenderer properties like width, color, etc.
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2; // Each bone only needs 2 points (start and end)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // Basic material for 2D
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.grey;


    }

    void Update()
    {
        UpdateVisualLinks();
    }

    void MoveTarget(Transform target)
    {
        target.position = endFactor.position;
    }

    void UpdateVisualLinks()
    {
        lineRenderer1.SetPosition(0, Joint0.position);
        lineRenderer1.SetPosition(1, Joint1.position);

        lineRenderer2.SetPosition(0, Joint1.position);
        lineRenderer2.SetPosition(1, Joint2.position);

        lineRenderer3.SetPosition(0, Joint2.position);
        lineRenderer3.SetPosition(1, endFactor.position);
    }
}
