using System;
using System.Xml;
using System.Collections.Generic;


/**
 * Can upgrade a block in a certain range on a triggered event.
 * <triggered_effect trigger="" action="UpgradeBlockInRange, Mods" block_name ="" range="1,1,1" require_materials="true" />
 */

public class MinEventActionUpgradeBlockInRange : MinEventActionTargetedBase
{

    /**
     * Executing the upgrades
     */

    public override void Execute(MinEventParams _params)
    {
        if (GameManager.Instance.World == null)
        {
            return;
        }
        World world = GameManager.Instance.World;

        for (int i = 0; i < this.targets.Count; i++)
        {
            Log.Out("Checking for Entity");
            if (this.targets[i] as Entity == null)
            {
                Log.Out("Null, continue");
                continue;
            }

            Vector3i currentPosition = new Vector3i(this.targets[i].GetPosition());
            Dictionary<Vector3i, Block> blockPositions = CoordinateHelper.GetBlocksFromCoordinates(world, CoordinateHelper.GetCoOrdinatesAround(currentPosition, this.range));
            foreach (KeyValuePair<Vector3i, Block> entry in blockPositions)
            {
                Block blockToCheck = entry.Value;
                if (blockToCheck.GetBlockName() != this.blockName)
                {
                    continue;
                }

                BlockHelpers.DoBlockUpgrade(entry.Key, this.targets[i], this.requireMaterials);
            }
        }
    }


    /**
     * Checks whether the event can happen.
     */

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
    {
        return base.CanExecute(_eventType, _params) && this.blockName != "" && this.range.x >= 0 && this.range.y >= 0 && this.range.z >= 0;
    }


    /**
     * Converts the XML into the drop info needed.
     */

    public override bool ParseXmlAttribute(XmlAttribute _attribute)
    {
        bool flag = base.ParseXmlAttribute(_attribute);
        if (!flag)
        {
            string name = _attribute.Name;
            if (name == "block_name")
            {
                this.blockName = _attribute.Value;
                return true;
            }

            if (name == "range")
            {
                this.range = StringHelpers.WriteStringToVector3i(_attribute.Value);
                if (range == Vector3i.zero)
                {
                    return false;
                }
            }

            if (name == "require_materials")
            {
                bool requireMaterials;
                if (!StringParsers.TryParseBool(_attribute.Value, out requireMaterials))
                {
                    throw new Exception("Could not parse value as an boolean.");
                }
                this.requireMaterials = requireMaterials;
            }

        }
        return flag;
    }

    private string blockName;
    private Vector3i range = Vector3i.one;
    private bool requireMaterials = false;
}
