using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    float Horinput, VerInput;

   [SerializeField] float MoveSpeed;
   [SerializeField]  bool Jump = false;
   [SerializeField] float JumpHeight = 0.4f;
   [SerializeField] float Gravity = 0.0045f;

    Vector3 HeightMovement;
    Vector3 HorizontalMovement;
    Vector3 VerticalMovement;


    CharacterController characterController;
    GameObject camera;
    CameraLogic cameraLogic;

    Animator animator;
    [SerializeField] List<AudioClip> EarthFootSteps = new List<AudioClip>();
    [SerializeField] List<AudioClip> StoneFootSteps = new List<AudioClip>();
    [SerializeField] List<AudioClip> PuddleFootSteps = new List<AudioClip>();
    [SerializeField] List<AudioClip> GrassFootSteps = new List<AudioClip>();
    AudioSource audioSource;

    [SerializeField] Transform LeftFoot;
    [SerializeField] Transform RightFoot;

    [SerializeField] Transform m_LeftHandTransform;

    WeaponLogic weaponLogic;

   [Header("Foot IK Variables")]
   [SerializeField, Tooltip("Foot ik goal position")] Vector3 targetPOS;
   [SerializeField, Tooltip("targetPos_Y_OffSet adjustment for foot IK position")] float targetPos_Y_OffSet;
   [SerializeField, Tooltip("Height restriction for ik position set")] float rayStartPos_Y_OffSet;

    bool m_isCrouching = false;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        camera = Camera.main.gameObject;
        weaponLogic = GetComponentInChildren<WeaponLogic>();

        if (camera)
        {
            cameraLogic = camera.GetComponent<CameraLogic>();
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C) && characterController.isGrounded)
        {
            m_isCrouching = !m_isCrouching; // this format allows the bool to be swaped from true to false and then from false to true (repeating that order)

            if (animator)
            {
                animator.SetBool("IsCrouching", m_isCrouching);
            }
        }
        if (m_isCrouching)
        {
            Horinput = 0f;
            VerInput = 0f;
            return;
        }
        Horinput = Input.GetAxis("Horizontal");
        VerInput = Input.GetAxis("Vertical");

       

        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            Jump = true;
        }

        if (animator)
        {
            animator.SetFloat("VerticalInput", VerInput);
            animator.SetFloat("HorizontalInput", Horinput);
        }

    }

     void FixedUpdate()
    {
        if (Jump)
        {
            HeightMovement.y = JumpHeight;
            Jump = false;
        }

        //Player rotaion according to Camera forward vector, Accesses camerLogic
        if (cameraLogic && (Mathf.Abs(Horinput) > 0.1f || Mathf.Abs(VerInput) > 0.1f))
        {
            transform.forward = cameraLogic.GetForwardVector(); // accessing the player forward vector to the cameras forward vector
        }

        HeightMovement.y -= Gravity * Time.deltaTime;

        // Movement assignment
        VerticalMovement = transform.forward * VerInput * MoveSpeed * Time.deltaTime;
        HorizontalMovement = transform.right * Horinput * MoveSpeed * Time.deltaTime;
        characterController.Move(HorizontalMovement + HeightMovement + VerticalMovement);


        // reseting the jump vector on plyaer grounding,  ready to add y value to the player position for next jump
        if (characterController.isGrounded)
        {
            HeightMovement.y = 0.0f;
        }


    }
                        
    void PlayFootStepSound(int FootStepsIndex)   // 0 = left foot   1 = right foot
    {
        if (FootStepsIndex == 0) 
        {
            RayCastToTerrain(LeftFoot.position);
        }
        else if (FootStepsIndex == 1)
        {
            RayCastToTerrain(RightFoot.position);
           
        }
    }

    //PLay through a random range of sounds within the list elements
     void PlayRandomSound(List<AudioClip> audioClips)
    {
        if (audioClips.Count > 0 && audioSource)
        {
            int randomNum = Random.Range(0, audioClips.Count );

            audioSource.PlayOneShot(audioClips[randomNum]);
        }
    }


    // Raycasting to the floot, checking the gameobject tag upon collision, playing appropriate sound .... calls PlayRandomSound()
    void RayCastToTerrain(Vector3 Position)
    {
        LayerMask layermask = LayerMask.GetMask("Terrain");
    
        Ray ray = new Ray(Position, Vector3.down);
        RaycastHit hit;
        Color color = Color.red;

        if (Physics.Raycast(ray, out hit, layermask))
        {
           string hitTag = hit.collider.gameObject.tag;

            if (hitTag == "Earth")
            {
                PlayRandomSound(EarthFootSteps);
            }
            else if (hitTag == "Stone")
            {
                PlayRandomSound(StoneFootSteps);
            }
            else if (hitTag == "Grass")
            {
                PlayRandomSound(GrassFootSteps);
            }
            else if (hitTag == "Puddle")
            {
                PlayRandomSound(PuddleFootSteps);
            }
        }

    }

   

    void OnAnimatorIK(int layerIndex)
    {
        if (animator && cameraLogic) // accesses the cameralogic and rotates neck and shoulders according to cameras orientation
        {
            animator.SetBoneLocalRotation(HumanBodyBones.Neck, Quaternion.Euler(cameraLogic.Rotation_X, 0, 0));
            animator.SetBoneLocalRotation(HumanBodyBones.RightShoulder, Quaternion.Euler(cameraLogic.Rotation_X, 0, 0));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftShoulder, Quaternion.Euler(cameraLogic.Rotation_X, 0, 0));
        }

        if (m_LeftHandTransform && animator)
        {
            if (!weaponLogic.isReloading()) // while not reloading this sets the left hand roatation and position
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, m_LeftHandTransform.transform.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, m_LeftHandTransform.transform.rotation);
            }
            else // while reloading we set the weights to 0 in order to release the left hand from the ik position and play reload animation properly 
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }

        if (LeftFoot)
        {
            updateFootIK(AvatarIKGoal.LeftFoot, LeftFoot);
        }

        if (RightFoot)
        {
            updateFootIK(AvatarIKGoal.RightFoot, RightFoot);
        }

    }

    // Raycasts to the floor and sets the position and rotation of the foot bones to the floors position and rotation
    void updateFootIK(AvatarIKGoal avatarIKgoal, Transform footTransform)
    {
        Vector3 rayStartPos = footTransform.transform.position;
        float rayStartPos_Y_OffSet = 0.50f;
        rayStartPos.y += rayStartPos_Y_OffSet;

        Ray ray = new Ray(rayStartPos, Vector3.down);
        RaycastHit rayHit;

        LayerMask obstacleLayerMask = LayerMask.GetMask("Obstacle");

        if (Physics.Raycast(ray, out rayHit, 1.0f, obstacleLayerMask))
        {
            targetPOS = rayHit.point;
            targetPOS.y += targetPos_Y_OffSet;
            
            animator.SetIKPositionWeight(avatarIKgoal, 1);
            animator.SetIKRotationWeight(avatarIKgoal, 1);
            animator.SetIKPosition(avatarIKgoal, targetPOS);
            animator.SetIKRotation(avatarIKgoal, footTransform.rotation);
        }
    }

}
