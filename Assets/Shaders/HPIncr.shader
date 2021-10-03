// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
Shader "Universal Render Pipeline/2D/HPIncr"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _ColorLight ("Light Color", Color) = (1, 1, 1, 1)
        
        _waveHeight ("Wave Height", Range(0,1)) = 0.1
        _waveSpeed ("Wave Speed", Float) = 0.2

        _botEdge ("Bottom Edge", Range(0, 1)) = 0
        _topEdge ("Top Edge", Range(0, 1)) = 0
        
        _LvlMax("Top waterline", Range(0, 1)) = 1
        _LvlMin("Bottom waterline", Range(0, 1)) = 0
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
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings {
                float4  positionCS  : SV_POSITION;
                float4  color       : COLOR;
                float2	uv          : TEXCOORD0;
            };

            // vars
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;
            half4 _NormalMap_ST;
            
            float _waveHeight;
            float _waveSpeed;
            float _topEdge;
            float _botEdge;

            float4 _Color;
            float4 _ColorLight;

            float _LvlMax;
            float _LvlMin;
            
            // vertex shader
            // modifies and passes vertices info to fragment shader
            Varyings vert(Attributes v) {
                Varyings o = (Varyings)0;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.color = v.color;
                return o;
            }

            
            // 1d noise
            float noise1D (float p){
                return  (sin(1.89 * p) + cos (3.56 * p))/2;
            }
            
            // fragment shader
            // returns color of each pixel in figure
            float4 frag(Varyings v) : SV_Target {
                float h = _waveHeight * (noise1D(v.uv.x + _Time.y * _waveSpeed)*0.5 - 0.5);
                float h1 = v.uv.y + h;
                float _t = lerp(_LvlMin, _LvlMax, _topEdge) + _LvlMin;
                float _b = lerp(_LvlMin, _LvlMax, _botEdge) + _LvlMin;


                if (h1 > _t || h1 < _b) return (float4)0;
                
                half4 mask = (half4)1;
                mask.a *= SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, v.uv).a;
                float sst = smoothstep(_botEdge, _topEdge, v.uv.y + h);

                float4 col = lerp(_Color, _ColorLight, sst*sst);

                return col * mask;
            }

            ENDHLSL
        }
    }
}