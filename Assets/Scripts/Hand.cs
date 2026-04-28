
using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public GameObject GunOne;
    // Update is called once per frame

    private void Start()
    {

    }

    void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var angle = Vector2.Angle(Vector2.right, mousePosition - transform.position);
        transform.eulerAngles = new Vector3(0, 0, transform.position.y < mousePosition.y ? angle : -angle);
    }
}
