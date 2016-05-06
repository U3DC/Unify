using System;
using System.Collections.Generic;

namespace Unify.UnifyCommon
{
    public class UnifyObject
    {
        // all
        public virtual string ObjType { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual string UniqueName { get; set; }
        public virtual bool Deleted { get; set; }

        // geometry
        public virtual string Layer { get; set; }

        // lights
        public virtual string LightType { get; set; }
        public virtual string Diffuse { get; set; } // shared w materials
        public virtual string Target { get; set; }
        public virtual double Intensity { get; set; }
        public virtual double Range { get; set; }
        public virtual double SpotAngle { get; set; }
        public virtual double ShadowIntensity { get; set; }
        public virtual string Location { get; set; }

        // materials
        public virtual string DiffuseTexture { get; set; }
        public virtual string SpecularColor { get; set; }
        public virtual string EmissionColor { get; set; }
        public virtual string ReflectionColor { get; set; }
        public virtual double Metallic { get; set; }
        public virtual string TransparencyTexture { get; set; }
        public virtual string EnvironmentTexture { get; set; }
        public virtual string BumpTexture { get; set; }
        public virtual double Transparency { get; set; }

        // meta data object
        public virtual Dictionary<string, string> MetaData { get; set; }
    }

    public class UnifyMetaData : UnifyObject
    {
        public override Dictionary<string, string> MetaData { get; set; }

        public UnifyMetaData(Dictionary<string, string> metaData)
        {
            this.MetaData = metaData;
        }
    }

    public class UnifyLight : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string LightType { get; set; }
        public override Guid Guid { get; set; }
        public override bool Deleted { get; set; }
        public override string Diffuse { get; set; }
        public override string Target { get; set; }
        public override double Intensity { get; set; }
        public override string Location { get; set; }
        public override double Range { get; set; }
        public override double SpotAngle { get; set; }
        public override double ShadowIntensity { get; set; }

        public UnifyLight()
        {

        }
    }

    public class UnifyMaterial : UnifyObject
    {
        public override string ObjType { get; set; }
        public override Guid Guid { get; set; }
        public override string Name { get; set; }
        public override string UniqueName { get; set; }
        public override string Diffuse { get; set; }
        public override string SpecularColor { get; set; }
        public override string EmissionColor { get; set; }
        public override string ReflectionColor { get; set; }
        public override double Metallic { get; set; }
        public override string DiffuseTexture { get; set; }
        public override string TransparencyTexture { get; set; }
        public override string EnvironmentTexture { get; set; }
        public override string BumpTexture { get; set; }
        public override double Transparency { get; set; }

        public UnifyMaterial()
        {

        }
    }
    public class UnifyGeometry : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string Layer { get; set; }
        public override Guid Guid { get; set; }

        public UnifyGeometry()
        {

        }
    }

    public class UnifyCamera : UnifyObject
    {
        public override string ObjType { get; set; }
        public override Guid Guid { get; set; }
        public override string Location { get; set; }

        public UnifyCamera()
        {

        }
    }
}
