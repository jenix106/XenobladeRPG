using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class DangerYellow : MonoBehaviour
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
            //creature.GetComponent<XenobladeStats>().baseAttackSpeedMultiplier = 1.5f;
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
            if (bow != null)
            {
                bow.arrowDrawDelay = 0.25f;
                bow.arrowNockDelay = 0.5f;
                bow.bowDrawDelay = 0.5f;
                bow.aimMoveSpeed = 15;
                bow.turnSpeed = 1.5f;
                bow.minMaxTimeToAttackFromAim.y = 2;
                bow.minMaxTimeBetweenAttack = new Vector2(2, 4);
            }
            if (cast != null)
            {
                cast.castMinMaxDelay = new Vector2(2, 3);
                cast.chargeDurationMultiplier = 0.5f;
                cast.spreadCone = new Vector2(0.3f, 0.3f);
            }
            yield break;
        }
    }
}
