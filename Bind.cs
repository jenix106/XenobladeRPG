using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class Bind : MonoBehaviour
    {
        public Creature creature;
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.currentLocomotion.SetSpeedModifier(this, 0, 0, 0, 0, 0);
            XenobladeEvents.InvokeOnDebuffAdded(ref creature, this);
            Destroy(this, 10);
        }
        public void OnDestroy()
        {
            creature.currentLocomotion.RemoveSpeedModifier(this);
            XenobladeEvents.InvokeOnDebuffRemoved(ref creature, this);
        }
    }
}
