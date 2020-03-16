using System;
using System.Xml;


/**
 * Can drop an item on a triggered event.
 */

public class MinEventActionDropItem : MinEventActionTargetedBase
{
    
    /**
     * Executing the drop
     */

    public override void Execute(MinEventParams _params)
    {
        for (int i = 0; i < this.targets.Count; i++)
        {
            Log.Out("Checking for Local Player");
            if (this.targets[i] as EntityAlive != null)
            {
                Log.Out("Player Found! Dropping item on ground.");
                EntityHelper.DropItemOnGround(this.targets[i], ItemClass.GetItem(this.item), this.count);
            }
            Log.Warning("Player not found...");
        }
    }

    
    /**
     * Checks whether the event can happen.
     */

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
    {
        return base.CanExecute(_eventType, _params) && this.item != "" && this.count > 0;
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
            if (name == "item")
            {
                this.item = _attribute.Value;
                return true;
            }

            if (name == "count")
            {
                int count;
                if (!int.TryParse(_attribute.Value, out count))
                {
                    throw new Exception("Could not parse value as an integer.");
                }
                if (count < 1)
                {
                    throw new Exception("Count must be positive.");
                }
                this.count = count;
                return true;
            }
            
        }
        return flag;
    }

    private string item;
    private int count = 1;
}
