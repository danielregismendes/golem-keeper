using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

[System.Serializable]
public class Attack : MonoBehaviour
{

    private AudioClip hitSound;
    private AudioClip hitSoundEnemy;
    private int damage;
    private int enemyDamage;
    private bool weapon;
    private float knockback;


    public void SetAttack(Hit hit)
    {

        damage = hit.damage;
        hitSound = hit.hitSound;
    }

    public void SetEnemyAttack(EnemyHit enemy)
    {
        enemyDamage = enemy.damage;
        hitSoundEnemy = enemy.collisionSound;
        if(enemy.knockback > 0) { knockback = enemy.knockback; }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        Player player = other.GetComponent<Player>();
        //CrashItem crashItem = other.GetComponent<CrashItem>();


        if (enemy != null)
        {
            if (enemy.GetHealth() > 0)
            {                                
                enemy.TookDamage(damage);
            }
        }

        if (player != null)
        {
            if(player.GetHealth() > 0)
            {
                Debug.Log("Dano do inimigo = " + enemyDamage);
                player.TookDamage(enemyDamage);
                player.Knockback(knockback);
            }
        }

        /*if (crashItem != null)
        {
            damage = 1;
            Debug.Log("Dano no barril = " + damage);
            crashItem.TookDamage(damage);

        }*/
     }
}
