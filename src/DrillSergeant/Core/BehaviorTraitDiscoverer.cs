using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Core;

internal class BehaviorTraitDiscoverer : TraitDiscoverer
{
    public override IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var attributeInfo = traitAttribute as ReflectionAttributeInfo;

        if (attributeInfo?.Attribute is BehaviorAttribute behavior)
        {
            if (string.IsNullOrWhiteSpace(behavior.Category) == false)
            {
                yield return new KeyValuePair<string, string>("Category", behavior.Category);
            }
        }
    }
}
