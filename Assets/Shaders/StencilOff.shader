Shader "Unlit/StencilOff"
{
    Properties
    {
        [IntRange] _stencilID ("Stencil ID", Range(0, 255)) = 0 
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"

        }

        Blend Zero One
        Zwrite Off
        
        Stencil
        {

            Ref[_stencilID]
            Comp always
            Pass replace
            Fail Keep
        }

        Pass
        {

        }
       
    }
}
