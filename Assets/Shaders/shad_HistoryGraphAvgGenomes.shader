Shader "Custom/shad_UIHistoryGraphAvgGenomes"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "black" {}
		_DataTex ("Data Texture", 2D) = "black" {}
		_ZoomFactorX ("ZoomFactorX", Range(0.01,2)) = 1.0
		_ZoomFactorY ("ZoomFactorY", Range(0.01,2)) = 1.0
		_ValueMultiplier ("ValueMultiplier", Float) = 1.0		
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			uniform fixed4 _Color;
			uniform fixed _ZoomFactorX;
			uniform fixed _ZoomFactorY;
			uniform fixed _WarpFactorX;
			uniform fixed _WarpFactorY;
			uniform fixed _ValueMultiplier;
			uniform sampler2D _MainTex;
			uniform sampler2D _DataTex;
			uniform float4 _DataTex_ST; 

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				//OUT.texcoord = IN.texcoord;
				OUT.texcoord = TRANSFORM_TEX( IN.texcoord, _DataTex );
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			

			fixed4 frag(v2f IN) : SV_Target
			{		    
				
				half2 finalCoords = IN.texcoord;
				float pivotY = 0.5;
				if((pivotY + (_ZoomFactorY * 0.5)) > 1) {
					pivotY = 1 - (_ZoomFactorY * 0.5);
				}
				if((pivotY - (_ZoomFactorY * 0.5)) < 0) {
					pivotY = (_ZoomFactorY * 0.5);
				}
				finalCoords.x = (finalCoords.x * _ZoomFactorX) + (1 - _ZoomFactorX); // hugs right-side of graph
				finalCoords.y = (finalCoords.y * _ZoomFactorY) + (pivotY - (_ZoomFactorY * 0.5));
				
				
				//float mult = 2;
				
				half4 sampleValues = tex2D(_DataTex, finalCoords);
				half4 negHue = half4(1.0, 0.75, 0.9, 1.0);
				half4 posHue = half4(0.75, 1.0, 0.9, 1.0);
				float brightness = ((sampleValues.r - 0.5) * _ValueMultiplier) + 0.5;
				//half4 brightness = half4(sampleValues.r, sampleValues.r, sampleValues.r, 1.0);
				half4 pixColor = half4(brightness, brightness, brightness, 1.0);
				if(brightness > 0.51) {
					pixColor = posHue * pixColor;
				}
				if(brightness < 0.49) {
					pixColor = negHue * pixColor;
				}
				
				
				
				//return float4(0.5, 0.5, 0.5, 1.0);
				return pixColor;
			}
		ENDCG
		}
	}
}
