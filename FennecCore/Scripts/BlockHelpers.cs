public static class BlockHelpers
{
    /**
     *  Returns the item stack for upgrading a block, as well as the target block it should upgrade to. 
     */

    public static ItemStack GetBlockUpgradeItems(Block block, out Block upgradeResult)
    {
        upgradeResult = null;
        // Check block has an upgrade path.
        if (block.UpgradeBlock.Equals(BlockValue.Air))
        {
            return null;
        }
        if (!block.Properties.Classes.ContainsKey("UpgradeBlock"))
        {
            return null;
        }
        DynamicProperties upgradeBlockClass = block.Properties.Classes["UpgradeBlock"];

        // Check that all necessary properties are defined.
        if (!upgradeBlockClass.Values.ContainsKey("ToBlock"))
        {
            return null;
        }
        if (!upgradeBlockClass.Values.ContainsKey("Item"))
        {
            return null;
        }
        if (!upgradeBlockClass.Values.ContainsKey("ItemCount"))
        {
            return null;
        }

        // Build block upgrade data
        ItemValue upgradeItem = ItemClass.GetItem(upgradeBlockClass.Values["Item"]);
        int upgradeItemCount;
        if (!int.TryParse(upgradeBlockClass.Values["ItemCount"], out upgradeItemCount))
        {
            return null;
        }

        upgradeResult = Block.GetBlockByName(upgradeBlockClass.Values["ToBlock"]);
        return new ItemStack(upgradeItem, upgradeItemCount);
    }


    /**
     * Can pass block name as a string if desired.
     */

    public static ItemStack GetBlockUpgradeItems(string blockName, out Block upgradeResult)
    {
        return GetBlockUpgradeItems(Block.GetBlockByName(blockName), out upgradeResult);
    }


    /**
     * Returns the downgrade block.
     */

    public static Block GetDowngradeBlock(Block block)
    {
        if (block.DowngradeBlock.Equals(BlockValue.Air))
        {
            return null;
        }
        return block.DowngradeBlock.Block;
    }


    /** 
     * Gets the upgrade sound
     */

    public static string GetBlockUpgradeSound(Block block)
    {
        return block.CustomUpgradeSound;
    }


    /**
     * Performs an upgrade to a block from an entity.
     */

    public static bool DoBlockUpgrade(Vector3i blockPos, Entity entity, bool requireItems = true)
    {
        // Check world is instantiated before trying to proceed.
        if (GameManager.Instance.World == null)
        {
            return false;
        }
        World world = GameManager.Instance.World;
        
        // Get the block upgrade path from blocks at this coordinate.
        Block blockToUpgrade = CoordinateHelper.GetBlockAtCoordinate(world, blockPos);
        Block upgradeResult;
        ItemStack upgradeItems = GetBlockUpgradeItems(blockToUpgrade, out upgradeResult);
        
        // Check that the block actually has an upgrade result before continuing.
        if (upgradeResult == null)
        {
            return false;
        }

        // If we don't require items, do the straight upgrade and we're done.
        if (!requireItems)
        {
            world.SetBlockRPC(blockPos, Block.GetBlockValue(upgradeResult.GetBlockName()));
            return true;
        }

        // If items are required, need to get inventory for the entity.
        Inventory inventory;

        if (entity is EntityPlayerLocal)
        {
            inventory = ((EntityPlayerLocal)entity).inventory;
        }
        else if (entity is EntityNPC)
        {
            inventory = ((EntityNPC)entity).inventory;
        }
        else if (entity is EntityVehicle)
        {
            inventory = ((EntityVehicle)entity).inventory;
        }
        else
        {
            Log.Out("No inventory found for entity.");
            return false;
        }

        bool removed = false;
        InventoryHelper.RemoveItemsInInventory(inventory.GetSlots(), upgradeItems, out removed);
        return removed;
    }


    /**
    * Performs a downgrade to a block from an entity.
    */

    public static bool DoBlockDowngrade(Vector3i blockPos)
    {
        // Check world is instantiated before trying to proceed.
        if (GameManager.Instance.World == null)
        {
            return false;
        }
        World world = GameManager.Instance.World;

        // Get the block upgrade path from blocks at this coordinate.
        Block blockToDowngrade  = CoordinateHelper.GetBlockAtCoordinate(world, blockPos);
        Block downgradeResult   = GetDowngradeBlock(blockToDowngrade);

        // Check that the block actually has an upgrade result before continuing.
        if (downgradeResult == null)
        {
            return false;
        }
        
        world.SetBlockRPC(blockPos, Block.GetBlockValue(downgradeResult.GetBlockName()));
        return true;
    }
}