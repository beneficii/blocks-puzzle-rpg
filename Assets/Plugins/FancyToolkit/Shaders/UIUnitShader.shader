Shader "UI/UnitSprite"
{
    Properties
    {
        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 1, 1, 0)
        _TeamColor("TeamColor", Color) = (0.2996618, 0.6053848, 0.6981132, 1)
        _TeamColor2("TeamColor2", Color) = (0, 0, 0, 0)
        _OutlineColor("OutlineColor", Color) = (0.09411766, 0.1254902, 0.1607843, 1)
		_Stencil("Stencil ID", Float) = 0
		_StencilComp("StencilComp", Float) = 8
		_StencilOp("StencilOp", Float) = 0
		_StencilReadMask("StencilReadMask", Float) = 255
		_StencilWriteMask("StencilWriteMask", Float) = 255
		_ColorMask("ColorMask", Float) = 15

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        
		ZTest [unity_GUIZTestMode]
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
			Stencil{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}
			ColorMask [_ColorMask]
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _Tint;
        float4 _TeamColor;
        float4 _OutlineColor;
        float4 _TeamColor2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_ReplaceColor_float(float3 In, float3 From, float3 To, float Range, out float3 Out, float Fuzziness)
        {
            float Distance = distance(From, In);
            Out = lerp(To, In, saturate((Distance - Range) / max(Fuzziness, 1e-5f)));
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_062a82b367b64a27b0877301cb37bdc3_Out_0 = _Tint;
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_R_1 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[0];
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_G_2 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[1];
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_B_3 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[2];
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_A_4 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[3];
            float4 _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGBA_4;
            float3 _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGB_5;
            float2 _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RG_6;
            Unity_Combine_float(_Split_09c8933eaff54c43ae0cad3fbc3d72d4_R_1, _Split_09c8933eaff54c43ae0cad3fbc3d72d4_G_2, _Split_09c8933eaff54c43ae0cad3fbc3d72d4_B_3, 0, _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGBA_4, _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGB_5, _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RG_6);
            float3 _Multiply_ae1d51cac2a041719d137e7610392849_Out_2;
            Unity_Multiply_float3_float3(_Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGB_5, (_Split_09c8933eaff54c43ae0cad3fbc3d72d4_A_4.xxx), _Multiply_ae1d51cac2a041719d137e7610392849_Out_2);
            UnityTexture2D _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.tex, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.samplerstate, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_R_4 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.r;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_G_5 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.g;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_B_6 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.b;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.a;
            float4 _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGBA_4;
            float3 _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGB_5;
            float2 _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RG_6;
            Unity_Combine_float(_SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_R_4, _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_G_5, _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_B_6, _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7, _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGBA_4, _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGB_5, _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RG_6);
            float4 _Property_05de753f286d4a0f8719d2f8dc2bcaec_Out_0 = _TeamColor;
            float3 _ReplaceColor_d3f7cbe9ea504a0ca13a9f6f3ed0cab4_Out_4;
            Unity_ReplaceColor_float(_Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGB_5, IsGammaSpace() ? float3(0.6039216, 0.3098039, 1) : SRGBToLinear(float3(0.6039216, 0.3098039, 1)), (_Property_05de753f286d4a0f8719d2f8dc2bcaec_Out_0.xyz), 0, _ReplaceColor_d3f7cbe9ea504a0ca13a9f6f3ed0cab4_Out_4, 0);
            float4 _Property_9849663a87d54367898ecdc1664af434_Out_0 = _TeamColor2;
            float3 _ReplaceColor_480dc375da344cce825638d6f267498e_Out_4;
            Unity_ReplaceColor_float(_ReplaceColor_d3f7cbe9ea504a0ca13a9f6f3ed0cab4_Out_4, IsGammaSpace() ? float3(0.7607843, 0.5529412, 1) : SRGBToLinear(float3(0.7607843, 0.5529412, 1)), (_Property_9849663a87d54367898ecdc1664af434_Out_0.xyz), 0, _ReplaceColor_480dc375da344cce825638d6f267498e_Out_4, 0);
            float4 _Property_b2318805e8f141f6ac87f19a1ae06918_Out_0 = _OutlineColor;
            float3 _ReplaceColor_83533fd681ba49c4b448d5ff162b85d6_Out_4;
            Unity_ReplaceColor_float(_ReplaceColor_480dc375da344cce825638d6f267498e_Out_4, IsGammaSpace() ? float3(0.2627451, 0.2039216, 0.3333333) : SRGBToLinear(float3(0.2627451, 0.2039216, 0.3333333)), (_Property_b2318805e8f141f6ac87f19a1ae06918_Out_0.xyz), 0, _ReplaceColor_83533fd681ba49c4b448d5ff162b85d6_Out_4, 0);
            float3 _Add_8c55510bec424a638bef04d4625efd4b_Out_2;
            Unity_Add_float3(_Multiply_ae1d51cac2a041719d137e7610392849_Out_2, _ReplaceColor_83533fd681ba49c4b448d5ff162b85d6_Out_4, _Add_8c55510bec424a638bef04d4625efd4b_Out_2);
            surface.BaseColor = _Add_8c55510bec424a638bef04d4625efd4b_Out_2;
            surface.Alpha = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
            // Render State
            Cull Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _Tint;
        float4 _TeamColor;
        float4 _OutlineColor;
        float4 _TeamColor2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            // GraphFunctions: <None>
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.tex, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.samplerstate, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_R_4 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.r;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_G_5 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.g;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_B_6 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.b;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.a;
            surface.Alpha = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
            // Render State
            Cull Back
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _Tint;
        float4 _TeamColor;
        float4 _OutlineColor;
        float4 _TeamColor2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            // GraphFunctions: <None>
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.tex, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.samplerstate, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_R_4 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.r;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_G_5 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.g;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_B_6 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.b;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.a;
            surface.Alpha = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

		ZTest [unity_GUIZTestMode]
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
			Stencil{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}
			ColorMask [_ColorMask]
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _Tint;
        float4 _TeamColor;
        float4 _OutlineColor;
        float4 _TeamColor2;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_ReplaceColor_float(float3 In, float3 From, float3 To, float Range, out float3 Out, float Fuzziness)
        {
            float Distance = distance(From, In);
            Out = lerp(To, In, saturate((Distance - Range) / max(Fuzziness, 1e-5f)));
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_062a82b367b64a27b0877301cb37bdc3_Out_0 = _Tint;
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_R_1 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[0];
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_G_2 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[1];
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_B_3 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[2];
            float _Split_09c8933eaff54c43ae0cad3fbc3d72d4_A_4 = _Property_062a82b367b64a27b0877301cb37bdc3_Out_0[3];
            float4 _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGBA_4;
            float3 _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGB_5;
            float2 _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RG_6;
            Unity_Combine_float(_Split_09c8933eaff54c43ae0cad3fbc3d72d4_R_1, _Split_09c8933eaff54c43ae0cad3fbc3d72d4_G_2, _Split_09c8933eaff54c43ae0cad3fbc3d72d4_B_3, 0, _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGBA_4, _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGB_5, _Combine_93fa7073d5514fe7bcb15fb7f65af55e_RG_6);
            float3 _Multiply_ae1d51cac2a041719d137e7610392849_Out_2;
            Unity_Multiply_float3_float3(_Combine_93fa7073d5514fe7bcb15fb7f65af55e_RGB_5, (_Split_09c8933eaff54c43ae0cad3fbc3d72d4_A_4.xxx), _Multiply_ae1d51cac2a041719d137e7610392849_Out_2);
            UnityTexture2D _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.tex, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.samplerstate, _Property_c1aa95ddba75487cb1dc076bbd6cf819_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_R_4 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.r;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_G_5 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.g;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_B_6 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.b;
            float _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7 = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_RGBA_0.a;
            float4 _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGBA_4;
            float3 _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGB_5;
            float2 _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RG_6;
            Unity_Combine_float(_SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_R_4, _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_G_5, _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_B_6, _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7, _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGBA_4, _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGB_5, _Combine_ac6db424b2a14c33b19fe9e25c1bae70_RG_6);
            float4 _Property_05de753f286d4a0f8719d2f8dc2bcaec_Out_0 = _TeamColor;
            float3 _ReplaceColor_d3f7cbe9ea504a0ca13a9f6f3ed0cab4_Out_4;
            Unity_ReplaceColor_float(_Combine_ac6db424b2a14c33b19fe9e25c1bae70_RGB_5, IsGammaSpace() ? float3(0.6039216, 0.3098039, 1) : SRGBToLinear(float3(0.6039216, 0.3098039, 1)), (_Property_05de753f286d4a0f8719d2f8dc2bcaec_Out_0.xyz), 0, _ReplaceColor_d3f7cbe9ea504a0ca13a9f6f3ed0cab4_Out_4, 0);
            float4 _Property_9849663a87d54367898ecdc1664af434_Out_0 = _TeamColor2;
            float3 _ReplaceColor_480dc375da344cce825638d6f267498e_Out_4;
            Unity_ReplaceColor_float(_ReplaceColor_d3f7cbe9ea504a0ca13a9f6f3ed0cab4_Out_4, IsGammaSpace() ? float3(0.7607843, 0.5529412, 1) : SRGBToLinear(float3(0.7607843, 0.5529412, 1)), (_Property_9849663a87d54367898ecdc1664af434_Out_0.xyz), 0, _ReplaceColor_480dc375da344cce825638d6f267498e_Out_4, 0);
            float4 _Property_b2318805e8f141f6ac87f19a1ae06918_Out_0 = _OutlineColor;
            float3 _ReplaceColor_83533fd681ba49c4b448d5ff162b85d6_Out_4;
            Unity_ReplaceColor_float(_ReplaceColor_480dc375da344cce825638d6f267498e_Out_4, IsGammaSpace() ? float3(0.2627451, 0.2039216, 0.3333333) : SRGBToLinear(float3(0.2627451, 0.2039216, 0.3333333)), (_Property_b2318805e8f141f6ac87f19a1ae06918_Out_0.xyz), 0, _ReplaceColor_83533fd681ba49c4b448d5ff162b85d6_Out_4, 0);
            float3 _Add_8c55510bec424a638bef04d4625efd4b_Out_2;
            Unity_Add_float3(_Multiply_ae1d51cac2a041719d137e7610392849_Out_2, _ReplaceColor_83533fd681ba49c4b448d5ff162b85d6_Out_4, _Add_8c55510bec424a638bef04d4625efd4b_Out_2);
            surface.BaseColor = _Add_8c55510bec424a638bef04d4625efd4b_Out_2;
            surface.Alpha = _SampleTexture2D_ac797339621143e8ae8b0f8d082c0350_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}