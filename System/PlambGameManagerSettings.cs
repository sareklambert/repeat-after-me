using System.Collections.Generic;
using UnityEngine;

namespace Plamb.System
{
    /// <summary>
    /// This class defines the settings for the game manager.
    /// </summary>
    [CreateAssetMenu(fileName = "PlambGameManagerSettings", menuName = "Plamb/GameManagerSettings", order = 2)]
    public class PlambGameManagerSettings : ScriptableObject
    {
        [Range (1, 10)]
        public float maxLoopDuration;
   
        [Range(1, 10)]
        public int maxClones;

        public List<Material> cloneMaterials;
        public List<Color> cloneColors;
    }
}
