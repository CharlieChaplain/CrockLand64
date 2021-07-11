Shader "Custom/ShowVertColorUnlit"
{
    Properties
    {
        _Emission("Emmisive Color", Color) = (0,0,0,0)
    }

        SubShader
    {
        Pass
        {
            Material
            {
                Emission[_Emission]
            }

            ColorMaterial AmbientAndDiffuse
            Lighting Off
        }
    }
        Fallback "VertexLit", 1


}