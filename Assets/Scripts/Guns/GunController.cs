using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shooting")]
    [SerializeField] private LayerMask shootableMask; // just to avoid player collisions
    [SerializeField] private float bulletTracerDisplayDuration;
    private bool canShoot;

    private void Start() {

        canShoot = true;

    }

    public IEnumerator Shoot(Gun gun, GunModel model) {

        if (!canShoot) yield break;

        canShoot = false;

        RaycastHit2D hitInfo = Physics2D.Raycast(model.GetMuzzle().position, model.GetMuzzle().right, gun.GetMaxFireRange(), shootableMask);

        if (hitInfo) {

            hitInfo.transform.GetComponent<Enemy>()?.TakeDamage(gun.GetDamage());
            Instantiate(model.GetImpactEffect(), hitInfo.point, Quaternion.identity);

            model.GetBulletTracer().SetPosition(0, model.GetMuzzle().position);
            model.GetBulletTracer().SetPosition(1, hitInfo.point);

        } else {

            model.GetBulletTracer().SetPosition(0, model.GetMuzzle().position);
            model.GetBulletTracer().SetPosition(1, model.GetMuzzle().position + model.GetMuzzle().right * gun.GetMaxFireRange());
            // bulletTracer.SetPosition(1, muzzle.position + muzzle.right * 100f); // illusion for infinite length tracer when missed

        }

        /*
        FOR BULLET PHYSICS BASED SHOOTING
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation).GetComponent<Bullet>().Initialize(gun.GetDamage());
        */

        // bullet tracer
        model.GetBulletTracer().enabled = true;
        yield return new WaitForSeconds(bulletTracerDisplayDuration);
        model.GetBulletTracer().enabled = false;

        Invoke("ResetCanShoot", 1 / gun.GetFireRate());
    }

    private void ResetCanShoot() {

        canShoot = true;

    }

    private void OnDrawGizmos() {

        /*
        for checking gun ranges
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(10f, 0f, 0f));
        */

    }
}
