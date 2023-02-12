using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class DangerRed : MonoBehaviour
    {
        Creature creature;
        BrainModuleMelee melee;
        BrainModuleDodge dodge;
        BrainModuleEquipment equipment;
        BrainModuleHitReaction hitReaction;
        BrainModuleDefense defense;
        BrainModuleBow bow;
        BrainModuleCast cast;
        public void Start()
        {
            creature = GetComponent<Creature>();
            //creature.GetComponent<XenobladeStats>().baseAttackSpeedMultiplier = 2.5f;
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            dodge = creature.brain.instance.GetModule<BrainModuleDodge>(false);
            equipment = creature.brain.instance.GetModule<BrainModuleEquipment>(false);
            hitReaction = creature.brain.instance.GetModule<BrainModuleHitReaction>(false);
            defense = creature.brain.instance.GetModule<BrainModuleDefense>(false);
            bow = creature.brain.instance.GetModule<BrainModuleBow>(false);
            cast = creature.brain.instance.GetModule<BrainModuleCast>(false);
            StartCoroutine(Apply());
        }
        public IEnumerator Apply()
        {
            while (!creature.loaded) yield return null;
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
            if (bow != null)
            {
                bow.arrowDrawDelay = 0.1f;
                bow.arrowNockDelay = 0.25f;
                bow.bowDrawDelay = 0.25f;
                bow.aimMoveSpeed = 25;
                bow.turnSpeed = 2.5f;
                bow.minMaxTimeToAttackFromAim.y = 1;
                bow.minMaxTimeBetweenAttack = new Vector2(1, 2);
            }
            if (cast != null)
            {
                cast.castMinMaxDelay = new Vector2(1, 2);
                cast.chargeDurationMultiplier = 0.25f;
                cast.spreadCone = new Vector2(0.1f, 0.1f);
            }
            yield break;
        }
    }
}
