using UnityEngine;
using System.Collections;

public class WeaponPlayer : WeaponMain
{
    public int manaCost;

    public override void StartShooting()
    {
        print(1);
        StartCoroutine(Shooting());
        StartCoroutine(MinusDelay());
    }

    IEnumerator Shooting()
    {
        print(3);
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            if (Input.GetMouseButton(0))
            {
                print(4);
                if (timeDelayStartShoot <= 0)
                {
                    Shoot();
                    yield return new WaitForSeconds(timeDelayShot);
                }
            }
        }
    }

    IEnumerator MinusDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            if(Input.GetMouseButton(0))
            {
                if (timeDelayStartShoot > timeDelayStartShootMin)
                {
                    timeDelayStartShoot -= 0.02f;
                }
            }
            else
            {
                if (timeDelayStartShoot < timeDelayStartShootMax)
                {
                    timeDelayStartShoot += 0.01f;
                }
            }
        }
    }
}
