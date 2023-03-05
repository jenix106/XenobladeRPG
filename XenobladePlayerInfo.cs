using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace XenobladeRPG
{
    public class XenobladePlayerInfo : MonoBehaviour
    {
        WristStats stats;
        public GameObject info;
        TMP_Text level;
        Image exp;
        TMP_Text points;
        TMP_Text pointsBlack;
        public void Start()
        {
            stats = GetComponent<WristStats>();
            exp = info.transform.Find("Exp").GetComponent<Image>();
            level = info.transform.Find("Level").GetComponent<TMP_Text>();
            points = info.transform.Find("Points").GetComponent<TMP_Text>();
            pointsBlack = info.transform.Find("PointsBlack").GetComponent<TMP_Text>();
        }
        public void Update()
        {
            info.transform.position = stats.transform.position;
            info.transform.rotation = stats.transform.rotation;
            info.SetActive(stats.isShown);
            level.text = XenobladeManager.GetLevel().ToString();
            float playerXP = XenobladeManager.GetCurrentXP();
            if (XenobladeManager.GetLevel() == 99) exp.fillAmount = 1;
            else exp.fillAmount = playerXP / XenobladeManager.GetXPNeeded();
            points.text = "EXP: " + XenobladeManager.GetCurrentXP().ToString();
            pointsBlack.text = "EXP: " + XenobladeManager.GetCurrentXP().ToString();
        }
    }
}
