using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if(currentHealth <= 0 ) return;
        currentHealth = Mathf.Max(currentHealth - damage, 0f);
        healthBar.SetHealth(currentHealth);

        if(currentHealth <= 0)
        {
            healthBar.gameObject.SetActive(false);
            StartCoroutine(ObjectDisable());
        }
    }
    IEnumerator ObjectDisable()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar.SetHealth(currentHealth);
    }
}
