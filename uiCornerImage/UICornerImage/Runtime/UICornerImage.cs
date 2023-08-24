using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System.Collections.Generic;

namespace UI.CornerImage.Runtime
{
    public class UICornerImage : Image
    {
        //每个角最大的三角形数，一般5-8个就有不错的圆角效果，设置Max防止不必要的性能浪费
        const int MaxTriangleNum = 30;
        const int MinTriangleNum = 1;

        public float Radius;
        public float Border;

        public bool EnableLeftTop = true;
        public bool EnableRightTop = true;
        public bool EnableLeftBottom = true;
        public bool EnableRightBottom = true;

        //使用几个三角形去填充每个角的四分之一圆
        [Range(MinTriangleNum, MaxTriangleNum)]
        public int TriangleNum;

        /**
         * v
         *             ^
         *             |
         *    --------w|--------
         *    |        |       |
         *    |        |(0, 0) |
         *----x--------|-------z---->
         *    |        |       |
         *    |        |       |
         *    --------y|--------
         *             |
         */
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Vector4 v = GetDrawingDimensions(false);
            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            var color32 = color;
            vh.Clear();
            //对radius的值做限制，必须在0-较小的边的1/2的范围内
            float radius = Radius;
            if (radius > (v.z - v.x) / 2) radius = (v.z - v.x) / 2;
            if (radius > (v.w - v.y) / 2) radius = (v.w - v.y) / 2;
            if (radius < 0) radius = 0;
            //计算出uv中对应的半径值坐标轴的半径
            float uvRadiusX = radius / (v.z - v.x);
            float uvRadiusY = radius / (v.w - v.y);

