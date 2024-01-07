using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class Claimable : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Claiming")]
    [SerializeField] private float addedMultiplier;

    [Header("Animations")]
    [SerializeField] private float colorFadeDuration;
    // create separate coroutines for each entity to allow them to claim at the same time and show visual feedback (color alternates)
    private Coroutine playerColorCoroutine;
    private Coroutine enemyColorCoroutine;

    private void Awake() {

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void Claim(EntityType entityType, Color claimColor, EffectType? effectType = null) {

        // TODO: claim script keeps alternating when two entities are claiming, giving player extra multiplier for a few frames (could be a feature)

        PlayerClaim playerClaim = GetComponent<PlayerClaim>();
        EnemyClaim enemyClaim = GetComponent<EnemyClaim>();

        if ((entityType == EntityType.Player && playerClaim && playerClaim.GetEffectType() == effectType) || (entityType == EntityType.Player && playerColorCoroutine != null) || (entityType == EntityType.Enemy && enemyClaim) || (entityType == EntityType.Enemy && enemyColorCoroutine != null)) // already claimed by entity (player done this way to make sure if effect types are different, they are still replaced)
            return;

        if (entityType == EntityType.Player)
            playerColorCoroutine = StartCoroutine(FadeColor(entityType, effectType, claimColor, colorFadeDuration));
        if (entityType == EntityType.Enemy)
            enemyColorCoroutine = StartCoroutine(FadeColor(entityType, effectType, claimColor, colorFadeDuration));

    }

    private IEnumerator FadeColor(EntityType entityType, EffectType? effectType, Color claimColor, float duration) {

        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < duration) {

            currentTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, claimColor, currentTime / duration);
            yield return null;

        }

        spriteRenderer.color = claimColor;
        RegisterClaim(entityType, effectType, claimColor);

        if (entityType == EntityType.Player)
            playerColorCoroutine = null;
        if (entityType == EntityType.Enemy)
            enemyColorCoroutine = null;

    }

    private void RegisterClaim(EntityType entityType, EffectType? effectType, Color claimColor) {

        if (entityType == EntityType.Player) {

            // destroy any existing claims
            foreach (PlayerClaim claim in GetComponents<PlayerClaim>())
                Destroy(claim);

            foreach (EnemyClaim claim in GetComponents<EnemyClaim>())
                Destroy(claim);

            gameObject.AddComponent<PlayerClaim>().Claim(claimColor, (EffectType) effectType); // claim for player

        } else if (entityType == EntityType.Enemy) {

            // destroy any existing claims
            foreach (PlayerClaim claim in GetComponents<PlayerClaim>())
                Destroy(claim);

            foreach (EnemyClaim claim in GetComponents<EnemyClaim>())
                Destroy(claim);

            gameObject.AddComponent<EnemyClaim>().Claim(); // claim for enemy

        }
    }

    public float GetMultiplierAddition() { return addedMultiplier; }

}
