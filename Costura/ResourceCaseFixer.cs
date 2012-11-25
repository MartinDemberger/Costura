using System;

public partial class InnerTask
{

    public void FixResourceCase()
    {
        foreach (var resource in Module.Resources)
        {
            if (resource.Name.StartsWith("costura.", StringComparison.InvariantCultureIgnoreCase))
            {
                resource.Name = resource.Name.ToLowerInvariant();
            }
        }
    }
}