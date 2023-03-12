namespace XenobladeRPG
{
    public enum SpikeType
    {
        /// <summary>
        /// Does not deal damage nor apply an effect.
        /// </summary>
        None,
        /// <summary>
        /// Deals damage/applies effect when attacked.
        /// </summary>
        Counter,
        /// <summary>
        /// Deals damage/applies effect when attacked while Toppled or Dazed.
        /// </summary>
        Topple,
        /// <summary>
        /// Deals damage/applies effect in a radius around the creature.
        /// </summary>
        CloseRange
    }
    public enum SpikeEffect
    {
        None,
        StrengthDown,
        EtherDown,
        PhysicalDefenseDown,
        EtherDefenseDown,
        AgilityDown,
        Sleep,
        Bind,
        Topple,
        Daze,
        Slow,
        Paralyze,
        InstantDeath
    }
}
