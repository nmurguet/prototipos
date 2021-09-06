using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    public BulletMovement bullet;

    public GameObject muzzle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shoot()
    {
        Instantiate(bullet, muzzle.transform.position, bullet.startingRotation);


    }
}