            if (Border > 0)
            {
                Vector4 vCorner = v + new Vector4(Border, Border, -Border, -Border);
                Vector4 uvCorner = uv;
                //0，1
                vh.AddVert(new Vector3(v.x, v.w - radius), color32, new Vector2(uv.x, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.x, v.y + radius), color32, new Vector2(uv.x, uv.y + uvRadiusY));

                //2，3，4，5
                vh.AddVert(new Vector3(v.x + radius, v.w), color32, new Vector2(uv.x + uvRadiusX, uv.w));
                vh.AddVert(new Vector3(v.x + radius, v.w - radius), color32,
                    new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.x + radius, v.y + radius), color32,
                    new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
                vh.AddVert(new Vector3(v.x + radius, v.y), color32, new Vector2(uv.x + uvRadiusX, uv.y));

                //6，7，8，9
                vh.AddVert(new Vector3(v.z - radius, v.w), color32, new Vector2(uv.z - uvRadiusX, uv.w));
                vh.AddVert(new Vector3(v.z - radius, v.w - radius), color32,
                    new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.z - radius, v.y + radius), color32,
                    new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
                vh.AddVert(new Vector3(v.z - radius, v.y), color32, new Vector2(uv.z - uvRadiusX, uv.y));

                //10，11
                vh.AddVert(new Vector3(v.z, v.w - radius), color32, new Vector2(uv.z, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.z, v.y + radius), color32, new Vector2(uv.z, uv.y + uvRadiusY));

                var cornerBeginIndex = vh.currentVertCount;
                //0，1
                vh.AddVert(new Vector3(vCorner.x, vCorner.w - radius), color32,
                    new Vector2(uvCorner.x, uvCorner.w - uvRadiusY));
                vh.AddVert(new Vector3(vCorner.x, vCorner.y + radius), color32,
                    new Vector2(uvCorner.x, uvCorner.y + uvRadiusY));

                //2，3，4，5
                vh.AddVert(new Vector3(vCorner.x + radius, vCorner.w), color32,
                    new Vector2(uvCorner.x + uvRadiusX, uvCorner.w));
                vh.AddVert(new Vector3(vCorner.x + radius, vCorner.w - radius), color32,
                    new Vector2(uvCorner.x + uvRadiusX, uvCorner.w - uvRadiusY));
                vh.AddVert(new Vector3(vCorner.x + radius, vCorner.y + radius), color32,
                    new Vector2(uvCorner.x + uvRadiusX, uvCorner.y + uvRadiusY));
                vh.AddVert(new Vector3(vCorner.x + radius, vCorner.y), color32,
                    new Vector2(uvCorner.x + uvRadiusX, uvCorner.y));

                //6，7，8，9
                vh.AddVert(new Vector3(vCorner.z - radius, vCorner.w), color32,
                    new Vector2(uvCorner.z - uvRadiusX, uvCorner.w));
                vh.AddVert(new Vector3(vCorner.z - radius, vCorner.w - radius), color32,
                    new Vector2(uvCorner.z - uvRadiusX, uvCorner.w - uvRadiusY));
                vh.AddVert(new Vector3(vCorner.z - radius, vCorner.y + radius), color32,
                    new Vector2(uvCorner.z - uvRadiusX, uvCorner.y + uvRadiusY));
                vh.AddVert(new Vector3(vCorner.z - radius, vCorner.y), color32,
                    new Vector2(uvCorner.z - uvRadiusX, uvCorner.y));

                //10，11
                vh.AddVert(new Vector3(vCorner.z, vCorner.w - radius), color32,
                    new Vector2(uvCorner.z, uvCorner.w - uvRadiusY));
                vh.AddVert(new Vector3(vCorner.z, vCorner.y + radius), color32,
                    new Vector2(uvCorner.z, uvCorner.y + uvRadiusY));

                //左边的矩形
                vh.AddTriangle(1, 0, cornerBeginIndex);
                vh.AddTriangle(1, 0 + cornerBeginIndex, 1 + cornerBeginIndex);
                //中间的矩形
                vh.AddTriangle(2 + cornerBeginIndex, 2, 6);
                vh.AddTriangle(2 + cornerBeginIndex, 6, 6 + cornerBeginIndex);
                vh.AddTriangle(5, 9 + cornerBeginIndex, 9);
                vh.AddTriangle(5 + cornerBeginIndex, 5, 9 + cornerBeginIndex);
                //右边的矩形
                vh.AddTriangle(10 + cornerBeginIndex, 11 + cornerBeginIndex, 10);
                vh.AddTriangle(11 + cornerBeginIndex, 10, 11);

                //开始构造四个角
                List<Vector2> vCenterList = new List<Vector2>();
                List<Vector2> uvCenterList = new List<Vector2>();
                List<int> vCenterVertList = new List<int>();

                //右上角的圆心
                vCenterList.Add(new Vector2(v.z - radius, v.w - radius));
                uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
                vCenterVertList.Add(7);

                //左上角的圆心
                vCenterList.Add(new Vector2(v.x + radius, v.w - radius));
                uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
                vCenterVertList.Add(3);

                //左下角的圆心
                vCenterList.Add(new Vector2(v.x + radius, v.y + radius));
                uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
                vCenterVertList.Add(4);

                //右下角的圆心
                vCenterList.Add(new Vector2(v.z - radius, v.y + radius));
                uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
                vCenterVertList.Add(8);

                #region corner

                var radiusCorner = radius;
                var uvRadiusCornerX = radiusCorner / (v.z - v.x);
                var uvRadiusCornerY = radiusCorner / (v.w - v.y);

                //右上角的圆心
                vCenterList.Add(new Vector2(vCorner.z - radiusCorner, vCorner.w - radiusCorner));
                uvCenterList.Add(new Vector2(uvCorner.z - uvRadiusCornerX, uvCorner.w - uvRadiusCornerY));
                vCenterVertList.Add(7 + cornerBeginIndex);

                //左上角的圆心
                vCenterList.Add(new Vector2(vCorner.x + radiusCorner, vCorner.w - radiusCorner));
                uvCenterList.Add(new Vector2(uvCorner.x + uvRadiusCornerX, uvCorner.w - uvRadiusCornerY));
                vCenterVertList.Add(3 + cornerBeginIndex);

                //左下角的圆心
                vCenterList.Add(new Vector2(vCorner.x + radiusCorner, vCorner.y + radiusCorner));
                uvCenterList.Add(new Vector2(uvCorner.x + uvRadiusCornerX, uvCorner.y + uvRadiusCornerY));
                vCenterVertList.Add(4 + cornerBeginIndex);

                //右下角的圆心
                vCenterList.Add(new Vector2(vCorner.z - radiusCorner, vCorner.y + radiusCorner));
                uvCenterList.Add(new Vector2(uvCorner.z - uvRadiusCornerX, uvCorner.y + uvRadiusCornerY));
                vCenterVertList.Add(8 + cornerBeginIndex);

                #endregion

                //每个三角形的顶角
                float degreeDelta = (Mathf.PI / 2 / TriangleNum);
                //当前的角度
                float curDegree = 0;

                List<int> vCornerRoundVertIndexList = new List<int>();
                for (int i = 0; i < vCenterVertList.Count; i++)
                {
                    int preVertNum = vh.currentVertCount;
                    vCornerRoundVertIndexList.Add(preVertNum);
                    for (int j = 0; j <= TriangleNum; j++)
                    {
                        float cosA = Mathf.Cos(curDegree);
                        float sinA = Mathf.Sin(curDegree);
                        Vector3 vPosition;
                        Vector3 uvPosition;

                        if (i == 0 || i == 4)
                        {
                            if (EnableRightTop)
                            {
                                if (i == 0)
                                {
                                    vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                        vCenterList[i].y + sinA * radius);
                                    uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                        uvCenterList[i].y + sinA * uvRadiusY);
                                }
                                else
                                {
                                    var index = i % 4;
                                    vPosition = new Vector3(vCenterList[index].x + cosA * (radius - Border),
                                        vCenterList[index].y + sinA * (radius - Border));
                                    uvPosition = new Vector2(uvCenterList[index].x + cosA * (uvRadiusX - Border),
                                        uvCenterList[index].y + sinA * (uvRadiusY - Border));
                                }
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else if (i == 1 || i == 5)
                        {
                            if (EnableLeftTop)
                            {
                                if (i == 1)
                                {
                                    vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                        vCenterList[i].y + sinA * radius);
                                    uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                        uvCenterList[i].y + sinA * uvRadiusY);
                                }
                                else
                                {
                                    var index = i % 4;
                                    vPosition = new Vector3(vCenterList[index].x + cosA * (radius - Border),
                                        vCenterList[index].y + sinA * (radius - Border));
                                    uvPosition = new Vector2(uvCenterList[index].x + cosA * (uvRadiusX - Border),
                                        uvCenterList[index].y + sinA * (uvRadiusY - Border));
                                }
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else if (i == 2 || i == 6)
                        {
                            if (EnableLeftBottom)
                            {
                                if (i == 2)
                                {
                                    vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                        vCenterList[i].y + sinA * radius);
                                    uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                        uvCenterList[i].y + sinA * uvRadiusY);
                                }
                                else
                                {
                                    var index = i % 4;
                                    vPosition = new Vector3(vCenterList[index].x + cosA * (radius - Border),
                                        vCenterList[index].y + sinA * (radius - Border));
                                    uvPosition = new Vector2(uvCenterList[index].x + cosA * (uvRadiusX - Border),
                                        uvCenterList[index].y + sinA * (uvRadiusY - Border));
                                }
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else if (i == 3 || i == 7)
                        {
                            if (EnableRightBottom)
                            {
                                if (i == 3)
                                {
                                    vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                        vCenterList[i].y + sinA * radius);
                                    uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                        uvCenterList[i].y + sinA * uvRadiusY);
                                }
                                else
                                {
                                    var index = i % 4;
                                    vPosition = new Vector3(vCenterList[index].x + cosA * (radius - Border),
                                        vCenterList[index].y + sinA * (radius - Border));
                                    uvPosition = new Vector2(uvCenterList[index].x + cosA * (uvRadiusX - Border),
                                        uvCenterList[index].y + sinA * (uvRadiusY - Border));
                                }
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else
                        {
                            vPosition = new Vector3(vCenterList[i].x + cosA * radius, vCenterList[i].y + sinA * radius);
                            uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                uvCenterList[i].y + sinA * uvRadiusY);
                        }

                        vh.AddVert(vPosition, color32, uvPosition);
                        curDegree += degreeDelta;
                    }

                    curDegree -= degreeDelta;
                }

