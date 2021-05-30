using Tetris.Manager;
using UnityEngine;

namespace Tetris.Shape
{
    public class TetrisShapeI : TetrisShape
    {
        public TetrisShapeI(Vector2Int keyNodePosition, Sprite color)
        {
            nodes = new TetrisNodeInfo[4];
            
            nodes[0] = new TetrisNodeInfo(keyNodePosition, color);
            nodes[1] = nodes[0] + new Vector2Int(0, -1);
            nodes[2] = nodes[0] + new Vector2Int(0, +1);
            nodes[3] = nodes[0] + new Vector2Int(0, +2);

            rotateType = EM_ACTION_TYPE.Up;
            
            rotateRuleUp = new RotateRule(1, -1, true);
            rotateRuleDown = new RotateRule(1, -1, true);
        }
    }
}