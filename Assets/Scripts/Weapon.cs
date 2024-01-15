using UnityEngine;

public class Weapon : MonoBehaviour
{
    // to compare the item.prefab with the gameObject inside the player's hand
    [SerializeField] private int prefabID;


    public int GetPrefabID
    {
        get { return prefabID; }
    }
}