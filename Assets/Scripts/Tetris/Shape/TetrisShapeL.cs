﻿using Tetris.Manager;
using UnityEngine;

namespace Tetris.Shape
{
    public class TetrisShapeL : TetrisShape
    {
        public TetrisShapeL(Vector2Int origin, Sprite color)
        {
            nodes = new TetrisNodeInfo[4];

            nodes[0] = new TetrisNodeInfo(origin, color);
            nodes[1] = nodes[0] + new Vector2Int(+1, +1);
            nodes[2] = nodes[0] + new Vector2Int(0, -1);
            nodes[3] = nodes[0] + new Vector2Int(0, +1);

            rotateType = EM_ACTION_TYPE.Up;
        }
    }
}