using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class XenobladeEvents : MonoBehaviour
    {
        public static event OnBuffAdded onBuffAdded;
        public static event OnBuffRemoved onBuffRemoved;
        public static event OnDebuffAdded onDebuffAdded;
        public static event OnDebuffRemoved onDebuffRemoved;
        public static event OnXenobladeDamage onXenobladeDamage;
        public static event OnBypassedDamage onBypassedDamage;
        public static void InvokeOnBuffAdded(ref Creature creature, Component buff = null, XenobladeManager.PlayerStatModifier playerStatModifier = null, XenobladeStats.StatModifier creatureStatModifier = null)
        {
            OnBuffAdded onBuffAdded = XenobladeEvents.onBuffAdded;
            if (onBuffAdded == null)
                return;
            onBuffAdded(ref creature, buff, playerStatModifier, creatureStatModifier);
        }
        public static void InvokeOnBuffRemoved(ref Creature creature, Component buff = null)
        {
            OnBuffRemoved onBuffRemoved = XenobladeEvents.onBuffRemoved;
            if (onBuffRemoved == null)
                return;
            onBuffRemoved(ref creature, buff);
        }
        public static void InvokeOnDebuffAdded(ref Creature creature, Component debuff = null, XenobladeManager.PlayerStatModifier playerStatModifier = null, XenobladeStats.StatModifier creatureStatModifier = null)
        {
            OnDebuffAdded onDebuffAdded = XenobladeEvents.onDebuffAdded;
            if (onDebuffAdded == null)
                return;
            onDebuffAdded(ref creature, debuff, playerStatModifier, creatureStatModifier);
        }
        public static void InvokeOnDebuffRemoved(ref Creature creature, Component debuff = null)
        {
            OnDebuffRemoved onDebuffRemoved = XenobladeEvents.onDebuffRemoved;
            if (onDebuffRemoved == null)
                return;
            onDebuffRemoved(ref creature, debuff);
        }
        public static void InvokeOnXenobladeDamage(ref CollisionInstance collisionInstance, ref Creature attacker, ref Creature defender, ref XenobladeDamageType damageType, EventTime eventTime)
        {
            OnXenobladeDamage onXenobladeDamage = XenobladeEvents.onXenobladeDamage;
            if (onXenobladeDamage == null)
                return;
            onXenobladeDamage(ref collisionInstance, ref attacker, ref defender, ref damageType, eventTime);
        }
        public static void InvokeOnBypassedDamage(ref CollisionInstance collisionInstance, ref Creature defender, XenobladeDamageType damageType, ref XenobladeIndicatorState state)
        {
            OnBypassedDamage onBypassedDamage = XenobladeEvents.onBypassedDamage;
            if (onBypassedDamage == null)
                return;
            onBypassedDamage(ref collisionInstance, ref defender, ref damageType, ref state);
        }
        public delegate void OnBuffAdded(ref Creature creature, Component buff = null, XenobladeManager.PlayerStatModifier playerStatModifier = null, XenobladeStats.StatModifier creatureStatModifier = null);
        public delegate void OnBuffRemoved(ref Creature creature, Component buff = null);
        public delegate void OnDebuffAdded(ref Creature creature, Component debuff = null, XenobladeManager.PlayerStatModifier playerStatModifier = null, XenobladeStats.StatModifier creatureStatModifier = null);
        public delegate void OnDebuffRemoved(ref Creature creature, Component debuff = null);
        public delegate void OnXenobladeDamage(ref CollisionInstance collisionInstance, ref Creature attacker, ref Creature defender, ref XenobladeDamageType damageType, EventTime eventTime);
        public delegate void OnBypassedDamage(ref CollisionInstance collisionInstance, ref Creature defender, ref XenobladeDamageType damageType, ref XenobladeIndicatorState state);
    }
}
