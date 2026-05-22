using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartLocScript : MonoBehaviour
{
    public List<Transform> inactivePoses;
    public Transform spawnPlace;

    private void Awake()
    {
        DataBase.SetWeapons();
        print(XmlSaver.Read());
        if (XmlSaver.Read() != null)
        {
            Instantiate(DataBase.GetPlayer(XmlSaver.Read().idPlayer), spawnPlace.transform.position, spawnPlace.transform.rotation);
        }
        else
        {
            Instantiate(DataBase.GetPlayer(2), spawnPlace.transform.position, spawnPlace.transform.rotation);
        }
        SetAllPosesWithoutPlayer();
    }

    public void CheckPoses(int numPl, GameObject inactive)
    {
        print(numPl);
        if (inactivePoses[numPl-1].childCount == 0)
        {
            GameObject t = Instantiate(inactive, inactivePoses[numPl-1]);
            t.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void SetAllPosesWithoutPlayer()
    {
        int ourId = XmlSaver.Read() == null ? 0 : XmlSaver.Read().idPlayer;
        for (int i = 1; i < inactivePoses.Count+1; i++)
        {
            if (ourId != i)
            {
                print("++++" + i + "+++++++");
                GameObject _inactivePlayer = DataBase.GetInactivePlayer(i);
                GameObject t = Instantiate(_inactivePlayer, inactivePoses[i-1]);
                t.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}
