using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts
{
    public class CellEntity : Grid
    {
        public bool isUsedForCalculation;
        public bool isOptimumPointForBomDrop;
        public List<CellEntity> neighborCellsList;
    }
}