using System;
using System.Collections.Generic;
using Tetris.Manager;
using UnityEngine;

namespace Tetris.Shape
{
    /// <summary>
    /// 结点信息
    /// </summary>
    [Serializable]
    public struct TetrisNodeInfo
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Vector2Int position;

        /// <summary>
        /// 颜色
        /// </summary>
        public Sprite color;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="nodePosition">坐标</param>
        /// <param name="nodeColor">结点颜色</param>
        public TetrisNodeInfo(Vector2Int nodePosition, Sprite nodeColor)
        {
            position = nodePosition;
            color = nodeColor;
        }

        /// <summary>
        /// 加号运算符
        /// </summary>
        /// <param name="selfNode">自身结点</param>
        /// <param name="offset">加法因子</param>
        /// <returns></returns>
        public static TetrisNodeInfo operator +(TetrisNodeInfo selfNode, Vector2Int offset)
        {
            selfNode.position += offset;
            return selfNode;
        }
    }

    /// <summary>
    /// 旋转规则
    /// </summary>
    public struct RotateRule
    {
        public Vector2Int rule;
        public readonly bool isSwap;

        public RotateRule(int x, int y, bool swap)
        {
            rule = new Vector2Int(x, y);
            isSwap = swap;
        }
    }

    /// <summary>
    /// 抽象形状类
    /// </summary>
    [Serializable]
    public abstract class TetrisShape
    {
        /// <summary>
        /// 结点
        /// </summary>
        protected TetrisNodeInfo[] nodes;
        
        /// <summary>
        /// 旋转类型
        /// </summary>
        protected EM_ACTION_TYPE rotateType;
        
        /// <summary>
        /// 结点个数
        /// </summary>
        public int NodeCount => nodes.Length;

        #region Public Function

        /// <summary>
        /// 获取此形状中所有结点
        /// </summary>
        /// <returns>该形状中所有的结点</returns>
        public TetrisNodeInfo[] GetNodesInfo()
        {
            return nodes;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public TetrisNodeInfo[] Rotate(bool immediately)
        {
            var newNodes = rotateType switch
            {
                EM_ACTION_TYPE.Up => RotateRight(immediately),
                EM_ACTION_TYPE.Down => RotateLeft(immediately),
                EM_ACTION_TYPE.Left => RotateUp(immediately),
                EM_ACTION_TYPE.Right => RotateDown(immediately),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return newNodes;
        }

        /// <summary>
        /// 移动
        /// </summary>
        public void MoveLeft()
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] += new Vector2Int(0, -1);
            }
        }

        /// <summary>
        /// 右移
        /// </summary>
        public void MoveRight()
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] += new Vector2Int(0, 1);
            }
        }

        /// <summary>
        /// 下落
        /// </summary>
        public void MoveDown()
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] += new Vector2Int(-1, 0);
            }
        }

        #endregion

        #region Protected Function

        /// <summary>
        /// 上旋转规则
        /// </summary>
        protected RotateRule rotateRuleUp = new RotateRule(-1, 1, true);

        /// <summary>
        /// 下旋转规则
        /// </summary>
        protected RotateRule rotateRuleDown = new RotateRule(-1, 1, true);

        /// <summary>
        /// 左旋转规则
        /// </summary>
        protected RotateRule rotateRuleLeft = new RotateRule(-1, 1, true);

        /// <summary>
        /// 右旋转规则
        /// </summary>
        protected RotateRule rotateRuleRight = new RotateRule(-1, 1, true);

        #endregion
    
        #region Private Function

        /// <summary>
        /// 向上旋转
        /// </summary>
        private TetrisNodeInfo[] RotateUp(bool immediately)
        {
            if (immediately)
            {
                rotateType = EM_ACTION_TYPE.Up;
            }

            return RotateShape(rotateRuleUp, immediately);
        }

        /// <summary>
        /// 向下旋转
        /// </summary>
        private TetrisNodeInfo[] RotateDown(bool immediately)
        {
            if (immediately)
            {
                rotateType = EM_ACTION_TYPE.Down;
            }

            return RotateShape(rotateRuleDown, immediately);
        }

        /// <summary>
        /// 向左旋转
        /// </summary>
        private TetrisNodeInfo[] RotateLeft(bool immediately)
        {
            if (immediately)
            {
                rotateType = EM_ACTION_TYPE.Left;
            }

            return RotateShape(rotateRuleLeft, immediately);
        }

        /// <summary>
        /// 向右旋转
        /// </summary>
        private TetrisNodeInfo[] RotateRight(bool immediately)
        {
            if (immediately)
            {
                rotateType = EM_ACTION_TYPE.Right;
            }

            return RotateShape(rotateRuleRight, immediately);
        }

        /// <summary>
        /// 按规则旋转非参考点的所有结点
        /// </summary>
        private static void RotateAllNode(IList<TetrisNodeInfo> oldNodes, RotateRule rule)
        {
            for (var i = 1; i < oldNodes.Count; i++)
            {
                var offset = oldNodes[i].position - oldNodes[0].position;

                if (rule.isSwap)
                {
                    oldNodes[i] = oldNodes[0] + new Vector2Int(offset.y, offset.x) * rule.rule;
                }
                else
                {
                    oldNodes[i] = oldNodes[0] + new Vector2Int(offset.x, offset.y) * rule.rule;
                }
            }
        }

        /// <summary>
        /// 按规则旋转形状
        /// </summary>
        private TetrisNodeInfo[] RotateShape(RotateRule rule, bool immediately)
        {
            // 旋转当前形状
            if (immediately)
            {
                RotateAllNode(nodes, rule);
                return null;
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                // 拷贝一份形状
                var newNodes = new TetrisNodeInfo[nodes.Length];
                for (var i = 0; i < nodes.Length; i++)
                {
                    newNodes[i].position = nodes[i].position;
                }

                // 对拷贝的形状进行旋转并返回
                RotateAllNode(newNodes, rule);
                return newNodes;
            }
        }

        #endregion
    }
}