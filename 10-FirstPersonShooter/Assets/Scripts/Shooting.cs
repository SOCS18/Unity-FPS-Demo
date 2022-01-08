using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Transform FPSCam;
    [SerializeField] private GameObject curGun;
    [SerializeField] private GameObject[] impactEffects;
    [SerializeField] private float impactForce;
    private float nextShootTime = 0;
    private int typeOfObjectHit = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!FPSCam)
            FPSCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(FPSCam.position, FPSCam.forward, Color.red);

        if (Input.GetButton("Fire1") && Time.time > nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + 1/ curGun.GetComponent<Gun>().shootRate;
        }
    }

    private void Shoot()
    {
        // muzzle flash
        curGun.GetComponent<Gun>().muzzleFlash.Play();
        // sound for shooting
        AudioSource s = curGun.GetComponent<AudioSource>();
        if (s != null)
            s.Play();

        if (Physics.Raycast(FPSCam.position, FPSCam.forward, out RaycastHit hitInfo, curGun.GetComponent<Gun>().shootRange))
        {
            // find the object being hit and have an impact on the object
            Debug.Log(hitInfo.collider.name + " " + Time.time);
            Rigidbody rb = hitInfo.rigidbody;
            if (rb != null)
                rb.AddForce((hitInfo.point - FPSCam.position).normalized * impactForce);
            // visual impact at the hitInfo.point
            if (hitInfo.collider.tag == "wodden")
            {
                typeOfObjectHit = 1;
            }
            else
            {
                typeOfObjectHit = 0;
            }
            gameObject.GetComponent<ParticleSystem>().Play();
            GameObject g = Instantiate(impactEffects[typeOfObjectHit], hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(g, 2);
            // applyDamage() for enemy
        }
    }
}
