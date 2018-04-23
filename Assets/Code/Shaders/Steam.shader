Shader "FX/Steam"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 color0 : COLOR0;
            };

            struct FragmentData {
                float2 texcoord0 : TEXCOORD0;
                float color0 : COLOR0;
            };

            float4 _MainTex_ST;

            float4 VertexProgram(VertexData vertex, out FragmentData fragment) : SV_POSITION {
                UNITY_INITIALIZE_OUTPUT(FragmentData, fragment);
                fragment.texcoord0 = TRANSFORM_TEX(vertex.texcoord0, _MainTex);
                fragment.texcoord0.y -= _Time.x;
                fragment.color0 = vertex.color0.r;
                return UnityObjectToClipPos(vertex.position); 
            }

            sampler2D _MainTex;
            half4 _Color;

            half4 FragmentProgram(FragmentData fragment) : SV_Target {
                half alpha = tex2D(_MainTex, fragment.texcoord0).a * fragment.color0;
                return half4(_Color.rgb, alpha * alpha * _Color.a);
            }
            ENDCG
        }
	}
}
