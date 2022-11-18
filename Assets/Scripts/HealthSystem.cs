using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
  public event EventHandler OnDead;
  public event EventHandler OnDamaged;

  [SerializeField] private int health = 100;
  private int healthMax;

  private void Awake()
  {
    healthMax = health;
  }

  public void TakeDamage(int damageAmount)
  {
    health -= damageAmount;

    OnDamaged?.Invoke(this, EventArgs.Empty);

    if (health <= 0)
    {
      health = 0;

      Die();
    }
  }

  private void Die()
  {
    OnDead?.Invoke(this, EventArgs.Empty);
  }

  public float GetHealthNormalised()
  {
    return (float)health / healthMax;
  }
}
