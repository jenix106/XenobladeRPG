using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;

namespace XenobladeRPG
{
    public class XenobladeRemoveBar : MonoBehaviour
    {
        Creature creature;
        public GameObject healthBar;
        Image health;
        Text creatureName;
        Text level;
        RectTransform sight;
        RectTransform hearing;
        RectTransform passive;
        RectTransform unique;
        Image color;
        Image breakStatus;
        Image toppleStatus;
        Image dazeStatus;
        Image sleepStatus;
        Image paralyze;
        Image artsSeal;
        Image bind;
        Image poison;
        Image burn;
        Image bleed;
        Image auraSeal;
        Image freeze;
        Image lockOn;
        Image confuse;
        Image pierce;
        Image strengthDown;
        Image strengthUp;
        Image etherDown;
        Image etherUp;
        Image physicalDefenseDown;
        Image physicalDefenseUp;
        Image etherDefenseDown;
        Image etherDefenseUp;
        Image monadoArmor;
        Image monadoShield;
        Image monadoEnchant;
        Image agilityUp;
        Image agilityDown;
        Image haste;
        Image slow;
        Image regen;
        Image damageHeal;
        Image debuffResist;
        Image reflect;
        Image damageImmunity;
        Image physicalArtsPlus;
        Image healthDown;
        Text points;
        Text pointsBlack;
        BrainModuleDetection detection;
        BrainModuleMelee melee;
        XenobladeStats stats;
        bool isParalyzed;
        bool isArtsSealed;
        bool isAuraSealed;
        bool isBinded;
        bool isPoisoned;
        bool isBurning;
        bool isBleeding;
        bool isShielded;
        bool isArmored;
        bool isEnchanted;
        bool isFreezing;
        bool isLockedOn;
        bool isConfused;
        bool isPierced;
        bool isRegen;
        bool isDamageHealed;
        bool isDebuffResistant;
        bool isReflecting;
        bool isDamageImmune;
        bool isPhysicalArtsPlus;
        float debuffTime;
        float buffTime;
        float speedMultiplier = 1f;
        List<Image> currentDebuffs = new List<Image>();
        List<Image> allDebuffs = new List<Image>();
        List<Image> currentBuffs = new List<Image>();
        List<Image> allBuffs = new List<Image>();
        Component[] components;
        StatusEffect[] statuses;
        int debuffCounter = 0;
        int buffCounter = 0;
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnDespawnEvent += Creature_OnDespawnEvent;
            creature.OnKillEvent += Creature_OnKillEvent;
            creature.ragdoll.OnSliceEvent += Ragdoll_OnSliceEvent;
            health = healthBar.transform.Find("Health").GetComponent<Image>();
            creatureName = healthBar.transform.Find("Name").GetComponent<Text>();
            level = healthBar.transform.Find("Level").GetComponent<Text>();
            sight = (RectTransform)healthBar.transform.Find("Sight").transform;
            hearing = (RectTransform)healthBar.transform.Find("Hearing").transform;
            passive = (RectTransform)healthBar.transform.Find("Passive").transform;
            unique = (RectTransform)healthBar.transform.Find("Unique").transform;
            color = healthBar.transform.Find("Color").GetComponent<Image>();
            AddBuffs();
            AddDebuffs();
            AddStatusEffects();
            points = healthBar.transform.Find("Points").GetComponent<Text>();
            pointsBlack = healthBar.transform.Find("PointsBlack").GetComponent<Text>();
            stats = creature.GetComponent<XenobladeStats>();
            if (creature.brain.instance.GetModule<BrainModuleDetection>(false) != null)
            {
                detection = creature.brain.instance.GetModule<BrainModuleDetection>(false);
                if (detection.sightDetectionHorizontalFov > 0 && detection.sightDetectionVerticalFov > 0 && stats.detectionTypeSight) sight.gameObject.SetActive(true);
                else if (detection.canHear && stats.detectionTypeSound) hearing.gameObject.SetActive(true);
                else passive.gameObject.SetActive(true);
            }
            if (creature.brain.instance.GetModule<BrainModuleMelee>(false) != null) melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            else passive.gameObject.SetActive(true);
            if (!stats.detectionTypeSight || (stats.detectionTypeSound && !stats.detectionTypeSight))
            {
                detection.sightDetectionHorizontalFov = 0;
                detection.sightDetectionVerticalFov = 0;
            }
            unique.gameObject.SetActive(stats.isUnique);
            if (stats.GetLevel() < 10) level.text = "00" + stats.GetLevel().ToString();
            else if (stats.GetLevel() < 100) level.text = "0" + stats.GetLevel().ToString();
            else level.text = stats.GetLevel().ToString();
            if (stats.GetLevel() <= XenobladeManager.GetLevel() - 6) color.color = Color.black;
            else if (stats.GetLevel() <= XenobladeManager.GetLevel() - 3) color.color = Color.blue;
            else if (stats.GetLevel() >= XenobladeManager.GetLevel() + 6) color.color = Color.red;
            else if (stats.GetLevel() >= XenobladeManager.GetLevel() + 3) color.color = Color.yellow;
            else color.color = Color.white;
            if (stats.creatureName == "")
                creatureName.text = creature.data.name;
            else creatureName.text = stats.creatureName;
            StartCoroutine(UpdateHealth());
        }
        public void AddStatusEffects()
        {
            Transform statusEffects = healthBar.transform.Find("StatusEffects").transform;
            breakStatus = statusEffects.Find("Break").GetComponent<Image>();
            toppleStatus = statusEffects.Find("Topple").GetComponent<Image>();
            dazeStatus = statusEffects.Find("Daze").GetComponent<Image>();
            sleepStatus = statusEffects.Find("Sleep").GetComponent<Image>();
        }
        public void AddDebuffs()
        {
            Transform debuffs = healthBar.transform.Find("Debuffs").transform;
            paralyze = debuffs.Find("Paralyze").GetComponent<Image>();
            artsSeal = debuffs.Find("ArtsSeal").GetComponent<Image>();
            bind = debuffs.Find("Bind").GetComponent<Image>();
            poison = debuffs.Find("Poison").GetComponent<Image>();
            burn = debuffs.Find("Burn").GetComponent<Image>();
            bleed = debuffs.Find("Bleed").GetComponent<Image>();
            auraSeal = debuffs.Find("AuraSeal").GetComponent<Image>();
            freeze = debuffs.Find("Freeze").GetComponent<Image>();
            strengthDown = debuffs.Find("StrengthDown").GetComponent<Image>();
            etherDown = debuffs.Find("EtherDown").GetComponent<Image>();
            physicalDefenseDown = debuffs.Find("PhysicalDefenseDown").GetComponent<Image>();
            etherDefenseDown = debuffs.Find("EtherDefenseDown").GetComponent<Image>();
            agilityDown = debuffs.Find("AgilityDown").GetComponent<Image>();
            slow = debuffs.Find("Slow").GetComponent<Image>();
            lockOn = debuffs.Find("LockOn").GetComponent<Image>();
            confuse = debuffs.Find("Confuse").GetComponent<Image>();
            pierce = debuffs.Find("Pierce").GetComponent<Image>();
            healthDown = debuffs.Find("HealthDown").GetComponent<Image>();
            allDebuffs.Add(paralyze);
            allDebuffs.Add(artsSeal);
            allDebuffs.Add(bind);
            allDebuffs.Add(poison);
            allDebuffs.Add(burn);
            allDebuffs.Add(bleed);
            allDebuffs.Add(auraSeal);
            allDebuffs.Add(freeze);
            allDebuffs.Add(strengthDown);
            allDebuffs.Add(etherDown);
            allDebuffs.Add(physicalDefenseDown);
            allDebuffs.Add(etherDefenseDown);
            allDebuffs.Add(agilityDown);
            allDebuffs.Add(slow);
            allDebuffs.Add(lockOn);
            allDebuffs.Add(confuse);
            allDebuffs.Add(pierce);
            allDebuffs.Add(healthDown);
            debuffTime = Time.time;
        }
        public void AddBuffs()
        {
            Transform buffs = healthBar.transform.Find("Buffs").transform;
            strengthUp = buffs.Find("StrengthUp").GetComponent<Image>();
            etherUp = buffs.Find("EtherUp").GetComponent<Image>();
            physicalDefenseUp = buffs.Find("PhysicalDefenseUp").GetComponent<Image>();
            etherDefenseUp = buffs.Find("EtherDefenseUp").GetComponent<Image>();
            agilityUp = buffs.Find("AgilityUp").GetComponent<Image>();
            haste = buffs.Find("Haste").GetComponent<Image>();
            monadoArmor = buffs.Find("Armor").GetComponent<Image>();
            monadoShield = buffs.Find("Shield").GetComponent<Image>();
            monadoEnchant = buffs.Find("Enchant").GetComponent<Image>();
            regen = buffs.Find("Regen").GetComponent<Image>();
            damageHeal = buffs.Find("DamageHeal").GetComponent<Image>();
            debuffResist = buffs.Find("DebuffResist").GetComponent<Image>();
            reflect = buffs.Find("Reflect").GetComponent<Image>();
            damageImmunity = buffs.Find("DamageImmunity").GetComponent<Image>();
            physicalArtsPlus = buffs.Find("PhysicalArtsPlus").GetComponent<Image>();
            allBuffs.Add(monadoArmor);
            allBuffs.Add(monadoShield);
            allBuffs.Add(monadoEnchant);
            allBuffs.Add(strengthUp);
            allBuffs.Add(etherUp);
            allBuffs.Add(physicalDefenseUp);
            allBuffs.Add(etherDefenseUp);
            allBuffs.Add(agilityUp);
            allBuffs.Add(haste);
            allBuffs.Add(regen);
            allBuffs.Add(damageHeal);
            allBuffs.Add(debuffResist);
            allBuffs.Add(reflect);
            allBuffs.Add(damageImmunity);
            allBuffs.Add(physicalArtsPlus);
            buffTime = Time.time;
        }
        private void Ragdoll_OnSliceEvent(RagdollPart ragdollPart, EventTime eventTime)
        {
            if (ragdollPart.data.sliceForceKill && eventTime == EventTime.OnStart && !creature.isKilled)
            {
                GameObject dmg = Instantiate(XenobladeLevelModule.damage);
                XenobladeDamage dmgComponent = dmg.AddComponent<XenobladeDamage>();
                dmgComponent.instance = new CollisionInstance(new DamageStruct(DamageType.Slash, 99999));
                dmgComponent.instance.damageStruct.hitRagdollPart = ragdollPart;
                dmgComponent.instance.damageStruct.active = false;
                dmgComponent.isCritical = true;
                dmgComponent.creature = creature;
            }
        }

