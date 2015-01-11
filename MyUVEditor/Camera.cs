using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace MyUVEditor
{
    public class Camera : IDisposable
    {
        static float defFOV = 0.43633194f; //25度
        public Vector3 Position { get; protected set; }
        public Vector3 Target { get; protected set; }
        public Vector3 UpDir { get; protected set; }
        public float FOV { get; protected set; }  //radian    less than 0 -> othro
        protected float scale = 1;
        protected float aspect = 1;
        public int Width { get { return Client.ClientSize.Width; } }
        public int Height { get { return Client.ClientSize.Height; } }
        public virtual Matrix World { get { return Matrix.Identity; } }
        public virtual Matrix View
        {
            get
            {
                return Matrix.LookAtLH(Position, Target, UpDir);
            }
        }
        public Matrix Projection
        {
            get
            {
                if (FOV > 0)
                    return Matrix.PerspectiveFovLH(FOV, (float)Width / Height, 1f, 1000);
                else
                {
                    float width = (float)Width / Height * scale * aspect;
                    float height = scale;
                    return Matrix.OrthoLH(width, height, 0.1f, 1000);
                }
            }
        }
        public Matrix Screen
        {
            get
            {
                Matrix sc = Matrix.Identity;
                sc.M11 = Width * 0.5f;
                sc.M22 = -Height * 0.5f;
                sc.M41 = Width * 0.5f;
                sc.M42 = Height * 0.5f;
                return sc;
            }
        }
        public Matrix WorldViewProjection { get; protected set; }
        public float TargetScreenZ
        {
            get
            {
                Matrix sc = Screen;
                Vector4 O = Vector4.Transform(
                    Vector3.Transform(
                        Target,
                        WorldViewProjection),
                    sc);
                return O.Z / O.W;
            }
        }
        private Control client;
        public Control Client
        {
            get { return client; }
            protected set { Console.WriteLine("client has been changed.value==null"+(value==null)); client = value; }
        }

        public Camera(Control client, float f = 0.43633194f)
        {
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
            FOV = f;
            Client = client;
            ResetWVPMatrix();
        }

        public Camera(DXView client, float f = 0.43633194f)
        {
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
            FOV = f;
            Client = client;
            ResetWVPMatrix();
        }

        public void SetEvent()
        {
            Client.MouseMove += (o, args) => { Camera_MouseMove(o, args); };
            Client.MouseWheel += (o, args) => { Camera_MouseWheel(o, args); };
        }

        public void SetClientSize(Control c)
        {
            ResetWVPMatrix();
        }

        protected void ResetWVPMatrix()
        {
            WorldViewProjection = World * View * Projection;
        }


        public Vector3 ScreenToLocal(Point p, float tgZ)
        {
            Vector3 mousePoint = new Vector3(p.X, p.Y, tgZ);
            Matrix invSC = Matrix.Invert(Screen);
            Matrix invWVP = Matrix.Invert(WorldViewProjection);
            return Vector3.TransformCoordinate(
                Vector3.TransformCoordinate(mousePoint, invSC),
                invWVP);
        }

        /// <summary>
        /// カメラ位置から指定スクリーン座標までの方向ベクトルをローカルで返す
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Ray ScreenToViewRay(Vector2 p)
        {
            Matrix invScreen = Matrix.Invert(Screen);
            Matrix invWVP = Matrix.Invert(WorldViewProjection);
            Vector4 pWorld4 = Vector4.Transform(Vector2.Transform(p, invScreen), invWVP);
            Vector3 pWorld3 = new Vector3(pWorld4.X, pWorld4.Y, pWorld4.Z) / pWorld4.W;
            return new Ray(Position, Vector3.Normalize(pWorld3 - Position));
        }

        protected void CameraMove(Point prev, Point tmp)
        {
            float tgZ = TargetScreenZ;
            Vector3 moved = Vector3.TransformCoordinate(ScreenToLocal(tmp, tgZ),World)
                - Vector3.TransformCoordinate(ScreenToLocal(prev, tgZ),World);
            Target -= moved;
            Position -= moved;
            ResetWVPMatrix();
        }

        protected virtual void CameraRotate(Point prev, Point tmp)
        {
            float d_theta = (tmp.Y - prev.Y) / 100f;
            float d_phi = (tmp.X - prev.X) / 100f;
            Vector3 eyeLine = Target - Position;
            Vector3 Cx = Vector3.Normalize(Vector3.Cross(UpDir, eyeLine));
            UpDir = Vector3.Normalize(Vector3.Cross(eyeLine, Cx));

            Matrix rotCx = Matrix.RotationAxis(Cx, d_theta);
            Matrix rotY = Matrix.RotationY(d_phi);
            eyeLine
                = Vector3.TransformCoordinate(
                Vector3.TransformCoordinate(eyeLine, rotCx), rotY);
            UpDir
                = Vector3.TransformCoordinate(
                Vector3.TransformCoordinate(UpDir, rotCx), rotY);
            Position = Target - eyeLine;
            UpDir.Normalize();
            ResetWVPMatrix();
        }

        protected void CameraDolly(int delta)
        {
            if (FOV > 0)
            {
                Vector3 tmpTtoP = (Position - Target) * (float)Math.Pow(1.001, -delta);
                float lsq = tmpTtoP.LengthSquared();
                if (lsq < 1e-6 || lsq > 1e+12)
                {
                    return;
                }
                Position = Target + tmpTtoP;
                ResetWVPMatrix();
            }
            else
            {
                this.scale *= (float)Math.Pow(0.999f, delta);
            }
        }
        protected void Camera_MouseMove(object sender, MouseEventArgs e)
        {
            var view = (DXView)sender;
            switch (e.Button)
            {
                case MouseButtons.Right:
                    CameraRotate(view.Prev_Point, e.Location);
                    break;
                case MouseButtons.Middle:
                    CameraMove(view.Prev_Point, e.Location);
                    break;
            }
        }

        protected void Camera_MouseWheel(object sender, MouseEventArgs e)
        {
            //Client = (Control)sender;
            CameraDolly(e.Delta);
        }

        #region 旧コード
        //protected float scale = 1f;

        protected Vector3 _target = new Vector3(0.5f, 0.5f, 0.0f);
        readonly Vector3 dir = new Vector3(0, 0, 10);
        readonly Vector3 up = new Vector3(0, -1, 0);


        public void ResetCamera()
        {
            this._target.X = 0.5f;
            this._target.Y = 0.5f;
            this._target.Z = 0;
            this.scale = 1;
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
        }

        public Matrix TransformProjection(Device device, float aspect)
        {
            float width = scale * device.Viewport.Width / device.Viewport.Height / aspect;
            return Matrix.OrthoLH(width, scale, 0.0f, 100f);
        }
        public Matrix TransformView()
        {
            return Matrix.LookAtLH(dir + _target, _target, up);
        }
        public void ScaleChange(int delta)
        {
            this.scale *= (float)Math.Pow(0.999f, delta);

        }


        public void MoveTarget(Device device, Point current, Point old, bool isShift)
        {
            Vector3 nowpoint = Screen2World(device, current);
            Vector3 oldpoint = Screen2World(device, old);
            if (isShift) { this._target -= (nowpoint - oldpoint) * 0.01f; }
            else { this._target -= nowpoint - oldpoint; }

        }

        public static Vector3 Screen2World(Device device, Point point)
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
            Vector4 tmp = Vector3.Transform(new Vector3(point.X, point.Y, 0), trans);
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

        public bool IsContainIt(Device device, Point p1, Point p2, Vector3 vecVertex)
        {
            return IsContainIt(Screen2World(device, p1), Screen2World(device, p2), vecVertex);
        }

        public bool PointIsNear(Device device, Point p, Vector3 vecVertex)
        {
            const int area = 4;
            Point p1 = new Point(p.X - area, p.Y - area);
            Point p2 = new Point(p.X + area, p.Y + area);
            return IsContainIt(device, p1, p2, vecVertex);
        }
        #endregion

        public void Dispose()
        {
            Client.MouseMove -= (o, args) => { Camera_MouseMove(o, args); };
            Client.MouseWheel -= (o, args) => { Camera_MouseWheel(o, args); };
            Client = null;
        }
    }
}
