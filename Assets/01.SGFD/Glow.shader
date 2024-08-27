Shader"Custom/GlowShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Float) = 1.0
        _GlowSize ("Glow Size", Float) = 1.0
    }
    SubShader
    {
        Tags {"Queue"="Overlay"}
        Pass
        {
Name"Glow"
            ZWrite
Off
            ZTest
Always
            Blend
SrcAlpha One

            // Vertex Shader
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
float4 _GlowColor;
float _GlowIntensity;
float _GlowSize;

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

half4 frag(v2f i) : COLOR
{
    float4 texColor = tex2D(_MainTex, i.uv);
    float glow = _GlowIntensity * texColor.a;
    return float4(_GlowColor.rgb * glow, texColor.a);
}
            ENDCG
        }

        Pass
        {
Name"GlowOutline"
            ZWrite
Off
            ZTest
Always
            Blend
SrcAlpha One

            // Vertex Shader
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
float4 _GlowColor;
float _GlowIntensity;
float _GlowSize;

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

half4 frag(v2f i) : COLOR
{
    float4 texColor = tex2D(_MainTex, i.uv);
    float glow = _GlowIntensity;
    float2 offset = float2(_GlowSize / _ScreenParams.x, _GlowSize / _ScreenParams.y);
    float4 glowColor = _GlowColor * glow;

    float4 finalColor = texColor * (1.0 - glowColor.a) + glowColor;
    return finalColor;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
