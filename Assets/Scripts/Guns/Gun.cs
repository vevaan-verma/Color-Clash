using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunData gunData;
    private GameCore gameCore;
    private PlayerController playerController;
    private LevelAudioManager audioManager;
    private Animator animator;
    private UIController uiController;

    [Header("Shooting")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Bullet bullet;
    private EntityType entityType;
    private LayerMask shootableMask;
    private int gunIndex;
    private int currAmmo;
    private bool isReloading;
    private bool shotReady;

    [Header("Tracer")]
    [SerializeField] private LineRenderer bulletTracer;
    [SerializeField] private float bulletTracerDisplayDuration;

    [Header("Impact")]
    [SerializeField] private GameObject impactEffect;
    private new Collider2D collider;

    /*
    IMPORTANT:
        - IMPORTANT: RELOAD ANIMATION MUST BE 1 SECOND LONG FOR SCALING TO WORK
    */

    // start function
    public void Initialize(EntityType entityType, LayerMask shootableMask, Collider2D collider, int gunIndex) {

        gameCore = FindObjectOfType<GameCore>();
        playerController = FindObjectOfType<PlayerController>();
        audioManager = FindObjectOfType<LevelAudioManager>();
        animator = GetComponent<Animator>();
        uiController = FindObjectOfType<UIController>();

        this.entityType = entityType;
        this.shootableMask = shootableMask;
        this.collider = collider;
        this.gunIndex = gunIndex;

        currAmmo = gunData.GetMagazineSize();
        shotReady = true;

    }

    public IEnumerator Shoot(float multiplier = 1f) {

        if (isReloading || !shotReady) yield break; // don't check if ammo is greater than 0 because reload is handled after this

        // reload if out of ammo
        if (currAmmo == 0) {

            StartCoroutine(Reload());
            yield break;

        }

        shotReady = false;

        if (gunData.GetShootSound() != null)
            audioManager.PlaySound(gunData.GetShootSound()); // play shoot sound

        if (gunData.UsesRaycastShooting()) {

            RaycastHit2D shootableHit = Physics2D.Raycast(muzzle.position, muzzle.right, gunData.GetMaxRange(), shootableMask); // for checking if a shootable is hit
            RaycastHit2D obstacleHit = Physics2D.Raycast(muzzle.position, muzzle.right, gunData.GetMaxRange(), gameCore.GetEnvironmentMask()); // for checking if an obstacle is in the way

            if (obstacleHit && (Vector2.Distance(muzzle.position, obstacleHit.point) <= Vector2.Distance(muzzle.position, shootableHit.point) || !shootableHit)) { // obstacle in the way or shot didn't hit shootable, but hit obstacle

                // impact effect
                Instantiate(impactEffect, obstacleHit.point, Quaternion.identity);

                // set bullet tracer points
                bulletTracer.SetPosition(0, muzzle.position);
                bulletTracer.SetPosition(1, obstacleHit.point);

            } else if (shootableHit) {

                bool? deathCaused = false; // to prevent impact effect when something dies (for better looking gfx)

                if (entityType == EntityType.Player)
                    deathCaused = shootableHit.transform.GetComponent<PhantomHealthManager>()?.TakeDamage(gunData.GetDamage() * multiplier); // damage enemy if player is shooter
                else if (entityType == EntityType.Phantom)
                    deathCaused = shootableHit.transform.GetComponent<PlayerHealthManager>()?.TakeDamage(gunData.GetDamage() * multiplier);  // damage player if enemy is shooter

                // impact effect
                if (deathCaused != null && !(bool) deathCaused)
                    Instantiate(impactEffect, shootableHit.point, Quaternion.identity);

                // set bullet tracer points
                bulletTracer.SetPosition(0, muzzle.position);
                bulletTracer.SetPosition(1, shootableHit.point);

            } else { // miss

                // set bullet tracer points
                bulletTracer.SetPosition(0, muzzle.position);
                bulletTracer.SetPosition(1, muzzle.position + muzzle.right * gunData.GetMaxRange());
                // bulletTracer.SetPosition(1, muzzle.position + muzzle.right * 100f); // illusion for infinite length tracer when missed

            }

            // display bullet tracer
            bulletTracer.enabled = true;
            yield return new WaitForSeconds(bulletTracerDisplayDuration);
            bulletTracer.enabled = false;

        } else {

            Instantiate(bullet, muzzle.position, muzzle.rotation).Initialize(entityType, gunData.GetDamage() * multiplier, muzzle.position, gunData.GetMaxRange(), collider); // instantiate bullet

        }

        currAmmo--;

        yield return new WaitForSeconds(1 / gunData.GetFireRate()); // use fire rate to prevent shooting
        shotReady = true;

    }

    private bool CanReload() {

        return currAmmo < gunData.GetMagazineSize() && !isReloading;

    }

    public IEnumerator Reload() {

        if (!CanReload()) yield break;

        isReloading = true;

        if (gunData.GetReloadSound() != null)
            audioManager.PlaySound(gunData.GetReloadSound()); // play reload sound

        // uiController.SetAmmoReloadingText(); // for notifying player of reload

        animator.SetTrigger("reload"); // trigger animation

        yield return new WaitForEndOfFrame(); // wait for end of frame so animation starts playing
        animator.speed = 1f / gunData.GetReloadTime(); // scale animation speed by reload time
        yield return new WaitForSeconds(gunData.GetReloadTime()); // wait for reload time
        currAmmo = GetMagazineSize(); // reload gun

        uiController.UpdateGunHUD(this, gunIndex); // update ui

        isReloading = false;
        shotReady = true; // reset fire rate cooldown

    }

    public void InstantReload() {

        if (!CanReload()) return;

        currAmmo = GetMagazineSize(); // reload gun

        if (entityType == EntityType.Player) // player is reloading
            uiController.UpdateGunHUD(this, gunIndex); // update ui

    }

    public Sprite GetIcon() { return gunData.GetIcon(); }

    public int GetCurrentAmmo() { return currAmmo; }

    public int GetMagazineSize() { return gunData.GetMagazineSize(); }

    public bool IsReloading() { return isReloading; }

}
