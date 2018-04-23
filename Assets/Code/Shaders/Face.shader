Shader "Cards/Face"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct FragmentData {
                float2 texcoord0 : TEXCOORD0;
            };

            float4 _MainTex_ST;

            float4 VertexProgram(VertexData vertex, out FragmentData fragment) : SV_POSITION {
                UNITY_INITIALIZE_OUTPUT(FragmentData, fragment);
                fragment.texcoord0 = TRANSFORM_TEX(vertex.texcoord0, _MainTex);
                return UnityObjectToClipPos(vertex.position); 
            }

            sampler2D _MainTex;
            half3 _Color;

            half4 FragmentProgram(FragmentData fragment) : SV_Target {
                return tex2D(_MainTex, fragment.texcoord0);
            }
            ENDCG
        }
    }
}
