using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [SerializeField] Transform m_BulletSpawnPosition;
    LineRenderer m_lineRenderer;

    [SerializeField] GameObject m_ImpactPOS;
    MeshRenderer m_ImpactMeshRenderer;

    float lineRendererLength = 10f;

    const int MAX_BULLETS = 30;
    int BulletCount = MAX_BULLETS;

    const float MAX_COOLDOWN = 0.1f;
    float shot_CoolDown = MAX_COOLDOWN;


    AudioSource audio;
    [SerializeField] AudioClip Shot;
    [SerializeField] AudioClip Reloading;
    [SerializeField] AudioClip EmptyClip;

    Animator m_Animator;

    bool m_isReloading = false;

    [SerializeField] GameObject gunShotCrack;


    // Start is called before the first frame update
    void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        audio = GetComponent<AudioSource>();
        m_Animator = GetComponentInParent<Animator>();

        if (m_ImpactPOS)
        {
            m_ImpactMeshRenderer = m_ImpactPOS.GetComponent<MeshRenderer>();
            m_ImpactMeshRenderer.enabled = false;
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateLineRenderer();
        
        //ShootingLogics

        if(shot_CoolDown > 0f)
        {
            shot_CoolDown -= Time.deltaTime;
        }
       
        if (Input.GetButtonDown("Fire1") && shot_CoolDown <= 0.0f && !m_isReloading)
        {
            if(BulletCount > 0)
            {
                Shoot();
            }

            else
            {
                PlaySound(EmptyClip);
            }

            shot_CoolDown = MAX_COOLDOWN;
        }
        //reloadinglogic

        if (Input.GetKeyDown(KeyCode.R) && !m_isReloading)
        {
            Reload();
        }
    }

    void PlaySound(AudioClip sound)
    {
        if(audio && sound)
        {
            audio.PlayOneShot(sound);
        }

    }
    void Shoot()
    {
     
        if (m_Animator)
        {
            m_Animator.SetTrigger("Fire");
        }
        PlaySound(Shot);
        --BulletCount;

        Ray ray = new Ray(m_BulletSpawnPosition.position, m_BulletSpawnPosition.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100.00f))
        {
            Vector3 impactPos = rayHit.point;
            impactPos += rayHit.normal * 0.001f;

            GameObject impactObj =  Instantiate(gunShotCrack, impactPos, Quaternion.identity, null);
            impactObj.transform.up = rayHit.normal;
        }


    }

    void Reload()
    {
      
        if (m_Animator)
        {
            m_Animator.SetTrigger("Reload");
        }
        m_isReloading = true;
        PlaySound(Reloading);
        BulletCount = MAX_BULLETS;
    }

    public void setReloadingState(bool isReloading)
    {
        m_isReloading = isReloading;
    }

    public bool isReloading()
    {
        return m_isReloading;
    }
    

    // laserLogic

    void updateLineRenderer()
    {
        m_lineRenderer.SetPosition(0, m_BulletSpawnPosition.transform.position);

        //Raycasting

        Ray ray = new Ray(m_BulletSpawnPosition.position, m_BulletSpawnPosition.transform.forward * lineRendererLength);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, lineRendererLength))
        {
            m_lineRenderer.SetPosition(1, rayHit.point); // set the length of the mine to the hit point on wall
            m_ImpactPOS.transform.position = rayHit.point; // set the impact dot position to the hit point on wall
            m_ImpactMeshRenderer.enabled = true; 
        }

        else
        {
            m_lineRenderer.SetPosition(1, m_BulletSpawnPosition.position + m_BulletSpawnPosition.transform.forward * lineRendererLength);
            m_ImpactMeshRenderer.enabled = false;
        }
        
      
    }
}
