// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/SelectionBox" {
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		//_Color("MainColor(RGB)", Color) = (1, 1, 1, 1)
		_RimColor("RimColor(RGB)", Color) = (1, 1, 1, 1)
		_RimPower("RimPower", float) = 1
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert   
			#pragma fragment frag    
			#include "UnityCG.cginc"
 
			sampler2D _MainTex;
			//float4 _Color;
			float4 _RimColor;
			float _RimPower;
 
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
				float4 srcPos : TEXCOORD1;
				float3 col : COLOR;
			};
			vertexOutput vert(appdata_full input)
			{
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.tex = input.texcoord;
				output.srcPos = input.vertex;
 
				float3 viewDir = normalize(ObjSpaceViewDir(input.vertex));
				float dotProduct = 1 - saturate(dot(input.normal, viewDir));
				//output.col = _RimColor.rgb * pow(dotProduct, _RimPower);
				float rimWidth = 0.7;
				output.col.rgb = smoothstep(1 - rimWidth, 1.0, dotProduct);
				output.col *= _RimColor;
				return output;
			}
			float4 frag(vertexOutput input) : COLOR
			{
				float4 a = float4(input.col, 1);
				float4 b = tex2D(_MainTex, input.tex);
				float4 c = a + b;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}