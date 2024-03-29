using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claimable : MonoBehaviour {

    [Header("References")]
    private PlayerHealthManager healthManager;
    private GameCore gameCore;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    [Header("Claiming")]
    [SerializeField] private float addedMultiplier;
    private EntityType claimer;

    [Header("Animations")]
    [SerializeField] private float claimDuration;
    // create separate coroutines for each entity to allow them to claim at the same time and show visual feedback (color alternates)
    private Coroutine playerColorCoroutine;
    private Coroutine phantomColorCoroutine;
    private Coroutine resetCoroutine;

    private void Start() {

        healthManager = FindObjectOfType<PlayerHealthManager>();
        gameCore = FindObjectOfType<GameCore>();
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startColor = spriteRenderer.color;

    }

    public void Claim(EntityType entityType, Color claimColor, EffectType? effectType = null) {

        PlayerClaim playerClaim = GetComponent<PlayerClaim>();
        PhantomClaim phantomClaim = GetComponent<PhantomClaim>();

        if ((entityType == EntityType.Player && ((playerClaim && playerClaim.GetEffectType() == effectType) || playerColorCoroutine != null || healthManager.IsDead())) || (entityType == EntityType.Phantom && (phantomClaim || phantomColorCoroutine != null))) // already claimed by entity (player done this way to make sure if effect types are different, they are still replaced)
            return;

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        if (entityType == EntityType.Player)
            playerColorCoroutine = StartCoroutine(StartClaim(entityType, effectType, claimColor, claimDuration));
        if (entityType == EntityType.Phantom)
            phantomColorCoroutine = StartCoroutine(StartClaim(entityType, effectType, claimColor, claimDuration));

    }

    private IEnumerator StartClaim(EntityType entityType, EffectType? effectType, Color claimColor, float claimDuration) {

        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < claimDuration) {

            currentTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, claimColor, currentTime / claimDuration);
            yield return null;

        }

        spriteRenderer.color = claimColor;
        RegisterClaim(entityType, effectType, claimColor);

        if (entityType == EntityType.Player)
            playerColorCoroutine = null;
        if (entityType == EntityType.Phantom)
            phantomColorCoroutine = null;

    }

    private void RegisterClaim(EntityType entityType, EffectType? effectType, Color claimColor) {

        if (entityType == EntityType.Player) {

            // destroy any existing claims
            foreach (PlayerClaim claim in GetComponents<PlayerClaim>())
                Destroy(claim);

            foreach (PhantomClaim claim in GetComponents<PhantomClaim>())
                Destroy(claim);

            gameObject.AddComponent<PlayerClaim>().Claim(this, claimColor, (EffectType) effectType); // claim for player

        } else if (entityType == EntityType.Phantom) {

            // destroy any existing claims
            foreach (PlayerClaim claim in GetComponents<PlayerClaim>())
                Destroy(claim);

            foreach (PhantomClaim claim in GetComponents<PhantomClaim>())
                Destroy(claim);

            gameObject.AddComponent<PhantomClaim>().Claim(); // claim for phantom

        }

        claimer = entityType;

    }

    public void OnClaimDestroy(EntityClaim entityClaim) {

        if (gameManager is LevelManager && ((LevelManager) gameManager).IsLevelObjectiveCompleted()) return; // no resetting after level is cleared (make sure game manager is level manager)

        if (gameCore.IsQuitting() || playerColorCoroutine != null || phantomColorCoroutine != null) return;

        // check if there is another entity claim on the claimable
        foreach (EntityClaim claim in GetComponents<EntityClaim>())
            if (claim != entityClaim)
                return;

        if (entityClaim is PlayerClaim && playerColorCoroutine != null)
            StopCoroutine(playerColorCoroutine);
        if (entityClaim is PhantomClaim && phantomColorCoroutine != null)
            StopCoroutine(phantomColorCoroutine);

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetClaim(startColor, claimDuration)); // reset claim

    }

    private IEnumerator ResetClaim(Color resetColor, float claimDuration) {

        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < claimDuration) {

            currentTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, resetColor, currentTime / claimDuration);
            yield return null;

        }

        spriteRenderer.color = resetColor;
        claimer = EntityType.None;

    }

    public float GetMultiplierAddition() { return addedMultiplier; }

    public EntityType GetClaimer() { return claimer; }

}
