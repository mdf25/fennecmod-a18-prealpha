using System;

public class IsRaining : TargetedCompareRequirementBase
{
    /**
     * Checks whether the weather is raining at night.
     */
    public override bool IsValid(MinEventParams _params)
    {
        if (!this.ParamsValid(_params))
        {
            return false;
        }
        if (this.target == null)
        {
            return false;
        }
        if (WeatherManager.theInstance == null)
        {
            return false;
        }
        return WeatherManager.theInstance.GetCurrentRainfallValue() > 0.2f;
    }
}
