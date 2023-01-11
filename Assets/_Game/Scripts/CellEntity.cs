using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts
{
    public class CellEntity : Grid
    {
        public bool isUsedForCalculation;
        public bool isOptimumPointForBomDrop;
        public bool isExploded;
        public BombEntity bombOnCell;
        public List<CellEntity> neighborCellsList;
    }
}