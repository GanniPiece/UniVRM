﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniVRM10
{
    /// <summary>
    /// JsonSchema から vrm10 のシリアライザーを生成する。
    /// 
    /// * glTF
    /// * vrm-specification
    /// 
    /// は SubModuleになった。 `$ git submodule update --init` しておくこと。
    /// 
    /// </summary>
    public static class Menu
    {
#if VRM_DEVELOP        
        [MenuItem(UniVRM10.VRMVersion.MENU + "/Generate from JsonSchema")]
        public static void Generate()
        {
            Run(false);
        }

        [MenuItem(UniVRM10.VRMVersion.MENU + "/Generate from JsonSchema(debug)")]
        public static void Parse()
        {
            Run(true);
        }
#endif

        struct GenerateInfo
        {
            public string JsonSchema;
            public string FormatDir;
            public string SerializerDir;

            public GenerateInfo(string jsonSchema, string formatDir, string serializerDir)
            {
                JsonSchema = jsonSchema;
                FormatDir = formatDir;
                SerializerDir = serializerDir;
            }

            public GenerateInfo(string jsonSchema, string formatDir) : this(jsonSchema, formatDir, formatDir)
            {
            }
        }

        static void Run(bool debug)
        {
            var projectRoot = new DirectoryInfo(Path.GetFullPath(Path.Combine(Application.dataPath, "../")));

            var gltf = new FileInfo(Path.Combine(projectRoot.FullName, "glTF/specification/2.0/schema/glTF.schema.json"));

            var args = new GenerateInfo[]
            {
                // VRMC_hdr_emissiveMultiplier
                new GenerateInfo(
                    "vrm-specification/specification/VRMC_materials_hdr_emissiveMultiplier-1.0_draft/schema/VRMC_materials_hdr_emissiveMultiplier.json",
                    "Assets/UniGLTF/Runtime/UniGLTF/Format/ExtensionsAndExtras/EmissiveMultiplier"
                ),

                // VRMC_vrm
                new GenerateInfo(
                    "vrm-specification/specification/VRMC_vrm-1.0_draft/schema/VRMC_vrm.schema.json",
                    "Assets/VRM10/Runtime/Format/Vrm"
                ),

                // VRMC_materials_mtoon
                new GenerateInfo(
                    "vrm-specification/specification/VRMC_materials_mtoon-1.0_draft/schema/VRMC_materials_mtoon.schema.json",
                    "Assets/VRMShaders/VRM10/Format/Runtime/MaterialsMToon",
                    "Assets/VRM10/Runtime/Format/MaterialsMToon"
                ),

                // VRMC_springBone
                new GenerateInfo(
                    "vrm-specification/specification/VRMC_springBone-1.0_draft/schema/VRMC_springBone.schema.json",
                    "Assets/VRM10/Runtime/Format/SpringBone"
                ),

                // VRMC_node_constraint
                new GenerateInfo(
                    "vrm-specification/specification/VRMC_node_constraint-1.0_draft/schema/VRMC_node_constraint.schema.json",
                    "Assets/VRM10/Runtime/Format/Constraints"
                ),
            };

            foreach (var arg in args)
            {
                var extensionSchemaPath = new FileInfo(Path.Combine(projectRoot.FullName, arg.JsonSchema));
                var parser = new UniGLTF.JsonSchema.JsonSchemaParser(gltf.Directory, extensionSchemaPath.Directory);
                var extensionSchema = parser.Load(extensionSchemaPath, "");

                var formatDst = new DirectoryInfo(Path.Combine(projectRoot.FullName, arg.FormatDir));
                Debug.Log($"Format.g Dir: {formatDst}");

                var serializerDst = new DirectoryInfo(Path.Combine(projectRoot.FullName, arg.SerializerDir));
                Debug.Log($"Serializer/Deserializer.g Dir: {serializerDst}");

                if (debug)
                {
                    Debug.Log(extensionSchema.Dump());
                }
                else
                {
                    GenerateUniGLTFSerialization.Generator.GenerateTo(extensionSchema, formatDst, serializerDst);
                }
            }
        }
    }
}
