using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class DangerBlue : MonoBehaviour
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
            //creature.GetComponent<XenobladeStats>().baseAttackSpeedMultiplier = 0.75f;
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
            if (bow != null)
            {
                bow.arrowDrawDelay = 0.75f;
                bow.arrowNockDelay = 1.25f;
                bow.bowDrawDelay = 1.25f;
                bow.aimMoveSpeed = 7.5f;
                bow.turnSpeed = 0.75f;
                bow.minMaxTimeToAttackFromAim.y = 5;
                bow.minMaxTimeBetweenAttack = new Vector2(5, 10);
            }
            if (cast != null)
            {
                cast.castMinMaxDelay = new Vector2(5, 6);
                cast.chargeDurationMultiplier = 1.25f;
                cast.spreadCone = new Vector2(0.75f, 0.75f);
            }
            yield break;
        }
    }
}
