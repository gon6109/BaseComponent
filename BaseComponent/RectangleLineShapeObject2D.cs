using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// 四角形の線
    /// </summary>
    public class RectangleLineShapeObject2D : asd.GeometryObject2D
    {

        /// <summary>
        /// 描画領域
        /// </summary>
        public asd.RectF DrawingArea
        {
            get => drawingArea;
            set
            {
                drawingArea = value;
                foreach (var (obj, i) in rect.Select(obj => obj.Shape).OfType<asd.LineShape>().Select((obj, i) => (obj, i)))
                {
                    obj.StartingPosition = drawingArea.Vertexes[i];
                    obj.EndingPosition = drawingArea.Vertexes[(i + 1) % 4];
                }
            }
        }

        /// <summary>
        /// 線の太さ
        /// </summary>
        public float Thickness
        {
            get => thickness;
            set
            {
                thickness = value;
                foreach (var item in rect.Select(obj => obj.Shape).OfType<asd.LineShape>())
                {
                    item.Thickness = value;
                }
            }
        }

        asd.GeometryObject2D[] rect =
        {
            new asd.GeometryObject2D(),
            new asd.GeometryObject2D(),
            new asd.GeometryObject2D(),
            new asd.GeometryObject2D()
        };
        private float thickness;
        private asd.RectF drawingArea;

        public RectangleLineShapeObject2D()
        {
            foreach (var item in rect)
            {
                item.Shape = new asd.LineShape();
                AddDrawnChild(item, (asd.ChildManagementMode)0b1111, asd.ChildTransformingMode.All, (asd.ChildDrawingMode)0b11);
            }
        }

        protected override void OnUpdate()
        {
            foreach (var item in rect)
            {
                item.CameraGroup = CameraGroup;
            }

            base.OnUpdate();
        }
    }
}
