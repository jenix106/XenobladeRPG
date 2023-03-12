using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;

namespace XenobladeRPG
{
    /// <summary>
    /// Controls the Player's stats and parameters, as well as tracking important values.
    /// </summary>
    public class XenobladeManager
    {
        protected static int PlayerLevel = 1;
        protected static int PlayerXP = 0;
        protected static int PlayerAP = 0;
        protected static int PlayerSP = 0;
        protected static int XPNeeded = 0;
        protected static int Strength = 0;
        protected static int BaseStrength = 0;
        protected static int PhysicalDefense = 0;
        protected static int Ether = 0;
        protected static int BaseEther = 0;
        protected static int EtherDefense = 0;
        protected static float Agility = 0;
        protected static float BaseAgility = 0;
        public static int StrengthHits = 1;
        public static int EtherHits = 1;
        protected static float CriticalRate = 0f;
        protected static float BaseCriticalRate = 0;
        protected static float BlockRate = 0f;
        protected static float BaseBlockRate = 0;
        public static List<StrengthModifier> strengthModifiers = new List<StrengthModifier>();
        public static List<EtherModifier> etherModifiers = new List<EtherModifier>();
        public static List<PhysicalDefenseModifier> physicalDefenseModifiers = new List<PhysicalDefenseModifier>();
        public static List<EtherDefenseModifier> etherDefenseModifiers = new List<EtherDefenseModifier>();
        public static List<AgilityModifier> agilityModifiers = new List<AgilityModifier>();
        public static List<CriticalRateModifier> criticalRateModifiers = new List<CriticalRateModifier>();
        public static List<BlockRateModifier> blockRateModifiers = new List<BlockRateModifier>();
        protected static float strengthMultiplier = 1;
        protected static float strengthModifier = 0;
        protected static float etherMultiplier = 1;
        protected static float etherModifier = 0;
        protected static float physicalDefenseMultiplier = 1;
        protected static float physicalDefenseModifier = 0;
        protected static float etherDefenseMultiplier = 1;
        protected static float etherDefenseModifier = 0;
        protected static float agilityMultiplier = 1;
        protected static float agilityModifier = 0;
        protected static float criticalRateModifier = 0;
        protected static float blockRateModifier = 0;
        public static bool isAuraSealed = false;
        public static Dictionary<CollisionInstance, XenobladeDamageType> bypassedCollisions = new Dictionary<CollisionInstance, XenobladeDamageType>();
        public static Dictionary<CollisionInstance, XenobladeDamageType> recordedCollisions = new Dictionary<CollisionInstance, XenobladeDamageType>();
        public static List<XenobladeDamage> xenobladeDamages = new List<XenobladeDamage>();
        public static int GetLevel() => PlayerLevel;
        /// <summary>
        /// Retrieves current Experience Points. Used when levelling up.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentXP() => PlayerXP;
        /// <summary>
        /// Retrieves current Art Points. Used when upgrading Arts.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentAP() => PlayerAP;
        /// <summary>
        /// Retrieves current Skill Points. Used when acquiring skills.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentSP() => PlayerSP;
        /// <summary>
        /// Retrieves how many Experience Points are required to level up.
        /// </summary>
        /// <returns></returns>
        public static int GetXPNeeded() => XPNeeded;
        /// <summary>
        /// Retrieves total Strength, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static int GetStrength() => Strength;
        /// <summary>
        /// Retrieves total Ether, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static int GetEther() => Ether;
        /// <summary>
        /// Retrieves total Physical Defense, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static int GetPhysicalDefense() => PhysicalDefense;
        /// <summary>
        /// Retrieves total Ether Defense, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static int GetEtherDefense() => EtherDefense;
        /// <summary>
        /// Retrieves total Agility, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static float GetAgility() => Agility;
        /// <summary>
        /// Retrieves total Critical Rate, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static float GetCriticalRate() => CriticalRate;
        /// <summary>
        /// Retrieves total Block Rate, including buffs/debuffs and equipment.
        /// </summary>
        /// <returns></returns>
        public static float GetBlockRate() => BlockRate;
        /// <summary>
        /// Retrieves the overall Strength multiplier.
        /// </summary>
        /// <returns></returns>
        public static float GetStrengthMultiplier() => strengthMultiplier;
        /// <summary>
        /// Retrieves the overall Ether multiplier.
        /// </summary>
        /// <returns></returns>
        public static float GetEtherMultiplier() => etherMultiplier;
        /// <summary>
        /// Retrieves the overall Physical Defense multiplier.
        /// </summary>
        /// <returns></returns>
        public static float GetPhysicalDefenseMultiplier() => physicalDefenseMultiplier;
        /// <summary>
        /// Retrieves the overall Ether Defense multiplier.
        /// </summary>
        /// <returns></returns>
        public static float GetEtherDefenseMultiplier() => etherDefenseMultiplier;
        /// <summary>
        /// Retrieves the overall Agility multiplier.
        /// </summary>
        /// <returns></returns>
        public static float GetAgilityMultiplier() => agilityMultiplier;
        /// <summary>
        /// Retrieves the sum of all Strength additions.
        /// </summary>
        /// <returns></returns>
        public static float GetStrengthModifier() => strengthModifier;
        /// <summary>
        /// Retrieves the sum of all Ether additions.
        /// </summary>
        /// <returns></returns>
        public static float GetEtherModifier() => etherModifier;
        /// <summary>
        /// Retrieves the sum of all Physical Defense additions.
        /// </summary>
        /// <returns></returns>
        public static float GetPhysicalDefenseModifier() => physicalDefenseModifier;
        /// <summary>
        /// Retrieves the sum of all Ether Defense additions.
        /// </summary>
        /// <returns></returns>
        public static float GetEtherDefenseModifier() => etherDefenseModifier;
        /// <summary>
        /// Retrieves the sum of all Critical Rate additions.
        /// </summary>
        /// <returns></returns>
        public static float GetCriticalRateModifier() => criticalRateModifier;
        /// <summary>
        /// Retrieves the sum of all Block Rate additions.
        /// </summary>
        /// <returns></returns>
        public static float GetBlockRateModifier() => blockRateModifier;
        /// <summary>
        /// Retrieves the sum of all Agility additions.
        /// </summary>
        /// <returns></returns>
        public static float GetAgilityModifier() => agilityModifier;
        /// <summary>
        /// Bypass Xenoblade damage rules and calculations for a specific <see cref="CollisionInstance"/>.
        /// </summary>
        /// <param name="collisionInstance"></param>
        /// <param name="damageType"></param>
        public static void BypassXenobladeDamage(CollisionInstance collisionInstance, XenobladeDamageType damageType) => bypassedCollisions.Add(collisionInstance, damageType);
        /// <summary>
        /// Heals the creature while following Xenoblade rules. Does not trigger <see cref="Creature.OnHealEvent"/>.
        /// </summary>
        /// <param name="healed"></param>
        /// <param name="healer"></param>
        /// <param name="healMultiplier"></param>
        public static void Heal(Creature healed, Creature healer, float healMultiplier)
        {
            float amount = (healer.player != null ? GetEther() : healer.GetComponent<XenobladeStats>().GetEther()) * healMultiplier;
            healed.currentHealth += Mathf.Max(amount, 0);
            healed.currentHealth = Mathf.Min(healed.currentHealth, healed.maxHealth);
            EventManager.InvokeCreatureHeal(healed, amount, healer); 
            GameObject healObject = Object.Instantiate(XenobladeLevelModule.heal);
            XenobladeHeal xenobladeHeal = healObject.AddComponent<XenobladeHeal>();
            xenobladeHeal.creature = healed;
            xenobladeHeal.amount = amount;
        }
        /// <summary>
        /// <see cref="Creature.Damage(CollisionInstance)">Damages</see> the creature while following Xenoblade rules. Eases damage calculations by providing variables beforehand.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="collisionInstance"></param>
        /// <param name="damageType"></param>
        /// <param name="baseDamageMultiplier"></param>
        /// <param name="additionalDamageDirection"></param>
        /// <param name="directionalDamageMultiplier"></param>
        public static void Damage(Creature attacker, Creature defender, CollisionInstance collisionInstance, XenobladeDamageType damageType, float baseDamageMultiplier = 1, DefenseDirection additionalDamageDirection = DefenseDirection.None, float directionalDamageMultiplier = 1)
        {
            xenobladeDamages.Add(new XenobladeDamage
            {
                attacker = attacker,
                defender = defender,
                collisionInstance = collisionInstance,
                damageType = damageType,
                baseDamageMultiplier = baseDamageMultiplier,
                additionalDamageDirection = additionalDamageDirection,
                directionalDamageMultiplier = directionalDamageMultiplier
            });
            defender.Damage(collisionInstance);
        }
        public class XenobladeDamage
        {
            public Creature attacker;
            public Creature defender;
            public CollisionInstance collisionInstance;
            public XenobladeDamageType damageType;
            public float baseDamageMultiplier;
            public DefenseDirection additionalDamageDirection;
            public float directionalDamageMultiplier;
        }
        public class StrengthModifier
        {
            public object handler;
            public float multiplier;
            public float modifier;
            public StrengthModifier(
                object handler,
                float multiplier = 1,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
                this.modifier = modifier;
            }
        }
        public class EtherModifier
        {
            public object handler;
            public float multiplier;
            public float modifier;
            public EtherModifier(
                object handler,
                float multiplier = 1,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
                this.modifier = modifier;
            }
        }
        public class PhysicalDefenseModifier
        {
            public object handler;
            public float multiplier;
            public float modifier;
            public PhysicalDefenseModifier(
                object handler,
                float multiplier = 1,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
                this.modifier = modifier;
            }
        }
        public class EtherDefenseModifier
        {
            public object handler;
            public float multiplier;
            public float modifier;
            public EtherDefenseModifier(
                object handler,
                float multiplier = 1,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
                this.modifier = modifier;
            }
        }
        public class AgilityModifier
        {
            public object handler;
            public float multiplier;
            public float modifier;
            public AgilityModifier(
                object handler,
                float multiplier = 1,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
                this.modifier = modifier;
            }
        }
        public class CriticalRateModifier
        {
            public object handler;
            public float modifier;
            public CriticalRateModifier(
                object handler,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.modifier = modifier;
            }
        }
        public class BlockRateModifier
        {
            public object handler;
            public float modifier;
            public BlockRateModifier(
                object handler,
                float modifier = 1
                )
            {
                this.handler = handler;
                this.modifier = modifier;
            }
        }
        /// <summary>
        /// Adds a Strength Modifier, which consists of a multiplier (multiplication) and a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="multiplier"></param>
        /// <param name="modifier"></param>
        public static void SetStrengthModifier(object handler, float multiplier = 1, float modifier = 0)
        {
            StrengthModifier statModifier = strengthModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
                statModifier.modifier = modifier;
            }
            else strengthModifiers.Add(new StrengthModifier(handler, multiplier, modifier));
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds an Ether Modifier, which consists of a multiplier (multiplication) and a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="multiplier"></param>
        /// <param name="modifier"></param>
        public static void SetEtherModifier(object handler, float multiplier = 1, float modifier = 0)
        {
            EtherModifier statModifier = etherModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
                statModifier.modifier = modifier;
            }
            else etherModifiers.Add(new EtherModifier(handler, multiplier, modifier));
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds a Physical Defense Modifier, which consists of a multiplier (multiplication) and a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="multiplier"></param>
        /// <param name="modifier"></param>
        public static void SetPhysicalDefenseModifier(object handler, float multiplier = 1, float modifier = 0)
        {
            PhysicalDefenseModifier statModifier = physicalDefenseModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
                statModifier.modifier = modifier;
            }
            else physicalDefenseModifiers.Add(new PhysicalDefenseModifier(handler, multiplier, modifier));
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds an Ether Defense Modifier, which consists of a multiplier (multiplication) and a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="multiplier"></param>
        /// <param name="modifier"></param>
        public static void SetEtherDefenseModifier(object handler, float multiplier = 1, float modifier = 0)
        {
            EtherDefenseModifier statModifier = etherDefenseModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
                statModifier.modifier = modifier;
            }
            else etherDefenseModifiers.Add(new EtherDefenseModifier(handler, multiplier, modifier));
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds an Agility Modifier, which consists of a multiplier (multiplication) and a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="multiplier"></param>
        /// <param name="modifier"></param>
        public static void SetAgilityModifier(object handler, float multiplier = 1, float modifier = 0)
        {
            AgilityModifier statModifier = agilityModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
                statModifier.modifier = modifier;
            }
            else agilityModifiers.Add(new AgilityModifier(handler, multiplier, modifier));
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds a Critical Rate Modifier, which consists of a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="modifier"></param>
        public static void SetCriticalRateModifier(object handler, float modifier = 0)
        {
            CriticalRateModifier statModifier = criticalRateModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.modifier = modifier;
            }
            else criticalRateModifiers.Add(new CriticalRateModifier(handler, modifier));
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds a Block Rate Modifier, which consists of a modifier (addition).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="modifier"></param>
        public static void SetBlockRateModifier(object handler, float modifier = 0)
        {
            BlockRateModifier statModifier = blockRateModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.modifier = modifier;
            }
            else blockRateModifiers.Add(new BlockRateModifier(handler, modifier));
            RefreshStatModifiers();
        }
        public static void RemoveStrengthModifier(object handler)
        {
            for (int index = 0; index < strengthModifiers.Count; ++index)
            {
                if (strengthModifiers[index].handler == handler)
                    strengthModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void RemoveEtherModifier(object handler)
        {
            for (int index = 0; index < etherModifiers.Count; ++index)
            {
                if (etherModifiers[index].handler == handler)
                    etherModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void RemovePhysicalDefenseModifier(object handler)
        {
            for (int index = 0; index < physicalDefenseModifiers.Count; ++index)
            {
                if (physicalDefenseModifiers[index].handler == handler)
                    physicalDefenseModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void RemoveEtherDefenseModifier(object handler)
        {
            for (int index = 0; index < etherDefenseModifiers.Count; ++index)
            {
                if (etherDefenseModifiers[index].handler == handler)
                    etherDefenseModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void RemoveAgilityModifier(object handler)
        {
            for (int index = 0; index < agilityModifiers.Count; ++index)
            {
                if (agilityModifiers[index].handler == handler)
                    agilityModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void RemoveCriticalRateModifier(object handler)
        {
            for (int index = 0; index < criticalRateModifiers.Count; ++index)
            {
                if (criticalRateModifiers[index].handler == handler)
                    criticalRateModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void RemoveBlockRateModifier(object handler)
        {
            for (int index = 0; index < blockRateModifiers.Count; ++index)
            {
                if (blockRateModifiers[index].handler == handler)
                    blockRateModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Strength Modifiers.
        /// </summary>
        public static void ClearStrengthModifiers()
        {
            strengthModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Ether Modifiers.
        /// </summary>
        public static void ClearEtherModifiers()
        {
            etherModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Physical Defense Modifiers.
        /// </summary>
        public static void ClearPhysicalDefenseModifiers()
        {
            physicalDefenseModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Ether Defense Modifiers.
        /// </summary>
        public static void ClearEtherDefenseModifiers()
        {
            etherDefenseModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Agility Modifiers.
        /// </summary>
        public static void ClearAgilityModifiers()
        {
            agilityModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Critical Rate Modifiers.
        /// </summary>
        public static void ClearCriticalRateModifiers()
        {
            criticalRateModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Removes all Block Rate Modifiers.
        /// </summary>
        public static void ClearBlockRateModifiers()
        {
            blockRateModifiers.Clear();
            RefreshStatModifiers();
        }
        public static void RefreshStatModifiers()
        {
            if (strengthModifiers.Count + etherModifiers.Count + physicalDefenseModifiers.Count + etherDefenseModifiers.Count + agilityModifiers.Count + criticalRateModifiers.Count + blockRateModifiers.Count == 0) 
            {
                strengthMultiplier = 1f;
                etherMultiplier = 1f;
                physicalDefenseMultiplier = 1f;
                etherDefenseMultiplier = 1f;
                agilityMultiplier = 1f;
                strengthModifier = 0;
                etherModifier = 0;
                physicalDefenseModifier = 0;
                etherDefenseModifier = 0;
                agilityModifier = 0f;
                criticalRateModifier = 0f;
                blockRateModifier = 0f;
            }
            else 
            {
                float num1 = 1f;
                float num2 = 0f;
                float num3 = 1f;
                float num4 = 0f;
                float num5 = 1f;
                float num6 = 0f;
                float num7 = 1f;
                float num8 = 0f;
                float num9 = 1f;
                float num10 = 0f;
                float num11 = 0f;
                float num12 = 0f;
                foreach (StrengthModifier modifier in strengthModifiers)
                {
                    num1 *= modifier.multiplier;
                    num2 += modifier.modifier;
                }
                foreach (EtherModifier modifier in etherModifiers)
                {
                    num3 *= modifier.multiplier;
                    num4 += modifier.modifier;
                }
                foreach (PhysicalDefenseModifier modifier in physicalDefenseModifiers)
                {
                    num5 *= modifier.multiplier;
                    num6 += modifier.modifier;
                }
                foreach (EtherDefenseModifier modifier in etherDefenseModifiers)
                {
                    num7 *= modifier.multiplier;
                    num8 += modifier.modifier;
                }
                foreach (AgilityModifier modifier in agilityModifiers)
                {
                    num9 *= modifier.multiplier;
                    num10 += modifier.modifier;
                }
                foreach (CriticalRateModifier modifier in criticalRateModifiers)
                {
                    num11 += modifier.modifier;
                }
                foreach (BlockRateModifier modifier in blockRateModifiers)
                {
                    num12 += modifier.modifier;
                }
                strengthMultiplier = num1;
                strengthModifier = num2;
                etherMultiplier = num3;
                etherModifier = num4;
                physicalDefenseMultiplier = num5;
                physicalDefenseModifier = num6;
                etherDefenseMultiplier = num7;
                etherDefenseModifier = num8;
                agilityMultiplier = num9;
                agilityModifier = num10;
                criticalRateModifier = num11;
                blockRateModifier = num12;
            }
            Strength = Mathf.RoundToInt((float)((BaseStrength + strengthModifier) * strengthMultiplier));
            Ether = Mathf.RoundToInt((float)((BaseEther + etherModifier) * etherMultiplier));
            PhysicalDefense = Mathf.RoundToInt((float)(physicalDefenseModifier * physicalDefenseMultiplier));
            EtherDefense = Mathf.RoundToInt((float)(etherDefenseModifier * etherDefenseMultiplier));
            Agility = Mathf.RoundToInt((float)((BaseAgility + agilityModifier) * agilityMultiplier));
            CriticalRate = BaseCriticalRate + criticalRateModifier;
            BlockRate = BaseBlockRate + blockRateModifier;
        }
        /// <summary>
        /// Removes all stat modifiers.
        /// </summary>
        public static void ClearStatModifiers()
        {
            strengthModifiers.Clear();
            etherModifiers.Clear();
            physicalDefenseModifiers.Clear();
            etherDefenseModifiers.Clear();
            agilityModifiers.Clear();
            criticalRateModifiers.Clear();
            blockRateModifiers.Clear();
            RefreshStatModifiers();
        }
        /// <summary>
        /// Adds Experience Points.
        /// </summary>
        /// <param name="xp"></param>
        public static void AddXP(int xp)
        {
            PlayerXP += xp;
            if (PlayerXP >= XPNeeded && PlayerLevel < 99)
            {
                while (PlayerXP >= XPNeeded && PlayerLevel < 99)
                {
                    LevelUp();
                }
            }
            else SaveToJSON();
        }
        /// <summary>
        /// Adds Art Points.
        /// </summary>
        /// <param name="ap"></param>
        public static void AddAP(int ap)
        {
            PlayerAP += ap;
            if (PlayerAP > 999999) PlayerAP = 999999;
            SaveToJSON();
        }
        /// <summary>
        /// Adds Skill Points.
        /// </summary>
        /// <param name="sp"></param>
        public static void AddSP(int sp)
        {
            PlayerSP += sp;
            SaveToJSON();
        }
        /// <summary>
        /// Removes Art Points.
        /// </summary>
        /// <param name="ap"></param>
        public static void RemoveAP(int ap)
        {
            PlayerAP -= ap;
            if (PlayerAP < 0) PlayerAP = 0;
            SaveToJSON();
        }
        /// <summary>
        /// Removes Skill Points.
        /// </summary>
        /// <param name="sp"></param>
        public static void RemoveSP(int sp)
        {
            PlayerSP -= sp;
            if (PlayerSP < 0) PlayerSP = 0;
            SaveToJSON();
        }
        private static void LevelUp()
        {
            PlayerXP -= XPNeeded;
            PlayerLevel++;
            BaseStrength += Mathf.RoundToInt((float)((float)StrengthHits / (float)(StrengthHits + EtherHits)) * 10);
            BaseEther += Mathf.RoundToInt((float)((float)EtherHits / (float)(StrengthHits + EtherHits)) * 10);
            BaseAgility += 1.5f;
            StrengthHits = 1;
            EtherHits = 1;
            float healthRatio = Player.local.creature.currentHealth / Player.local.creature.maxHealth;
            Player.local.creature.maxHealth += 50;
            Player.local.creature.currentHealth = Player.local.creature.maxHealth * healthRatio;
            foreach (Creature creatureStats in Creature.allActive)
            {
                if (creatureStats.GetComponent<XenobladeStats>() is XenobladeStats stats)
                {
                    stats.RefreshStatModifiers();
                    stats.RefreshLevelModifiers();
                }
            }
            foreach (Item itemStats in Item.allActive)
            {
                if(itemStats.data.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule stats)
                {
                    stats.RefreshLevel();
                }
            }
            float xpNeeded = 40;
            for (int i = 1; i <= PlayerLevel; i++)
            {
                if (i < 7) xpNeeded += 20;
                xpNeeded *= 1.11f;
            }
            XPNeeded = Mathf.RoundToInt(xpNeeded * 2);
            RefreshStatModifiers();
            SaveToJSON();
        }
        /// <summary>
        /// Loads all base stats and parameters from a character's save file. If none exists, it generates a new one.
        /// </summary>
        public static void LoadFromSave()
        {
            if (File.Exists(DataManager.GetLocalSavePath() + Player.characterData.ID + ".xcrpg_save"))
            {
                XenobladeValues xenobladeValues = JsonConvert.DeserializeObject<XenobladeValues>(File.ReadAllText(DataManager.GetLocalSavePath() + Player.characterData.ID + ".xcrpg_save"));
                PlayerLevel = Mathf.Clamp(xenobladeValues.PlayerLevel, 1, 99);
                PlayerXP = Mathf.Max(xenobladeValues.PlayerXP, 0);
                PlayerAP = Mathf.Clamp(xenobladeValues.PlayerAP, 0, 999999);
                PlayerSP = Mathf.Max(xenobladeValues.PlayerSP, 0);
                BaseStrength = Mathf.Max(xenobladeValues.Strength, 0);
                BaseEther = Mathf.Max(xenobladeValues.Ether, 0);
                BaseAgility = Mathf.Max(xenobladeValues.Agility, 0);
                StrengthHits = Mathf.Max(xenobladeValues.StrengthHits, 1);
                EtherHits = Mathf.Max(xenobladeValues.EtherHits, 1);
                BaseCriticalRate = Mathf.Clamp(xenobladeValues.CriticalRate, 0, 1);
                BaseBlockRate = Mathf.Clamp(xenobladeValues.BlockRate, 0, 1);
                Debug.Log("Loaded Xenoblade RPG save: " + Player.characterData.ID);
            }
            else
            {
                Debug.Log("Xenoblade RPG save not found for this character. Generating new save: " + Player.characterData.ID);
                PlayerLevel = 1;
                PlayerXP = 0;
                PlayerAP = 0;
                PlayerSP = 0;
                BaseStrength = 0;
                BaseEther = 0;
                BaseAgility = 0;
                StrengthHits = 1;
                EtherHits = 1;
                BaseCriticalRate = 0;
                BaseBlockRate = 0;
                SaveToJSON();
            }
            float xpNeeded = 40;
            for (int i = 1; i <= PlayerLevel; i++)
            {
                if (i < 7) xpNeeded += 20;
                xpNeeded *= 1.11f;
            }
            XPNeeded = (int)xpNeeded * 2;
            RefreshStatModifiers();
        }
        /// <summary>
        /// Saves all base stats and parameters to a character's save file. If none exists, it generates a new one.
        /// </summary>
        public static void SaveToJSON()
        {
            XenobladeValues xenobladeValues = new XenobladeValues()
            {
                PlayerLevel = PlayerLevel,
                PlayerXP = PlayerXP,
                PlayerAP = PlayerAP,
                PlayerSP = PlayerSP,
                Strength = BaseStrength,
                Ether = BaseEther,
                Agility = BaseAgility,
                StrengthHits = StrengthHits,
                EtherHits = EtherHits,
                CriticalRate = BaseCriticalRate,
                BlockRate = BaseBlockRate
            };
            string contents = JsonConvert.SerializeObject(xenobladeValues, Formatting.Indented);
            File.WriteAllText(DataManager.GetLocalSavePath() + Player.characterData.ID + ".xcrpg_save", contents);
        }
    }
}

