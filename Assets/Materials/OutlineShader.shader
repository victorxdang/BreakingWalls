
Shader "Custom/Outline"
{
	Properties
	{
		_Color("Outline Color", Color) = (1, 1, 1, 1)
		_MainColor ("Main Color", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_Width ("Outline Width", Range(1.0, 5.0)) = 1.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float3 normal : NORMAL;
	};

	float _Width;
	float4 _MainColor;

	v2f vert(appdata v)
	{
		v.vertex.xyz *= _Width;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}

	ENDCG

	SubShader
	{
		Pass // render outline
		{
			ZWrite Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _MainColor;
			}

			ENDCG
		}

		Pass // normal render 
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}

			Lighting On

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
		}
	}
}