        public IEnumerator UpdateHealth()
        {
            while (!creature.loaded) yield return null;
            creature.maxHealth += 500 * (stats.GetLevel() - 1);
            if (stats.isUnique) creature.maxHealth += 500 * (stats.GetLevel() - 1);
            if (stats.overrideHealth > -1) creature.maxHealth = stats.overrideHealth;
            creature.currentHealth = creature.maxHealth;
        }
        private void Creature_OnKillEvent(CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                creature.brain.Load(creature.brain.instance.id);
                Destroy(healthBar);
            }
        }

        public void Update()
        {
            if (!creature.isKilled && healthBar != null)
            {
                healthBar.transform.position = creature.ragdoll.headPart.transform.position + Vector3.up * 0.5f;
                if (Spectator.local?.cam != null && Spectator.local.state != Spectator.State.Disabled)
                    healthBar.transform.rotation = Quaternion.LookRotation(-(Spectator.local.cam.transform.position - healthBar.transform.position).normalized);
                else if (Player.local?.head?.cam != null)
                    healthBar.transform.rotation = Quaternion.LookRotation(-(Player.local.head.cam.transform.position - healthBar.transform.position).normalized);
                health.fillAmount = creature.currentHealth / creature.maxHealth;
                points.text = ((int)creature.currentHealth).ToString() + "/" + ((int)creature.maxHealth).ToString();
                pointsBlack.text = ((int)creature.currentHealth).ToString() + "/" + ((int)creature.maxHealth).ToString();
                if (stats.GetLevel() < 10) level.text = "00" + stats.GetLevel().ToString();
                else if (stats.GetLevel() < 100) level.text = "0" + stats.GetLevel().ToString();
                else level.text = stats.GetLevel().ToString();
                if (stats.GetLevel() <= XenobladeManager.GetLevel() - 6) color.color = Color.black;
                else if (stats.GetLevel() <= XenobladeManager.GetLevel() - 3) color.color = Color.blue;
                else if (stats.GetLevel() >= XenobladeManager.GetLevel() + 6) color.color = Color.red;
                else if (stats.GetLevel() >= XenobladeManager.GetLevel() + 3) color.color = Color.yellow;
                else color.color = Color.white;
                if (detection != null && stats.GetLevel() <= XenobladeManager.GetLevel() - 6 && !stats.isUnique && !stats.forceDetection)
                {
                    detection.sightDetectionHorizontalFov = 0f;
                    detection.sightDetectionVerticalFov = 0f;
                }
                else if (detection != null && stats.detectionTypeSound && !stats.detectionTypeSight && detection.canHear && detection.alertednessIncreased)
                {
                    detection.sightDetectionHorizontalFov = 360f;
                    detection.sightDetectionVerticalFov = 360f;
                }
                else if (detection != null && stats.detectionTypeSound && !stats.detectionTypeSight && detection.canHear && !detection.alertednessIncreased)
                {
                    detection.sightDetectionHorizontalFov = 0f;
                    detection.sightDetectionVerticalFov = 0f;
                }
                components = creature.GetComponents(typeof(Component));
                statuses = creature.GetComponents<StatusEffect>();
                CheckAgility();
                UpdateStatusEffects();
                UpdateBuffs();
                UpdateDebuffs();
            }
        }
        public void CheckAgility()
        {
            if (creature.currentLocomotion.speedModifiers.Count == 0)
            {
                speedMultiplier = 1f;
            }
            else
            {
                float num = 1f;
                foreach (Locomotion.SpeedModifier speedModifier in creature.currentLocomotion.speedModifiers)
                {
                    num *= speedModifier.forwardSpeedMultiplier;
                }
                speedMultiplier = num;
            }
        }
        public void UpdateBuffs()
        {
            isEnchanted = components.Any(component => component.ToString().ToLower().Contains("enchantaura"));
            isShielded = components.Any(component => component.ToString().ToLower().Contains("shieldaura"));
            isArmored = components.Any(component => component.ToString().ToLower().Contains("armoraura"));
            isRegen = components.Any(component => component.ToString().ToLower().Contains("regen"));
            isReflecting = components.Any(component => component.ToString().ToLower().Contains("reflect"));
            isDebuffResistant = components.Any(component => component.ToString().ToLower().Contains("debuffResist"));
            isDamageHealed = components.Any(component => component.ToString().ToLower().Contains("damageHeal"));
            isDamageImmune = components.Any(component => component.ToString().ToLower().Contains("damageImmunity") || component.ToString().ToLower().Contains("barrier"));
            isPhysicalArtsPlus = components.Any(component => component.ToString().ToLower().Contains("physicalArtsPlus"));
            if (isArmored && !currentBuffs.Contains(monadoArmor)) currentBuffs.Add(monadoArmor); else if (!isArmored) currentBuffs.Remove(monadoArmor);
            if (isShielded && !currentBuffs.Contains(monadoShield)) currentBuffs.Add(monadoShield); else if (!isShielded) currentBuffs.Remove(monadoShield);
            if (isEnchanted && !currentBuffs.Contains(monadoEnchant)) currentBuffs.Add(monadoEnchant); else if (!isEnchanted) currentBuffs.Remove(monadoEnchant);
            if (stats.GetStrengthMultiplier() > 1 && !currentBuffs.Contains(strengthUp)) currentBuffs.Add(strengthUp); else if (stats.GetStrengthMultiplier() <= 1) currentBuffs.Remove(strengthUp);
            if (stats.GetEtherMultiplier() > 1 && !currentBuffs.Contains(etherUp)) currentBuffs.Add(etherUp); else if (stats.GetEtherMultiplier() <= 1) currentBuffs.Remove(etherUp);
            if (stats.GetPhysicalDefenseModifier() > 0 && !currentBuffs.Contains(physicalDefenseUp)) currentBuffs.Add(physicalDefenseUp); else if (stats.GetPhysicalDefenseModifier() <= 0) currentBuffs.Remove(physicalDefenseUp);
            if (stats.GetEtherDefenseModifier() > 0 && !currentBuffs.Contains(etherDefenseUp)) currentBuffs.Add(etherDefenseUp); else if (stats.GetEtherDefenseModifier() <= 0) currentBuffs.Remove(etherDefenseUp);
            if (stats.GetAgilityMultiplier() > 1 && !currentBuffs.Contains(agilityUp)) currentBuffs.Add(agilityUp); else if (stats.GetAgilityMultiplier() <= 1) currentBuffs.Remove(agilityUp);
            if (isRegen && !currentBuffs.Contains(regen)) currentBuffs.Add(regen); else if (!isRegen) currentBuffs.Remove(regen);
            if (isReflecting && !currentBuffs.Contains(reflect)) currentBuffs.Add(reflect); else if (!isReflecting) currentBuffs.Remove(reflect);
            if (isDebuffResistant && !currentBuffs.Contains(debuffResist)) currentBuffs.Add(debuffResist); else if (!isDebuffResistant) currentBuffs.Remove(debuffResist);
            if (isDamageHealed && !currentBuffs.Contains(damageHeal)) currentBuffs.Add(damageHeal); else if (!isDamageHealed) currentBuffs.Remove(damageHeal);
            if (isDamageImmune && !currentBuffs.Contains(damageImmunity)) currentBuffs.Add(damageImmunity); else if (!isDamageImmune) currentBuffs.Remove(damageImmunity);
            if (isPhysicalArtsPlus && !currentBuffs.Contains(physicalArtsPlus)) currentBuffs.Add(physicalArtsPlus); else if (!isPhysicalArtsPlus) currentBuffs.Remove(physicalArtsPlus);
            if (melee?.animationSpeedMultiplier > stats.GetBaseAttackSpeedMultiplier() && !currentBuffs.Contains(haste)) currentBuffs.Add(haste); else if (melee?.animationSpeedMultiplier <= stats.GetBaseAttackSpeedMultiplier()) currentBuffs.Remove(haste);
            buffCounter = Mathf.Clamp(buffCounter, 0, currentBuffs.Count - 1);
            foreach (Image buff in allBuffs)
            {
                if (!currentBuffs.Contains(buff)) buff.gameObject.SetActive(false);
            }
            if (Time.time - buffTime >= 3 && currentBuffs.Count > 0)
            {
                if (buffCounter <= currentBuffs.Count - 1) buffCounter++;
                if (buffCounter > currentBuffs.Count - 1) buffCounter = 0;
                foreach (Image buff in currentBuffs)
                {
                    if (currentBuffs.IndexOf(buff) == buffCounter) buff.gameObject.SetActive(true);
                    else buff.gameObject.SetActive(false);
                }
                buffTime = Time.time;
            }
            if (currentBuffs.Count > 0)
            {
                if (buffCounter <= currentBuffs.Count - 1 && !currentBuffs[buffCounter].gameObject.activeSelf) currentBuffs[buffCounter].gameObject.SetActive(true);
                else if (buffCounter > currentBuffs.Count - 1) buffCounter = currentBuffs.Count - 1;
            }
            if (currentBuffs.Count == 0)
            {
                foreach (Image buff in allBuffs) buff.gameObject.SetActive(false);
                buffTime = Time.time;
                buffCounter = 0;
            }
        }
        public void UpdateStatusEffects()
        {
            if (creature.brain.currentStagger == Brain.Stagger.Full || creature.brain.currentStagger == Brain.Stagger.LightAndMedium && creature.state == Creature.State.Alive) breakStatus.gameObject.SetActive(true);
            else breakStatus.gameObject.SetActive(false);
            if (creature.state == Creature.State.Destabilized) toppleStatus.gameObject.SetActive(true);
            else toppleStatus.gameObject.SetActive(false);
            if (creature.state == Creature.State.Destabilized && (stats.isDazed || creature.brain.isElectrocuted)) dazeStatus.gameObject.SetActive(true);
            else dazeStatus.gameObject.SetActive(false);
            if (creature.ragdoll.state == Ragdoll.State.Inert && !creature.isKilled) sleepStatus.gameObject.SetActive(true);
            else sleepStatus.gameObject.SetActive(false);
            stats.isSleeping = sleepStatus.gameObject.activeSelf;
            stats.isBroken = breakStatus.gameObject.activeSelf;
            stats.isToppled = toppleStatus.gameObject.activeSelf;
            stats.isDazed = dazeStatus.gameObject.activeSelf;
            if (sleepStatus.gameObject.activeSelf)
            {
                dazeStatus.gameObject.SetActive(false);
                toppleStatus.gameObject.SetActive(false);
                breakStatus.gameObject.SetActive(false);
            }
            if (dazeStatus.gameObject.activeSelf)
            {
                toppleStatus.gameObject.SetActive(false);
                breakStatus.gameObject.SetActive(false);
            }
            if (toppleStatus.gameObject.activeSelf)
            {
                breakStatus.gameObject.SetActive(false);
            }
        }
        public void UpdateDebuffs()
        {
            isParalyzed = creature.brain.isElectrocuted;
            isArtsSealed = creature.mana.currentMana <= 0.5f;
            isBinded = !creature.currentLocomotion.allowMove;
            isPoisoned = components.Any(component => component.ToString().ToLower().Contains("poision") || component.ToString().ToLower().Contains("venom")) || (!statuses.IsNullOrEmpty() && statuses.Any(status => status.damageType == XenobladeDamageType.Poison));
            isBurning = components.Any(component => component.ToString().ToLower().Contains("burn") || component.ToString().ToLower().Contains("fire") || component.ToString().ToLower().Contains("blaze")) || (!statuses.IsNullOrEmpty() && statuses.Any(status => status.damageType == XenobladeDamageType.Blaze));
            isBleeding = components.Any(component => component.ToString().ToLower().Contains("bleed") || component.ToString().ToLower().Contains("eateraura")) || (!statuses.IsNullOrEmpty() && statuses.Any(status => status.damageType == XenobladeDamageType.Bleed));
            isAuraSealed = components.Any(component => component.ToString().ToLower().Contains("purgeaura"));
            isFreezing = components.Any(component => component.ToString().ToLower().Contains("freeze") || component.ToString().ToLower().Contains("frost") || component.ToString().ToLower().Contains("chill") || component.ToString().ToLower().Contains("frozen")) || (!statuses.IsNullOrEmpty() && statuses.Any(status => status.damageType == XenobladeDamageType.Chill));
            isLockedOn = components.Any(component => component.ToString().ToLower().Contains("lockon")); 
            isConfused = components.Any(component => component.ToString().ToLower().Contains("confuse") || component.ToString().ToLower().Contains("confusion"));
            isPierced = components.Any(component => component.ToString().ToLower().Contains("pierce"));
            if (isParalyzed && !currentDebuffs.Contains(paralyze)) currentDebuffs.Add(paralyze); else if (!isParalyzed) currentDebuffs.Remove(paralyze);
            if (isArtsSealed && !currentDebuffs.Contains(artsSeal)) currentDebuffs.Add(artsSeal); else if (!isArtsSealed) currentDebuffs.Remove(artsSeal);
            if (isBinded && !currentDebuffs.Contains(bind)) currentDebuffs.Add(bind); else if (!isBinded) currentDebuffs.Remove(bind);
            if (isPoisoned && !currentDebuffs.Contains(poison)) currentDebuffs.Add(poison); else if (!isPoisoned) currentDebuffs.Remove(poison);
            if (isBurning && !currentDebuffs.Contains(burn)) currentDebuffs.Add(burn); else if (!isBurning) currentDebuffs.Remove(burn);
            if (isBleeding && !currentDebuffs.Contains(bleed)) currentDebuffs.Add(bleed); else if (!isBleeding) currentDebuffs.Remove(bleed);
            if (isAuraSealed && !currentDebuffs.Contains(auraSeal)) currentDebuffs.Add(auraSeal); else if (!isAuraSealed) currentDebuffs.Remove(auraSeal);
            if (isFreezing && !currentDebuffs.Contains(freeze)) currentDebuffs.Add(freeze); else if (!isFreezing) currentDebuffs.Remove(freeze);
            if (isLockedOn && !currentDebuffs.Contains(lockOn)) currentDebuffs.Add(lockOn); else if (!isLockedOn) currentDebuffs.Remove(lockOn);
            if (isConfused && !currentDebuffs.Contains(confuse)) currentDebuffs.Add(confuse); else if (!isConfused) currentDebuffs.Remove(confuse);
            if (isPierced && !currentDebuffs.Contains(pierce)) currentDebuffs.Add(pierce); else if (!isPierced) currentDebuffs.Remove(pierce);
            if (stats.GetHealthMultiplier() < 1 && !currentDebuffs.Contains(healthDown)) currentDebuffs.Add(healthDown); else if (stats.GetHealthMultiplier() >= 1) currentDebuffs.Remove(healthDown);
            if (stats.GetPhysicalDefenseModifier() < 0 && !currentDebuffs.Contains(physicalDefenseDown)) currentDebuffs.Add(physicalDefenseDown); else if (stats.GetPhysicalDefenseModifier() >= 0) currentDebuffs.Remove(physicalDefenseDown);
            if (stats.GetEtherDefenseModifier() < 0 && !currentDebuffs.Contains(etherDefenseDown)) currentDebuffs.Add(etherDefenseDown); else if (stats.GetEtherDefenseModifier() >= 0) currentDebuffs.Remove(etherDefenseDown);
            if (stats.GetStrengthMultiplier() < 1 && !currentDebuffs.Contains(strengthDown)) currentDebuffs.Add(strengthDown); else if (stats.GetStrengthMultiplier() >= 1) currentDebuffs.Remove(strengthDown);
            if (stats.GetEtherMultiplier() < 1 && !currentDebuffs.Contains(etherDown)) currentDebuffs.Add(etherDown); else if (stats.GetEtherMultiplier() >= 1) currentDebuffs.Remove(etherDown);
            if (stats.GetAgilityMultiplier() < 1f && !currentDebuffs.Contains(agilityDown)) currentDebuffs.Add(agilityDown); else if (stats.GetAgilityMultiplier() >= 1f) currentDebuffs.Remove(agilityDown);
            if (melee?.animationSpeedMultiplier < stats.GetBaseAttackSpeedMultiplier() && !currentDebuffs.Contains(slow)) currentDebuffs.Add(slow); else if (melee?.animationSpeedMultiplier >= stats.GetBaseAttackSpeedMultiplier()) currentDebuffs.Remove(slow);
            debuffCounter = Mathf.Clamp(debuffCounter, 0, currentDebuffs.Count - 1);
            foreach (Image debuff in allDebuffs)
            {
                if (!currentDebuffs.Contains(debuff)) debuff.gameObject.SetActive(false);
            }
            if (Time.time - debuffTime >= 3 && currentDebuffs.Count > 0)
            {
                if (debuffCounter <= currentDebuffs.Count - 1) debuffCounter++;
                if (debuffCounter > currentDebuffs.Count - 1) debuffCounter = 0;
                foreach (Image debuff in currentDebuffs)
                {
                    if (currentDebuffs.IndexOf(debuff) == debuffCounter) debuff.gameObject.SetActive(true);
                    else debuff.gameObject.SetActive(false);
                }
                debuffTime = Time.time;
            }
            if (currentDebuffs.Count > 0)
            {
                if (debuffCounter <= currentDebuffs.Count - 1 && !currentDebuffs[debuffCounter].gameObject.activeSelf) currentDebuffs[debuffCounter].gameObject.SetActive(true);
                else if (debuffCounter > currentDebuffs.Count - 1) debuffCounter = currentDebuffs.Count - 1;
            }
            if (currentDebuffs.Count == 0)
            {
                foreach (Image debuff in allDebuffs) debuff.gameObject.SetActive(false);
                debuffTime = Time.time;
                debuffCounter = 0;
            }
        }

        private void Creature_OnDespawnEvent(EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                if (healthBar)
                    Destroy(healthBar);
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            creature.OnKillEvent -= Creature_OnKillEvent;
            creature.OnDespawnEvent -= Creature_OnDespawnEvent;
            creature.ragdoll.OnSliceEvent -= Ragdoll_OnSliceEvent;
        }
    }
}
