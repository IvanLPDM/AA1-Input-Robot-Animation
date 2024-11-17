using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

enum phase
{
    R1, R2, R3, R4, R5, R6, R7, R8, R9, R10, R11, R12
}


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

    public Transform pinza1;
    public Transform pinza2;

    public Transform pinzaClose1;
    public Transform pinzaClose2;

    public Transform Stud_target;
    public Transform Workbench_destination;

    private Rigidbody rb;

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

    public bool PickStudAnimStart;


    public float rotationSpeed = 50f;

    private bool picked_Target = false;
    private bool closePinzas = false;

    private phase phaseRotate = phase.R1;


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

        pinzaClose1.gameObject.SetActive(false);
        pinzaClose2.gameObject.SetActive(false);

        rb = Stud_target.GetComponent<Rigidbody>();
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

        if(!closePinzas)
        {
            dropTarget();
            
        }
        else
        {
            catchTarget();
        }

        if(PickStudAnimStart)
        {
            PickStudAnim();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            closePinzas = !closePinzas;
            picked_Target = true;
        }

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
        float deltaX = pointB.x - pointA.x;
        float deltaY = pointB.y - pointA.y;

        float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

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
        pinza1.gameObject.SetActive(false);
        pinza2.gameObject.SetActive(false);

        pinzaClose1.gameObject.SetActive(true);
        pinzaClose2.gameObject.SetActive(true);

        float distancia = Vector3.Distance(Stud_target.position, endFactor.position);

        if(distancia <= 1.0f)
        {
            rb.isKinematic = true; 
            rb.detectCollisions = false;
            Stud_target.SetParent(endFactor);
        }
        //
    }

    void dropTarget()
    {
        pinza1.gameObject.SetActive(true);
        pinza2.gameObject.SetActive(true);

        pinzaClose1.gameObject.SetActive(false);
        pinzaClose2.gameObject.SetActive(false);

        rb.isKinematic = false;
        rb.detectCollisions = true;
        Stud_target.SetParent(null);
    }

    void PickStudAnim()
    {
        switch(phaseRotate)
        {
            case phase.R1:

                if (angle0_y <= 130.104)
                {
                    angle0_y += rotationSpeed * Time.deltaTime;
                }
                else
                    phaseRotate = phase.R2;

                    break;

            case phase.R2:

                if (angle3_z <= 115.113)
                {
                    angle3_z += rotationSpeed * Time.deltaTime;
                }
                else
                    phaseRotate = phase.R3;

                if(angle4_z <= -40.718)
                {
                    angle4_z -= rotationSpeed * Time.deltaTime;
                }

                break;
            case phase.R3:

                if (angle3_z >= 90)
                {
                    angle3_z -= rotationSpeed * Time.deltaTime;
                }
                else
                    phaseRotate = phase.R4;

                if (angle4_z <= -40.718)
                {
                    angle4_z -= rotationSpeed * Time.deltaTime;
                }

                break;
            case phase.R4:

                if (angle5_z >= 34.906)
                {
                    angle5_z -= rotationSpeed * Time.deltaTime;
                }
                else
                {
                    closePinzas = !closePinzas;
                    picked_Target = true;
                    phaseRotate = phase.R5;
                }

                break;
            case phase.R5:

                if (angle0_y >= -31.004)
                {
                    angle0_y -= rotationSpeed * Time.deltaTime;
                }
                else
                    phaseRotate = phase.R6;

                if (angle4_z <= -110)
                {
                    angle4_z += rotationSpeed * Time.deltaTime;
                }

                break;
            case phase.R6:

                if (angle3_z >= 50)
                {
                    angle3_z -= rotationSpeed * Time.deltaTime;
                }
                else
                phaseRotate = phase.R7;

                if (angle4_z <= -70)
                {
                    angle4_z += rotationSpeed * Time.deltaTime;
                }

                break;
            case phase.R7:

                if (angle5_z >= -15)
                {
                    angle5_z -= rotationSpeed * Time.deltaTime;
                }
                else
                {
                    closePinzas = !closePinzas;
                    picked_Target = true;
                    phaseRotate = phase.R8;
                }

                break;
            case phase.R8:

                break;
            case phase.R9:

                break;
        }

        CalculateFK();
    }
}
