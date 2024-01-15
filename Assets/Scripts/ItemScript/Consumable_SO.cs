using UnityEngine;


[CreateAssetMenu(fileName = "Items/Consumables", menuName = "Items/Consumable", order = 2)]
public class Consumable_SO : Item
{
    public enum ConsumableType
    {
        Healing,
        DamageIncrease,
        MsIncrease,
    }
    public float buffValue;
    public int duration;
    public ConsumableType type;
    public int level;

    public override void Use()
    {
        Debug.Log("Potion");
        if (usable)
        {
            if (PlayerManager.instance.playerLevel < level)
            {
                Debug.Log("This potion is dangerous for you, please go level up");
                return;
            }
            Inventory.instance.UsePotion(this);
            Inventory.instance.RemoveItem(this);
        }
        //Drink Consumable
    }

}
