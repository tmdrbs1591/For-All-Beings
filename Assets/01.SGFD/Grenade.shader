Shader "Unlit/Grenade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color_from_texture = tex2D(_MainTex, i.uv);
                float4 blue = float4(i.uv.x, sin(i.uv.y * 10 + _Time.y * 10), 1, 1);
                // uv.y
                //float4 col = float4(192.0 / 225.0, 112.0/ 225.0 , 43.0 / 225.0, 225.0 / 225.0);

                return color_from_texture + sin(_Time.y * 2.0) * blue;
            }
            ENDCG
        }
    }
}
