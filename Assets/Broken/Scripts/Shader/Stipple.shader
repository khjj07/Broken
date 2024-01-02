Shader "Unlit/Stipple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _DitherTex ("Dither Texture", 2D) = "white" { }
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.5
        _AliasingStrength ("Aliasing Strength", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
    
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DitherTex;
            float _DitherStrength;
            float _AliasingStrength;

            v2f vert (appdata v)
            {
                v2f o;
                float aliasingOffset = _AliasingStrength * (0.5 - tex2Dlod(_MainTex, float4(v.uv / v.vertex.w, 0, 2.5)).r);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex) + float4(aliasingOffset, aliasingOffset, 0, 0);
                o.color = v.vertex.y;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv / i.vertex.w);
    
                float ditherValue = tex2D(_DitherTex, i.vertex.xy / i.vertex.w).r;
                col.rgb += (ditherValue - 0.5) * _DitherStrength;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
