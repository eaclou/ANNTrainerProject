Shader "Custom/ArenaBGShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_OwnPosX ("OwnPosX", Range(-1,1)) = 0.0
		_OwnPosY ("OwnPosY", Range(-1,1)) = 0.0
		_TargetPosX ("TargetPosX", Range(-1,1)) = 0.0
		_TargetPosY ("TargetPosY", Range(-1,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass {
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma vertex vert
			#pragma fragment frag

			// Use shader model 3.0 target, to get nicer looking lighting
			//#pragma target 3.0

			
			// uniforms
			uniform fixed4 _Color;	
			//sampler2D _MainTex;
			uniform fixed _OwnPosX;
			uniform fixed _OwnPosY;	
			uniform fixed _TargetPosX;
			uniform fixed _TargetPosY;	

			struct vertexInput {
				float4 vertex : POSITION; // position in object coordinates
				float4 tangent : TANGENT; // vector orthogonal to the surface normal
				float3 normal : NORMAL; // surface normal vector (in object coordinates; usually normalized)
				float4 texcoord : TEXCOORD0; // 0th set of texture coordinates ( UV)
				float4 texcoord1 : TEXCOORD1; // 1st set of ...
				fixed4 color : COLOR;  // vertex color
			};
			
			struct fragmentInput
			{
				float4 pos : SV_Position;
				float4 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			fragmentInput vert( vertexInput i) 
			{
				fragmentInput o;
				o.pos = mul( UNITY_MATRIX_MVP, i.vertex );
				o.color = _Color;
				
				// DEBUG options - sets color to other variables
				//o.color = i.texcoord;
				o.uv = i.texcoord;
				//o.color = i.texcoord1;
				//o.color = i.vertex;
				//o.color = i.vertex + float4(0.5, 0.5, 0.5, 0.0 );  // add .5 to offset if the model verts go from -0.5 to 0.5
				//o.color = i.tangent;
				//o.color = float4( i.normal * 0.5 + 0.5, 1.0 ); // scale and bias the normal to get it in range 0-1
				//o.color = i.color // vertex colors
				//o.color = float4( i.normal, 1.0);
				
				return o;
			}
			
			half4 frag( fragmentInput i ) : COLOR
			{
				float4 heatMap = float4(0.0, 0.0, 0.0, 1.0);				
				// Target:
				float targetRadius = 0.08;
				float targetDeltaX = _TargetPosX - i.uv.x;
				float targetDeltaY = _TargetPosY - i.uv.y;
				float targetdist = sqrt(targetDeltaX * targetDeltaX + targetDeltaY * targetDeltaY);
				if( targetdist < targetRadius )
				{
					//heatMap += float4(0.1, 0.1, 0.1, 1.0);
					i.color = lerp(float4(0.0, 0.0, 0.0, 1.0), i.color, targetdist/targetRadius);
				}
				// Agent:
				float radius = 0.08;
				float deltaX = _OwnPosX - i.uv.x;
				float deltaY = _OwnPosY - i.uv.y;
				float dist = sqrt(deltaX * deltaX + deltaY * deltaY);
				if( dist < radius )
				{
					//heatMap = float4(0.1, 0.1, 0.1, 1.0);
					i.color = lerp(float4(0.0, 0.0, 0.0, 1.0), i.color, dist/radius);
				}
				
				//return float4(deltaX, _OwnPosX, 0.0, 1.0);			
				return i.color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
