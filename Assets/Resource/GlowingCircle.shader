Shader "Custom/GlowingRingWithTransparentCenter"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1) // 圆环的颜色
        _GlowColor ("Glow Color", Color) = (1, 0, 0, 1) // 发光的颜色
        _InnerRadius ("Inner Radius", Range(0.0, 1.0)) = 0.4 // 内半径
        _OuterRadius ("Outer Radius", Range(0.0, 1.0)) = 0.5 // 外半径
        _GlowIntensity ("Glow Intensity", Range(0.0, 5.0)) = 1.0 // 发光强度
        _GlowFalloff ("Glow Falloff", Range(0.0, 1.0)) = 0.1 // 发光衰减范围
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // 使用 Alpha 混合
            ZWrite Off                     // 禁用深度写入
            Cull Off                       // 禁用背面剔除

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
                // 计算 UV 坐标到中心的距离
                float2 center = float2(0.5, 0.5); // 中心点
                float dist = distance(i.uv, center);

                // 主圆环颜色和透明度调整
                float ringMask = smoothstep(_InnerRadius - 0.01, _InnerRadius + 0.01, dist) * 
                                 (1.0 - smoothstep(_OuterRadius - 0.01, _OuterRadius + 0.01, dist));
                float4 mainRing = lerp(float4(0, 0, 0, 0), _MainColor, ringMask); // 圆环内部透明
                
                // 发光效果
                float glowMask = smoothstep(_OuterRadius + _GlowFalloff, _OuterRadius, dist);
                float4 glow = _GlowColor * glowMask * _GlowIntensity;

                // 合并主圆环和发光效果
                return mainRing + glow;
            }
            ENDCG
        }
    }
}
