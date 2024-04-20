public partial class BtGrid
{
    public class SavedState
    {
        BtBlockData[,] blocks;

        public SavedState(BtGrid grid)
        {
            var width = BtGrid.width;
            var height = BtGrid.height;
            var arr = new BtBlockData[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    arr[x, y] = grid.blocks[x, y]?.data;
                }
            }

            blocks = arr;
        }

        public void Load(BtGrid grid)
        {
            var width = BtGrid.width;
            var height = BtGrid.height;
            var arr = new BtBlock[width, height];

            grid.columnBlockCount = new int[width];
            grid.rowBlockCount = new int[height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var oldBlock = grid.blocks[x, y];
                    if (oldBlock) Destroy(oldBlock.gameObject);

                    var data = blocks[x, y];

                    if (data)
                    {
                        arr[x, y] = grid.PlaceBlock(x, y, data);
                    }
                }
            }

            grid.blocks = arr;
        }
    }
}

