using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridBoard
{
    public class TileDragModule : MonoBehaviour, Board.IModule
    {
        [SerializeField] float clickCooldown = 1f;

        Board board;

        Tile currentTile;
        bool leftTileSpace;

        CooldownComponent cooldown;
        bool ClickExpired => cooldown.Expired;

        public void InitBoard(Board board)
        {
            this.board = board;
            board.OnMousePosChanged.AddListener(HandleBoardMousePosition);
        }

        void MouseDown()
        {
            var mouseTile = board.mouseTile;
            if (!mouseTile) return;

            cooldown = new CooldownComponent(clickCooldown);

            leftTileSpace = false;
            currentTile = mouseTile;

        }

        void MouseUp()
        {
            if (!currentTile) return;

            if (!leftTileSpace)
            {
                if (!ClickExpired && currentTile.CanClick) currentTile.Click();
            }
            else
            {
                var targetTile = board.mouseTile;
                if (targetTile != currentTile && currentTile.CanDrag)
                {
                    currentTile.DragTo(board, board.mousePos, board.mouseTile);
                }
            }

            currentTile.transform.localPosition = Vector2.zero;
            currentTile.SetRenderLayer(Tile.RenderLayer.Board);
            currentTile = null;
            foreach (var item in board.GetNonEmptyTiles())
            {
                item.SetAttention(false);
            }
        }

        private void Update()
        {
            if (!board) return;

            if (Input.GetMouseButtonDown(0))
            {
                MouseDown();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
            }

            if (currentTile && leftTileSpace && currentTile.CanDrag)
            {
                currentTile.transform.position = Helpers.MouseToWorldPosition();
                // ToDo: add landing indicator / shadow
            }
        }

        void HandleBoardMousePosition(Vector2Int? position)
        {
            if (leftTileSpace || !currentTile) return;
            leftTileSpace = true;
            currentTile.SetRenderLayer(Tile.RenderLayer.Hover);

            foreach (var item in board.GetNonEmptyTiles())
            {
                if (item.CanAccept(currentTile)) item.SetAttention(true);
            }
        }

    }
}
