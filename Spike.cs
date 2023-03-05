using System.Linq;
using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class Spike : MonoBehaviour
    {
        public Creature creature;
        public SpikeType type = SpikeType.None;
        public SpikeEffect effect = SpikeEffect.None;
        public Creature attacker;
        public float damage = 0;
        public float debuffPercent = 0;
        public float distance = 0;
        CollisionInstance collisionInstance;
        XenobladeStats attackerStats;
        XenobladeStats creatureStats;
        float time;
        XenobladeManager.PlayerStatModifier playerStatModifier;
        XenobladeStats.StatModifier creatureStatModifier;
        public void Start()
        {
            creature = GetComponent<Creature>();
            creatureStats = creature.GetComponent<XenobladeStats>();
            creature.OnKillEvent += Creature_OnKillEvent;
            XenobladeEvents.onXenobladeDamage += XenobladeEvents_onXenobladeDamage;
            collisionInstance = new CollisionInstance(new DamageStruct(DamageType.Unknown, damage));
            XenobladeManager.BypassXenobladeDamage(collisionInstance, XenobladeDamageType.Spike);
            time = Time.time;
        }

        private void XenobladeEvents_onXenobladeDamage(ref CollisionInstance collisionInstance, ref Creature attacker, ref Creature defender, ref XenobladeDamageType damageType, EventTime eventTime)
        {
            if(defender == creature && !attacker.isKilled && attacker != creature && eventTime == EventTime.OnEnd && damageType != XenobladeDamageType.Spike && (creatureStats != null ? !creatureStats.isAuraSealed : !XenobladeManager.isAuraSealed))
            {
                attackerStats = attacker.GetComponent<XenobladeStats>();
                collisionInstance.damageStruct.hitRagdollPart = attacker.ragdoll.rootPart;
                if (Vector3.Distance(creature.transform.position, attacker.transform.position) <= distance && attacker.faction != creature.faction && collisionInstance.sourceColliderGroup != collisionInstance.targetColliderGroup)
                {
                    if (collisionInstance.damageStruct.damage > 0 && ((type == SpikeType.Counter && creature.state == Creature.State.Alive) || (type == SpikeType.Topple && creature.state == Creature.State.Destabilized))) attacker.Damage(collisionInstance);
                    if (effect != SpikeEffect.None)
                    {
                        switch (effect)
                        {
                            case SpikeEffect.StrengthDown:
                                if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1 - debuffPercent);
                                else attackerStats.SetStatModifier(this, 1, 1 - debuffPercent);
                                break;
                            case SpikeEffect.EtherDown:
                                if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1 - debuffPercent);
                                else attackerStats.SetStatModifier(this, 1, 1, 1 - debuffPercent);
                                break;
                            case SpikeEffect.PhysicalDefenseDown:
                                if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1, 1 - debuffPercent);
                                else attackerStats.SetStatModifier(this, 1, 1, 1, 1, -debuffPercent);
                                break;
                            case SpikeEffect.EtherDefenseDown:
                                if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1, 1, 1 - debuffPercent);
                                else attackerStats.SetStatModifier(this, 1, 1, 1, 1, 1, -debuffPercent);
                                break;
                            case SpikeEffect.AgilityDown:
                                if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1, 1, 1, 1 - debuffPercent);
                                else attackerStats.SetStatModifier(this, 1, 1, 1, 1 - debuffPercent);
                                break;
                            case SpikeEffect.Sleep:
                                if (attackerStats != null)
                                {
                                    Destroy(attacker.GetComponent<Sleep>());
                                    attacker.gameObject.AddComponent<Sleep>();
                                }
                                break;
                            case SpikeEffect.Bind:
                                Destroy(attacker.GetComponent<Bind>());
                                attacker.gameObject.AddComponent<Bind>();
                                break;
                            case SpikeEffect.Topple:
                                if (attackerStats != null)
                                {
                                    attacker.ragdoll.SetState(Ragdoll.State.Destabilized);
                                }
                                break;
                            case SpikeEffect.Daze:
                                if (attackerStats != null)
                                {
                                    attacker.ragdoll.SetState(Ragdoll.State.Destabilized);
                                    attackerStats.isDazed = true;
                                }
                                break;
                            case SpikeEffect.Slow:
                                if (attackerStats != null && attacker.brain.instance.GetModule<BrainModuleMelee>(false) is BrainModuleMelee melee)
                                {
                                    Destroy(attacker.GetComponent<Slow>());
                                    attacker.gameObject.AddComponent<Slow>();
                                    attacker.GetComponent<Slow>().debuffPercent = debuffPercent;
                                }
                                break;
                            case SpikeEffect.Paralyze:
                                if (attackerStats != null)
                                {
                                    attacker.TryElectrocute(1, 5, true, false, Catalog.GetData<EffectData>("ImbueLightningRagdoll"));
                                }
                                break;
                            case SpikeEffect.InstantDeath:
                                CollisionInstance instantDeath = new CollisionInstance(new DamageStruct(DamageType.Unknown, attacker.maxHealth));
                                instantDeath.damageStruct.hitRagdollPart = attacker.ragdoll.rootPart;
                                XenobladeManager.BypassXenobladeDamage(instantDeath, XenobladeDamageType.InstantDeath);
                                attacker.Damage(instantDeath);
                                break;
                        }
                        if (XenobladeManager.statModifiers.Any(match => match.handler == this) || attackerStats.statModifiers.Any(match => match.handler == this))
                        {
                            playerStatModifier = XenobladeManager.statModifiers.Find(match => match.handler == this);
                            creatureStatModifier = attackerStats.statModifiers.Find(match => match.handler == this);
                            XenobladeEvents.InvokeOnDebuffAdded(ref attacker, null, playerStatModifier, creatureStatModifier);
                        }
                    }
                }
            }
        }

        private void Creature_OnKillEvent(CollisionInstance collisionInstance, EventTime eventTime)
        {
            XenobladeEvents.onXenobladeDamage -= XenobladeEvents_onXenobladeDamage;
            Destroy(this);
        }

        public void Setup(SpikeType spikeType, SpikeEffect spikeEffect, float spikeDamage, float spikeDebuffPercent, float spikeDistance)
        {
            type = spikeType;
            effect = spikeEffect;
            damage = spikeDamage;
            debuffPercent = spikeDebuffPercent;
            distance = spikeDistance;
        }
        public void FixedUpdate()
        {
            if (Time.time - 5 >= time && type == SpikeType.CloseRange && (creatureStats != null ? !creatureStats.isAuraSealed : !XenobladeManager.isAuraSealed))
            {
                time = Time.time;
                foreach (Creature enemy in Creature.allActive)
                {
                    if (!enemy.isKilled && enemy != creature && enemy.faction != creature.faction && Vector3.Distance(creature.transform.position, enemy.transform.position) <= distance)
                    {
                        attackerStats = enemy.GetComponent<XenobladeStats>();
                        if (damage > 0)
                        {
                            collisionInstance.damageStruct.hitRagdollPart = enemy.ragdoll.rootPart;
                            creature.Damage(collisionInstance);
                        }
                        if (effect != SpikeEffect.None)
                        {
                            switch (effect)
                            {
                                case SpikeEffect.StrengthDown:
                                    if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1 - debuffPercent);
                                    else attackerStats.SetStatModifier(this, 1, 1 - debuffPercent);
                                    break;
                                case SpikeEffect.EtherDown:
                                    if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1 - debuffPercent);
                                    else attackerStats.SetStatModifier(this, 1, 1, 1 - debuffPercent);
                                    break;
                                case SpikeEffect.PhysicalDefenseDown:
                                    if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1, 1 - debuffPercent);
                                    else attackerStats.SetStatModifier(this, 1, 1, 1, 1, -debuffPercent);
                                    break;
                                case SpikeEffect.EtherDefenseDown:
                                    if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1, 1, 1 - debuffPercent);
                                    else attackerStats.SetStatModifier(this, 1, 1, 1, 1, 1, -debuffPercent);
                                    break;
                                case SpikeEffect.AgilityDown:
                                    if (attackerStats == null) XenobladeManager.SetStatModifier(this, 1, 1, 1, 1, 1 - debuffPercent);
                                    else attackerStats.SetStatModifier(this, 1, 1, 1, 1 - debuffPercent);
                                    break;
                                case SpikeEffect.Sleep:
                                    if (attackerStats != null)
                                    {
                                        Destroy(enemy.GetComponent<Sleep>());
                                        enemy.gameObject.AddComponent<Sleep>();
                                    }
                                    break;
                                case SpikeEffect.Bind:
                                    Destroy(enemy.GetComponent<Bind>());
                                    enemy.gameObject.AddComponent<Bind>();
                                    break;
                                case SpikeEffect.Topple:
                                    if (attackerStats != null)
                                    {
                                        enemy.ragdoll.SetState(Ragdoll.State.Destabilized);
                                    }
                                    break;
                                case SpikeEffect.Daze:
                                    if (attackerStats != null)
                                    {
                                        enemy.ragdoll.SetState(Ragdoll.State.Destabilized);
                                        attackerStats.isDazed = true;
                                    }
                                    break;
                                case SpikeEffect.Slow:
                                    if (attackerStats != null && enemy.brain.instance.GetModule<BrainModuleMelee>(false) is BrainModuleMelee melee)
                                    {
                                        Destroy(enemy.GetComponent<Slow>());
                                        enemy.gameObject.AddComponent<Slow>();
                                        enemy.GetComponent<Slow>().debuffPercent = debuffPercent;
                                    }
                                    break;
                                case SpikeEffect.Paralyze:
                                    if (attackerStats != null)
                                    {
                                        enemy.TryElectrocute(1, 5, true, false, Catalog.GetData<EffectData>("ImbueLightningRagdoll"));
                                    }
                                    break;
                                case SpikeEffect.InstantDeath:
                                    CollisionInstance instantDeath = new CollisionInstance(new DamageStruct(DamageType.Unknown, enemy.maxHealth));
                                    instantDeath.damageStruct.hitRagdollPart = enemy.ragdoll.rootPart;
                                    XenobladeManager.BypassXenobladeDamage(instantDeath, XenobladeDamageType.InstantDeath);
                                    enemy.Damage(instantDeath);
                                    break;
                            }
                            if (XenobladeManager.statModifiers.Any(match => match.handler == this) || attackerStats.statModifiers.Any(match => match.handler == this))
                            {
                                playerStatModifier = XenobladeManager.statModifiers.Find(match => match.handler == this);
                                creatureStatModifier = attackerStats.statModifiers.Find(match => match.handler == this);
                                Creature creature = enemy;
                                XenobladeEvents.InvokeOnDebuffAdded(ref creature, null, playerStatModifier, creatureStatModifier);
                            }
                        }
                    }
                }
            }
        }
    }
}
