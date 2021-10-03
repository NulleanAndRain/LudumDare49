Shader "Universal Render Pipeline/2D/PotionPixelized" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Mask("Potion mask", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)
        
        _ColDark ("Darker Color", Color)   = (1, 1, 1, 1)
        _ColMain ("Main Color", Color)     = (1, 1, 1, 1)

        _level ("Current Level", Range(0, 1)) = 1
        _waveHeight ("Wave Height", Range(0,1)) = 0.1
        _waveSpeed ("Wave Speed", Float) = 1
        
        _PixelCount("Pixel Count", Range(1, 1024)) = 16
        _Aspect("Height to width aspect rate", Float) = 2

        _LvlMax("Top waterline", Range(0, 1)) = 1
        _LvlMin("Bottom waterline", Range(0, 1)) = 0

        _Seed("Random seed", Float) = 0.123
    }

    // нить жизни простых смертных обрывается здесь
    // дальше бога нет
    //---------------------------------------------
    
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL

    SubShader {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass {
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

            TEXTURE2D(_Mask);
            SAMPLER(sampler_Mask);

            float4 _Color;
            float _level;
            float _waveHeight;
            float _waveSpeed;

            float4 _ColDark;
            float4 _ColMain;

            int _PixelCount;
            Float _Aspect;

            float _LvlMax;
            float _LvlMin;
            float _Seed;
            
            // gradient noise
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

            // simplex noise
            float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 permute(float3 x) { return mod289(((x*34.0)+1.0)*x); }

            float snoise(float2 v) {
                const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                                    0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                                    -0.577350269189626,  // -1.0 + 2.0 * C.x
                                    0.024390243902439); // 1.0 / 41.0
                float2 i  = floor(v + dot(v, C.yy) );
                float2 x0 = v -   i + dot(i, C.xx);
                float2 i1;
                i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;
                i = mod289(i); // Avoid truncation effects in permutation
                float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
                    + i.x + float3(0.0, i1.x, 1.0 ));

                float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
                m = m*m ;
                m = m*m ;
                float3 x = (2.0 * frac(C.www * p) - 1.0);
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;
                m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
                float3 g;
                g.x  = a0.x  * x0.x  + h.x  * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;
                return 130.0 * dot(m, g);
            }

            // 1d noise
            float noise1D (float p){
                return (sin(1.22 * p) * cos (3.56 * p) + sin(2.3546 * p) * cos (0.646 * p))/2;
            }

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


            float pixelize(float p, int pixelCount){
                return round((p * pixelCount)-0.5) / pixelCount;
            }
            float2 pixelize(float2 p, int pixelCount){
                return float2(pixelize(p.x, pixelCount / _Aspect), pixelize(p.y, pixelCount));
            }

            float solarize(float c) {
                c = saturate(c);
                c = c * 10 - 5;
                c = (c * c * c + 125) / 250;
                return c;
            }

            // fragment shader
            // returns color of each pixel in figure
            float4 frag(Varyings v) : SV_Target {
                float4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, v.uv);
                float4 potionMask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, v.uv);
                float4 potionCol = (float4) 0;


                float2 uv = pixelize(v.uv, _PixelCount);
                float h = v.uv.y + 
                    _waveHeight * 
                    (noise1D(
                        v.uv.x + _Time.x *
                        cos(_Seed * _Time.x * 1.5116) *
                        sin(_Time.x / _Seed * 0.346) * 
                        _waveSpeed
                    ) * 
                    0.5 - 0.5);
                
                float _l = lerp (_LvlMin, _LvlMax, _level) + _LvlMin;

                if (h > _l) potionMask = (float4)0;
                if (h < _LvlMin) potionMask = (float4)0;

                float h1 = pixelize(
                    snoise(
                        float2(
                            uv.x  + _Time.y * _waveSpeed * _Seed * 0.341 + 1.21 * _SinTime.z,
                            uv.y + _Time.x * _waveSpeed * _Seed * 35.165 + 2.5646 * _CosTime.x
                        )
                    ),
                    _PixelCount);
                float h2 = pixelize(unity_gradientNoise(uv + _SinTime.y).x, _PixelCount);

                float ll = saturate((h1 + h2) / 2);

                potionCol = lerp(_ColDark, _ColMain, ll) * potionMask.a;


                float4 col = potionCol * (1 - mainCol.a) + mainCol * mainCol.a;
                col.a = saturate(potionCol.a + mainCol.a);

                return col * _Color;
            }

            ENDHLSL
        }
    }
}