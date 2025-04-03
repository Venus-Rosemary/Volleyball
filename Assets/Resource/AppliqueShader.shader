
Shader "Unlit/DecalShader"
{
	Properties{
		[HDR] _Color ("Tint", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}
 
	SubShader{
		Tags{ "RenderType"="Transparent" "Queue"="Transparent-400" "DisableBatching"="True"}//ѡ��͸����Ⱦ��Ҫ������͸��������Ⱦ���֮������Ⱦ
 
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite off //�ر����д�룬ͶӰ����Ҫ
 
		Pass{
			CGPROGRAM
 
			#include "UnityCG.cginc"
 
			//���嶥���ƬԪ��ɫ������
			#pragma vertex vert
			#pragma fragment frag
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
 
			fixed4 _Color;
 
			//������ͼ��Ҫ��C#���뿪��������ͼCamera.main.depthTextureMode = DepthTextureMode.Depth
			sampler2D_float _CameraDepthTexture;
 
			struct appdata{
				float4 vertex : POSITION;
			};
 
			struct v2f{
				float4 position : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};
 
			v2f vert(appdata v){
				v2f o;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.position = UnityWorldToClipPos(worldPos);
				o.ray = worldPos - _WorldSpaceCameraPos;//���������������ȥ������������꣬�õ�������������ķ�������
				o.screenPos = ComputeScreenPos (o.position);//���������A����Ļ�ռ��λ��
				return o;
			}
 
			float3 getProjectedObjectPos(float2 screenPos, float3 worldRay){
				//������ĻĿ���������������õ�һ����ȣ�ע������������������ģ����ǽе�B����ȣ�����ֻ�����˵�A����Ļ����
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenPos);
				depth = Linear01Depth (depth) * _ProjectionParams.z;
				
				worldRay = normalize(worldRay);
				worldRay /= dot(worldRay, -UNITY_MATRIX_V[2].xyz);//��һ�������٣��������worldRay * depth������������������������ֱ�߾��룬����Unity�У���near plane��far plane��plane�ϵ�����һ�㵽�������ȶ���һ���ģ�������������������������ķ���õ�������ʵ�ʵ�λ����
				//with that reconstruct world and object space positions
				float3 worldPos = _WorldSpaceCameraPos + worldRay * depth; //����õ���B�����������
				float3 objectPos =  mul (unity_WorldToObject, float4(worldPos,1)).xyz; //����B�任��A�������������ϵ��
				clip(0.5 - abs(objectPos));//Cube�����������һ������Чֵ�Ǵ�-0.5��0.5����������Ҫ����
				objectPos += 0.5;//��������ʱ�����귶ΧΪ0~1,Ҫ��0.5
				return objectPos;
			}
 
			fixed4 frag(v2f i) : SV_TARGET{
				float2 screenUv = i.screenPos.xy / i.screenPos.w;
				float2 uv = getProjectedObjectPos(screenUv, i.ray).xz;//���������uv����0����1��ֱ�ӵ���uv��������
				fixed4 col = tex2D(_MainTex, uv);
				col *= _Color;
				return col;
			}
 
			ENDCG
		}
	}
}
