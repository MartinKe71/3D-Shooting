using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ShootingController : MonoBehaviour
{
    public CinemachineFreeLook thirdPersonCam;
    public GameObject crossPrefab;
    public Transform canvas;
    public Animator anim;
    private MovementManager input;
    private float horizontalOffset;
    private float cameraZoom;
    public int hitPoint;
    public float zoomoutX;
    public float zoominX;
    public float aimTime;
    public bool aiming;
    public bool shooting;
    public bool deadEye;

    public List<Transform> targets = new List<Transform>();
    private List<Transform> crossList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        aiming = false;
        input = GetComponent<MovementManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SetGun();
        anim.SetFloat("Speed", input.speed);
        if (aiming && targets.Count > 0)
        {
            UpdateCross();
        }
    }

    private void UpdateCross()
    {
        for (int i = 0; i < crossList.Count; i++)
        {
            crossList[i].position = Camera.main.WorldToScreenPoint(targets[i].position);
        }
    }

    private void SetGun()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Aim(true);
            if (Input.GetMouseButton(2))
            {
                deadEye = true;
                Debug.Log("DeadEye activated");
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            shooting = true;
            Shoot();
            deadEye = false;
        }
        else if (Input.GetMouseButtonUp(1) && aiming)
        {
            if (targets.Count > 0)
            {
                DeadEye(true);
            }
            else
            {
                Aim(false);
            }           
        }      
        
        if (aiming && deadEye)
        {
            input.LookAt(Camera.main.transform.forward + (Camera.main.transform.right * .1f));

            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit);

            Transform enemyHit = hit.transform;

            if (enemyHit == null)
            {
                return;
            }
            if (!hit.collider.CompareTag("Enemy"))
            {
                return;
            }

            if (!targets.Contains(enemyHit) && hit.transform.GetComponentInParent<EnemyController>().health > 0)
            {
                targets.Add(enemyHit);

                GameObject cross = Instantiate(crossPrefab, canvas);
                cross.transform.position = Camera.main.WorldToScreenPoint(enemyHit.position);
                crossList.Add(cross.transform);


                hit.transform.GetComponentInParent<EnemyController>().takeHit(hitPoint);
            }
        }
    }

    private void DeadEye(bool state)
    {
        deadEye = state;
        float animSpeed = state ? 2 : 1;
        anim.speed = animSpeed;

        if (!state)
        {
            targets.Clear();

            foreach (Transform t in crossList)
            {
                Destroy(t.gameObject);
            }
            crossList.Clear();
        }
    }

    private void Shoot()
    {
        if (deadEye)
        {
            Debug.Log("Deadeye shoot!");
        }
        anim.SetTrigger("Shoot");
    }

    private void Aim(bool state)
    {
        cameraZoom = state ? 25 : 35;
        float xOffset = state ? zoominX : zoomoutX;
        float xOriginalOffset = state ? zoomoutX : zoominX;
        DOVirtual.Float(thirdPersonCam.m_Lens.FieldOfView, cameraZoom, aimTime, SetZoom);
        DOVirtual.Float(xOriginalOffset, xOffset, aimTime, SetHorizontalOffset);
        if (state == false)
            transform.DORotate(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z), aimTime);
        anim.SetBool("Aim", state);
        aiming = state;
    }

    private void SetZoom(float value)
    {
        thirdPersonCam.m_Lens.FieldOfView = value;
    }

    private void SetHorizontalOffset(float horizontalOffset)
    {
        for (int i = 0; i < 3; i++)
        {
            CinemachineComposer c = thirdPersonCam.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            c.m_TrackedObjectOffset.x = horizontalOffset;
        }
    }
}
