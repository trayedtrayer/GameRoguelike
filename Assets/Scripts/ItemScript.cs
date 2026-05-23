using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public DataBase.Item item;
    public void SetItem(DataBase.Item _item)
    {
        item = _item;
        item.countItem = Random.Range(1, 6);
        print(_item.GetSprite());
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.5f);
    }
}
