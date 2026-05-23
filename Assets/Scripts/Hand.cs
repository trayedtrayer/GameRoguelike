
using JetBrains.Annotations;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public GameObject GunOne;
    public GameObject GunTwo;
    public int activeWeapon;
    bool isCooldown;
    public float TimeCooldown;
    public SpriteRenderer skin;
    public SpriteRenderer weapon;
    private GameObject weaponToTake;
    public Image weaponOneImage;
    public Image weaponTwoImage;
    // Update is called once per frame

    private void Start()
    {

    }

    void Update()
    {
        if (weapon != null)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var angle = Vector2.Angle(Vector2.right, mousePosition - transform.position);
            transform.eulerAngles = new Vector3(0, 0, transform.position.y < mousePosition.y ? angle : -angle);
            if (angle > 90)
            {
                skin.flipX = true;
                weapon.flipY = true;
            }
            else
            {
                skin.flipX = false;
                weapon.flipY = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!isCooldown)
            {
                SetActiveWeapon(0);
                StartCooldown();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!isCooldown)
            {
                SetActiveWeapon(1);
                StartCooldown();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (activeWeapon == 0)
            {
                if (weaponToTake != null)
                {
                    CreateWeapon(0);
                }
            }
            else
            {
                if (weaponToTake != null)
                {
                    CreateWeapon(1);
                }
            }
        }
    }

    void CreateWeapon(int _numWeapon)
    {
        if (_numWeapon == 0)
        {
            Instantiate(GunOne.GetComponentInChildren<WeaponMain>().prefabWeapon, transform.position, transform.rotation);
            Destroy(GunOne.transform.GetChild(0).gameObject);
            GameObject w = Instantiate(weaponToTake.GetComponent<WeaponOnTheGround>().prefab, GunOne.transform);
            StartCoroutine(SetSkins(0));
            weaponOneImage.sprite = w.GetComponentInChildren<WeaponMain>().spriteWeapon;
        }
        else
        {
            Instantiate(GunTwo.GetComponentInChildren<WeaponMain>().prefabWeapon, transform.position, transform.rotation);
            Destroy(GunTwo.transform.GetChild(0).gameObject);
            GameObject w = Instantiate(weaponToTake.GetComponent<WeaponOnTheGround>().prefab, GunTwo.transform);
            StartCoroutine(SetSkins(1));
            weaponTwoImage.sprite = w.GetComponentInChildren<WeaponMain>().spriteWeapon;
        }
        Destroy(weaponToTake.gameObject);
    }

    public void SetActiveWeapon(int _numWeapon)
    {
        if (_numWeapon == 0)
        {
            activeWeapon = 0;
            GunOne.SetActive(true);
            GunTwo.SetActive(false);
            StartCoroutine(SetSkins(0));
            weaponOneImage.sprite = GunOne.GetComponentInChildren<WeaponMain>().spriteWeapon;
        }
        else
        {
            activeWeapon = 1;
            GunOne.SetActive(false);
            GunTwo.SetActive(true);
            StartCoroutine(SetSkins(1));
            weaponTwoImage.sprite = GunTwo.GetComponentInChildren<WeaponMain>().spriteWeapon;
        }
    }

    IEnumerator SetSkins(int _num)
    {
        yield return new WaitForSeconds(0.002f);
        if (_num == 0) { weapon = GunOne.GetComponentInChildren<SpriteRenderer>(); }
        else { weapon = GunTwo.GetComponentInChildren<SpriteRenderer>(); }
    }

    public void ShowWeapon(TextMeshProUGUI _text, WeaponOnTheGround _weapon)
    {
        _text.text = "Press E to take " + _weapon.prefab.GetComponentInChildren<WeaponMain>().weaponName;
        weaponToTake = _weapon.gameObject;
    }

    public void CreateWeaponForSave(GameObject gameObject, int _numWeapon)
    {
        weaponToTake = gameObject;
        if (_numWeapon == 0)
        {
            Destroy(GunOne.transform.GetChild(0).gameObject);
            Instantiate(weaponToTake.gameObject, GunOne.transform);
            StartCoroutine(SetSkins(0));
            weaponOneImage.sprite = GunOne.GetComponentInChildren<WeaponMain>().spriteWeapon;
        }
        if (_numWeapon == 1)
        {
            Destroy(GunTwo.transform.GetChild(0).gameObject);
            GameObject t = Instantiate(weaponToTake.gameObject, GunTwo.transform);
            StartCoroutine(SetSkins(1));
            weaponTwoImage.sprite = GunTwo.GetComponentInChildren<WeaponMain>().spriteWeapon;
            SetActiveWeapon(0);
        }
        weaponToTake = null;
    }

    void StartCooldown()
    {
        isCooldown = true;
        Invoke("EndCooldown", TimeCooldown);
    }

    void EndCooldown()
    {
        isCooldown = false;
    }

   public string GetNameActiveWeapon()
   {
        if(activeWeapon == 0)
        {
            return GunOne.GetComponentInChildren<WeaponMain>().weaponName;
        }
        else
        {
            return GunTwo.GetComponentInChildren<WeaponMain>().weaponName;
        }
    }
}
