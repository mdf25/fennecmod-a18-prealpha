using System;
using System.Xml;
using UnityEngine;


/**
 * Can drop an item on a triggered event.
 */

public class MinEventActionSpawnEntity : MinEventActionTargetedBase
{

    /**
     * Executing the drop
     */

    public override void Execute(MinEventParams _params)
    {
        if (this.minDistance > this.maxDistance)
        {
            this.minDistance = this.maxDistance;
        }

        for (int i = 0; i < this.targets.Count; i++)
        {
            Log.Out("Checking for Entity");
            if (this.targets[i] as Entity == null)
            {
                Log.Out("Null, continue");
                continue;
            }

            Vector3 currentPosition = this.targets[i].GetPosition();
            EntityHelper.SpawnEntitiesAroundPosition(this.entitygroup, currentPosition, 1, 3, this.count);
        }
    }


    /**
     * Checks whether the event can happen.
     */

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
    {
        return base.CanExecute(_eventType, _params) && this.entitygroup != "" && this.minDistance >= 0 && this.maxDistance >= 0 && this.count > 0;
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
            if (name == "entitygroup")
            {
                this.entitygroup = _attribute.Value;
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

            if (name == "min_distance")
            {
                int minDistance;
                if (!int.TryParse(_attribute.Value, out minDistance))
                {
                    throw new Exception("Could not parse value as an integer.");
                }
                if (minDistance < 0)
                {
                    throw new Exception("Minimum distance must be non-negative.");
                }
                this.minDistance = minDistance;
                return true;
            }

            if (name == "max_distance")
            {
                int maxDistance;
                if (!int.TryParse(_attribute.Value, out maxDistance))
                {
                    throw new Exception("Could not parse value as an integer.");
                }
                if (maxDistance < 0)
                {
                    throw new Exception("Minimum distance must be non-negative.");
                }
                this.maxDistance = maxDistance;
                return true;
            }

        }
        return flag;
    }

    private string entitygroup;
    private int count = 1;
    private int minDistance = 1;
    private int maxDistance = 3;
}
