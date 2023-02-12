using ThunderRoad;
using UnityEngine;
using HarmonyLib;

namespace XenobladeRPG
{
    public class XenobladePatcher
    {
        public static void DoPatching()
        {
            Harmony harmony = new Harmony("XenobladeRPG");
            harmony.PatchAll();
        }
    }
    public class XenobladeIndicatorState
    {
        public bool IsCritical { get; set; }
        public bool IsResisted { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDefended { get; set; }
        public bool IsMissed { get; set; }
        public bool IsAlive { get; set; }
    }
    [HarmonyPatch(typeof(Creature), nameof(Creature.Damage))]
    public class XenobladeDamagePatch
    {
        static void DetermineHitRateModifiers(float attackerLevel, float defenderLevel, float attackerAgility, float defenderAgility, out float blockRateModifier, out float physicalHitRate, out float etherHitRate, out float evadeRate)
        {
            float levelDifference = attackerLevel - defenderLevel;
            float initialEtherHitRate = 1;
            float initialPhysicalHitRate = ((attackerAgility - defenderAgility) / 100) + 1;
            float initialEvadeRate = (defenderAgility - attackerAgility) / 100;
            blockRateModifier = Mathf.Clamp(0.05f * -levelDifference, -1, 1);
            if (Mathf.Abs(levelDifference) >= 10)
            {
                physicalHitRate = initialPhysicalHitRate + (2 * Mathf.Sign(levelDifference));
                etherHitRate = initialEtherHitRate + (1.1f * Mathf.Sign(levelDifference));
                evadeRate = Mathf.Clamp(initialEvadeRate + (2 * -Mathf.Sign(levelDifference)), 0, 0.95f);
            }
            else if (Mathf.Abs(levelDifference) >= 6)
            {
                physicalHitRate = initialPhysicalHitRate + ((1.2f + (0.2f * (Mathf.Abs(levelDifference) - 6))) * Mathf.Sign(levelDifference));
                etherHitRate = initialEtherHitRate + ((0.66f + (0.11f * (Mathf.Abs(levelDifference) - 6))) * Mathf.Sign(levelDifference));
                evadeRate = Mathf.Clamp(initialEvadeRate + ((1.2f + (0.2f * (Mathf.Abs(levelDifference) - 6))) * -Mathf.Sign(levelDifference)), 0, 0.95f);
            }
            else if (Mathf.Abs(levelDifference) >= 3)
            {
                physicalHitRate = initialPhysicalHitRate + ((0.24f + (0.08f * (Mathf.Abs(levelDifference) - 3))) * Mathf.Sign(levelDifference));
                etherHitRate = initialEtherHitRate + ((0.18f + (0.06f * (Mathf.Abs(levelDifference) - 3))) * Mathf.Sign(levelDifference));
                evadeRate = Mathf.Clamp(initialEvadeRate + ((0.24f + (0.08f * (Mathf.Abs(levelDifference) - 3))) * -Mathf.Sign(levelDifference)), 0, 0.95f);
            }
            else
            {
                physicalHitRate = initialPhysicalHitRate;
                etherHitRate = initialEtherHitRate;
                evadeRate = Mathf.Clamp(initialEvadeRate, 0, 0.95f);
            }
        }
        static void DetermineHitDirection(Creature attacker, Creature defender, out DefenseDirection defenseDirection)
        {
            float forwardDot = Vector3.Dot(defender.transform.forward, (attacker.transform.position - defender.transform.position).normalized);
            if (forwardDot >= 0)
            {
                if (Vector3.Dot(defender.transform.right, (attacker.transform.position - defender.transform.position).normalized) > forwardDot) defenseDirection = DefenseDirection.Right;
                else if (Vector3.Dot(-defender.transform.right, (attacker.transform.position - defender.transform.position).normalized) > forwardDot) defenseDirection = DefenseDirection.Left;
                else defenseDirection = DefenseDirection.Front;
            }
            else
            {
                if (Vector3.Dot(defender.transform.right, (attacker.transform.position - defender.transform.position).normalized) > -forwardDot) defenseDirection = DefenseDirection.Right;
                else if (Vector3.Dot(-defender.transform.right, (attacker.transform.position - defender.transform.position).normalized) > -forwardDot) defenseDirection = DefenseDirection.Left;
                else defenseDirection = DefenseDirection.Back;
            }
        }
        static void Prefix(Creature __instance, ref CollisionInstance collisionInstance, out XenobladeIndicatorState __state)
        {
            bool isCritical = false;
            bool isSneakAttack = false;
            bool isResisted = false;
            bool isBlocked = false;
            bool isDefended = false;
            bool isMissed = false;
            float physicalDamageMult = 1;
            float etherDamageMult = 1;
            XenobladeDamageType type = XenobladeDamageType.Unknown;
            __state = new XenobladeIndicatorState();
            if (!__instance.isKilled && collisionInstance?.damageStruct != null && collisionInstance.damageStruct.damage > 0 && collisionInstance.damageStruct.active && collisionInstance.damageStruct.damage < float.PositiveInfinity && !XenobladeManager.bypassedCollisions.ContainsKey(collisionInstance))
            {
                float damageTotal = 0;
                Creature attacker;
                Item weapon = null;
                if (collisionInstance?.casterHand?.ragdollHand?.ragdoll?.creature != null)
                {
                    attacker = collisionInstance?.casterHand?.ragdollHand?.ragdoll?.creature;
                }
                else if (collisionInstance?.sourceColliderGroup?.collisionHandler?.item?.lastHandler?.creature != null)
                {
                    attacker = collisionInstance?.sourceColliderGroup?.collisionHandler?.item?.lastHandler?.creature;
                    weapon = collisionInstance?.sourceColliderGroup?.collisionHandler?.item;
                }
                else if (collisionInstance?.sourceColliderGroup?.collisionHandler?.ragdollPart?.ragdoll?.creature != null)
                {
                    attacker = collisionInstance?.sourceColliderGroup?.collisionHandler?.ragdollPart?.ragdoll?.creature;
                }
                else
                {
                    attacker = Player.local.creature;
                }
                if (attacker == __instance && __instance.lastInteractionCreature != null)
                {
                    attacker = __instance.lastInteractionCreature;
                }
                DetermineHitDirection(attacker, __instance, out DefenseDirection direction);
                try
                {
                    XenobladeStats attackerStats = attacker.GetComponent<XenobladeStats>();
                    XenobladeStats defenderStats = __instance.GetComponent<XenobladeStats>();
                    XenobladeWeaponModule weaponStats = weapon?.data?.GetModule<XenobladeWeaponModule>();
                    int attackerLevel = attackerStats != null ? attackerStats.GetLevel() : XenobladeManager.GetLevel();
                    int defenderLevel = defenderStats != null ? defenderStats.GetLevel() : XenobladeManager.GetLevel();
                    int attackerAgility = attackerStats != null ? attackerStats.GetAgility() : Mathf.FloorToInt(XenobladeManager.GetAgility());
                    int defenderAgility = defenderStats != null ? defenderStats.GetAgility() : Mathf.FloorToInt(XenobladeManager.GetAgility());
                    if (attackerLevel >= defenderLevel + 6) physicalDamageMult = 2f;
                    else if (attackerLevel >= defenderLevel + 3) physicalDamageMult = 1.5f;
                    else if (attackerLevel <= defenderLevel - 3) physicalDamageMult = 0.75f;
                    else if (attackerLevel <= defenderLevel - 6) physicalDamageMult = 0.5f;
                    if (attackerLevel >= defenderLevel + 7) etherDamageMult = 1.5f; 
                    else if (attackerLevel >= defenderLevel + 4) etherDamageMult = 1.25f;
                    else if (attackerLevel <= defenderLevel + 4) etherDamageMult = 0.75f;
                    else if (attackerLevel <= defenderLevel + 7) etherDamageMult = 0.5f;
                    DetermineHitRateModifiers(attackerLevel, defenderLevel, attackerAgility, defenderAgility, out float blockRateModifier, out float physicalHitRate, out float etherHitRate, out float evadeRate);
                    float blockRate = defenderStats ? defenderStats.GetPhysicalDefense() : XenobladeManager.GetBlockRate();
                    float criticalRate = attackerStats ? attackerStats.GetCriticalRate() : XenobladeManager.GetCriticalRate();
                    if (defenderStats != null && (direction == defenderStats.defenseDirection ||
                        ((direction == DefenseDirection.Front || direction == DefenseDirection.Back) && defenderStats.defenseDirection == DefenseDirection.FrontAndBack) ||
                        (direction == DefenseDirection.Right || direction == DefenseDirection.Left) && defenderStats.defenseDirection == DefenseDirection.Side ||
                        defenderStats.defenseDirection == DefenseDirection.All))
                    {
                        isDefended = true;
                    }
                    if (collisionInstance?.damageStruct.damageType == DamageType.Energy || collisionInstance?.damageStruct.damageType == DamageType.Unknown || collisionInstance.damageStruct.damager == null)
                    {
                        damageTotal += collisionInstance.damageStruct.damage + (attacker.isPlayer ? XenobladeManager.GetEther() : (attackerStats ? attackerStats.GetEther() : 0)) + ((weaponStats != null && attacker.isPlayer) ? Random.Range(weaponStats.attackDamageRange.x, weaponStats.attackDamageRange.y + 1) : 0);
                        if (Random.Range(1, 101) <= 100 - (etherHitRate * 100)) isResisted = true;
                        if (__instance.state == Creature.State.Destabilized) isResisted = false;
                        if ((Random.Range(1, 101) <= criticalRate * 100 || __instance.state == Creature.State.Destabilized) && !isResisted) isCritical = true;
                        if (defenderStats?.GetEtherDefense() <= 0) isDefended = false;
                        if (isDefended) damageTotal *= 1 - defenderStats.GetEtherDefense();
                        if (__instance.isPlayer)
                        {
                            damageTotal = Mathf.Max(damageTotal - XenobladeManager.GetEtherDefense(), 0);
                        }
                        if (attacker.isPlayer && !isResisted) XenobladeManager.EtherHits++;
                        damageTotal *= etherDamageMult;
                        type = XenobladeDamageType.Ether;
                    }
                    else
                    {
                        damageTotal += collisionInstance.damageStruct.damage + (attacker.isPlayer ? XenobladeManager.GetStrength() : (attackerStats ? attackerStats.GetStrength() : 0)) + ((weaponStats != null && attacker.isPlayer) ? Random.Range(weaponStats.attackDamageRange.x, weaponStats.attackDamageRange.y + 1) : 0);
                        if (Random.Range(1, 101) <= 100 - (physicalHitRate * 100)) isMissed = true;
                        if (Random.Range(1, 101) <= (evadeRate * 100)) isMissed = true;
                        if (__instance.state == Creature.State.Destabilized || collisionInstance.damageStruct.penetration == DamageStruct.Penetration.Skewer || collisionInstance.damageStruct.damage == float.PositiveInfinity) isMissed = false;
                        if (Random.Range(1, 101) <= (blockRate + blockRateModifier) * 100) isBlocked = true;
                        if (Random.Range(1, 101) <= (criticalRate * 100) && !isBlocked) isCritical = true;
                        if (defenderStats?.GetPhysicalDefense() <= 0) isDefended = false;
                        if (isDefended) damageTotal *= 1 - defenderStats.GetPhysicalDefense();
                        if (__instance.isPlayer)
                        {
                            damageTotal = Mathf.Max(damageTotal - XenobladeManager.GetPhysicalDefense(), 0);
                        }
                        if (attacker.isPlayer && !isMissed) XenobladeManager.StrengthHits++;
                        damageTotal *= physicalDamageMult;
                        type = XenobladeDamageType.Physical;
                    }
                    if (defenderStats.isToppled && (collisionInstance.targetColliderGroup?.collisionHandler?.ragdollPart?.type == RagdollPart.Type.Head || __instance.brain.isElectrocuted)) defenderStats.isDazed = true;
                }
                catch
                {
                    if (collisionInstance?.damageStruct != null)
                    {
                        if (collisionInstance.damageStruct.damageType == DamageType.Energy || collisionInstance?.damageStruct.damageType == DamageType.Unknown)
                        {
                            damageTotal += collisionInstance.damageStruct.damage + XenobladeManager.GetEther();
                            isResisted = false;
                            if (__instance.state == Creature.State.Destabilized) isCritical = true;
                            if (attacker.isPlayer) XenobladeManager.EtherHits++;
                            type = XenobladeDamageType.Ether;
                        }
                        else
                        {
                            damageTotal += collisionInstance.damageStruct.damage + XenobladeManager.GetStrength();
                            isBlocked = false;
                            if (attacker.isPlayer) XenobladeManager.StrengthHits++;
                            type = XenobladeDamageType.Physical;
                        }
                    }
                }
                if (collisionInstance != null && !__instance.isPlayer && (__instance?.brain?.currentTarget == null || (__instance?.brain != null && __instance.brain.currentTarget != attacker)))
                {
                    damageTotal *= 5;
                    isBlocked = false;
                    isResisted = false;
                    isSneakAttack = true;
                    isCritical = true;
                    isMissed = false;
                }
                if (__instance.GetComponent<XenobladeStats>() is XenobladeStats asleep && asleep.isSleeping) isCritical = true;
                if ((collisionInstance?.sourceCollider?.attachedRigidbody != null && collisionInstance?.targetCollider?.attachedRigidbody != null) || collisionInstance?.casterHand != null)
                {
                    if (!isSneakAttack && isCritical)
                    {
                        damageTotal *= 1.25f + (__instance.state == Creature.State.Destabilized ? 0.25f : 0);
                    }
                }
                collisionInstance.damageStruct.damage = damageTotal;
                if (isBlocked)
                {
                    collisionInstance.damageStruct.damage *= 0.5f;
                    if (!attacker.isPlayer)
                        attacker.TryPush(Creature.PushType.Parry, (attacker.transform.position - __instance.transform.position).normalized, 1);
                    collisionInstance.damageStruct.pushLevel = -1;
                }
                if (isResisted)
                {
                    collisionInstance.damageStruct.damage = 0;
                    collisionInstance.damageStruct.pushLevel = -1;
                }
                if (isMissed)
                {
                    collisionInstance.damageStruct.damager?.UnPenetrate(collisionInstance);
                    collisionInstance.effectInstance?.Stop();
                    collisionInstance.damageStruct.Reset(true);
                }
            }
            __state.IsCritical = isCritical;
            __state.IsResisted = isResisted;
            __state.IsBlocked = isBlocked;
            __state.IsDefended = isDefended;
            __state.IsMissed = isMissed;
            __state.IsAlive = !__instance.isKilled;
            if (isMissed)
            {
                if (!__instance.isKilled && collisionInstance?.damageStruct != null)
                {
                    GameObject dmg = Object.Instantiate(XenobladeLevelModule.damage);
                    XenobladeDamage xenobladeDamage = dmg.AddComponent<XenobladeDamage>();
                    xenobladeDamage.creature = __instance;
                    xenobladeDamage.instance = collisionInstance;
                    xenobladeDamage.isCritical = isCritical;
                    xenobladeDamage.isResisted = isResisted;
                    xenobladeDamage.isBlocked = isBlocked;
                    xenobladeDamage.isDefended = isDefended;
                    xenobladeDamage.isMissed = isMissed;
                }
            }
            if (!XenobladeManager.recordedCollisions.ContainsKey(collisionInstance))
            {
                if (XenobladeManager.bypassedCollisions.ContainsKey(collisionInstance))
                    XenobladeManager.recordedCollisions.Add(collisionInstance, XenobladeManager.bypassedCollisions[collisionInstance]);
                else
                    XenobladeManager.recordedCollisions.Add(collisionInstance, type);
            }
        }
        static void Postfix(Creature __instance, ref CollisionInstance collisionInstance, XenobladeIndicatorState __state)
        {
            if (__state.IsAlive && collisionInstance?.damageStruct != null && collisionInstance.damageStruct.active && collisionInstance.damageStruct.damage < float.PositiveInfinity && collisionInstance.damageStruct.damage >= 0)
            {
                GameObject dmg = Object.Instantiate(XenobladeLevelModule.damage);
                XenobladeDamage xenobladeDamage = dmg.AddComponent<XenobladeDamage>();
                xenobladeDamage.creature = __instance;
                xenobladeDamage.instance = collisionInstance;
                xenobladeDamage.isCritical = __state.IsCritical;
                xenobladeDamage.isResisted = __state.IsResisted;
                xenobladeDamage.isBlocked = __state.IsBlocked;
                xenobladeDamage.isDefended = __state.IsDefended;
                xenobladeDamage.isMissed = __state.IsMissed;
            }
        }
    }
}
