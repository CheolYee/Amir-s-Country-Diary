using System.Collections.Generic;
using UnityEngine;

namespace PBG_01_PUSE
{
    public class Inventory : MonoBehaviour
    {
        [field: SerializeField] public List<GameObject> PuseCount { get; set; }
    }

}
