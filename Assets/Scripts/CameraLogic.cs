using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    Vector3 CameraTarget;
    GameObject PLayer;

   [SerializeField] float CameraTargetOffset_X;
   [SerializeField] float CameraTargetOffset_Y;
   public float Rotation_Y;

    // ROtation Range
    public float Rotation_X;
    const float Min_X = -15.0f;
    const float Max_X =  15.0f;

    // Zoom Range
    [SerializeField] float Distance_Z;
    const float Min_Z = 1.76f;
    const float Max_Z =  8.0f;

    bool isAiming = false;
    float aimPos_X = 0.65f;
    float aimPos_Y = 1.55f;
    float aimPos_Z = -0.7f;

    float m_Aimrotation_Y;

    // Start is called before the first frame update
    void Start()
    {
        PLayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CameraTarget = PLayer.transform.position; // set camera target to player
        CameraTarget.y += CameraTargetOffset_Y; // set camera height to Camera Offset.Y
        CameraTarget.x += CameraTargetOffset_X; // set camera offset.x

        if (Input.GetButtonDown("Fire3"))
        {
            isAiming = !isAiming;
            if (isAiming)
            {
                m_Aimrotation_Y = m_Aimrotation_Y;
                PLayer.transform.rotation = Quaternion.Euler(0, m_Aimrotation_Y, 0); 
            }
            else
            {
                Rotation_Y = m_Aimrotation_Y;
            }
        }

        if (Input.GetButton("Fire2")) // while holding down right mouse click
        {
            Rotation_Y += Input.GetAxis("Mouse X");
            Rotation_X += Input.GetAxis("Mouse Y");

            Rotation_X =  Mathf.Clamp(Rotation_X, Min_X, Max_X); //Restict the roatation around the X Axis
        }

        Distance_Z -= Input.GetAxis("Mouse ScrollWheel"); // zoom input
        Distance_Z = Mathf.Clamp(Distance_Z, Min_Z, Max_Z); // zoom range

    }

     void LateUpdate()
    {
        if (!isAiming)
        {
            Quaternion CameraRotation = Quaternion.Euler(Rotation_X, Rotation_Y, 0); // roatation statement
            Vector3 CameraOffset = new Vector3(0, 0, -Distance_Z); // Zoom statement


            transform.position = CameraTarget + CameraRotation * CameraOffset; // assign the position of the camera to the target added by the product of the rotation multiplied by the zoom
            transform.LookAt(CameraTarget);
        }
        else
        {
            CameraTarget = PLayer.transform.position;
            Vector3 cameraOffSet = new Vector3(aimPos_X, aimPos_Y, aimPos_Z);

            Quaternion CameraRotation = PLayer.transform.rotation;

            transform.position = CameraTarget + CameraRotation * cameraOffSet;

        }
        
    }



    //Functions below are for player logic to access


    public Vector3 GetForwardVector()
    {
        Quaternion rotation = Quaternion.Euler(0, Rotation_Y, 0);
        return rotation * Vector3.forward;
    }

    public float GetRotationX()
    {
        return Rotation_X;
    }
}
