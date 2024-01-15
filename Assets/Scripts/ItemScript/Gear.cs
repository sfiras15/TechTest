using UnityEngine;


[CreateAssetMenu(fileName = "Items/Gear", menuName = "Items/Gear", order = 2)]
public class Gear : Item
{
    public int healthBoost;
    public int damageBoost;
    public int speedBoost;
    public int cost;
    public int level;
    public Gear[] components;

    public override void Use()
    {
        
    }

}
