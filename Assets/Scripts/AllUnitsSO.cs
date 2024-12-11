using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllUnitsSO", menuName = "AllUnitsSO", order = 1)]
public class AllUnitsSO : ScriptableObject
{
    public List<GameObject> allUnits;
}
