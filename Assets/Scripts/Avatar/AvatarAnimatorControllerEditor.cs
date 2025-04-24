#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Editor tool for creating avatar animation controllers
    /// </summary>
    public class AvatarAnimatorControllerEditor : EditorWindow
    {
        // Animation clips
        private AnimationClip idleClip;
        private AnimationClip walkClip;
        private AnimationClip runClip;
        private AnimationClip jumpClip;
        private AnimationClip fallClip;
        private AnimationClip landClip;
        private AnimationClip mountClip;
        private AnimationClip dismountClip;
        private AnimationClip idleMountedClip;
        
        // Optional clips for more variety
        private List<AnimationClip> idleVariations = new List<AnimationClip>();
        
        // Settings
        private string controllerName = "AvatarAnimatorController";
        private string savePath = "Assets/Animations/";
        private RuntimeAnimatorController existingController;
        private bool includeVehicleMounting = true;
        private bool useBlendTrees = true;
        private bool includeRandomIdleVariations = false;
        
        // Animation transition settings
        private float transitionDuration = 0.25f;
        private float idleVariationChance = 0.1f;
        
        [MenuItem("TequilaSunrise/Avatar/Create Animation Controller")]
        public static void ShowWindow()
        {
            GetWindow<AvatarAnimatorControllerEditor>("Avatar Animation Controller");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Avatar Animation Controller Generator", EditorStyles.boldLabel);
            
            // Controller settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Settings", EditorStyles.boldLabel);
            controllerName = EditorGUILayout.TextField("Controller Name", controllerName);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            existingController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Existing Controller (Optional)", existingController, typeof(RuntimeAnimatorController), false);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animation Clips", EditorStyles.boldLabel);
            
            // Required animations
            idleClip = (AnimationClip)EditorGUILayout.ObjectField("Idle", idleClip, typeof(AnimationClip), false);
            walkClip = (AnimationClip)EditorGUILayout.ObjectField("Walk", walkClip, typeof(AnimationClip), false);
            runClip = (AnimationClip)EditorGUILayout.ObjectField("Run", runClip, typeof(AnimationClip), false);
            jumpClip = (AnimationClip)EditorGUILayout.ObjectField("Jump", jumpClip, typeof(AnimationClip), false);
            fallClip = (AnimationClip)EditorGUILayout.ObjectField("Fall", fallClip, typeof(AnimationClip), false);
            landClip = (AnimationClip)EditorGUILayout.ObjectField("Land", landClip, typeof(AnimationClip), false);
            
            // Vehicle mounting options
            EditorGUILayout.Space();
            includeVehicleMounting = EditorGUILayout.Toggle("Include Vehicle Mounting", includeVehicleMounting);
            
            if (includeVehicleMounting)
            {
                EditorGUI.indentLevel++;
                mountClip = (AnimationClip)EditorGUILayout.ObjectField("Mount", mountClip, typeof(AnimationClip), false);
                dismountClip = (AnimationClip)EditorGUILayout.ObjectField("Dismount", dismountClip, typeof(AnimationClip), false);
                idleMountedClip = (AnimationClip)EditorGUILayout.ObjectField("Idle Mounted", idleMountedClip, typeof(AnimationClip), false);
                EditorGUI.indentLevel--;
            }
            
            // Advanced options
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Advanced Options", EditorStyles.boldLabel);
            useBlendTrees = EditorGUILayout.Toggle("Use Blend Trees for Movement", useBlendTrees);
            
            // Idle variations
            includeRandomIdleVariations = EditorGUILayout.Toggle("Include Idle Variations", includeRandomIdleVariations);
            if (includeRandomIdleVariations)
            {
                EditorGUI.indentLevel++;
                idleVariationChance = EditorGUILayout.Slider("Variation Chance", idleVariationChance, 0.01f, 0.5f);
                
                // Display list of idle variations with add/remove buttons
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Idle Variations", EditorStyles.boldLabel, GUILayout.Width(100));
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    idleVariations.Add(null);
                }
                if (GUILayout.Button("Remove", GUILayout.Width(70)) && idleVariations.Count > 0)
                {
                    idleVariations.RemoveAt(idleVariations.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel++;
                for (int i = 0; i < idleVariations.Count; i++)
                {
                    idleVariations[i] = (AnimationClip)EditorGUILayout.ObjectField($"Variation {i + 1}", idleVariations[i], typeof(AnimationClip), false);
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            
            // Transition settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transition Settings", EditorStyles.boldLabel);
            transitionDuration = EditorGUILayout.Slider("Transition Duration", transitionDuration, 0.0f, 1.0f);
            
            // Create button
            EditorGUILayout.Space();
            GUI.enabled = idleClip != null; // At minimum, we need an idle animation
            if (GUILayout.Button("Create Animation Controller"))
            {
                CreateAnimatorController();
            }
            GUI.enabled = true;
        }
        
        private void CreateAnimatorController()
        {
            // Create the directory if it doesn't exist
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            // Create the controller
            AnimatorController controller;
            string controllerPath = $"{savePath}{controllerName}.controller";
            
            if (existingController != null)
            {
                // Copy the existing controller
                string sourcePath = AssetDatabase.GetAssetPath(existingController);
                AssetDatabase.CopyAsset(sourcePath, controllerPath);
                controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            }
            else
            {
                // Create a new controller
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            }
            
            // Add parameters if they don't exist
            AddParameterIfMissing(controller, "Speed", AnimatorControllerParameterType.Float);
            AddParameterIfMissing(controller, "Grounded", AnimatorControllerParameterType.Bool);
            AddParameterIfMissing(controller, "Jump", AnimatorControllerParameterType.Trigger);
            
            if (includeVehicleMounting)
            {
                AddParameterIfMissing(controller, "IsMounted", AnimatorControllerParameterType.Bool);
            }
            
            // Get base layer
            AnimatorControllerLayer baseLayer = controller.layers[0];
            AnimatorStateMachine rootStateMachine = baseLayer.stateMachine;
            
            // Clear existing states for clean rebuild (optional)
            if (rootStateMachine.states.Length > 0)
            {
                if (EditorUtility.DisplayDialog("Confirm Rebuild", 
                    "This will remove all existing states and transitions in the controller. Continue?", 
                    "Yes", "No"))
                {
                    ClearStateMachine(rootStateMachine);
                }
                else
                {
                    return;
                }
            }
            
            // Create states
            AnimatorState idleState = CreateStateIfNotExists(rootStateMachine, "Idle", idleClip);
            
            // Create movement states/blend trees
            AnimatorState moveState;
            if (useBlendTrees && walkClip != null && runClip != null)
            {
                moveState = CreateStateIfNotExists(rootStateMachine, "Movement", null);
                BlendTree blendTree = new BlendTree();
                blendTree.name = "Movement Blend";
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = "Speed";
                blendTree.AddChild(walkClip, 0.5f);
                blendTree.AddChild(runClip, 1.0f);
                
                moveState.motion = blendTree;
                AssetDatabase.AddObjectToAsset(blendTree, controllerPath);
            }
            else
            {
                // Create separate states for walk and run
                AnimatorState walkState = null;
                AnimatorState runState = null;
                
                if (walkClip != null)
                {
                    walkState = CreateStateIfNotExists(rootStateMachine, "Walk", walkClip);
                    CreateTransition(idleState, walkState, 
                        new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Greater, parameter = "Speed", threshold = 0.1f },
                                               new AnimatorCondition { mode = AnimatorConditionMode.Less, parameter = "Speed", threshold = 1.0f } });
                    
                    CreateTransition(walkState, idleState, 
                        new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Less, parameter = "Speed", threshold = 0.1f } });
                }
                
                if (runClip != null)
                {
                    runState = CreateStateIfNotExists(rootStateMachine, "Run", runClip);
                    
                    if (walkState != null)
                    {
                        CreateTransition(walkState, runState, 
                            new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Greater, parameter = "Speed", threshold = 1.0f } });
                        
                        CreateTransition(runState, walkState, 
                            new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Less, parameter = "Speed", threshold = 1.0f } });
                    }
                    else
                    {
                        CreateTransition(idleState, runState, 
                            new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Greater, parameter = "Speed", threshold = 1.0f } });
                    }
                    
                    CreateTransition(runState, idleState, 
                        new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Less, parameter = "Speed", threshold = 0.1f } });
                }
                
                moveState = walkState ?? runState;
            }
            
            // Set the idle state as default
            rootStateMachine.defaultState = idleState;
            
            // Add jump states
            if (jumpClip != null)
            {
                AnimatorState jumpState = CreateStateIfNotExists(rootStateMachine, "Jump", jumpClip);
                jumpState.timeParameterActive = false;
                jumpState.tag = "Jump";
                
                // Add transition from any state to jump
                var anyStateToJump = rootStateMachine.AddAnyStateTransition(jumpState);
                anyStateToJump.hasExitTime = false;
                anyStateToJump.duration = 0.1f;
                anyStateToJump.AddCondition(AnimatorConditionMode.If, 0, "Jump");
                anyStateToJump.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
                
                // Add transition from jump to fall or back to idle/move
                if (fallClip != null)
                {
                    AnimatorState fallState = CreateStateIfNotExists(rootStateMachine, "Fall", fallClip);
                    fallState.tag = "Air";
                    
                    var jumpToFall = jumpState.AddTransition(fallState);
                    jumpToFall.hasExitTime = true;
                    jumpToFall.exitTime = 0.8f;
                    jumpToFall.duration = 0.2f;
                    jumpToFall.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
                    
                    // Add landing transition
                    if (landClip != null)
                    {
                        AnimatorState landState = CreateStateIfNotExists(rootStateMachine, "Land", landClip);
                        
                        var fallToLand = fallState.AddTransition(landState);
                        fallToLand.hasExitTime = false;
                        fallToLand.duration = 0.1f;
                        fallToLand.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
                        
                        // Transition back to idle or movement
                        var landToIdle = landState.AddTransition(idleState);
                        landToIdle.hasExitTime = true;
                        landToIdle.exitTime = 0.8f;
                        landToIdle.duration = 0.2f;
                        landToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                        
                        if (moveState != null)
                        {
                            var landToMove = landState.AddTransition(moveState);
                            landToMove.hasExitTime = true;
                            landToMove.exitTime = 0.8f;
                            landToMove.duration = 0.2f;
                            landToMove.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                        }
                    }
                    else
                    {
                        // Direct transition from fall to idle/move
                        var fallToIdle = fallState.AddTransition(idleState);
                        fallToIdle.hasExitTime = false;
                        fallToIdle.duration = 0.2f;
                        fallToIdle.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
                        fallToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                        
                        if (moveState != null)
                        {
                            var fallToMove = fallState.AddTransition(moveState);
                            fallToMove.hasExitTime = false;
                            fallToMove.duration = 0.2f;
                            fallToMove.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
                            fallToMove.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                        }
                    }
                }
                else
                {
                    // Direct transition from jump to idle/move
                    var jumpToIdle = jumpState.AddTransition(idleState);
                    jumpToIdle.hasExitTime = true;
                    jumpToIdle.exitTime = 0.8f;
                    jumpToIdle.duration = 0.2f;
                    jumpToIdle.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
                    jumpToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                    
                    if (moveState != null)
                    {
                        var jumpToMove = jumpState.AddTransition(moveState);
                        jumpToMove.hasExitTime = true;
                        jumpToMove.exitTime = 0.8f;
                        jumpToMove.duration = 0.2f;
                        jumpToMove.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
                        jumpToMove.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                    }
                }
            }
            
            // Add vehicle mounting if enabled
            if (includeVehicleMounting)
            {
                // Create a sub-state machine for the mounted state
                AnimatorStateMachine mountedStateMachine = rootStateMachine.AddStateMachine("Mounted");
                
                // Mount/dismount animations
                AnimatorState mountState = null;
                AnimatorState dismountState = null;
                AnimatorState idleMountedState = null;
                
                if (mountClip != null)
                {
                    mountState = CreateStateIfNotExists(rootStateMachine, "Mount", mountClip);
                    var idleToMount = idleState.AddTransition(mountState);
                    idleToMount.hasExitTime = false;
                    idleToMount.duration = 0.2f;
                    idleToMount.AddCondition(AnimatorConditionMode.If, 0, "IsMounted");
                }
                
                if (idleMountedClip != null)
                {
                    idleMountedState = CreateStateIfNotExists(mountedStateMachine, "IdleMounted", idleMountedClip);
                    mountedStateMachine.defaultState = idleMountedState;
                    
                    if (mountState != null)
                    {
                        var mountToIdleMounted = mountState.AddTransition(idleMountedState);
                        mountToIdleMounted.hasExitTime = true;
                        mountToIdleMounted.exitTime = 0.9f;
                        mountToIdleMounted.duration = 0.1f;
                    }
                    else
                    {
                        // Direct transition if no mount animation
                        var anyStateToMounted = rootStateMachine.AddAnyStateTransition(idleMountedState);
                        anyStateToMounted.hasExitTime = false;
                        anyStateToMounted.duration = 0.2f;
                        anyStateToMounted.AddCondition(AnimatorConditionMode.If, 0, "IsMounted");
                    }
                }
                
                if (dismountClip != null)
                {
                    dismountState = CreateStateIfNotExists(rootStateMachine, "Dismount", dismountClip);
                    
                    if (idleMountedState != null)
                    {
                        var idleMountedToDismount = idleMountedState.AddTransition(dismountState);
                        idleMountedToDismount.hasExitTime = false;
                        idleMountedToDismount.duration = 0.2f;
                        idleMountedToDismount.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMounted");
                    }
                    
                    var dismountToIdle = dismountState.AddTransition(idleState);
                    dismountToIdle.hasExitTime = true;
                    dismountToIdle.exitTime = 0.9f;
                    dismountToIdle.duration = 0.1f;
                }
                else if (idleMountedState != null)
                {
                    // Direct transition back to idle if no dismount animation
                    var idleMountedToIdle = idleMountedState.AddTransition(idleState);
                    idleMountedToIdle.hasExitTime = false;
                    idleMountedToIdle.duration = 0.2f;
                    idleMountedToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMounted");
                }
            }
            
            // Add idle variations if enabled
            if (includeRandomIdleVariations && idleVariations.Count > 0)
            {
                for (int i = 0; i < idleVariations.Count; i++)
                {
                    if (idleVariations[i] != null)
                    {
                        AnimatorState idleVarState = CreateStateIfNotExists(rootStateMachine, $"IdleVariation{i+1}", idleVariations[i]);
                        
                        // Random transition from idle to variation
                        var idleToVar = idleState.AddTransition(idleVarState);
                        idleToVar.hasExitTime = true;
                        idleToVar.exitTime = 0.95f;
                        idleToVar.duration = 0.05f;
                        idleToVar.canTransitionToSelf = false;
                        idleToVar.AddCondition(AnimatorConditionMode.Less, idleVariationChance, "Speed");
                        
                        // Return to idle after variation completes
                        var varToIdle = idleVarState.AddTransition(idleState);
                        varToIdle.hasExitTime = true;
                        varToIdle.exitTime = 0.9f;
                        varToIdle.duration = 0.1f;
                    }
                }
            }
            
            // Add movement transitions if not already added
            if (useBlendTrees && moveState != null)
            {
                CreateTransition(idleState, moveState, 
                    new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Greater, parameter = "Speed", threshold = 0.1f } });
                
                CreateTransition(moveState, idleState, 
                    new AnimatorCondition[] { new AnimatorCondition { mode = AnimatorConditionMode.Less, parameter = "Speed", threshold = 0.1f } });
            }
            
            // Save the controller
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Select the created controller
            Selection.activeObject = controller;
            
            EditorUtility.DisplayDialog("Success", $"Animation controller created at {controllerPath}", "OK");
        }
        
        private void AddParameterIfMissing(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            // Check if parameter already exists
            foreach (var param in controller.parameters)
            {
                if (param.name == name && param.type == type)
                {
                    return;
                }
            }
            
            // Add the parameter
            controller.AddParameter(name, type);
        }
        
        private AnimatorState CreateStateIfNotExists(AnimatorStateMachine stateMachine, string name, AnimationClip clip)
        {
            // Check if state already exists
            foreach (var stateInfo in stateMachine.states)
            {
                if (stateInfo.state.name == name)
                {
                    if (clip != null)
                    {
                        stateInfo.state.motion = clip;
                    }
                    return stateInfo.state;
                }
            }
            
            // Create new state
            AnimatorState state = stateMachine.AddState(name);
            if (clip != null)
            {
                state.motion = clip;
            }
            
            return state;
        }
        
        private void CreateTransition(AnimatorState sourceState, AnimatorState destState, AnimatorCondition[] conditions)
        {
            // Create a transition between states
            if (sourceState != null && destState != null)
            {
                var transition = sourceState.AddTransition(destState);
                transition.hasExitTime = false;
                transition.duration = transitionDuration;
                
                if (conditions != null)
                {
                    foreach (var condition in conditions)
                    {
                        transition.AddCondition(condition.mode, condition.threshold, condition.parameter);
                    }
                }
            }
        }
        
        private void ClearStateMachine(AnimatorStateMachine stateMachine)
        {
            // Get all states
            var states = new List<AnimatorState>();
            foreach (var stateInfo in stateMachine.states)
            {
                states.Add(stateInfo.state);
            }
            
            // Remove all states
            foreach (var state in states)
            {
                stateMachine.RemoveState(state);
            }
            
            // Get all sub-state machines
            var subStateMachines = new List<AnimatorStateMachine>();
            foreach (var subStateMachineInfo in stateMachine.stateMachines)
            {
                subStateMachines.Add(subStateMachineInfo.stateMachine);
            }
            
            // Remove all sub-state machines
            foreach (var subStateMachine in subStateMachines)
            {
                stateMachine.RemoveStateMachine(subStateMachine);
            }
            
            // Clear EntryTransitions and AnyStateTransitions
            while (stateMachine.entryTransitions.Length > 0)
            {
                stateMachine.RemoveEntryTransition(stateMachine.entryTransitions[0]);
            }
            
            while (stateMachine.anyStateTransitions.Length > 0)
            {
                stateMachine.RemoveAnyStateTransition(stateMachine.anyStateTransitions[0]);
            }
        }
    }
}
#endif 