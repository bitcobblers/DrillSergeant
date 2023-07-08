using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

// ReSharper disable once UnusedType.Global
internal class BehaviorTraitDiscoverer : TraitDiscoverer
{
    public override IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var attributeInfo = traitAttribute as ReflectionAttributeInfo;

        if (attributeInfo?.Attribute is not BehaviorAttribute behavior)
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(behavior.Feature) == false)
        {
            yield return new KeyValuePair<string, string>("Feature", behavior.Feature);
        }
    }
}
