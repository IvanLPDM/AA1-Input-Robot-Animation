using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public class MyRobotController_6 : MonoBehaviour
{
    public Transform Joint0;
    public Transform Joint1;
    public Transform Joint2;
    public Transform Joint3;
    public Transform Joint4;
    public Transform Joint5;
    public Transform Joint6;
    public Transform endFactor;

    public Transform Stud_target;
    public Transform Workbench_destination;

    public float minAngle;
    public float maxAngle;

    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;
    public LineRenderer lineRenderer4;
    public LineRenderer lineRenderer5;
    public LineRenderer lineRenderer6;
    public LineRenderer lineRenderer7;

    private float l1;
    private float l2;
    private float l3;
    private float l4;
    private float l5;
    private float l6;
    private float l7;

    private float angle0_z = 0f;
    private float angle0_y = 0f;
    private float angle0_x = 0f;
    private float angle1_x = 0f;
    private float angle1_y = 0f;
    private float angle1_z = 0f;
    private float angle2 = 0f;
    private float angle2_z = 0f;
    private float angle3_z = 0f;
    private float angle4_z = 0f;
    private float angle5_z = 0f;
    private float angle6_z = 0f;

    private float initialAngle0;
    private float initialAngle1;
    private float initialAngle2;
    private float initialAngle3;
    private float initialAngle4;
    private float initialAngle5;
    private float initialAngle6;

    public float minAngle_Q;
    public float MaxAngle_A;
    public float MinAngle_W;
    public float MaxAngle_S;
    public float MinAngle_E;
    public float MaxAngle_D;
    public float MinAngle_R;
    public float MaxAngle_F;


    public float rotationSpeed = 50f;

    private bool picked_Target = false;


    // Start is called before the first frame update
    void Start()
    {
        l1 = Vector3.Distance(Joint0.position, Joint1.position);
        l2 = Vector3.Distance(Joint1.position, Joint2.position);
        l3 = Vector3.Distance(Joint2.position, Joint3.position);
        l4 = Vector3.Distance(Joint3.position, Joint4.position);
        l5 = Vector3.Distance(Joint4.position, Joint5.position);
        l6 = Vector3.Distance(Joint5.position, Joint6.position);
        l7 = Vector3.Distance(Joint6.position, endFactor.position);

        InitializeLineRenderer(lineRenderer1);
        InitializeLineRenderer(lineRenderer2);
        InitializeLineRenderer(lineRenderer3);
        InitializeLineRenderer(lineRenderer4);
        InitializeLineRenderer(lineRenderer5);
        InitializeLineRenderer(lineRenderer6);
        InitializeLineRenderer(lineRenderer7);

        IniciarAngulos();
    }

    void IniciarAngulos()
    {
        initialAngle0 = CalculateAngleBetweenPoints(Joint0.position, Joint1.position);
        angle0_z = initialAngle0;

        initialAngle1 = CalculateAngleBetweenPoints(Joint1.position, Joint2.position) ;
        angle1_z = initialAngle1 - angle0_z;

        initialAngle2 = CalculateAngleBetweenPoints(Joint2.position, Joint3.position) ;
        angle2_z = initialAngle2 - (angle0_z + angle1_z);

        initialAngle3 = CalculateAngleBetweenPoints(Joint3.position, Joint4.position);
        angle3_z = initialAngle3 - (angle0_z + angle1_z + angle2_z);

        initialAngle4 = CalculateAngleBetweenPoints(Joint4.position, Joint5.position);
        angle4_z = initialAngle4 - (angle0_z + angle1_z + angle2_z + angle3_z);

        initialAngle5 = CalculateAngleBetweenPoints(Joint5.position, Joint6.position);
        angle5_z = initialAngle5 - (angle0_z + angle1_z + angle2_z + angle3_z + angle4_z);

        initialAngle6 = CalculateAngleBetweenPoints(Joint6.position, endFactor.position);
        angle6_z = initialAngle5 - (angle0_z + angle1_z + angle2_z + angle3_z + angle4_z + angle5_z);
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
        if (Input.GetKey(KeyCode.Q)) angle0_y += rotationSpeed * Time.deltaTime; 
        if (Input.GetKey(KeyCode.A)) angle0_y -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) angle3_z += rotationSpeed * Time.deltaTime; 
        if (Input.GetKey(KeyCode.S)) angle3_z -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) angle4_z += rotationSpeed * Time.deltaTime; 
        if (Input.GetKey(KeyCode.D)) angle4_z -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.R)) angle5_z += rotationSpeed * Time.deltaTime; 
        if (Input.GetKey(KeyCode.F)) angle5_z -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space)) picked_Target = true;

        // Aplicar los límites
        //angle0_y = Mathf.Clamp(angle0_y, minAngle_Q, MaxAngle_A);
        angle3_z = Mathf.Clamp(angle3_z, MinAngle_W, MaxAngle_S);
        angle4_z = Mathf.Clamp(angle4_z, MinAngle_E, MaxAngle_D);
        angle5_z = Mathf.Clamp(angle5_z, MinAngle_R, MaxAngle_F);

        // Calcular la posición de cada joint utilizando FK
        CalculateFK();
    }


    void CalculateFK()
    {
        Vector3 position0 = Joint0.position;
        
        Quaternion rotation1 = Quaternion.Euler(angle0_x, angle0_y, angle0_z);
        Vector3 position1 = position0 + rotation1 * Vector3.right * l1;
        Joint1.position = position1;

        Quaternion rotation2 = rotation1 * Quaternion.Euler(angle1_x, angle1_y, angle1_z);
        Vector3 position2 = position1 + rotation2 * Vector3.right * l2;
        Joint2.position = position2;

        Quaternion rotation3 = rotation2 * Quaternion.Euler(0, 0, angle2_z);
        Vector3 position3 = position2 + rotation3 * Vector3.right * l3;
        Joint3.position = position3;

        Quaternion rotation4 = rotation3 * Quaternion.Euler(0, 0, angle3_z);
        Vector3 position4 = position3 + rotation4 * Vector3.right * l4;
        Joint4.position = position4;

        Quaternion rotation5 = rotation4 * Quaternion.Euler(0, 0, angle4_z);
        Vector3 position5 = position4 + rotation5 * Vector3.right * l5;
        Joint5.position = position5;

        Quaternion rotation6 = rotation5 * Quaternion.Euler(0, 0, angle5_z);
        Vector3 position6 = position5 + rotation6 * Vector3.right * l6;
        Joint6.position = position6;

        Quaternion rotation7 = rotation6 * Quaternion.Euler(0, 0, angle6_z);
        Vector3 positionEndEffector = position6 + rotation7 * Vector3.right * l7;
        endFactor.position = positionEndEffector;

        Joint1.rotation = rotation1;
        Joint2.rotation = rotation2;
        Joint3.rotation = rotation3;
        Joint4.rotation = rotation4;
        Joint5.rotation = rotation5;
        Joint6.rotation = rotation6;
        endFactor.rotation = rotation7;
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
        lineRenderer3.SetPosition(1, Joint3.position);

        lineRenderer4.SetPosition(0, Joint3.position);
        lineRenderer4.SetPosition(1, Joint4.position);

        lineRenderer5.SetPosition(0, Joint4.position);
        lineRenderer5.SetPosition(1, Joint5.position);

        lineRenderer6.SetPosition(0, Joint5.position);
        lineRenderer6.SetPosition(1, Joint6.position);

        lineRenderer7.SetPosition(0, endFactor.position);
        lineRenderer7.SetPosition(1, endFactor.position);
    }

    void catchTarget()
    {
        Stud_target.position = endFactor.position;
    }
}
