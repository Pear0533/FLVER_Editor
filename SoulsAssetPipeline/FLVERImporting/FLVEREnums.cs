using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsAssetPipeline.FLVERImporting
{
    public enum TextureChannelSemantic
    {
        /// <summary>
        /// No texture map. This is only used as a failsafe for when one forgets to assign a map.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Diffuse or albedo.
        /// </summary>
        Diffuse,

        /// <summary>
        /// Can be specular color, reflectance, or weird PBR material attributes.
        /// </summary>
        Specular,

        /// <summary>
        /// Exclusively only refers to a texture that sets the PBR gloss value.
        /// </summary>
        Shininess,

        /// <summary>
        /// Normal map.
        /// </summary>
        Normals,

        /// <summary>
        /// Refers to a generic looped normal map to deep fry the material's surface.
        /// 
        /// Often does not point to any texture map.
        /// </summary>
        DetailNormals,

        /// <summary>
        /// ??? DS2 only. 
        /// </summary>
        AdditionalNormals,

        /// <summary>
        /// Additional normal map overlayed over the standard one when equipment is broken. 
        /// Almost always points to a shared global texture that is a looping set of scratches and cracks.
        /// </summary>
        EquipmentBrokenNormals,

        /// <summary>
        /// Emissive map. Color is directly outputted from model and onto the screen pixels.
        /// Used for extremely bright glows.
        /// 
        /// In DS3 characters this frequently points to the "Lord of Cinder" / "Embered" 
        /// orange sheen effect shared global texture.
        /// </summary>
        Emissive,

        /// <summary>
        /// Mask for emissive map.
        /// </summary>
        EmissiveMask,

        /// <summary>
        /// Blend-texture masking map. Used for models with 2 different sets of textures. This map acts as a bridge between them, 
        /// crossfading the color according to how bright this map is at a specific pixel.
        /// </summary>
        Blendmask,

        /// <summary>
        /// Unknown, but name suggests it could be used for liquid materials?
        /// </summary>
        Flow,

        /// <summary>
        /// Lightmap 1.
        /// </summary>
        Lightmap1,

        // <summary>
        /// Lightmap 2.
        /// </summary>
        Lightmap2,

        /// <summary>
        /// Global illumination.
        /// </summary>
        GlobalIllumination,

        ////////////////////////////////////////
        // DS2
        ////////////////////////////////////////

        /// <summary>
        /// Unknown. Seen in DS2 as g_VectorTexture on hair materials, for example.
        /// </summary>
        Vector,

        ////////////////////////////////////////
        // DS3
        ////////////////////////////////////////

        /// <summary>
        /// Unknown. Not the same as <see cref="Blendmask"/>.
        /// </summary>
        Mask,

        /// <summary>
        /// Unknown, but name suggests that it may blend between non-edge and edge type rendering (different ways of handling depth buffer / stencilling).
        /// </summary>
        BlendEdge,

        /// <summary>
        /// Unknown, but name suggests that it may define the opacity of decal-splashed blood.
        /// </summary>
        BloodMask,

        /// <summary>
        /// Unknown, but name suggests it's a 3D displacement map for dynamic tesselated displacement vertex shader.
        /// </summary>
        Displacement,

        /// <summary>
        /// Subsurface scattering mask texture. Tells how strongly light should scatter through the material.
        /// </summary>
        ScatteringMask,

        /// <summary>
        /// Unknown but name suggests it makes things brighter.
        /// </summary>
        Highlight,

        /// <summary>
        /// Unknown but name suggests it is literally the opacity of the material.
        /// </summary>
        Opacity,

        /// <summary>
        /// Unknown but name suggests it is literally the opacity of the material.
        /// </summary>
        Height,

        /// <summary>
        /// Unknown. Only in DS3.
        /// </summary>
        DS3Burning,

        ////////////////////////////////////////
        // SDT
        ////////////////////////////////////////

        /// <summary>
        /// Metallicness of material. Only in SDT.
        /// </summary>
        Metallic,

        /// <summary>
        /// Unknown. Only in SDT.
        /// </summary>
        SDTMask1,

        /// <summary>
        /// Unknown. Only in SDT.
        /// </summary>
        SDTMask3,

        /// <summary>
        /// Unknown. Only in SDT.
        /// </summary>
        SDTRipple,

        /// <summary>
        /// Ambient occlusion map. Misspelled as "AmbientOcculusion" in SDT.
        /// </summary>
        AmbientOcclusion,

        /// <summary>
        /// Unknown. Only in SDT.
        /// </summary>
        SDTStar,

        /// <summary>
        /// Unknown. Only in SDT. Probably has to do with clouds tbh.
        /// </summary>
        SDTCloudMask2,

        /// <summary>
        /// Unknown. Only in SDT. 
        /// </summary>
        SDTFoam1
    }
}
