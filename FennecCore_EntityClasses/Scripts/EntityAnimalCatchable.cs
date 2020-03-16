using System;
using UnityEngine;

public class EntityAnimalCatchable : EntityAnimalRabbit
{
    public override void CopyPropertiesFromEntityClass()
    {
        base.CopyPropertiesFromEntityClass();

        EntityClass entityClass = EntityClass.list[this.entityClass];
        if (!entityClass.Properties.Values.ContainsKey(this.propItemToReturn))
        {
            throw new Exception("Entity " + entityClass.entityClassName + " does not have a " + this.propItemToReturn + " property.");
        }

        this.itemReturned = ItemClass.GetItem(entityClass.Properties.Values[this.propItemToReturn], false);
        if (this.itemReturned.IsEmpty())
        {
            throw new Exception("Item with name '" + entityClass.Properties.Values[this.propItemToReturn] + "' not found!");
        }

        this.holdingItem = ItemClass.GetItem("meleeHandPlayer", false);
        if (entityClass.Properties.Values.ContainsKey(this.propHoldingItem))
        {
            this.holdingItem = ItemClass.GetItem(entityClass.Properties.Values[this.propHoldingItem], false);
            if (this.holdingItem.IsEmpty())
            {
                throw new Exception("Item with name '" + EntityClass.Properties.Values[this.propHoldingItem] + "' not found!");
            }
        }
    }


    /**
     * When the player activates this entityclass with an empty hand, try to pick it up.
     */

    public override void Kill(DamageResponse _dmgResponse)
    {
        if (_dmgResponse.Source.getEntityId() == -1)
        {
            base.Kill(_dmgResponse);
            Log.Warning("Killer was not a player.");
            return;
        }

        EntityPlayerLocal entityPlayerLocal = GameManager.Instance.World.GetLocalPlayerFromID(_dmgResponse.Source.getEntityId()) as EntityPlayerLocal;
        if (!(entityPlayerLocal is EntityPlayerLocal))
        {
            base.Kill(_dmgResponse);
            Log.Warning("Killer was an entity, but not a player.");
            return;
        }

        LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
        if (uiforPlayer.xui.isUsingItemActionEntryUse)
        {
            base.Kill(_dmgResponse);
            Log.Warning("XUI interfering.");
            return;
        }

        ItemStack pickup = new ItemStack(this.itemReturned, 1);
        if (!entityPlayerLocal.inventory.CanTakeItem(pickup) & !entityPlayerLocal.bag.CanTakeItem(pickup))
        {
            base.Kill(_dmgResponse);
            Log.Warning("Canot take item - no room in inventory.");
            return;
        }

        Log.Out("Player is holding: " + entityPlayerLocal.inventory.holdingItem.GetItemName());
        if (entityPlayerLocal.inventory.holdingItem.GetItemName() == "meleeHandPlayer")
        {
            Log.Out("Server, despawn the entity.");
            GameManager.Instance.World.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);

            Log.Out("Player, add the item.");
            entityPlayerLocal.inventory.AddItem(pickup);
            return;
        }

        Log.Warning("Cannot activate.");
        base.Kill(_dmgResponse);
        return;
    }

    private string propHoldingItem  = "HoldingItem";
    private string propItemToReturn = "ItemToReturn";
    private ItemValue itemReturned;
    private ItemValue holdingItem;
}
