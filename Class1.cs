using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

namespace XenobladeRPG
{
    public class XenobladeManager
    {
        public static float PlayerLevel = 1;
        public static float PlayerXP = 0;
        public static void LoadFromSave()
        {
            if (File.Exists(Path.Combine(Application.streamingAssetsPath, "Mods/Xenoblade RPG/Saves/XenobladeValues.json")))
            {
                XenobladeValues xenobladeValues = JsonConvert.DeserializeObject<XenobladeValues>(File.ReadAllText((Path.Combine(Application.streamingAssetsPath, "Mods/Xenoblade RPG/Saves/XenobladeValues.json"))));
                PlayerLevel = (int)Mathf.Clamp(xenobladeValues.PlayerLevel, 1, 99);
                PlayerXP = (int)Mathf.Max(xenobladeValues.PlayerXP, 0);
            }
            else
            {
                SaveToJSON();
            }
        }
        public static void SaveToJSON()
        {
            XenobladeValues xenobladeValues = new XenobladeValues()
            {
                PlayerLevel = PlayerLevel,
                PlayerXP = PlayerXP
            };
            string contents = JsonConvert.SerializeObject(xenobladeValues, Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "Mods/Xenoblade RPG/Saves/XenobladeValues.json"), contents);
        }
    }
    public class XenobladeValues
    {
        public float PlayerLevel { get; set; }
        public float PlayerXP { get; set; }
    }
    public class XCRPGCheck : LevelModule { }
    public class HealthBarsLevelModule : LevelModule
    {
        public static HealthBarsLevelModule local;
        public static GameObject currentBar;
        public static GameObject playerInfo;
        public static GameObject damage;
        public override IEnumerator OnLoadCoroutine()
        {
            local = this;
            XenobladeManager.LoadFromSave();
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onCreatureHit += EventManager_onCreatureHit;
            WaveSpawner.OnWaveSpawnerStartRunningEvent.AddListener(call => UpdateHealth(call.waveData));
            Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeUI", value =>
            {
                currentBar = value;
            }, "XenobladeRPG");
            Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladePlayerUI", value =>
            {
                playerInfo = value;
            }, "XenobladeRPG");
            Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeDamageUI", value =>
            {
                damage = value;
            }, "XenobladeRPG");
            return base.OnLoadCoroutine();
        }
        public void UpdateHealth(WaveData spawner)
        {
            Player.local.creature.maxHealth += 200 * (XenobladeManager.PlayerLevel - 1); 
            Player.local.creature.currentHealth += 200 * (XenobladeManager.PlayerLevel - 1);
        }
        public override void OnUnload()
        {
            base.OnUnload();
            XenobladeManager.SaveToJSON();
            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            EventManager.onCreatureHit -= EventManager_onCreatureHit;
        }

        private void EventManager_onCreatureHit(Creature creature, CollisionInstance collisionInstance)
        {
            if (collisionInstance.targetColliderGroup?.collisionHandler?.ragdollPart?.ragdoll?.creature != null && collisionInstance.IsDoneByPlayer() && !creature.isPlayer)
            {
                collisionInstance.damageStruct.damage *= XenobladeManager.PlayerLevel;
                creature.currentHealth -= collisionInstance.damageStruct.damage;
                if(creature.currentHealth <= 0.0)
                {
                    creature.currentHealth = 0.0f;
                    creature.Kill(collisionInstance);
                }
            }
            else if(collisionInstance.targetColliderGroup?.collisionHandler?.ragdollPart?.ragdoll?.creature != null && !collisionInstance.IsDoneByPlayer())
            {
                if (collisionInstance.sourceColliderGroup)
                {
                    if (collisionInstance.sourceColliderGroup.collisionHandler.item?.lastHandler?.creature?.GetComponent<XenobladeStats>() != null)
                    {
                        collisionInstance.damageStruct.damage *= collisionInstance.sourceColliderGroup.collisionHandler.item.lastHandler.creature.GetComponent<XenobladeStats>().level;
                        creature.currentHealth -= collisionInstance.damageStruct.damage;
                        if (creature.currentHealth <= 0.0)
                        {
                            creature.currentHealth = 0.0f;
                            creature.Kill(collisionInstance);
                        }
                    }
                    else if (collisionInstance.sourceColliderGroup.collisionHandler.item?.lastHandler?.creature?.player != null)
                    {
                        collisionInstance.damageStruct.damage *= XenobladeManager.PlayerLevel;
                        creature.currentHealth -= collisionInstance.damageStruct.damage;
                        if (creature.currentHealth <= 0.0)
                        {
                            creature.currentHealth = 0.0f;
                            creature.Kill(collisionInstance);
                        }
                    }

                }
                else if (collisionInstance.casterHand?.mana?.creature?.GetComponent<XenobladeStats>() != null)
                {
                    collisionInstance.damageStruct.damage *= collisionInstance.casterHand.mana.creature.GetComponent<XenobladeStats>().level;
                    creature.currentHealth -= collisionInstance.damageStruct.damage;
                    if (creature.currentHealth <= 0.0)
                    {
                        creature.currentHealth = 0.0f;
                        creature.Kill(collisionInstance);
                    }
                }
            }
            else if (collisionInstance.damageStruct.hitRagdollPart != null && collisionInstance.sourceColliderGroup == null)
            {
                collisionInstance.damageStruct.damage *= XenobladeManager.PlayerLevel;
                creature.currentHealth -= collisionInstance.damageStruct.damage;
                if (creature.currentHealth <= 0.0)
                {
                    creature.currentHealth = 0.0f;
                    creature.Kill(collisionInstance);
                }
            }
            GameObject dmg = GameObject.Instantiate(damage);
            dmg.AddComponent<XenobladeDamage>().instance = collisionInstance;
            dmg.GetComponent<XenobladeDamage>().creature = creature;
        }

        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd && !creature.isPlayer)
            {
                XenobladeManager.PlayerXP += Mathf.Clamp(Mathf.Clamp((creature.GetComponent<XenobladeStats>().level + 10) - XenobladeManager.PlayerLevel, 0, 20) * 5, 0, 100);
                if (XenobladeManager.PlayerXP >= XenobladeManager.PlayerLevel * 100 && XenobladeManager.PlayerLevel < 99)
                {
                    while (XenobladeManager.PlayerXP >= XenobladeManager.PlayerLevel * 100 && XenobladeManager.PlayerLevel < 99)
                    {
                        XenobladeManager.PlayerXP -= XenobladeManager.PlayerLevel * 100;
                        XenobladeManager.PlayerLevel++;
                        Player.local.creature.maxHealth += 200;
                        Player.local.creature.currentHealth += 200;
                        foreach(Creature creatureStats in Creature.allActive)
                        {
                            if(creatureStats.GetComponent<XenobladeStats>() != null)
                            creatureStats.GetComponent<XenobladeStats>().PlayerLevelUp();
                        }
                    }
                }
                XenobladeManager.SaveToJSON();
            }
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (creature != Player.local.creature)
            {
                if (creature.brain.instance.GetModule<XenobladeBrainModule>(false) == null)
                {
                    XenobladeBrainModule obj = new XenobladeBrainModule();
                    creature.brain.instance.modules.Add(obj);
                    obj.Load(creature);
                }
                creature.gameObject.AddComponent<XenobladeRemoveBar>().healthBar = GameObject.Instantiate(currentBar);
            }
            if(creature == Player.local.creature)
            {
                creature.maxHealth += 200 * (XenobladeManager.PlayerLevel - 1);
                creature.currentHealth = creature.maxHealth;
                foreach(WristStats wristStat in creature.GetComponentsInChildren<WristStats>())
                {
                    wristStat.gameObject.AddComponent<XenobladePlayerInfo>().info = GameObject.Instantiate(playerInfo);
                }
            }
        }
        public class XenobladeDamage : MonoBehaviour
        {
            public CollisionInstance instance;
            public Creature creature;
            Image critical;
            Text damage;
            public bool isCritical = false;
            public void Start()
            {
                critical = transform.Find("Critical").GetComponent<Image>();
                damage = transform.Find("Damage").GetComponent<Text>();
                if (instance.contactPoint != Vector3.zero)
                    transform.position = instance.contactPoint;
                else if(instance.damageStruct.hitRagdollPart != null)
                {
                    transform.position = instance.damageStruct.hitRagdollPart.transform.position;
                }
                if (isCritical || UnityEngine.Random.Range(1, 101) <= 10)
                {
                    critical.gameObject.SetActive(true);
                    if (instance.damageStruct.active)
                    {
                        creature.currentHealth -= instance.damageStruct.damage / 2;
                        if (creature.currentHealth <= 0.0)
                        {
                            creature.currentHealth = 0.0f;
                            creature.Kill(instance);
                        }
                    }
                }
                damage.text = ((int)instance.damageStruct.damage).ToString();
                Destroy(gameObject, 1f);
            }
            public void Update()
            {
                if (Spectator.local?.cam != null && Spectator.local.state != Spectator.State.Disabled)
                    transform.rotation = Quaternion.LookRotation(-(Spectator.local.cam.transform.position - transform.position).normalized);
                else if (Player.local?.head?.cam != null)
                    transform.rotation = Quaternion.LookRotation(-(Player.local.head.cam.transform.position - transform.position).normalized);
                transform.position += (Vector3.up * 0.5f) * Time.deltaTime;
                Color tempCrit = critical.color;
                tempCrit.a -= Time.deltaTime * (50/255);
                critical.color = tempCrit;
                Color tempDmg = damage.color;
                tempDmg.a -= Time.deltaTime;
                damage.color = tempDmg;
            }
        }
        public class XenobladePlayerInfo : MonoBehaviour
        {
            WristStats stats;
            public GameObject info;
            Text level;
            Image exp;
            Text points;
            Text pointsBlack;
            public void Start()
            {
                stats = GetComponent<WristStats>();
                exp = info.transform.Find("Exp").GetComponent<Image>();
                level = info.transform.Find("Level").GetComponent<Text>();
                points = info.transform.Find("Points").GetComponent<Text>();
                pointsBlack = info.transform.Find("PointsBlack").GetComponent<Text>();
            }
            public void Update()
            {
                info.transform.position = stats.transform.position;
                info.transform.rotation = stats.transform.rotation;
                info.SetActive(stats.isShown);
                level.text = XenobladeManager.PlayerLevel.ToString();
                exp.fillAmount = XenobladeManager.PlayerXP / (XenobladeManager.PlayerLevel * 100);
                if (XenobladeManager.PlayerLevel == 99) exp.fillAmount = 1;
                points.text = "EXP: " + XenobladeManager.PlayerXP.ToString();
                pointsBlack.text = "EXP: " + XenobladeManager.PlayerXP.ToString();
            }
        }
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
            Text points;
            Text pointsBlack;
            int levelInt;
            BrainModuleDetection detection;
            XenobladeStats stats;
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
                points = healthBar.transform.Find("Points").GetComponent<Text>();
                pointsBlack = healthBar.transform.Find("PointsBlack").GetComponent<Text>();
                stats = creature.GetComponent<XenobladeStats>(); 
                levelInt = stats.level;
                if (creature.brain.instance.GetModule<BrainModuleDetection>(false) != null)
                {
                    detection = creature.brain.instance.GetModule<BrainModuleDetection>(false);
                    if (detection.sightDetectionHorizontalFov > 0 && detection.sightDetectionVerticalFov > 0) sight.gameObject.SetActive(true);
                    else if (detection.canHear) hearing.gameObject.SetActive(true);
                    else passive.gameObject.SetActive(true);
                }
                else passive.gameObject.SetActive(true);
                unique.gameObject.SetActive(stats.isUnique);
                if (levelInt < 10) level.text = "00" + levelInt.ToString();
                else if (levelInt < 100) level.text = "0" + levelInt.ToString();
                else level.text = levelInt.ToString();
                if (levelInt <= XenobladeManager.PlayerLevel - 6) color.color = Color.black;
                else if (levelInt <= XenobladeManager.PlayerLevel - 3) color.color = Color.blue;
                else if (levelInt >= XenobladeManager.PlayerLevel + 6) color.color = Color.red;
                else if (levelInt >= XenobladeManager.PlayerLevel + 3) color.color = Color.yellow;
                else if (levelInt >= XenobladeManager.PlayerLevel - 2 && levelInt <= XenobladeManager.PlayerLevel + 2) color.color = Color.white;
                if (stats.creatureName == "")
                    creatureName.text = creature.data.name;
                else creatureName.text = stats.creatureName;
                StartCoroutine(UpdateHealth());
            }

            private void Ragdoll_OnSliceEvent(RagdollPart ragdollPart, EventTime eventTime)
            {
                if (ragdollPart.data.sliceForceKill && eventTime == EventTime.OnStart && !creature.isKilled)
                {
                    GameObject dmg = GameObject.Instantiate(damage);
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
                creature.maxHealth += 200 * (levelInt - 1);
                creature.currentHealth = creature.maxHealth;
            }
            private void Creature_OnKillEvent(CollisionInstance collisionInstance, EventTime eventTime)
            {
                if (eventTime == EventTime.OnStart)
                {
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
                    if (levelInt <= XenobladeManager.PlayerLevel - 6) color.color = Color.black;
                    else if (levelInt <= XenobladeManager.PlayerLevel - 3) color.color = Color.blue;
                    else if (levelInt >= XenobladeManager.PlayerLevel + 6) color.color = Color.red;
                    else if (levelInt >= XenobladeManager.PlayerLevel + 3) color.color = Color.yellow;
                    else if (levelInt >= XenobladeManager.PlayerLevel - 2 && levelInt <= XenobladeManager.PlayerLevel + 2) color.color = Color.white;
                    if (detection != null && levelInt <= XenobladeManager.PlayerLevel - 6 && !stats.isUnique)
                    {
                        detection.sightDetectionHorizontalFov = 0f;
                        detection.sightDetectionVerticalFov = 0f;
                    }
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
    public class XenobladeBrainModule : BrainData.Module
    {
        public int minLevel = 1;
        public int maxLevel = 99;
        public int minLevelVariation = -6;
        public int maxLevelVariation = 6;
        public bool playerRelative = true;
        public bool isUnique = false;
        public string creatureName = "";
        public override void Load(Creature creature)
        {
            base.Load(creature);
            if (creature.gameObject.GetComponent<XenobladeStats>() != null) GameObject.Destroy(creature.gameObject.GetComponent<XenobladeStats>());
            creature.gameObject.AddComponent<XenobladeStats>().Setup(minLevel, maxLevel, minLevelVariation, maxLevelVariation, playerRelative, isUnique, creatureName);
        }
    }
    public class XenobladeStats : MonoBehaviour
    {
        Creature creature;
        public int level;
        public int minLevel;
        public int maxLevel;
        public int minLevelVariation;
        public int maxLevelVariation;
        public bool playerRelative;
        public bool isUnique;
        public string creatureName;
        public void Start()
        {
            creature = GetComponent<Creature>();
            if (level >= XenobladeManager.PlayerLevel + 6) creature.gameObject.AddComponent<DangerRed>();
            else if (level >= XenobladeManager.PlayerLevel + 3) creature.gameObject.AddComponent<DangerYellow>();
            else if (level <= XenobladeManager.PlayerLevel - 6) creature.gameObject.AddComponent<DangerBlack>();
            else if (level <= XenobladeManager.PlayerLevel - 3) creature.gameObject.AddComponent<DangerBlue>();
        }
        public void Setup(int min, int max, int minVar, int maxVar, bool relative, bool unique, string name)
        {
            minLevel = min;
            maxLevel = max;
            minLevelVariation = minVar;
            maxLevelVariation = maxVar;
            playerRelative = relative;
            isUnique = unique;
            creatureName = name;
            if (!playerRelative)
                level = !isUnique ? Math.Max(UnityEngine.Random.Range(minLevel, maxLevel + 1) + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1) : Mathf.Clamp(UnityEngine.Random.Range(minLevel, maxLevel + 1) + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1, 99);
            else
                level = !isUnique ? Math.Max((int)XenobladeManager.PlayerLevel + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1) : Mathf.Clamp(UnityEngine.Random.Range(minLevel, maxLevel + 1) + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1, 99);
        }
        public void PlayerLevelUp()
        {
            if (creature.gameObject.GetComponent<DangerRed>() != null) Destroy(creature.gameObject.GetComponent<DangerRed>());
            if (creature.gameObject.GetComponent<DangerYellow>() != null) Destroy(creature.gameObject.GetComponent<DangerYellow>());
            if (creature.gameObject.GetComponent<DangerBlack>() != null) Destroy(creature.gameObject.GetComponent<DangerBlack>());
            if (creature.gameObject.GetComponent<DangerBlue>() != null) Destroy(creature.gameObject.GetComponent<DangerBlue>());
            if (level >= XenobladeManager.PlayerLevel + 6) creature.gameObject.AddComponent<DangerRed>();
            else if (level >= XenobladeManager.PlayerLevel + 3) creature.gameObject.AddComponent<DangerYellow>();
            else if (level <= XenobladeManager.PlayerLevel - 6) creature.gameObject.AddComponent<DangerBlack>();
            else if (level <= XenobladeManager.PlayerLevel - 3) creature.gameObject.AddComponent<DangerBlue>();
        }
    }
    public class DangerRed : MonoBehaviour
    {
        Creature creature;
        BrainModuleMelee melee;
        BrainModuleDodge dodge;
        BrainModuleEquipment equipment;
        BrainModuleHitReaction hitReaction;
        BrainModuleDefense defense;
        Dictionary<RagdollPart, bool> sliceParts = new Dictionary<RagdollPart, bool>();
        Dictionary<RagdollPart, float> damageParts = new Dictionary<RagdollPart, float>();
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnDespawnEvent += Creature_OnDespawnEvent;
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            dodge = creature.brain.instance.GetModule<BrainModuleDodge>(false);
            equipment = creature.brain.instance.GetModule<BrainModuleEquipment>(false);
            hitReaction = creature.brain.instance.GetModule<BrainModuleHitReaction>(false);
            defense = creature.brain.instance.GetModule<BrainModuleDefense>(false);
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (!sliceParts.ContainsKey(part)) sliceParts.Add(part, part.sliceAllowed);
                part.sliceAllowed = false;
                if (!damageParts.ContainsKey(part)) damageParts.Add(part, part.data.damageMultiplier);
            }
            foreach (RagdollPart part in damageParts.Keys)
            {
                part.data.damageMultiplier *= 0.25f;
            }
            if (melee != null && melee.meleeEnabled)
            {
                melee.animationSpeedMultiplier = 2.5f;
                melee.armSpringMultiplier = 100f;
                melee.armMaxForceMultiplier = 100f;
                melee.meleeMax = 4;
                melee.minMaxTimeBetweenAttack.x = 0f;
                melee.minMaxTimeBetweenAttack.y = 0.5f;
            }
            if (dodge != null && dodge.enabled)
            {
                dodge.dodgeChance = 1f;
                dodge.dodgeSpeed = 2f;
                dodge.dodgeWhenGrabbed = true;
                dodge.dodgeWhenWeaponGrabbed = true;
            }
            if (equipment != null)
            {
                equipment.allowArmGrabDisarm = false;
                equipment.allowArmStabDisarm = false;
                equipment.grabDisarmPushLevel = 4;
                equipment.handHitDisarmPushLevel = 4;
            }
            if (hitReaction != null)
            {
                hitReaction.recoverySpeed = 2f;
                hitReaction.parryRecoilCooldown = 0.5f;
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushMagicBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushHitBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushGrabThrowBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.None;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
            }
            if (defense != null && defense.enabled)
            {
                defense.armSpringMultiplier = 100f;
                defense.armMaxForceMultiplier = 100f;
            }
        }

        private void Creature_OnDespawnEvent(EventTime eventTime)
        {
            if(eventTime == EventTime.OnStart)
            {
                creature.OnDespawnEvent -= Creature_OnDespawnEvent;
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (sliceParts.ContainsKey(part)) part.sliceAllowed = sliceParts[part];
                if (damageParts.ContainsKey(part)) part.data.damageMultiplier = damageParts[part];
            }
        }
    }
    public class DangerYellow : MonoBehaviour
    {
        Creature creature;
        BrainModuleMelee melee;
        BrainModuleDodge dodge;
        BrainModuleEquipment equipment;
        BrainModuleHitReaction hitReaction;
        BrainModuleDefense defense;
        Dictionary<RagdollPart, float> damageParts = new Dictionary<RagdollPart, float>();
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnDespawnEvent += Creature_OnDespawnEvent;
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            dodge = creature.brain.instance.GetModule<BrainModuleDodge>(false);
            equipment = creature.brain.instance.GetModule<BrainModuleEquipment>(false);
            hitReaction = creature.brain.instance.GetModule<BrainModuleHitReaction>(false);
            defense = creature.brain.instance.GetModule<BrainModuleDefense>(false);
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (!damageParts.ContainsKey(part)) damageParts.Add(part, part.data.damageMultiplier);
            }
            foreach (RagdollPart part in damageParts.Keys)
            {
                part.data.damageMultiplier *= 0.5f;
            }
            if (melee != null && melee.meleeEnabled)
            {
                melee.animationSpeedMultiplier = 1.5f;
                melee.armSpringMultiplier = 50f;
                melee.armMaxForceMultiplier = 50f;
                melee.meleeMax = 3;
                melee.minMaxTimeBetweenAttack.x = 0.5f;
                melee.minMaxTimeBetweenAttack.y = 1f;
            }
            if (dodge != null && dodge.enabled)
            {
                dodge.dodgeChance = 0.5f;
                dodge.dodgeSpeed = 1.5f;
                dodge.dodgeWhenGrabbed = true;
                dodge.dodgeWhenWeaponGrabbed = true;
            }
            if (equipment != null)
            {
                equipment.allowArmGrabDisarm = false;
                equipment.allowArmStabDisarm = false;
                equipment.grabDisarmPushLevel = 3;
                equipment.handHitDisarmPushLevel = 3;
            }
            if (hitReaction != null)
            {
                hitReaction.recoverySpeed = 1.5f;
                hitReaction.parryRecoilCooldown = 1;
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushMagicBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushHitBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushGrabThrowBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerLight;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerMedium;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
            }
            if (defense != null && defense.enabled)
            {
                defense.armSpringMultiplier = 50f;
                defense.armMaxForceMultiplier = 50f;
            }
        }

        private void Creature_OnDespawnEvent(EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                creature.OnDespawnEvent -= Creature_OnDespawnEvent;
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (damageParts.ContainsKey(part)) part.data.damageMultiplier = damageParts[part];
            }
        }
    }
    public class DangerBlue : MonoBehaviour
    {
        Creature creature;
        BrainModuleMelee melee;
        BrainModuleDodge dodge;
        BrainModuleEquipment equipment;
        BrainModuleHitReaction hitReaction;
        BrainModuleDefense defense;
        Dictionary<RagdollPart, float> damageParts = new Dictionary<RagdollPart, float>();
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnDespawnEvent += Creature_OnDespawnEvent;
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            dodge = creature.brain.instance.GetModule<BrainModuleDodge>(false);
            equipment = creature.brain.instance.GetModule<BrainModuleEquipment>(false);
            hitReaction = creature.brain.instance.GetModule<BrainModuleHitReaction>(false);
            defense = creature.brain.instance.GetModule<BrainModuleDefense>(false);
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (!damageParts.ContainsKey(part)) damageParts.Add(part, part.data.damageMultiplier);
            }
            foreach (RagdollPart part in damageParts.Keys)
            {
                part.data.damageMultiplier *= 1.25f;
            }
            if (melee != null && melee.meleeEnabled)
            {
                melee.animationSpeedMultiplier = 0.75f;
                melee.armSpringMultiplier = 1.5f;
                melee.armMaxForceMultiplier = 7.5f;
                melee.meleeMax = 1;
                melee.minMaxTimeBetweenAttack.x = 2f;
                melee.minMaxTimeBetweenAttack.y = 3f;
            }
            if (dodge != null && dodge.enabled)
            {
                dodge.dodgeChance = 0.025f;
                dodge.dodgeSpeed = 0.75f;
                dodge.dodgeWhenGrabbed = false;
                dodge.dodgeWhenWeaponGrabbed = false;
            }
            if (equipment != null)
            {
                equipment.allowArmGrabDisarm = true;
                equipment.allowArmStabDisarm = true;
                equipment.grabDisarmPushLevel = 1;
                equipment.handHitDisarmPushLevel = 1;
            }
            if (hitReaction != null)
            {
                hitReaction.recoverySpeed = 0.75f;
                hitReaction.parryRecoilCooldown = 2f;
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushMagicBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushHitBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushGrabThrowBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
            }
            if (defense != null && defense.enabled)
            {
                defense.armSpringMultiplier = 0.75f;
                defense.armMaxForceMultiplier = 7.5f;
            }
        }

        private void Creature_OnDespawnEvent(EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                creature.OnDespawnEvent -= Creature_OnDespawnEvent;
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (damageParts.ContainsKey(part)) part.data.damageMultiplier = damageParts[part];
            }
        }
    }
    public class DangerBlack : MonoBehaviour
    {
        Creature creature;
        BrainModuleMelee melee;
        BrainModuleDodge dodge;
        BrainModuleEquipment equipment;
        BrainModuleHitReaction hitReaction;
        BrainModuleDefense defense;
        Dictionary<RagdollPart, float> damageParts = new Dictionary<RagdollPart, float>();
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnDespawnEvent += Creature_OnDespawnEvent;
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            dodge = creature.brain.instance.GetModule<BrainModuleDodge>(false);
            equipment = creature.brain.instance.GetModule<BrainModuleEquipment>(false);
            hitReaction = creature.brain.instance.GetModule<BrainModuleHitReaction>(false);
            defense = creature.brain.instance.GetModule<BrainModuleDefense>(false);
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (!damageParts.ContainsKey(part)) damageParts.Add(part, part.data.damageMultiplier);
            }
            foreach (RagdollPart part in damageParts.Keys)
            {
                part.data.damageMultiplier *= 1.5f;
            }
            if (melee != null && melee.meleeEnabled)
            {
                melee.animationSpeedMultiplier = 0.5f;
                melee.armSpringMultiplier = 1f;
                melee.armMaxForceMultiplier = 5f;
                melee.meleeMax = 1;
                melee.minMaxTimeBetweenAttack.x = 3f;
                melee.minMaxTimeBetweenAttack.y = 4f;
            }
            if (dodge != null && dodge.enabled)
            {
                dodge.dodgeChance = 0f;
                dodge.dodgeSpeed = 0f;
                dodge.dodgeWhenGrabbed = false;
                dodge.dodgeWhenWeaponGrabbed = false;
            }
            if (equipment != null)
            {
                equipment.allowArmGrabDisarm = true;
                equipment.allowArmStabDisarm = true;
                equipment.grabDisarmPushLevel = 0;
                equipment.handHitDisarmPushLevel = 0;
            }
            if (hitReaction != null)
            {
                hitReaction.recoverySpeed = 0.5f;
                hitReaction.parryRecoilCooldown = 3f;
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushMagicBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushHitBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushGrabThrowBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
            }
            if (defense != null && defense.enabled)
            {
                defense.armSpringMultiplier = 0.5f;
                defense.armMaxForceMultiplier = 5f;
            }
        }

        private void Creature_OnDespawnEvent(EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                creature.OnDespawnEvent -= Creature_OnDespawnEvent;
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                if (damageParts.ContainsKey(part)) part.data.damageMultiplier = damageParts[part];
            }
        }
    }
}

