using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PillTypes
{
    GREEN, RED
}


public class ItensSpawner : MonoBehaviour
{
    List<GameObject> pillItens = new List<GameObject>();
    [SerializeField]
    GameObject greenPill = null;
    [SerializeField]
    GameObject redPill = null;

    [SerializeField]
    RollerLifeAgent currentAgent;
    public static readonly int NUMBER_OF_GREEN_PILLS = 3;
    public static readonly int NUMBER_OF_RED_PILLS = 3;

    [SerializeField]
    private int minRandomRangePositionX = 1;
    [SerializeField]
    private int maxRandomRangePositionX = 15;
    [SerializeField]
    private int xFactorRedPill = 7;
    [SerializeField]
    private int xFactorGreenPill = 5;

    public void SpawnItems()
    {
        
        pillItens.ForEach(e =>
        {
            Destroy(e);
        });
        pillItens.Clear();
        
        for(int i = 0; i < NUMBER_OF_RED_PILLS; i++)
        {
            var newPill = Instantiate(redPill, this.transform);
            newPill.transform.localPosition = new Vector3(Random.Range(minRandomRangePositionX, maxRandomRangePositionX) + (xFactorRedPill * (i % 2)), 0.44f, ( i % 2 > 0? 13.35f : -2.91f));
            pillItens.Add(newPill);
        }

        for (int i = 0; i < NUMBER_OF_GREEN_PILLS; i++)
        {
            var newPill = Instantiate(greenPill, this.transform);
            newPill.transform.localPosition = new Vector3(Random.Range(minRandomRangePositionX, maxRandomRangePositionX) + (xFactorGreenPill * (i % 4)), 0.44f, (i % 2 > 0? 5.86f : 0.04f));
            pillItens.Add(newPill);
        }
    }

    public void CollectedPill(PillTypes type, GameObject pill)
    {
        switch (type)
        {
            case PillTypes.GREEN:
                currentAgent.GetGreenPill();
            break;
            case PillTypes.RED:
                currentAgent.GetRedPill();
                break;
        }
        pillItens.Remove(pill);
        Destroy(pill);
    }
}
