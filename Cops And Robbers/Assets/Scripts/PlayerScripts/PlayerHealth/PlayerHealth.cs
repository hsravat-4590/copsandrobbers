﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
*@author Peter
*/

/// <summary>
/// This script is responible for the fucntion of the healthbar and Dmg for testing
/// </summary>
namespace Me.DerangedSenators.CopsAndRobbers
{
    public class PlayerHealth : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float currentHealth;

    public HealthBar healthBar;

        /// <summary>
        /// At the start of the project the player's health will equal to the max health
        /// </summary>
        public void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        /// <summary>
        /// This function checks if a user Used Space if it does the player will take 1.5 dmg
        /// </summary>
        public void Update()
        {
            // Pressing the "spacebar" deals 2.3f amout of damgage
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Damage(2.3f);
            }

            //If players health reaches 0 It is removed form the scene
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// This function will lower the current player health by set amount
        /// </summary>
        /// <param name="damage">Amount of damage taken, to update health bar</param>
        public void Damage(float damage)
        {
            currentHealth = currentHealth - damage;
            healthBar.SetHealth(currentHealth);
        }
    }
}

   

    

