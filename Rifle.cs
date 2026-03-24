using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class Rifle : MonoBehaviour
{
    [Header("Rifle")]
    public Camera cam;
    public float giveDamage = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;
    public PlayerScript player;
    public Animator animator;

    //Rate of Fire
    [Header("Rifle Ammunation and Shooting")]
    private float nextTimeToShoot = 0f;
    private int maximumAmmunation = 20;
    private int mag = 50;
    private int presentAmmunation;
    public float reloadingTime = 1.3f;
    private bool setReloading = false;

    [Header("Rifle effects")]
    public ParticleSystem muzzleSpark;
    public GameObject WoodedEffect;
    public GameObject goreEffect;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip reloadingSound;

    private void Awake()
    {
        presentAmmunation = maximumAmmunation;
    }

    void Update()
    {
        if (setReloading)
        {
            return;
        }

        if(presentAmmunation <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
                    
        if(player.mobileInputs == true)
        {
            if (CrossPlatformInputManager.GetButton("Shoot") && Time.time >= nextTimeToShoot)
            {
                animator.SetBool("Fire",true);
                animator.SetBool("Idle",false);
                nextTimeToShoot = Time.time + 1f/fireCharge;
                Shoot();
            }

            else if (CrossPlatformInputManager.GetButton("Shoot") && player.currentPlayerSpeed > 0)
            {
                animator.SetBool("Idle",false);
                animator.SetBool("FireWalk",true);
            }

            else if(CrossPlatformInputManager.GetButton("Shoot") && CrossPlatformInputManager.GetButton("Aim"))
            {
                animator.SetBool("Idle",false);
                animator.SetBool("IdleAim",true);
                animator.SetBool("FireWalk",true);
                animator.SetBool("Walk",true);
                animator.SetBool("Reloading",false);
            }

            else
            {
                animator.SetBool("Fire",false);
                animator.SetBool("Idle",true);
                animator.SetBool("FireWalk",false);
            }
        }

        else
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
            {
                animator.SetBool("Fire",true);
                animator.SetBool("Idle",false);
                nextTimeToShoot = Time.time + 1f/fireCharge;
                Shoot();
            }

            else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetButton("Fire1") && Input.GetKey(KeyCode.UpArrow))
            {
                animator.SetBool("Idle",false);
                animator.SetBool("FireWalk",true);
            }

            else if(Input.GetButton("Fire1") && Input.GetButton("Fire2"))
            {
                animator.SetBool("Idle",false);
                animator.SetBool("IdleAim",true);
                animator.SetBool("FireWalk",true);
                animator.SetBool("Walk",true);
                animator.SetBool("Reloading",false);
            }

            else
            {
                animator.SetBool("Fire",false);
                animator.SetBool("Idle",true);
                animator.SetBool("FireWalk",false);
            }
        }

    }

    void Shoot()
    {

        if(mag == 0)
        {
            //show ammo finish
        }
        presentAmmunation-- ;
        
        if(presentAmmunation == 0)
        {
            mag-- ;
        }

        //updating UI
        AmmoCount.occurrence.UpdateAmmoText(presentAmmunation);
        AmmoCount.occurrence.UpdateMagText(mag);

        muzzleSpark.Play();
        audioSource.PlayOneShot(shootingSound);
        RaycastHit hitInfo;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
        {
            Debug.Log(hitInfo.transform.name);

            Objects objects = hitInfo.transform.GetComponent<Objects>();

            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

            if(objects != null)
            {
                objects.objectHitDamage(giveDamage);
                GameObject WoodGo = Instantiate(WoodedEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(WoodGo, 1f);
            }
            else if(enemy != null)
            {
                enemy.enemyHitDamage(giveDamage);
                GameObject goreGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreGo, 1f);
            }
        }
    }

    IEnumerator Reload()
    {
        player.playerSpeed = 0f;
        player.playerSprint = 0f;
        setReloading = true;
        Debug.Log("Reloading...");

        //animation and audio
        animator.SetBool("Reloading",true);
        audioSource.PlayOneShot(reloadingSound);

        yield return new WaitForSeconds(reloadingTime);
        
        //animation
        animator.SetBool("Reloading",false);

        presentAmmunation = maximumAmmunation;
        player.playerSpeed = 1.9f;
        player.playerSprint = 3f;
        setReloading = false;
    }
}