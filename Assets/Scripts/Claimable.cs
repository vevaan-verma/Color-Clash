using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claimable : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Claiming")]
    [SerializeField][Tooltip("Added onto the effect multiplier")] private float addedMultiplier;

    [Header("Animations")]
    [SerializeField] private float colorFadeDuration;
    private Coroutine colorCoroutine;

    private void Awake() {

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void Claim(EntityType entityType, Color targetColor, EffectType? effectType = null) {

        PlayerClaim playerClaim = GetComponent<PlayerClaim>();
        EnemyClaim enemyClaim = GetComponent<EnemyClaim>();

        if ((entityType == EntityType.Player && playerClaim && playerClaim.GetEffectType().Equals(effectType)) || (entityType == EntityType.Enemy && enemyClaim)) // already claimed by entity (player done this way to make sure if effect types are different, they are still replaced)
            return;

        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);

        colorCoroutine = StartCoroutine(FadeColor(entityType, effectType, targetColor, colorFadeDuration));

    }

    private IEnumerator FadeColor(EntityType entityType, EffectType? effectType, Color targetColor, float duration) {

        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < duration) {

            currentTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, currentTime / duration);
            yield return null;

        }

        spriteRenderer.color = targetColor;

        // TODO: fix issue where if two entities are claiming the same platform it keeps adding and removing claims every frame
        if (entityType == EntityType.Player) {

            // destroy any existing claims
            foreach (PlayerClaim claim in GetComponents<PlayerClaim>())
                Destroy(claim);

            foreach (EnemyClaim claim in GetComponents<EnemyClaim>())
                Destroy(claim);

            gameObject.AddComponent<PlayerClaim>().Claim((EffectType) effectType); // claim for player

        } else if (entityType == EntityType.Enemy) {

            // destroy any existing claims
            foreach (PlayerClaim claim in GetComponents<PlayerClaim>())
                Destroy(claim);

            foreach (EnemyClaim claim in GetComponents<EnemyClaim>())
                Destroy(claim);

            gameObject.AddComponent<EnemyClaim>().Claim(); // claim for enemy

        }

        colorCoroutine = null;

    }

    public float GetMultiplierAddition() { return addedMultiplier; }

}
