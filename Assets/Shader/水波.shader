Shader "Unlit/水波"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _time1 ("time", Range(0, 10)) = 0
        _zhong ("Centr", Vector) = (0, 0, 0 ,0) //第三个值为振幅  第四个值为波长
        _Zhi ("bi", Range(0, 20)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #pragma exclude_renderers gles gles3  // 应该添加
            #pragma target 3.0  // 需要明确指定
            
            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Zhi; //长宽的比值
            float _time1; // 时间
            float4 _zhong;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 变换之后的uv;
                float2 p = float2(0.5 + (i.uv.x - 0.5) * _Zhi,i.uv.y );

                float2 lw = p*2-1;
                float2 zhongxin = _zhong.xy;
                float _Zhen = _zhong.z;
                float _Bo = _zhong.w;
                float xiangdui = length(lw - zhongxin);

                float off = sin( ( _time1 - xiangdui)*6.28f/_Bo)* 0.1f * _Zhen  *  step(1, xiangdui < _time1 && xiangdui > _time1 - _Bo);

                i.uv += float2(lw.x / xiangdui * off,lw.y / xiangdui* off);
                fixed4 col = tex2D(_MainTex, i.uv);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
