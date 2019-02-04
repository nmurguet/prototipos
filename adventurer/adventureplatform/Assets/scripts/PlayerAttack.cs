using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    private float timeBtwAttack;

    public float startTimeBtwAttack;

    public Transform attackPos;

    public float attackRange;

    public LayerMask whatisEnemy;

    public int damage;

    // Update is called once per frame
    void Update()
    {
        if (timeBtwAttack <= 0){
            //then you can attack
            if (Input.GetKey(KeyCode.Q))
            {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatisEnemy);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                     
                }


            }
            timeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;

        }
        
    }
}
