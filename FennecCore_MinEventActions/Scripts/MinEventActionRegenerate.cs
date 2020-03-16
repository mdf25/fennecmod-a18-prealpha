using System;
using System.Xml;


/**
 * Can revive on a triggered event.
 */

public class MinEventActionRegenerate : MinEventActionTargetedBase
{

    /**
     * Executing: If entity is dead, set it alive again and at max health.
     */

    public override void Execute(MinEventParams _params)
    {
        for (int i = 0; i < this.targets.Count; i++)
        {
            Log.Out("Checking for Entity");
            if (this.targets[i] as Entity != null)
            {
                Log.Out("Found Entity");
                this.targets[i].SetAlive();
                this.targets[i].AddHealth(this.targets[i].GetMaxHealth() / 2);
            }
        }
    }


    /**
     * Checks whether the event can happen.
     */

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
    {
        Log.Out("Can Run.");
        return true;
    }


    /**
     * Converts the XML into the drop info needed.
     */

    public override bool ParseXmlAttribute(XmlAttribute _attribute)
    {
        bool flag = base.ParseXmlAttribute(_attribute);
        return flag;
    }
}
