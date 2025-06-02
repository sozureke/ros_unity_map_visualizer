Shader "Custom/OptionalOutline" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.02
        [Toggle] _UseOutline ("Use Outline", Float) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        // Outline Pass
        Pass {
            Cull Off
            // Only render this pass if _UseOutline is enabled
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;
            float _UseOutline;

            v2f vert (appdata v) {
                v2f o;
                // Only offset vertex if outline is enabled
                float3 offset = _UseOutline > 0 ? v.normal * _OutlineWidth : 0;
                o.pos = UnityObjectToClipPos(v.vertex + offset);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Only draw outline color if _UseOutline is enabled
                return _UseOutline > 0 ? _OutlineColor : fixed4(0,0,0,0);
            }
            ENDCG
        }

        // Main Color Pass
        Pass {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return _Color;
            }
            ENDCG
        }
    }
}