                for (int i = 0; i < vCenterVertList.Count / 2; i++)
                {
                    int preVertNum = vCornerRoundVertIndexList[i];
                    int preVertInnerNum = vCornerRoundVertIndexList[i + 4];
                    for (int j = 0; j <= TriangleNum - 1; j++)
                    {
                        vh.AddTriangle(preVertInnerNum + j, preVertNum + j + 1, preVertNum + j);
                        vh.AddTriangle(preVertInnerNum + j + 1, preVertNum + j + 1, preVertNum + j);
                        vh.AddTriangle(preVertNum + j, preVertInnerNum + j + 1, preVertInnerNum + j);
                    }

                    if (i == 0 && EnableRightTop)
                    {
                        vh.AddTriangle(10, 10 + cornerBeginIndex, preVertInnerNum);
                        vh.AddTriangle(6, 6 + cornerBeginIndex, preVertInnerNum + TriangleNum);
                    }
                    else if (i == 1 && EnableLeftTop)
                    {
                        vh.AddTriangle(2, 2 + cornerBeginIndex, preVertInnerNum);
                        vh.AddTriangle(0, 0 + cornerBeginIndex, preVertInnerNum + TriangleNum);
                    }
                    else if (i == 2 && EnableLeftBottom)
                    {
                        vh.AddTriangle(1, 1 + cornerBeginIndex, preVertInnerNum);
                        vh.AddTriangle(5, 5 + cornerBeginIndex, preVertInnerNum + TriangleNum);
                    }
                    else if (i == 3 && EnableRightBottom)
                    {
                        vh.AddTriangle(9, 9 + cornerBeginIndex, preVertInnerNum);
                        vh.AddTriangle(11, 11 + cornerBeginIndex, preVertInnerNum + TriangleNum);
                    }
                }

