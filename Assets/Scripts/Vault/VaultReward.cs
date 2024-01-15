using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultReward : MonoBehaviour {

    [Header("Effect")]
    [SerializeField] private EffectValue effectValue;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.transform.CompareTag("Player")) { // collider is player

            FindObjectOfType<PlayerEffectManager>().AddEffectMultiplier(effectValue.GetEffectType(), effectValue.GetEffectMultiplier()); // give player multiplier bonus
            Destroy(gameObject); // destroy

        }
    }
}
