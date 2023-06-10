Shader "Custom/ChromaKey" {
    Properties{
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Key Color", Color) = (0, 1, 0, 1)
        _Range("Range", Range(0, 1)) = 0.1
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _Range;
                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }
                fixed4 frag(v2f i) : SV_Target {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float mask = distance(col.rgb, _Color.rgb) - _Range;
                    mask = clamp(mask, 0, 1);
                    col.a *= mask;
                    return col;
                }
                ENDCG
            }
        }
}
