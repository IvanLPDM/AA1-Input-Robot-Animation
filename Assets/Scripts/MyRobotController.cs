using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;
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

    public float angle0_z = 0f;
    public float angle0_y = 0f;
    public float angle0_x = 0f;

    public float angle1 = 0f;
    public float angle1_z = 0f;

    public float angle2 = 0f;
    public float angle2_z = 0f;

    private float initialAngle0;
    private float initialAngle1;
    private float initialAngle2;
    private float initialAngle3;


    public float rotationSpeed = 50f;

    private bool picked_Target = false;


    // Start is called before the first frame update
    void Start()
    {
        l1 = Vector3.Distance(Joint0.position, Joint1.position);
        l2 = Vector3.Distance(Joint1.position, Joint2.position);
        l3 = Vector3.Distance(Joint2.position, endFactor.position);

        InitializeLineRenderer(lineRenderer1);
        InitializeLineRenderer(lineRenderer2);
        InitializeLineRenderer(lineRenderer3);

        IniciarAngulos();
    }

    void IniciarAngulos()
    {
        initialAngle0 = CalculateAngleBetweenPoints(Joint0.position, Joint1.position);
        angle0_z = initialAngle0;


        initialAngle1 = CalculateAngleBetweenPoints(Joint1.position, Joint2.position) ;
        angle1_z = initialAngle1 - angle0_z;

        initialAngle2 = CalculateAngleBetweenPoints(Joint2.position, endFactor.position) ;
        angle2_z = initialAngle2 - (angle0_z + angle1_z);


        //initialAngle3 = CalculateAngleBetweenPoints(endFactor.position, Joint1.position);


        Debug.Log(initialAngle0);
        Debug.Log(initialAngle1);
        Debug.Log(initialAngle2);
        //angle4_z = initialAngle3;
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
        InputHope();
        UpdateVisualLinks();


        if(picked_Target)
        {
            catchTarget(); 
        }
    }

    void InputHope()
    {
        // Captura de inputs para rotar los joints
        if (Input.GetKey(KeyCode.Q)) angle0_y += rotationSpeed * Time.deltaTime; // Rotación Joint1
        if (Input.GetKey(KeyCode.A)) angle0_y -= rotationSpeed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.W)) angle1_y += rotationSpeed * Time.deltaTime; 
        //if (Input.GetKey(KeyCode.S)) angle1_y -= rotationSpeed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.E)) angle1 += rotationSpeed * Time.deltaTime; // Rotación Joint2
        //if (Input.GetKey(KeyCode.D)) angle1 -= rotationSpeed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.R)) angle2 += rotationSpeed * Time.deltaTime; // Rotación Joint3
        //if (Input.GetKey(KeyCode.F)) angle2 -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space)) picked_Target = true;

        // Calcular la posición de cada joint utilizando FK
        CalculateFK();
    }


    void CalculateFK()
    {
        Vector3 position0 = Joint0.position;
        
        Quaternion rotation1 = Quaternion.Euler(angle0_x, angle0_y, angle0_z);
        Vector3 position1 = position0 + rotation1 * Vector3.right * l1;
        Joint1.position = position1;

        Quaternion rotation2 = rotation1 * Quaternion.Euler(0, angle1, angle1_z);
        Vector3 position2 = position1 + rotation2 * Vector3.right * l2;
        Joint2.position = position2;

        Quaternion rotation3 = rotation2 * Quaternion.Euler(0, angle2, angle2_z);
        Vector3 positionEndEffector = position2 + rotation3 * Vector3.right * l3;
        endFactor.position = positionEndEffector;

        Joint1.rotation = rotation1;
        Joint2.rotation = rotation2;
        endFactor.rotation = rotation3;
    }

    float CalculateAngleBetweenPoints(Vector3 pointA, Vector3 pointB)
    {
        // Diferencia en X y Y
        float deltaX = pointB.x - pointA.x;
        float deltaY = pointB.y - pointA.y;

        // Calcula el ángulo en radianes y conviértelo a grados
        float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

        // Devuelve el ángulo
        return angle;
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

    void catchTarget()
    {
        Stud_target.position = endFactor.position;
    }
}
