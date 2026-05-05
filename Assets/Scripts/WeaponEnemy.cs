using Unity.VisualScripting;
using UnityEngine;

public class WeaponEnemy : WeaponMain
{
    bool isUp;
    public bool isMelee;
    public void Shooting()
    {
        Shoot();
        if (isMelee)
        {
            GetComponent<Animator>().SetBool("inUp", !isUp);
            isUp = !isUp;
        }
    }
}
