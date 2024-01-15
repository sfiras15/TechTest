using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class XpToLevel
{
    public int level;
    public int xp;
}
/// <summary>
/// handles the xp/level of the player
/// </summary>
public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;

    // Stores the variable to be used in the enemyAi script which will reduce FindObjectOfType calls overall 
    public Transform playerTransform;

    public int playerLevel;

    private int currentPlayerXp;
    private int nextLevelXp;

    [SerializeField] private Bar xpBar;

    [SerializeField] private XpToLevel[] xpToLevelsList;
    //Event to update damage the player and update his healthbar
    public static event Action<float> onPlayerDamaged;
    private Dictionary<int, int> xpToLevelDictionary = new Dictionary<int, int>();


   
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        playerTransform = FindObjectOfType<InputPlayer>().gameObject.transform;
        foreach (XpToLevel element in xpToLevelsList)
        {
            xpToLevelDictionary.Add(element.level, element.xp);
        }
    }

    public void AddXp(int value)
    {
        currentPlayerXp += value;
        UpdateLevel(currentPlayerXp);
        xpBar.UpdateBar(nextLevelXp, currentPlayerXp);
    }

    public void UpdateLevel(int xp)
    {
        for (int i = playerLevel; i < xpToLevelDictionary.Count; i++)
        {
            if (xp >= xpToLevelDictionary[i])
            {
                playerLevel = i;
                nextLevelXp = xpToLevelDictionary[i + 1];
            }
        }
    }

    public void UpdatePlayerHealth(float value)
    {
        if (onPlayerDamaged != null)
        {
            onPlayerDamaged.Invoke(value);
        }
    }



}
