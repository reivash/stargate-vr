using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthController : MonoBehaviour
{

    [SerializeField] float health;
    [SerializeField] float maxHealth;
    [SerializeField] float timeToHeal = 2f;
    
    private ColorAdjustments colorAdjustments;

    private float lastDamageTime = -1;

    void Start()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet<ColorAdjustments>( out colorAdjustments);
        colorAdjustments.active = false;
    }

   public void TakeDamage(float damage) {
        colorAdjustments.active  = true;
        //colorAdjustments.postExposure.value += 1;
        lastDamageTime = Time.time;
        health -= damage;

        Color currentColor = colorAdjustments.colorFilter.value;
        currentColor.r = Mathf.Max(currentColor.r - 0.25f, 0f);
        colorAdjustments.colorFilter.value = currentColor;

        if (health <= 0) {
            // Die.
            // TODO: Add death audio.
        }
    }

    private void Update() {
        if (lastDamageTime == -1f) {
            return; 
        }

        float timeSinceLastCall = Time.time - lastDamageTime;
        if (timeSinceLastCall > timeToHeal) {
            health = maxHealth;
            colorAdjustments.active = false;
            lastDamageTime = -1;

            Color currentColor = colorAdjustments.colorFilter.value;
            currentColor.r = 255f;
            colorAdjustments.colorFilter.value = currentColor;
        }
    }
}
