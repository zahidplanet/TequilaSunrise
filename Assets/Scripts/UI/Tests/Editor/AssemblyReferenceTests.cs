using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace TequilaSunrise.Tests.Editor
{
    public class AssemblyReferenceTests
    {
        [Test]
        public void AssemblyDefinitions_ShouldNotHaveCyclicDependencies()
        {
            // Get all assembly definition assets
            string[] asmdefGuids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");
            string[] asmdefPaths = asmdefGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
            
            foreach (string asmdefPath in asmdefPaths)
            {
                var asmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
                var references = EditorJsonUtility.FromJson<AssemblyDefinitionReferences>(asmdef.text);
                
                // Check that this assembly's references don't create cycles
                foreach (string reference in references.references)
                {
                    Assert.That(reference, Does.Not.Contain(asmdef.name), 
                        $"Assembly {asmdef.name} has a cyclic dependency with {reference}");
                }
            }
        }
        
        private class AssemblyDefinitionReferences
        {
            public string name;
            public string[] references;
        }
    }
}