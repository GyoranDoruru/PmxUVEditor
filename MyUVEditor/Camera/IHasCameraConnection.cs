using System;
namespace MyUVEditor.Camera
{
    interface IHasCameraConnection
    {
        ICamera Camera { get; set; }
    }
}
