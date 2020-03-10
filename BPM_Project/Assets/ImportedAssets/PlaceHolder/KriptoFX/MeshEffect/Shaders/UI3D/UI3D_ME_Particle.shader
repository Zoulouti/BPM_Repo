Shader "KriptoFX/ME/UI3D/Particle" {
	Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
	 [HideInInspector]_Cutout ("_Cutout", Float) = 0.2
	 [HideInInspector]_InvFade ("Soft Particles Factor", Float) = 1.0
	 [HideInInspector]_FresnelStr ("Fresnel Strength", Float) = 1.0
	CullMode ("Cull Mode", int) = 0 //0 = off, 2=back

	[Space]
	[Toggle(DISABLE_UI_CULLING)] _DisableCulling("Disable culling? (disables UI depth test)", Float) = 0
	[Toggle(CAST_UI_CULLING_TO_SCREEN_SPACE)] _CastUICullingToScreen("Cast UI culling to screen space", Float) = 0

	[Space]
	[Enum(UnityEngine.Rendering.BlendMode)] _Blend("Blend mode", Float) = 1
	[Enum(UnityEngine.Rendering.BlendMode)] _Blend2("Blend mode subset", Float) = 1
			
	[Space]
	[Toggle(SOFT_COLLISION_MODE)] _SoftCollisionMode("Enable Soft particle?", Float) = 0
	_UISoftModeFadeSmooth("Soft particle factor", Range(0,5)) = 1
			
	[HideInInspector][Toggle(USE_CLIPPING_MASK)] _UseClippingMask("UseClippingMask?", Float) = 0
	[HideInInspector]_ClippingMaskVal("_ClippingMaskVal", Range(0,1)) = 1
	[HideInInspector][KeywordEnum(Inside, Outside)] ClippingMode ("Clipping mode", Float) = 0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "RFX1"="Particle"}
				Blend [_Blend] [_Blend2]
				Lighting On
				Cull [CullMode] 
				ZWrite Off
				
	SubShader {
		Pass {
				
			//ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#pragma multi_compile_fog
			#pragma shader_feature BlendAdd BlendAlpha BlendMul
			#pragma shader_feature VertLight_OFF VertLight4_ON
			#pragma shader_feature FrameBlend_OFF FrameBlend_ON
			#pragma shader_feature SoftParticles_OFF SoftParticles_ON

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _Cutout;
			half _FresnelStr;
			half _BloomThreshold;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				half4 color : COLOR;
#ifdef FrameBlend_OFF
				float2 texcoord : TEXCOORD0;

#if UNITY_VERSION == 600
				float4 texcoords : TEXCOORD0;
				float texcoordBlend : TEXCOORD1;

				float2 texcoord : TEXCOORD0;
				float4 texcoordBlendFrame : TEXCOORD1;


			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
#ifdef FrameBlend_OFF
				float2 texcoord : TEXCOORD0;

				float4 texcoord : TEXCOORD0;
				fixed blend : TEXCOORD1;

				UNITY_FOG_COORDS(2)
				float4 projPos : TEXCOORD3;
				//
#ifdef FresnelFade_ON
				float fresnel : TEXCOORD4;

				float2 depthTexUV : TEXCOORD5;
				float worldZPos : TEXCOORD6;

			};

			
			float3 VertexLight4 (float4 vertex)
			{
				float3 viewpos = UnityObjectToViewPos(vertex).xyz;
				float3 light = 0;
				//[unroll(4)]
				for (int i = 0; i < 4; i++) {
					float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					float lengthSq = dot(toLight, toLight);
					float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
					light += unity_LightColor[i].rgb * atten;
				}
				return light + unity_AmbientSky + unity_AmbientEquator / 10 + unity_AmbientGround / 10;
			}

			float3 ComputeVertexLight(float4 vert, float4 norm)
			{
				float3 light = 1;
				#ifdef VertLight4_ON
					light = VertexLight4(vert);
				
			
				return light;
			}


			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos (o.vertex);
				o.projPos.xy *= _DepthPyramidScale.xy;
				COMPUTE_EYEDEPTH(o.projPos.z);
			//
				o.color = v.color;
				o.color.rgb *= ComputeVertexLight(v.vertex, v.normal);

#ifdef FrameBlend_OFF
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

#if UNITY_VERSION == 600
				o.texcoord.xy = TRANSFORM_TEX(v.texcoords.xy, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoords.zw, _MainTex);
				o.blend = v.texcoordBlend;

				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoordBlendFrame.xy, _MainTex);
				o.blend = v.texcoordBlendFrame.z;


#ifdef FresnelFade_ON
				o.fresnel = abs(dot(normalize(v.normal), normalize(ObjSpaceViewDir(v.vertex))));
				o.fresnel = saturate((pow(o.fresnel, _FresnelStr)) * 2);

				UNITY_TRANSFER_FOG(o,o.vertex);
				
				float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldZPos = wPos.z;
				o.depthTexUV = calcUIDepthTexUv(wPos, svPositionUIToScreenPos(o.vertex));
				return o;
			}
			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			half4 frag (v2f i) : SV_Target
			{
				makeUI3DClipping(i.depthTexUV, i.worldZPos);
					float z = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
					float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(z));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * (sceneZ-partZ));
					i.color.a *= fade;
				 
				
			
			#ifdef FrameBlend_OFF
				half4 tex = tex2D(_MainTex, i.texcoord);
			
				//half4 tex = Tex2DInterpolated(_MainTex, i.texcoord, _Tiling);
				half4 tex1 = tex2D(_MainTex, i.texcoord.xy);
				half4 tex2 = tex2D(_MainTex, i.texcoord.zw);
				half4 tex = lerp(tex1, tex2, i.blend);
			

				half4 res = 2 * tex * _TintColor;

			#ifdef Clip_ON
				res.a = step(_Cutout, tex.a) * res.a;
				

			#ifdef Clip_ON_Alpha
				res.a = step(1-i.color.a + _Cutout, tex.a);
				res.rgb *= i.color.rgb;
				

			#if !defined(Clip_ON_Alpha)
				res *= i.color;
				
			
				res.a = saturate(res.a);
				//res *= i.color;
			#ifdef FresnelFade_ON
				res.a *= i.fresnel;
			
				

			#ifdef BlendAdd
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(0,0,0,0)); 
			
			#ifdef BlendAlpha
				UNITY_APPLY_FOG(i.fogCoord, res);
			
			#ifdef BlendMul
				res = lerp(half4(1,1,1,1), res, res.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(1,1,1,1)); // fog towards white due to our blend mode
			#endif
			
				return res;
			}
			ENDCG 
		}
	}	
}
	CustomEditor "UIParticleShaderEditor"
}