                if (!EnableLeftTop)
                {
                    vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
                    vh.AddVert(new Vector3(v.x + Border, v.w - Border), color32,
                        new Vector2(uv.x + Border, uv.w - Border));
                    vh.AddTriangle(0, 0 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(2, 2 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(0 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                    vh.AddTriangle(2 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                }

                if (!EnableRightTop)
                {
                    vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
                    vh.AddVert(new Vector3(v.z - Border, v.w - Border), color32,
                        new Vector2(uv.z - Border, uv.w - Border));
                    vh.AddTriangle(6, 6 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(10, 10 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(6 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                    vh.AddTriangle(10 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                }

                if (!EnableLeftBottom)
                {
                    vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector3(v.x + Border, v.y + Border), color32,
                        new Vector2(uv.x + Border, uv.y + Border));
                    vh.AddTriangle(1, 1 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(5, 5 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(1 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                    vh.AddTriangle(5 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                }

                if (!EnableRightBottom)
                {
                    vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));
                    vh.AddVert(new Vector3(v.z - Border, v.y + Border), color32,
                        new Vector2(uv.z - Border, uv.y + Border));
                    vh.AddTriangle(9, 9 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(11, 11 + cornerBeginIndex, vh.currentVertCount - 2);
                    vh.AddTriangle(9 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                    vh.AddTriangle(11 + cornerBeginIndex, vh.currentVertCount - 1, vh.currentVertCount - 2);
                }
            }
            else
            {
                //0，1
                vh.AddVert(new Vector3(v.x, v.w - radius), color32, new Vector2(uv.x, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.x, v.y + radius), color32, new Vector2(uv.x, uv.y + uvRadiusY));

                //2，3，4，5
                vh.AddVert(new Vector3(v.x + radius, v.w), color32, new Vector2(uv.x + uvRadiusX, uv.w));
                vh.AddVert(new Vector3(v.x + radius, v.w - radius), color32,
                    new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.x + radius, v.y + radius), color32,
                    new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
                vh.AddVert(new Vector3(v.x + radius, v.y), color32, new Vector2(uv.x + uvRadiusX, uv.y));

                //6，7，8，9
                vh.AddVert(new Vector3(v.z - radius, v.w), color32, new Vector2(uv.z - uvRadiusX, uv.w));
                vh.AddVert(new Vector3(v.z - radius, v.w - radius), color32,
                    new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.z - radius, v.y + radius), color32,
                    new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
                vh.AddVert(new Vector3(v.z - radius, v.y), color32, new Vector2(uv.z - uvRadiusX, uv.y));

                //10，11
                vh.AddVert(new Vector3(v.z, v.w - radius), color32, new Vector2(uv.z, uv.w - uvRadiusY));
                vh.AddVert(new Vector3(v.z, v.y + radius), color32, new Vector2(uv.z, uv.y + uvRadiusY));

                //左边的矩形
                vh.AddTriangle(1, 0, 3);
                vh.AddTriangle(1, 3, 4);
                //中间的矩形
                vh.AddTriangle(5, 2, 6);
                vh.AddTriangle(5, 6, 9);
                //右边的矩形
                vh.AddTriangle(8, 7, 10);
                vh.AddTriangle(8, 10, 11);

                //开始构造四个角
                List<Vector2> vCenterList = new List<Vector2>();
                List<Vector2> uvCenterList = new List<Vector2>();
                List<int> vCenterVertList = new List<int>();

                //右上角的圆心
                vCenterList.Add(new Vector2(v.z - radius, v.w - radius));
                uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
                vCenterVertList.Add(7);

                //左上角的圆心
                vCenterList.Add(new Vector2(v.x + radius, v.w - radius));
                uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
                vCenterVertList.Add(3);

                //左下角的圆心
                vCenterList.Add(new Vector2(v.x + radius, v.y + radius));
                uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
                vCenterVertList.Add(4);

                //右下角的圆心
                vCenterList.Add(new Vector2(v.z - radius, v.y + radius));
                uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
                vCenterVertList.Add(8);

                //每个三角形的顶角
                float degreeDelta = (Mathf.PI / 2 / TriangleNum);
                //当前的角度
                float curDegree = 0;

                for (int i = 0; i < vCenterVertList.Count; i++)
                {
                    int preVertNum = vh.currentVertCount;
                    for (int j = 0; j <= TriangleNum; j++)
                    {
                        float cosA = Mathf.Cos(curDegree);
                        float sinA = Mathf.Sin(curDegree);
                        Vector3 vPosition;
                        Vector3 uvPosition;

                        if (i == 0)
                        {
                            if (EnableRightTop)
                            {
                                vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                    vCenterList[i].y + sinA * radius);
                                uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                    uvCenterList[i].y + sinA * uvRadiusY);
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else if (i == 1)
                        {
                            if (EnableLeftTop)
                            {
                                vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                    vCenterList[i].y + sinA * radius);
                                uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                    uvCenterList[i].y + sinA * uvRadiusY);
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else if (i == 2)
                        {
                            if (EnableLeftBottom)
                            {
                                vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                    vCenterList[i].y + sinA * radius);
                                uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                    uvCenterList[i].y + sinA * uvRadiusY);
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else if (i == 3)
                        {
                            if (EnableRightBottom)
                            {
                                vPosition = new Vector3(vCenterList[i].x + cosA * radius,
                                    vCenterList[i].y + sinA * radius);
                                uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                    uvCenterList[i].y + sinA * uvRadiusY);
                            }
                            else
                            {
                                vPosition = new Vector3(vCenterList[i].x, vCenterList[i].y);
                                uvPosition = new Vector2(uvCenterList[i].x, uvCenterList[i].y);
                            }
                        }
                        else
                        {
                            vPosition = new Vector3(vCenterList[i].x + cosA * radius, vCenterList[i].y + sinA * radius);
                            uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                                uvCenterList[i].y + sinA * uvRadiusY);
                        }

                        vh.AddVert(vPosition, color32, uvPosition);
                        curDegree += degreeDelta;
                    }

                    curDegree -= degreeDelta;
                    for (int j = 0; j <= TriangleNum - 1; j++)
                    {
                        vh.AddTriangle(vCenterVertList[i], preVertNum + j + 1, preVertNum + j);
                    }
                }

                if (!EnableLeftTop)
                {
                    vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
                    vh.AddTriangle(0, 2, vh.currentVertCount - 1);

                    vh.AddVert(
                        new Vector2(v.x + radius, v.w - radius), color32,
                        new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
                    vh.AddTriangle(0, 2, vh.currentVertCount - 1);
                }

                if (!EnableRightTop)
                {
                    vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
                    vh.AddTriangle(6, 10, vh.currentVertCount - 1);

                    vh.AddVert(
                        new Vector2(v.z - radius, v.w - radius),
                        color32,
                        new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
                    vh.AddTriangle(6, 10, vh.currentVertCount - 1);
                }

                if (!EnableLeftBottom)
                {
                    vh.AddVert(new Vector3(v.x, v.y),
                        color32, new Vector2(uv.x, uv.y));
                    vh.AddTriangle(1, 5, vh.currentVertCount - 1);

                    vh.AddVert(
                        new Vector2(v.x + radius, v.y + radius),
                        color32,
                        new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
                    vh.AddTriangle(1, 5, vh.currentVertCount - 1);
                }

                if (!EnableRightBottom)
                {
                    vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));
                    vh.AddTriangle(9, 11, vh.currentVertCount - 1);
                    vh.AddVert(
                        new Vector2(v.z - radius, v.y + radius),
                        color32,
                        new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
                    vh.AddTriangle(9, 11, vh.currentVertCount - 1);
                }
            }
        }

        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
            Rect r = GetPixelAdjustedRect();
            var size = overrideSprite == null
                ? new Vector2(r.width, r.height)
                : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
            //Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            var v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

            v = new Vector4(
                r.x + r.width * v.x,
                r.y + r.height * v.y,
                r.x + r.width * v.z,
                r.y + r.height * v.w
            );

            return v;
        }
    }
}