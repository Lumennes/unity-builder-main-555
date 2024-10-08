﻿using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuild
    {
        private static string BUILD_PATCH;
        private static InfoYG infoYG;
        private static string indexFile;
        private static string methodName;
        private enum CodeType { HeadNative, BodyNative, JS, Head, Body, Init0, Init1, Init2, Init, Start };

        public static void ModifyIndex(string buildPatch)
        {
            infoYG = YG2.infoYG;
            BUILD_PATCH = buildPatch;
            string filePath = Path.Combine(buildPatch, "index.html");
#if PLATFORM_WEBGL
            indexFile = File.ReadAllText(filePath);

            Type type = typeof(ModifyBuild);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
            {
                if (method.Name != nameof(ModifyIndex) && method.GetParameters().Length == 0)
                {
                    methodName = method.Name;
                    ModifyBuild scrCopy = new ModifyBuild();
                    method.Invoke(scrCopy, BindingFlags.Static | BindingFlags.Public, null, null, null);
                }
            }
#endif
            int.TryParse(BuildLog.ReadProperty("Build number"), out int buildNumInt);
            buildNumInt += 1;
            string buildNum = buildNumInt.ToString();

            File.WriteAllText(filePath, indexFile);
            Debug.Log($"<color=#00FF00>{InfoYG.NAME_PLUGIN} - Build complete!  Build number: {buildNum}</color>");

#if PLATFORM_WEBGL
            string logText = $"{InfoYG.NAME_PLUGIN} v{InfoYG.VERSION_YG2}  build: {buildNum}";
#if YandexGamePlatform
            string initFunction = $"LogStyledMessage('{logText}');";
            AddIndexCode(initFunction, CodeType.JS);
#else
            string initFunction = $"<script>console.log('%c' + '{logText}', 'color: #FFDF73; background-color: #454545');</script>";
            AddIndexCode(initFunction, CodeType.BodyNative);
#endif
#endif
        }

        public static void ModifyIndex()
        {
            string buildPatch = BuildLog.ReadProperty("Build path");

            if (buildPatch != null)
            {
                ModifyIndex(buildPatch);
                Process.Start("explorer.exe", buildPatch.Replace("/", "\\"));
            }
            else
            {
                Debug.LogError("Path not found:\n" + buildPatch);
            }
        }

        static void AddIndexCode(string code, CodeType addCodeType)
        {
            string commentHelper;

            if (addCodeType == CodeType.HeadNative)
                commentHelper = "</head>";
            else if (addCodeType == CodeType.BodyNative)
                commentHelper = "</body>";
            else if (addCodeType == CodeType.Head)
                commentHelper = "<!-- Additional head modules -->";
            else if (addCodeType == CodeType.Body)
                commentHelper = "<!-- Additional body modules -->";
            else if (addCodeType == CodeType.Init0)
                commentHelper = "// Additional init0 modules";
            else if (addCodeType == CodeType.Init1)
                commentHelper = "// Additional init1 modules";
            else if (addCodeType == CodeType.Init2)
                commentHelper = "// Additional init2 modules";
            else if (addCodeType == CodeType.Init)
                commentHelper = "// Additional init modules";
            else if (addCodeType == CodeType.Start)
                commentHelper = "// Additional start modules";
            else
                commentHelper = "// Additional script modules";

            StringBuilder sb = new StringBuilder(indexFile);
            int insertIndex = sb.ToString().IndexOf(commentHelper);
            if (insertIndex >= 0)
            {
                if (addCodeType != CodeType.HeadNative && addCodeType != CodeType.BodyNative)
                    insertIndex += commentHelper.Length;

                sb.Insert(insertIndex, "\n" + code + "\n");
                indexFile = sb.ToString();
            }
        }

        public static string FileTextCopy(string fileName, bool customInPluginYG = false)
        {
            string file;
            if (!customInPluginYG)
                file = $"{InfoYG.PATCH_PC_MODULES}/{methodName}/Scripts/Editor/CopyCode/{fileName}";
            else
                file = $"{InfoYG.PATCH_PC_YG2}";

            return File.ReadAllText(file);
        }
    }
}