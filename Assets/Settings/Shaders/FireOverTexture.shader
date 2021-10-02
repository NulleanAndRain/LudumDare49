Shader "Universal Render Pipeline/2D/FireOverTexture"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Emission("Emission Map", 2D) = "white" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        _Color("Color", Color) = (1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        
        _LightLevel("Light Level", Float) = 1

        _PixelCount("Pixel Count", Range(1, 1024)) = 16
        _PixelCountVert("Vertical Pixel Count", Range(1, 1024)) = 16
        _Intensity("Fire Intensity", Range(0, 2)) = 1
        _OffsetBottom("Bottom Offset", Range(-1, 1)) = 0
        _OffsetTop("Top Offset", Range(-1, 1)) = 0
        
        _Speed0("Vertical Speed", Range(-10, 10)) = -1
        _Speed1("Speed 1", Range(-10, 10)) = 0
        _Speed2("Speed 2", Range(-10, 10)) = 0
        _Speed3("Speed 3", Range(-10, 10)) = 0

        _Scale1("Scale 1", Range(0.1, 20)) = 5
        _Scale2("Scale 2", Range(0.1, 20)) = 5
        _Scale3("Scale 3", Range(0.1, 20)) = 5

        [HDR] _FireColor1("Fire color 1", Color) = (1, 1, 1, 1)
        [HDR] _FireColor2("Fire color 2", Color) = (1, 1, 1, 1)
        [HDR] _FireColor3("Fire color 3", Color) = (1, 1, 1, 1)

        _Step1("Step Color 1", Range(0, 1)) = 0.3
        _Step2("Step Color 2", Range(0, 1)) = 0.5
        _Step3("Step Color 3", Range(0, 1)) = 0.6
        _Step4("Step Color 4", Range(0, 1)) = 0.75
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float4 color       : COLOR;
                float2 uv          : TEXCOORD0;
                float2 lightingUV  : TEXCOORD1;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _MainTex_ST;
            half4 _NormalMap_ST;

            float4 _Color;
            float _LightLevel;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            
            //
            
            TEXTURE2D(_Emission);
            SAMPLER(sampler_Emission);
            half4 _Emission_ST;

            int _PixelCount;
            int _PixelCountVert;
            float _Intensity;
            float _OffsetBottom;
            float _OffsetTop;

            float _getRange() {
                float res = 1 - _OffsetTop - _OffsetBottom;
                if (res <= 0) return 0.000001;
                return res;
            }

            static const float _OffsetRange = _getRange();

            float _Speed0;
            float _Speed1;
            float _Speed2;
            float _Speed3;

            float _Scale1;
            float _Scale2;
            float _Scale3;

            float _Step1;
            float _Step2;
            float _Step3;
            float _Step4;

            float4 _FireColor1;
            float4 _FireColor2;
            float4 _FireColor3;
            float4 _FireColor4;
            //

            float2 unity_gradientNoise_dir(float2 p) {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p) {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            //

            float4 firePixel(float2 uv, float4 col){
                float2 uv_loc = round(uv * _PixelCount) / _PixelCount;

                float uv_y = _Speed0 * _Time.x + uv_loc.y;

                float uv_grad = saturate(round((1 -  uv.y / _OffsetRange + _OffsetBottom) * _PixelCountVert) / _PixelCountVert);

                float4 noice = ((
                        unity_gradientNoise(float2(uv_loc.x + _Time.x * _Speed1, uv_y) * _Scale1) +
                        unity_gradientNoise(float2(uv_loc.x + _Time.x * _Speed2, uv_y) * _Scale2) + 
                        unity_gradientNoise(float2(uv_loc.x + _Time.x * _Speed3, uv_y) * _Scale3)
                ) / 3 + uv_grad) * col.y * _Intensity;
                
                if(noice.r <= _Step1){
                    return (float4)0;
                }
                
                float4 color;
                
                if (noice.r <= _Step2) {
                    color = float4(_FireColor1.xyz * 0.8, _FireColor1.w);
                } else if (noice.r <= _Step3) {
                    color = _FireColor1;
                } else if (noice.r <= _Step4) {
                    color = _FireColor2;
                } else {
                    color = _FireColor3;
                }
                
                color *= noice;
                color.w *= col.y;

                return color;
            }

            //

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;
                o.color = v.color;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * half4(_Color.xyz * _LightLevel, 1);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);


                float4 fireCol = SAMPLE_TEXTURE2D(_Emission, sampler_Emission, i.uv);
                if (fireCol.r) { 
                    fireCol = firePixel(i.uv, fireCol); 
                    return fireCol * fireCol.a + CombinedShapeLightShared(main, mask, i.lightingUV) * (1 - fireCol.a);
                }

                return CombinedShapeLightShared(main, mask, i.lightingUV);
            }

            ENDHLSL
        }
        

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
                float4 tangent      : TANGENT;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
                float3  normalWS		: TEXCOORD1;
                float3  tangentWS		: TEXCOORD2;
                float3  bitangentWS		: TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;  // Is this the right way to do this?
            
            float4 _Color;
            float _LightLevel;

            
            //
            
            TEXTURE2D(_Emission);
            SAMPLER(sampler_Emission);
            half4 _Emission_ST;

            //


            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.uv = attributes.uv;
                o.color = attributes.color;
                o.normalWS = TransformObjectToWorldDir(float3(0, 0, -1));
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            float4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * half4(_Color.xyz * _LightLevel, 1);
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

                half4 fireEmission  = SAMPLE_TEXTURE2D(_Emission, sampler_Emission, i.uv);

                if(fireEmission.x) {
                    return (float4)0.5;
                    return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz) * 2 ;
                }

                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }


        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            
            float4 _Color;
            float _LightLevel;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.uv = attributes.uv;
                o.color = attributes.color;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                //return i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * half4(_Color.xyz * _LightLevel, 1);
                return float4(1, 1, 1, 1);
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
