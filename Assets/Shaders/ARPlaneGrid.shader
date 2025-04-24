Shader "TequilaSunrise/AR/PlaneGrid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,0.5)
        _GridSize ("Grid Size", Float) = 0.1
        _GridLineWidth ("Grid Line Width", Range(0.0, 0.05)) = 0.02
        _VerticalAlignment ("Vertical Alignment", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GridSize;
            float _GridLineWidth;
            float _VerticalAlignment;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            float grid(float2 pos, float size, float lineWidth)
            {
                float2 grid = abs(frac(pos / size - 0.5) - 0.5) / fwidth(pos) * lineWidth;
                return min(grid.x, grid.y);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos;
                if (_VerticalAlignment == 90)
                {
                    pos = i.worldPos.xy;
                }
                else if (_VerticalAlignment == 180)
                {
                    pos = i.worldPos.xz;
                }
                else
                {
                    pos = i.worldPos.xz;
                }
                
                float gridValue = 1.0 - min(grid(pos, _GridSize, _GridLineWidth), 1.0);
                
                fixed4 col = _Color;
                col.a *= gridValue * _Color.a;
                
                return col;
            }
            ENDCG
        }
    }
} 