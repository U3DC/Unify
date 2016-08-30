using System;
using System.Collections.Generic;

namespace Unify.UnifyCommon
{
    public abstract class UnifyObject
    {
        public virtual string ObjType { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual string UniqueName { get; set; }
        public virtual bool Deleted { get; set; }
    }

    public class UnifyLayer : UnifyObject
    {
        public bool MeshCollider { get; set; }

        public UnifyLayer()
        {

        }
    }

    public class UnifyMetaData : UnifyObject
    {
        public Dictionary<string, string> MetaData { get; set; }

        public UnifyMetaData(Dictionary<string, string> metaData)
        {
            this.MetaData = metaData;
        }
    }

    public class UnifyLight : UnifyObject
    {
        public string LightType { get; set; }
        public string Diffuse { get; set; }
        public string Target { get; set; }
        public double Intensity { get; set; }
        public string Location { get; set; }
        public double Range { get; set; }
        public double SpotAngle { get; set; }
        public double ShadowIntensity { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }

        public UnifyLight()
        {

        }
    }

    public class UnifyMaterial : UnifyObject
    {
        public string Diffuse { get; set; }
        public string SpecularColor { get; set; }
        public string EmissionColor { get; set; }
        public string ReflectionColor { get; set; }
        public double Metallic { get; set; }
        public string DiffuseTexture { get; set; }
        public string TransparencyTexture { get; set; }
        public string EnvironmentTexture { get; set; }
        public string BumpTexture { get; set; }
        public double Transparency { get; set; }

        public UnifyMaterial()
        {

        }
    }
    public class UnifyGeometry : UnifyObject
    {
        public string Layer { get; set; }
        public bool MeshCollider { get; set; }

        public UnifyGeometry()
        {

        }
    }

    public class UnifyCamera : UnifyObject
    {
        public string Location { get; set; }
        public string Target { get; set; }

        public bool IsPlayerJumpCamera { get; set; }
        public bool IsPlayerOriginCamera { get; set; }

        public UnifyCamera()
        {

        }
    }
}
