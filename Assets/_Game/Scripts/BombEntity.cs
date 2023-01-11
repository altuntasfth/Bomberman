namespace _Game.Scripts
{
    public class BombEntity : Grid
    {
        public bool isInOptimumCell;
        public bool isExploded;
        public CellEntity currentCell;

        public void Explode()
        {
            for (var i = 0; i < currentCell.neighborCellsList.Count; i++)
            {
                CellEntity cell = currentCell.neighborCellsList[i];
                if (!cell.isExploded)
                {
                    cell.gameObject.SetActive(false);
                    cell.isExploded = true;
                }
            }

            if (!currentCell.isExploded)
            {
                currentCell.isExploded = true;
                currentCell.gameObject.SetActive(false);
            }
            
            this.gameObject.SetActive(false);
            this.isExploded = true;
        }
    }
}