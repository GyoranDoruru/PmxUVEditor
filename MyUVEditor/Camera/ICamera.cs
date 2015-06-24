using System;
using System.Windows.Forms;
using SlimDX;
using System.Drawing;

namespace MyUVEditor.Camera
{
    public interface ICamera : IDisposable
    {
        Control Client { get; }
        Vector3 Position { get; }
        Vector3 Direction { get; }
        Matrix WorldViewProjection { get; }
        Matrix World { get; }
        Matrix View { get; }
        Matrix Projection { get; }
        Matrix Screen { get; }
        void ResetCamera();
        Ray ScreenToWorldRay(Point p);
        Vector3 ScreenToWorldVec(Point p);
        Vector3 ScreenToWorldVec(Point p, float z);
        bool IsNear(Point screenPoint, Vector3 worldPos,float rad);
        bool IsContainIt(Vector3 rec1, Vector3 rec2, Vector3 worldPos);
        bool IsContainIt(Point rec1, Point rec2, Point scPos);
    }
}
