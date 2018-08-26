

namespace Mahjong
{
    public struct RuleSet
    {
        public bool Akadora { get; set; }
        public bool Uradora { get; set; }
        public bool Kanuradora { get; set; }
    }

    public class RuleSets
    {
        public static RuleSet DefaultRules = new RuleSet()
        {
            Akadora = false,
            Uradora = true,
            Kanuradora = true
        };
    }

}
