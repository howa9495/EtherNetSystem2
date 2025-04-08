Shader "Post-processing/Shader/Sobel"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/NormalBuffer.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity;
    float _Thickness;
    float4 _Color;
    float _DepthMultiplier;
    float _DepthBias;    
    float _NormalMultiplier;
    float _NormalBias;

    TEXTURE2D_X(_MainTex);

    float Sobel_Basic(float topLeft, float top, float topRight,
                       float left, float right,
                       float buttomLeft,float buttom, float buttomRight)
    {
        float x =  topLeft + 2 * left + buttomLeft - topRight   - 2 * right -  buttomRight;
        float y = -topLeft - 2 * top  - topRight   + buttomLeft + 2 * buttom + buttomRight;
        return sqrt(x * x + y * y);
    }

    float Sobel_Scharr(float topLeft, float top, float topRight,
                       float left, float right,
                       float buttomLeft,float buttom, float buttomRight)
    {
        float x = -3 * topLeft - 10 * left - 3 * buttomLeft + 3 * topRight   + 10 * right  + 3 * buttomRight;
        float y =  3 * topLeft + 10 * top  + 3 * topRight   - 3 * buttomLeft - 10 * buttom - 3 * buttomRight;
        return sqrt(x * x + y * y);
    }

    float SobelSampleDepth(float2 uv, float offsetU, float offsetV)
    {
        float topLeft     = SampleCameraDepth(uv+ float2(-offsetU,  offsetV));
        float top         = SampleCameraDepth(uv+ float2(        0, offsetV));
        float topRight    = SampleCameraDepth(uv+ float2( offsetU,  offsetV));

        float left        = SampleCameraDepth(uv+ float2(-offsetU,        0));
        float center      = SampleCameraDepth(uv+ float2(        0,       0));
        float right       = SampleCameraDepth(uv+ float2( offsetU,        0));

        float buttomLeft  = SampleCameraDepth(uv+ float2(-offsetU, -offsetV));
        float buttom      = SampleCameraDepth(uv+ float2(        0,-offsetV));
        float buttomRight = SampleCameraDepth(uv+ float2( offsetU, -offsetV));

        return Sobel_Scharr( abs(topLeft - center),    abs(top - center),    abs(topRight - center),
                            abs(left - center),                             abs(right - center),
                            abs(buttomLeft - center), abs(buttom - center), abs(buttomRight - center));
    }

    float Sobel_Basic(float3 topLeft, float3 top, float3 topRight,
                       float3 left, float3 right,
                       float3 buttomLeft,float3 buttom, float3 buttomRight)
    {
        float3 x =  topLeft + 2 * left + buttomLeft - topRight   - 2 * right -  buttomRight;
        float3 y = -topLeft - 2 * top  - topRight   + buttomLeft + 2 * buttom + buttomRight;
        return sqrt(dot(x, x)+ dot(y , y));
    }

    float Sobel_Scharr(float3 topLeft, float3 top, float3 topRight,
                       float3 left, float3 right,
                       float3 buttomLeft,float3 buttom, float3 buttomRight)
    {
        float3 x = -3 * topLeft - 10 * left - 3 * buttomLeft + 3 * topRight   + 10 * right  + 3 * buttomRight;
        float3 y =  3 * topLeft + 10 * top  + 3 * topRight   - 3 * buttomLeft - 10 * buttom - 3 * buttomRight;
        return sqrt(dot(x, x)+ dot(y , y));
    }
    
    float3 SampleWorldNormal(float2 uv)
    {
        //if the camera is invaild - early out
        if(SampleCameraDepth(uv) <= 0)
        return float3(0, 0, 0);


        NormalData normalData;
        DecodeFromNormalBuffer(uv * _ScreenSize.xy, normalData);

        return normalData.normalWS;
    }

    
        float SobelSampleNormal(float2 uv, float offsetU, float offsetV)
    {
        float3 topLeft     = SampleWorldNormal(uv+ float2(-offsetU,  offsetV));
        float3 top         = SampleWorldNormal(uv+ float2(        0, offsetV));
        float3 topRight    = SampleWorldNormal(uv+ float2( offsetU,  offsetV));

        float3 left        = SampleWorldNormal(uv+ float2(-offsetU,        0));
        float3 center      = SampleWorldNormal(uv+ float2(        0,       0));
        float3 right       = SampleWorldNormal(uv+ float2( offsetU,        0));

        float3 buttomLeft  = SampleWorldNormal(uv+ float2(-offsetU, -offsetV));
        float3 buttom      = SampleWorldNormal(uv+ float2(        0,-offsetV));
        float3 buttomRight = SampleWorldNormal(uv+ float2( offsetU, -offsetV));

        return Sobel_Scharr( abs(topLeft - center),    abs(top - center),    abs(topRight - center),
                            abs(left - center),                             abs(right - center),
                            abs(buttomLeft - center), abs(buttom - center), abs(buttomRight - center));
    }
    

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // Note that if HDUtils.DrawFullScreen is not used to render the post process, you don't need to call ClampAndScaleUVForBilinearPostProcessTexture.

        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord.xy)).xyz;

        //determine our offsets
        float offsetU = _Thickness / _ScreenSize.x;
        float offsetV = _Thickness / _ScreenSize.y;

        //run the sobel sampling of the depth buffer
        float sobelDepth = SobelSampleDepth(input.texcoord.xy, offsetU, offsetV);
              sobelDepth = pow(abs(saturate(sobelDepth)* _DepthMultiplier), _DepthBias);

        // Apply greyscale effect
        //float3 color = lerp(sourceColor, Luminance(sourceColor), _Intensity);

        float sobelNormal = SobelSampleNormal(input.texcoord.xy, offsetU, offsetV);
               sobelNormal = pow(abs(saturate(sobelNormal))* _NormalMultiplier, _NormalBias);

        float outlineIntensity = saturate(max(sobelDepth, sobelNormal));

        //Apply the sobel effect
        float3 color = lerp(sourceColor, _Color, outlineIntensity * _Intensity);

        //return float4(sobelNormal, sobelNormal, sobelNormal, 1);
        return float4(color, 1);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Sobel"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
