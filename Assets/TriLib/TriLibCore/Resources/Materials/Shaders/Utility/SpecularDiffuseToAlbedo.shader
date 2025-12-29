Shader "Hidden/TriLib/SpecularDiffuseToAlbedo"
{
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _DiffuseTexture;
			sampler2D _SpecularTexture;
			float4 _DiffuseColor;
			float4 _SpecularColor;
			float _Glossiness;
			int _OutputBaseColor;

			void specularToMetalness(float4 diffuseBase, float3 specularBase, out float4 baseColor, out float metalness)
			{
				float3 dielectricSpecular = 0.04;
				float epsilon = 1e-5; 
				float diffuseBrightness = dot(diffuseBase.xyz, float3(0.2126, 0.7152, 0.0722));
				float specularBrightness = dot(specularBase, float3(0.2126, 0.7152, 0.0722)); 
				float oneMinusSpecularStrength = 1.0 - max(max(specularBase.r, specularBase.g), specularBase.b);
				if (specularBrightness < dielectricSpecular.r)
				{
					metalness = 0.0;
				}
				else
				{
					float a = dielectricSpecular.r;
					float b = diffuseBrightness * oneMinusSpecularStrength / (1.0 - dielectricSpecular.r) + specularBrightness - 2.0 * dielectricSpecular.r;
					float c = dielectricSpecular.r - specularBrightness;
					float D = b * b - 4.0 * a * c;
					metalness = saturate((-b + sqrt(D)) / (2.0 * a));
				}
				float3 baseColorFromDiffuse = diffuseBase.xyz * (oneMinusSpecularStrength / (1.0 - dielectricSpecular.r) / max(1.0 - metalness, epsilon));
				float3 baseColorFromSpecular = (specularBase - dielectricSpecular * (1.0 - metalness)) / max(metalness, epsilon);
				baseColor = float4(lerp(baseColorFromDiffuse, baseColorFromSpecular, metalness * metalness), diffuseBase.a);
				baseColor = saturate(baseColor);
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 diffuseBase = tex2D(_DiffuseTexture, i.uv) * _DiffuseColor;
				float4 specularAndGlossiness = tex2D(_SpecularTexture, i.uv);
				float3 specularBase = specularAndGlossiness.xyz * _SpecularColor.xyz;
				float glossiness = max(_Glossiness, specularAndGlossiness.w);
				float4 baseColor;
				float metalness;
				specularToMetalness(diffuseBase, specularBase, baseColor, metalness);
				if (_OutputBaseColor)
				{
					return baseColor;
				}
				return float4(metalness, metalness, metalness, glossiness);
			}
			ENDCG
		}
	}
}
