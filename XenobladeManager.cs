using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;
using System.IO;

namespace XenobladeRPG
{
    public class XenobladeManager
    {
        protected static int PlayerLevel = 1;
        protected static int PlayerXP = 0;
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
        public static List<PlayerStatModifier> statModifiers = new List<PlayerStatModifier>();
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
        public static int GetLevel() => PlayerLevel;
        public static int GetCurrentXP() => PlayerXP;
        public static int GetXPNeeded() => XPNeeded;
        public static int GetStrength() => Strength;
        public static int GetEther() => Ether;
        public static int GetPhysicalDefense() => PhysicalDefense;
        public static int GetEtherDefense() => EtherDefense;
        public static float GetAgility() => Agility;
        public static float GetCriticalRate() => CriticalRate;
        public static float GetBlockRate() => BlockRate;
        public static float GetStrengthMultiplier() => strengthMultiplier;
        public static float GetEtherMultiplier() => etherMultiplier;
        public static float GetPhysicalDefenseMultiplier() => physicalDefenseMultiplier;
        public static float GetEtherDefenseMultiplier() => etherDefenseMultiplier;
        public static float GetAgilityMultiplier() => agilityMultiplier;
        public static float GetStrengthModifier() => strengthModifier;
        public static float GetEtherModifier() => etherModifier;
        public static float GetPhysicalDefenseModifier() => physicalDefenseModifier;
        public static float GetEtherDefenseModifier() => etherDefenseModifier;
        public static float GetCriticalRateModifier() => criticalRateModifier;
        public static float GetBlockRateModifier() => blockRateModifier;
        public static float GetAgilityModifier() => agilityModifier;
        public static void BypassXenobladeDamage(CollisionInstance collisionInstance, XenobladeDamageType damageType) => bypassedCollisions.Add(collisionInstance, damageType);
        public class PlayerStatModifier
        {
            public object handler;
            public float strengthMultiplier;
            public float etherMultiplier;
            public float physicalDefenseMultiplier;
            public float etherDefenseMultiplier;
            public float agilityMultiplier;
            public float strengthModifier;
            public float etherModifier;
            public float physicalDefenseModifier;
            public float etherDefenseModifier;
            public float agilityModifier;
            public float criticalRateModifier;
            public float blockRateModifier;
            public PlayerStatModifier(
              object handler,
              float strengthMultiplier,
              float etherMultiplier,
              float physicalDefenseMultiplier,
              float etherDefenseMultiplier,
              float agilityMultiplier,
              float strengthModifier,
              float etherModifier,
              float physicalDefenseModifier,
              float etherDefenseModifier, 
              float agilityModifier,
              float criticalRateModifier,
              float blockRateModifier)
            {
                this.handler = handler;
                this.strengthMultiplier = strengthMultiplier;
                this.etherMultiplier = etherMultiplier;
                this.physicalDefenseMultiplier = physicalDefenseMultiplier;
                this.etherDefenseMultiplier = etherDefenseMultiplier;
                this.agilityMultiplier = agilityMultiplier;
                this.strengthModifier = strengthModifier;
                this.etherModifier = etherModifier;
                this.physicalDefenseModifier = physicalDefenseModifier;
                this.etherDefenseModifier = etherDefenseModifier;
                this.agilityModifier = agilityModifier;
                this.criticalRateModifier = criticalRateModifier;
                this.blockRateModifier = blockRateModifier;
            }
        }
        public static void SetStatModifier(
           object handler,
           float strengthMultiplier = 1,
           float etherMultiplier = 1,
           float physicalDefenseMultiplier = 1,
           float etherDefenseMultiplier = 1,
           float agilityMultiplier = 1,
           float strengthModifier = 0,
           float etherModifier = 0,
           float physicalDefenseModifier = 0,
           float etherDefenseModifier = 0,
           float agilityModifier = 0,
           float criticalRateModifier = 0,
           float blockRateModifier = 0)
        {
            PlayerStatModifier statModifier = statModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.strengthMultiplier = strengthMultiplier;
                statModifier.etherMultiplier = etherMultiplier;
                statModifier.physicalDefenseMultiplier = physicalDefenseMultiplier;
                statModifier.etherDefenseMultiplier = etherDefenseMultiplier;
                statModifier.agilityMultiplier = agilityMultiplier;
                statModifier.strengthModifier = strengthModifier;
                statModifier.etherModifier = etherModifier;
                statModifier.physicalDefenseModifier = physicalDefenseModifier;
                statModifier.etherDefenseModifier = etherDefenseModifier;
                statModifier.agilityModifier = agilityModifier;
                statModifier.criticalRateModifier = criticalRateModifier;
                statModifier.blockRateModifier = blockRateModifier;
            }
            else
                statModifiers.Add(new PlayerStatModifier(handler, strengthMultiplier, etherMultiplier, physicalDefenseMultiplier, etherDefenseMultiplier, agilityMultiplier,
                    strengthModifier, etherModifier, physicalDefenseModifier, etherDefenseModifier, agilityModifier, criticalRateModifier, blockRateModifier));
            RefreshStatModifiers();
        }
        public static void RefreshStatModifiers()
        {
            if (statModifiers.Count == 0)
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
                float num2 = 1f;
                float num3 = 1f;
                float num4 = 1f;
                float num5 = 0f;
                float num6 = 0f;
                float num7 = 0f;
                float num8 = 0f;
                float num9 = 0f;
                float num10 = 0f;
                float num11 = 1f;
                float num12 = 0f;
                foreach (PlayerStatModifier statModifier in statModifiers)
                {
                    num1 *= statModifier.strengthMultiplier;
                    num2 *= statModifier.etherMultiplier;
                    num3 *= statModifier.physicalDefenseMultiplier;
                    num4 *= statModifier.etherDefenseMultiplier;
                    num5 += statModifier.strengthModifier;
                    num6 += statModifier.etherModifier;
                    num7 += statModifier.physicalDefenseModifier;
                    num8 += statModifier.etherDefenseModifier;
                    num9 += statModifier.criticalRateModifier;
                    num10 += statModifier.blockRateModifier;
                    num11 *= statModifier.agilityMultiplier;
                    num12 += statModifier.agilityModifier;
                }
                strengthMultiplier = num1;
                etherMultiplier = num2;
                physicalDefenseMultiplier = num3;
                etherDefenseMultiplier = num4;
                strengthModifier = num5;
                etherModifier = num6;
                physicalDefenseModifier = num7;
                etherDefenseModifier = num8;
                criticalRateModifier = num9;
                blockRateModifier = num10;
                agilityMultiplier = num11;
                agilityModifier = num12;
            }
            Strength = Mathf.RoundToInt((float)((BaseStrength + strengthModifier) * strengthMultiplier));
            Ether = Mathf.RoundToInt((float)((BaseEther + etherModifier) * etherMultiplier));
            PhysicalDefense = Mathf.RoundToInt((float)(physicalDefenseModifier * physicalDefenseMultiplier));
            EtherDefense = Mathf.RoundToInt((float)(etherDefenseModifier * etherDefenseMultiplier));
            Agility = Mathf.RoundToInt((float)((BaseAgility + agilityModifier) * agilityMultiplier));
            CriticalRate = BaseCriticalRate + criticalRateModifier;
            BlockRate = BaseBlockRate + blockRateModifier;
        }
        public static void RemoveStatModifier(object handler)
        {
            for (int index = 0; index < statModifiers.Count; ++index)
            {
                if (statModifiers[index].handler == handler)
                    statModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public static void ClearStatModifiers()
        {
            statModifiers.Clear();
            RefreshStatModifiers();
        }
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
        public static void LoadFromSave()
        {
            if (File.Exists(DataManager.GetLocalSavePath() + Player.characterData.ID + ".xcrpg_save"))
            {
                XenobladeValues xenobladeValues = JsonConvert.DeserializeObject<XenobladeValues>(File.ReadAllText(DataManager.GetLocalSavePath() + Player.characterData.ID + ".xcrpg_save"));
                PlayerLevel = Mathf.Clamp(xenobladeValues.PlayerLevel, 1, 99);
                PlayerXP = Mathf.Max(xenobladeValues.PlayerXP, 0);
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
        public static void SaveToJSON()
        {
            XenobladeValues xenobladeValues = new XenobladeValues()
            {
                PlayerLevel = PlayerLevel,
                PlayerXP = PlayerXP,
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

