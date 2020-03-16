using System;

public class IsCloudy : TargetedCompareRequirementBase
{
    /**
     * Checks whether the weather is overcast.
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
        return WeatherManager.GetCloudThickness() > 0.4f;
    }
}
