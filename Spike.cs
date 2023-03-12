using UnityEngine;
using ThunderRoad;
using System.Collections.Generic;
using System.Linq;

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
        public float duration = 0;
        CollisionInstance collision;
        XenobladeStats attackerStats;
        XenobladeStats creatureStats;
        float time;
        Dictionary<Creature, float> debuffTimes = new Dictionary<Creature, float>();
        List<Creature> remove = new List<Creature>();
        public void Start()
        {
            creature = GetComponent<Creature>();
            creatureStats = creature.GetComponent<XenobladeStats>();
            creature.OnKillEvent += Creature_OnKillEvent;
            XenobladeEvents.onXenobladeDamage += XenobladeEvents_onXenobladeDamage;
            collision = new CollisionInstance(new DamageStruct(DamageType.Unknown, damage));
            time = Time.time;
        }

        private void XenobladeEvents_onXenobladeDamage(ref CollisionInstance collisionInstance, ref Creature attacker, ref Creature defender, ref XenobladeDamageType damageType, EventTime eventTime)
        {
            if(defender == creature && !attacker.isKilled && attacker != creature && eventTime == EventTime.OnEnd && (damageType == XenobladeDamageType.Physical || damageType == XenobladeDamageType.Ether) && (creatureStats != null ? !creatureStats.isAuraSealed : !XenobladeManager.isAuraSealed))
            {
                attackerStats = attacker.GetComponent<XenobladeStats>();
                collision.damageStruct.hitRagdollPart = attacker.ragdoll.rootPart;
                if (Vector3.Distance(creature.transform.position, attacker.transform.position) <= distance && attacker.faction != creature.faction && collisionInstance.sourceColliderGroup != collisionInstance.targetColliderGroup)
                {
                    debuffTimes.Remove(attacker);
                    debuffTimes.Add(attacker, Time.time);
                    if (collisionInstance.damageStruct.damage > 0 && ((type == SpikeType.Counter && creature.state == Creature.State.Alive) || (type == SpikeType.Topple && creature.state == Creature.State.Destabilized)))
                        XenobladeManager.Damage(creature, attacker, collision, XenobladeDamageType.Spike);
                    if (effect != SpikeEffect.None)
                    {
                        switch (effect)
                        {
                            case SpikeEffect.StrengthDown:
                                if (attackerStats == null) XenobladeManager.SetStrengthModifier(this, 1 - debuffPercent);
                                else attackerStats.SetStrengthModifier(this, 1 - debuffPercent);
                                XenobladeEvents.InvokeOnDebuffAdded(this, attacker);
                                break;
                            case SpikeEffect.EtherDown:
                                if (attackerStats == null) XenobladeManager.SetEtherModifier(this, 1 - debuffPercent);
                                else attackerStats.SetEtherModifier(this, 1 - debuffPercent);
                                XenobladeEvents.InvokeOnDebuffAdded(this, attacker);
                                break;
                            case SpikeEffect.PhysicalDefenseDown:
                                if (attackerStats == null) XenobladeManager.SetPhysicalDefenseModifier(this, 1 - debuffPercent);
                                else attackerStats.SetPhysicalDefenseModifier(this, -debuffPercent);
                                XenobladeEvents.InvokeOnDebuffAdded(this, attacker);
                                break;
                            case SpikeEffect.EtherDefenseDown:
                                if (attackerStats == null) XenobladeManager.SetEtherDefenseModifier(this, 1 - debuffPercent);
                                else attackerStats.SetEtherDefenseModifier(this, -debuffPercent);
                                XenobladeEvents.InvokeOnDebuffAdded(this, attacker);
                                break;
                            case SpikeEffect.AgilityDown:
                                if (attackerStats == null) XenobladeManager.SetAgilityModifier(this, 1 - debuffPercent);
                                else attackerStats.SetAgilityModifier(this, 1 - debuffPercent);
                                XenobladeEvents.InvokeOnDebuffAdded(this, attacker);
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
                                attacker.gameObject.AddComponent<Sleep>();
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
                                XenobladeManager.Damage(creature, attacker, instantDeath, XenobladeDamageType.InstantDeath);
                                break;
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

        public void Setup(SpikeType spikeType, SpikeEffect spikeEffect, float spikeDamage, float spikeDebuffPercent, float spikeDistance, float spikeDuration)
        {
            type = spikeType;
            effect = spikeEffect;
            damage = spikeDamage;
            debuffPercent = spikeDebuffPercent;
            distance = spikeDistance;
            duration = spikeDuration;
        }
        public void FixedUpdate()
        {
            foreach(Creature creature in debuffTimes.Keys)
            {
                if(Time.time - duration >= debuffTimes[creature])
                {
                    if(creature.player != null)
                    {
                        XenobladeManager.RemoveStrengthModifier(this);
                        XenobladeManager.RemoveEtherModifier(this);
                        XenobladeManager.RemovePhysicalDefenseModifier(this);
                        XenobladeManager.RemoveEtherDefenseModifier(this);
                        XenobladeManager.RemoveAgilityModifier(this);
                    }
                    else if (creature.GetComponent<XenobladeStats>() is XenobladeStats xenobladeStats)
                    {
                        xenobladeStats.RemoveStrengthModifier(this);
                        xenobladeStats.RemoveEtherModifier(this);
                        xenobladeStats.RemovePhysicalDefenseModifier(this);
                        xenobladeStats.RemoveEtherDefenseModifier(this);
                        xenobladeStats.RemoveAgilityModifier(this);
                    }
                    remove.Add(creature);
                }
            }
            foreach(Creature creature in remove)
            {
                debuffTimes.Remove(creature);
            }
            remove.Clear();
            if (Time.time - 5 >= time && type == SpikeType.CloseRange && (creatureStats != null ? !creatureStats.isAuraSealed : !XenobladeManager.isAuraSealed))
            {
                time = Time.time;
                foreach (Creature enemy in Creature.allActive)
                {
                    if (!enemy.isKilled && enemy != creature && enemy.faction != creature.faction && Vector3.Distance(creature.transform.position, enemy.transform.position) <= distance)
                    {
                        debuffTimes.Remove(enemy);
                        debuffTimes.Add(enemy, Time.time);
                        attackerStats = enemy.GetComponent<XenobladeStats>();
                        if (damage > 0)
                        {
                            collision.damageStruct.hitRagdollPart = enemy.ragdoll.rootPart;
                            XenobladeManager.Damage(creature, enemy, collision, XenobladeDamageType.Spike);
                        }
                        if (effect != SpikeEffect.None)
                        {
                            switch (effect)
                            {
                                case SpikeEffect.StrengthDown:
                                    if (attackerStats == null) XenobladeManager.SetStrengthModifier(this, 1 - debuffPercent);
                                    else attackerStats.SetStrengthModifier(this, 1 - debuffPercent);
                                    XenobladeEvents.InvokeOnDebuffAdded(this, enemy);
                                    break;
                                case SpikeEffect.EtherDown:
                                    if (attackerStats == null) XenobladeManager.SetEtherModifier(this, 1 - debuffPercent);
                                    else attackerStats.SetEtherModifier(this, 1 - debuffPercent);
                                    XenobladeEvents.InvokeOnDebuffAdded(this, enemy);
                                    break;
                                case SpikeEffect.PhysicalDefenseDown:
                                    if (attackerStats == null) XenobladeManager.SetPhysicalDefenseModifier(this, 1 - debuffPercent);
                                    else attackerStats.SetPhysicalDefenseModifier(this, -debuffPercent);
                                    XenobladeEvents.InvokeOnDebuffAdded(this, enemy);
                                    break;
                                case SpikeEffect.EtherDefenseDown:
                                    if (attackerStats == null) XenobladeManager.SetEtherDefenseModifier(this, 1 - debuffPercent);
                                    else attackerStats.SetEtherDefenseModifier(this, -debuffPercent);
                                    XenobladeEvents.InvokeOnDebuffAdded(this, enemy);
                                    break;
                                case SpikeEffect.AgilityDown:
                                    if (attackerStats == null) XenobladeManager.SetAgilityModifier(this, 1 - debuffPercent);
                                    else attackerStats.SetAgilityModifier(this, 1 - debuffPercent);
                                    XenobladeEvents.InvokeOnDebuffAdded(this, enemy);
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
                                    enemy.gameObject.AddComponent<Bind>().duration = duration;
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
                                        enemy.gameObject.AddComponent<Slow>().duration = duration;
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
                                    XenobladeManager.Damage(creature, enemy, instantDeath, XenobladeDamageType.InstantDeath);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
