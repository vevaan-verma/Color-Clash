using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunData gunData;
    private Animator animator;
    private UIController uiController;

    [Header("Shooting")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Bullet bullet;
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

    public void Initialize(Collider2D collider, int gunIndex) {

        animator = GetComponent<Animator>();
        uiController = FindObjectOfType<UIController>();

        this.collider = collider;
        this.gunIndex = gunIndex;

        currAmmo = gunData.GetMagazineSize();
        shotReady = true;

    }

    public IEnumerator Shoot(LayerMask shootableMask, ShooterType shooterType) {

        if (!CanShoot()) yield break;

        shotReady = false;

        if (gunData.UsesRaycastShooting()) {

            RaycastHit2D hitInfo = Physics2D.Raycast(muzzle.position, muzzle.right, gunData.GetMaxRange(), shootableMask);

            if (hitInfo) {

                bool? deathCaused = false; // to prevent impact effect when something dies (for better looking gfx)

                if (shooterType == ShooterType.Player)
                    deathCaused = hitInfo.transform.GetComponent<EnemyController>()?.TakeDamage(gunData.GetDamage()); // damage enemy if player is shooter
                else if (shooterType == ShooterType.Enemy)
                    deathCaused = hitInfo.transform.GetComponent<PlayerController>()?.TakeDamage(gunData.GetDamage());  // damage player if enemy is shooter

                if (deathCaused != null && !(bool) deathCaused)
                    Instantiate(impactEffect, hitInfo.point, Quaternion.identity); // instantiate impact effect

                // set bullet tracer points
                bulletTracer.SetPosition(0, muzzle.position);
                bulletTracer.SetPosition(1, hitInfo.point);

            } else {

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

            Instantiate(bullet, muzzle.position, muzzle.rotation).Initialize(shooterType, gunData.GetDamage(), muzzle.position, gunData.GetMaxRange(), collider); // instantiate bullet

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

    }

    private bool CanShoot() {

        return !isReloading && currAmmo > 0 && shotReady;

    }

    public Sprite GetIcon() { return gunData.GetIcon(); }
    public int GetCurrentAmmo() { return currAmmo; }
    public int GetMagazineSize() { return gunData.GetMagazineSize(); }
    public bool IsReloading() { return isReloading; }

}
