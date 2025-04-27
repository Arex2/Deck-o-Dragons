Shader "Custom/UI/Dissolve"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		[Header(Dissolve)]
		_TransitionTex ("Transition Texture", 2D) = "white" {}
		_ParamTex ("Parameter Texture", 2D) = "white" {}
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
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ INVERT OVERRIDE ADD SUBTRACT FILL

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;

			sampler2D _TransitionTex;
			sampler2D _ParamTex;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				/* Unused vertex data
				float4 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				half2 uv : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				half3 eParam : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.uv = IN.texcoord0;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
				#endif

				OUT.color = IN.color * _Color;

				OUT.eParam = IN.texcoord1;

				// Apply rotation
				OUT.eParam.xy = OUT.eParam.xy * 2 - 1;

				float rotation = IN.texcoord1.w;

				float c = cos(rotation);
				float s = sin(rotation);

				float2x2 mat = float2x2(c, -s, s, c);

				OUT.eParam.xy = mul(mat, OUT.eParam.xy);

				OUT.eParam.xy = OUT.eParam.xy * 0.5 + 0.5;

				return OUT;
			}

			float GetChannelXPos(int channel)
			{
				const int TOTAL_CHANNELS = 2;

				return float(channel) / float(TOTAL_CHANNELS - 1);
			}

			float invLerp(float from, float to, float value)
			{
  				return (value - from) / (to - from);
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float4 param = tex2D(_ParamTex, float2(GetChannelXPos(0), IN.eParam.z));
				fixed dissolveAmount = param.x;

				if (dissolveAmount >= 1)
				{
					#ifdef UNITY_UI_ALPHACLIP
					clip (-1);
					#endif

					return fixed4(0, 0, 0, 0);
				}

				half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				if (dissolveAmount <= 0)
				{
					#ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
					#endif

					#if OVERRIDE
					return fixed4(0, 0, 0, 0);
					#else
					return color;
					#endif
				}
				
				float alpha = tex2D(_TransitionTex, IN.eParam.xy).a;

				#if INVERT
				alpha = 1 - alpha;
				#endif

    			fixed width = param.y / 4;
   				fixed softness = param.z;
				float4 edgeColor = tex2D(_ParamTex, float2(GetChannelXPos(1), IN.eParam.z));
				float factor = alpha - dissolveAmount * ( 1 + width ) + width;
				fixed edgeLerp = step(factor, color.a) * saturate((width - factor) * 16 / softness);

				#if OVERRIDE
				float4 overrideColor = lerp(float4(0, 0, 0, 0), edgeColor, edgeLerp);
				#elif FILL
				//#if FILL
				color = lerp(color, edgeColor, edgeLerp);

				#elif ADD
				color += edgeColor * edgeLerp;

				#elif SUBTRACT
				color -= edgeColor * edgeLerp;

				#else // Multiply
				color = lerp(color, color * edgeColor, edgeLerp);
				#endif

				float saturateFactor = saturate((factor) * 32 / softness);

				#if OVERRIDE
				overrideColor.a *= saturateFactor;
				#else
				color.a *= saturateFactor;
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				#if OVERRIDE
				return overrideColor;
				#else
				return color;
				#endif
			}
		ENDCG
		}
	}

    FallBack "UI/Default"
}
