using System;
using System.Windows.Forms;
using SlimDX;
using System.Drawing;

namespace MyUVEditor.Camera
{
    public class Camera3D : IDisposable,ICamera
    {
        static float defFOV = 0.43633194f; //25度
        public Vector3 Position { get; protected set; }
        public Vector3 Direction { get { return Vector3.Normalize(Target - Position); } }
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
                    return Matrix.OrthoLH(width, height, 1.0f, 1000);
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

        public Camera3D(Control client, float f = 0.43633194f)
        {
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
            FOV = f;
            Client = client;
            SetEvent();
            ResetWVPMatrix();
        }

        public Camera3D(DXView client, float f = 0.43633194f)
        {
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
            FOV = f;
            Client = client;
            SetEvent();
            ResetWVPMatrix();
        }

        public void SetEvent()
        {
            Client.ClientSizeChanged += (o, args) => { SetClientSize((Control)o); };
            Client.MouseMove += (o, args) => { Camera_MouseMove(o, args); };
            Client.MouseWheel += (o, args) => { Camera_MouseWheel(o, args); };
            Client.MouseDoubleClick += (o, args) => { Camera_MouseDoubleClick(o, args); };
        }

        public void SetClientSize(Control c)
        {
            ResetWVPMatrix();
        }

        protected void ResetWVPMatrix()
        {
            WorldViewProjection = World * View * Projection;
        }

        public Vector3 ScreenToWorldVec(Point p)
        {
            return ScreenToWorldVec(p, TargetScreenZ);
        }

        public Vector3 ScreenToWorldVec(Point p, float tgZ)
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
        public Ray ScreenToWorldRay(Point p)
        {
            Matrix invScreen = Matrix.Invert(Screen);
            Matrix invWVP = Matrix.Invert(WorldViewProjection);
            Vector3 pWorld3 = ScreenToWorldVec(p, 0);

            return new Ray(Position, Vector3.Normalize(pWorld3 - Position));
        }

        protected Point PrevPoint;

        protected virtual void CameraMove(Point tmp)
        {
            float tgZ = TargetScreenZ;
            Vector3 moved = Vector3.TransformCoordinate(ScreenToWorldVec(tmp, tgZ),World)
                - Vector3.TransformCoordinate(ScreenToWorldVec(PrevPoint, tgZ),World);
            Target -= moved;
            Position -= moved;
            ResetWVPMatrix();
        }

        protected virtual void CameraRotate(Point tmp)
        {
            float d_theta = (tmp.Y - PrevPoint.Y) / 100f;
            float d_phi = (tmp.X - PrevPoint.X) / 100f;
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

        protected virtual void CameraDolly(int delta)
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
            }
            else
            {
                this.scale *= (float)Math.Pow(0.999f, delta);
            }
            ResetWVPMatrix();
        }
        protected void Camera_MouseMove(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    CameraRotate(e.Location);
                    break;
                case MouseButtons.Middle:
                    CameraMove(e.Location);
                    break;
            }
            PrevPoint = e.Location;
        }

        protected void Camera_MouseWheel(object sender, MouseEventArgs e)
        {
            //Client = (Control)sender;
            CameraDolly(e.Delta);
        }

        protected void Camera_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ResetCamera();
        }

        public virtual void ResetCamera()
        {
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
            ResetWVPMatrix();
        }

        //world
        public bool IsContainIt(Vector3 rec1, Vector3 rec2, Vector3 comp)
        {
            return BoundingBox.Contains(
                new BoundingBox(rec1, rec2), comp) == ContainmentType.Contains;
        }
        //screen
        public bool IsContainIt(Point rec1, Point rec2, Point scPos)
        {
            if ((scPos.X - rec1.X) * (scPos.X - rec2.X) > 0)
                return false;
            if ((scPos.Y - rec1.Y) * (scPos.Y - rec2.Y) > 0)
                return false;
            return true;
        }

        public void Dispose()
        {
            Client.ClientSizeChanged -= (o, args) => { SetClientSize((Control)o); };
            Client.MouseMove -= (o, args) => { Camera_MouseMove(o, args); };
            Client.MouseWheel -= (o, args) => { Camera_MouseWheel(o, args); };
            Client.MouseDoubleClick -= (o, args) => { Camera_MouseDoubleClick(o, args); };
            Client = null;
        }




        public bool IsNear(Point screenPoint, Vector3 worldPos,float rad)
        {
            float dis;
            return Ray.Intersects(
                ScreenToWorldRay(screenPoint),
                new BoundingSphere(worldPos, rad),
                out dis);
        }

    }
}
