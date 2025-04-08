Shader "Custom/GlowingRingWithTransparentCenter"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1) // Բ������ɫ
        _GlowColor ("Glow Color", Color) = (1, 0, 0, 1) // �������ɫ
        _InnerRadius ("Inner Radius", Range(0.0, 1.0)) = 0.4 // �ڰ뾶
        _OuterRadius ("Outer Radius", Range(0.0, 1.0)) = 0.5 // ��뾶
        _GlowIntensity ("Glow Intensity", Range(0.0, 5.0)) = 1.0 // ����ǿ��
        _GlowFalloff ("Glow Falloff", Range(0.0, 1.0)) = 0.1 // ����˥����Χ
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // ʹ�� Alpha ���
            ZWrite Off                     // �������д��
            Cull Off                       // ���ñ����޳�

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

            float4 _MainColor;
            float4 _GlowColor;
            float _InnerRadius;
            float _OuterRadius;
            float _GlowIntensity;
            float _GlowFalloff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ���� UV ���굽���ĵľ���
                float2 center = float2(0.5, 0.5); // ���ĵ�
                float dist = distance(i.uv, center);

                // ��Բ����ɫ��͸���ȵ���
                float ringMask = smoothstep(_InnerRadius - 0.01, _InnerRadius + 0.01, dist) * 
                                 (1.0 - smoothstep(_OuterRadius - 0.01, _OuterRadius + 0.01, dist));
                float4 mainRing = lerp(float4(0, 0, 0, 0), _MainColor, ringMask); // Բ���ڲ�͸��
                
                // ����Ч��
                float glowMask = smoothstep(_OuterRadius + _GlowFalloff, _OuterRadius, dist);
                float4 glow = _GlowColor * glowMask * _GlowIntensity;

                // �ϲ���Բ���ͷ���Ч��
                return mainRing + glow;
            }
            ENDCG
        }
    }
}
