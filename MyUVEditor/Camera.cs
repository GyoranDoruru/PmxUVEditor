using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace MyUVEditor
{
    public class Camera
    {
        private Device device;
        private float vpWidth;
        private float vpHeight;
        private float scale = 1f;

        private Vector3 _target = new Vector3(0.5f, 0.5f, 0.0f);
        readonly Vector3 dir = new Vector3(0, 0, 10);
        readonly Vector3 up = new Vector3(0,-1,0);

        public Camera(Device device) { this.device = device; }
        public void ResetCamera(Device device)
        {
            this.device = device;
            this._target.X = 0.5f;
            this._target.Y = 0.5f;
            this._target.Z = 0;
            this.scale = 1;
        }

        public void SetViewPortSize(Form1 form)
        {
            this.vpWidth = form.ViewPanel.Width;
            this.vpHeight = form.ViewPanel.Height;
        }

        public Matrix TransformProjection(float aspect)
        {
            return Matrix.OrthoLH(scale * this.vpWidth / this.vpHeight/aspect, scale, 0.0f, 100f);
        }
        public Matrix TransformView()
        {
            return Matrix.LookAtLH(dir + _target, _target, up);
        }
        public void ScaleChange(int delta)
        {
            this.scale *= (float)Math.Pow(0.999f, delta);

        }


        public void MoveTarget(Point current, Point old,bool isShift)
        {
            Vector3 nowpoint = Screen2World(this.device,current);
            Vector3 oldpoint = Screen2World(this.device,old);
            if (isShift) { this._target -= (nowpoint - oldpoint) * 0.01f; }
            else { this._target -= nowpoint - oldpoint; }

        }

        public static Vector3 Screen2World(Device device,Point point)
        {
            Matrix project = device.GetTransform(TransformState.Projection);
            Matrix view = device.GetTransform(TransformState.View);
            float height = device.Viewport.Height;
            float width = device.Viewport.Width;
            Matrix viewport = Matrix.Identity;
            viewport.M11 = width / 2.0f;
            viewport.M22 = -height / 2.0f;
            viewport.M41 = width / 2.0f;
            viewport.M42 = height / 2.0f;
            project.Invert(); view.Invert(); viewport.Invert();
            Matrix trans = viewport * project * view;
            Vector4 tmp = Vector3.Transform(new Vector3(point.X,point.Y, 0), trans);
            return new Vector3(tmp.X, tmp.Y, 0);
        }

        public bool IsContainIt(Vector3 vec1, Vector3 vec2, Vector3 vecVertex)
        {
            float fx = (vecVertex.X - vec1.X) * (vecVertex.X - vec2.X);
            if (fx > 0) return false;
            float fy = (vecVertex.Y - vec1.Y) * (vecVertex.Y - vec2.Y);
            if (fy > 0) return false;
            else return true;
        }

        public bool IsContainIt(Point p1, Point p2, Vector3 vecVertex)
        {
            return IsContainIt(Screen2World(this.device,p1), Screen2World(this.device,p2), vecVertex);
        }

        public bool PointIsNear(Point p, Vector3 vecVertex)
        {
            const int area = 4;
            Point p1 = new Point(p.X - area, p.Y - area);
            Point p2 = new Point(p.X + area, p.Y + area);
            return IsContainIt(p1, p2, vecVertex);
        }

    }
}
