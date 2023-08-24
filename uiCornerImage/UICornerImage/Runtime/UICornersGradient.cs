using UnityEngine;
using UnityEngine.UI;

namespace UI.CornerImage.Runtime
{
    [AddComponentMenu("UI/Effects/4 Corners Gradient")]
    public class UICornersGradient : BaseMeshEffect
    {
        public Color m_StartColor = Color.white;
        public Color m_EndColor = Color.white;

        /// <summary>
        /// true 左右，false为上下
        /// </summary>
        public float m_Angle = 0f;

        private Color m_bottomLeftColor = Color.white;
        private Color m_bottomRightColor = Color.white;
        private Color m_topLeftColor = Color.white;
        private Color m_topRightColor = Color.white;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (enabled)
            {
                m_topLeftColor = m_StartColor;
                m_topRightColor = m_StartColor;
                m_bottomLeftColor = m_EndColor;
                m_bottomRightColor = m_EndColor;

                Rect rect = graphic.rectTransform.rect;
                UIGradientUtils.Matrix2x3 localPositionMatrix =
                    UIGradientUtils.LocalPositionMatrix(rect, UIGradientUtils.RotationDir(m_Angle));

                UIVertex vertex = default(UIVertex);
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector2 normalizedPosition = localPositionMatrix * vertex.position;
                    vertex.color *= UIGradientUtils.Bilerp(m_bottomLeftColor, m_bottomRightColor, m_topLeftColor,
                        m_topRightColor, normalizedPosition);
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}