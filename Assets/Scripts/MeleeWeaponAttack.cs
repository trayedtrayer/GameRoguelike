using UnityEngine;
using System.Collections;

public class MeleeWeaponAttack : WeaponMain
{
    bool isUp;

    public override void StartShooting()
    {
       StartCoroutine(Shooting());
    }

    protected IEnumerator Shooting()
    {
        while (true)
        {
            print("STARTUEM");
            yield return new WaitForSeconds(0.02f);
            if (Input.GetMouseButton(0))
            {
                print("PIZDEC");
                GetComponent<Animator>().SetBool("inUp", !isUp);
                Shoot();
                isUp = !isUp;
                yield return new WaitForSeconds(timeDelayShot);
            }
        }
    }
}
