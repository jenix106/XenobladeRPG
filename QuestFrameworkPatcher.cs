using ThunderRoad;
using UnityEngine;
using HarmonyLib;
using QuestFramework;
using QuestFramework.Interactions;
using QuestFramework.POI;

namespace XenobladeRPG
{
    public class QuestFrameworkPatcher
    {
        public static void DoPatching()
        {
            Harmony harmony = new Harmony("XenobladeRPG-QuestFramework");
            var questPostfix = typeof(UpdateQuestPatch).GetMethod(nameof(UpdateQuestPatch.Postfix));
            harmony.Patch(typeof(QuestData).GetMethod(nameof(QuestData.UpdateQuest)), null, new HarmonyMethod(questPostfix));
        }
    }
    [HarmonyPatch(typeof(QuestData), nameof(QuestData.UpdateQuest))]
    public class UpdateQuestPatch
    {
        public static void Postfix(QuestSaveData.Quest.State state)
        {
            if(state == QuestSaveData.Quest.State.Completed)
            {
                float xpGained = 10;
                float apGained = 10;
                float spGained = 10;
                for (int i = 1; i <= XenobladeManager.GetLevel(); i++)
                {
                    xpGained += Mathf.Pow(2, (3 + Mathf.FloorToInt(i / 7)));
                    apGained += 6;
                }
                XenobladeManager.AddXP(Mathf.RoundToInt(xpGained));
                XenobladeManager.AddAP(Mathf.RoundToInt(apGained));
                XenobladeManager.AddSP(Mathf.RoundToInt(spGained));
                Debug.Log("Quest completed. Adding XP: " + xpGained);
            }
        }
    }
}
