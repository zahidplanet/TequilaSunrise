using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class CreateAvatarAnimations : EditorWindow
{
    private string controllerName = "AvatarAnimationController";
    private string controllerPath = "Assets/Animations";
    
    [MenuItem("Tools/Create Avatar Animations")]
    public static void ShowWindow()
    {
        GetWindow<CreateAvatarAnimations>("Create Avatar Animations");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Create Avatar Animation Controller", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        controllerName = EditorGUILayout.TextField("Controller Name:", controllerName);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Save Location:", EditorStyles.boldLabel);
        controllerPath = EditorGUILayout.TextField("Path:", controllerPath);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will create a basic animation controller with Idle, Walk, Run, and Jump states.", MessageType.Info);
        
        if (GUILayout.Button("Create Animation Controller"))
        {
            CreateAnimationController();
        }
    }
    
    private void CreateAnimationController()
    {
        // Ensure the directory exists
        if (!Directory.Exists(controllerPath))
        {
            Directory.CreateDirectory(controllerPath);
        }
        
        // Create a new AnimatorController
        string fullPath = Path.Combine(controllerPath, controllerName + ".controller");
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(fullPath);
        
        // Get the root state machine
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        
        // Create states
        AnimatorState idleState = rootStateMachine.AddState("Idle");
        AnimatorState walkState = rootStateMachine.AddState("Walk");
        AnimatorState runState = rootStateMachine.AddState("Run");
        AnimatorState jumpState = rootStateMachine.AddState("Jump");
        
        // Set Idle as the default state
        rootStateMachine.defaultState = idleState;
        
        // Create parameters
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("FreeFall", AnimatorControllerParameterType.Bool);
        
        // Create transitions
        // Idle -> Walk
        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        idleToWalk.duration = 0.25f;
        
        // Walk -> Idle
        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
        walkToIdle.duration = 0.25f;
        
        // Walk -> Run
        AnimatorStateTransition walkToRun = walkState.AddTransition(runState);
        walkToRun.AddCondition(AnimatorConditionMode.Greater, 4f, "Speed");
        walkToRun.duration = 0.25f;
        
        // Run -> Walk
        AnimatorStateTransition runToWalk = runState.AddTransition(walkState);
        runToWalk.AddCondition(AnimatorConditionMode.Less, 4f, "Speed");
        runToWalk.duration = 0.25f;
        
        // Any -> Jump
        AnimatorStateTransition anyToJump = rootStateMachine.AddAnyStateTransition(jumpState);
        anyToJump.AddCondition(AnimatorConditionMode.If, 0f, "Jump");
        anyToJump.duration = 0.1f;
        
        // Jump -> Idle (when grounded)
        AnimatorStateTransition jumpToIdle = jumpState.AddTransition(idleState);
        jumpToIdle.AddCondition(AnimatorConditionMode.If, 0f, "Grounded");
        jumpToIdle.duration = 0.25f;
        
        EditorUtility.DisplayDialog("Success", "Animation controller created at " + fullPath, "OK");
        
        // Select the created controller in the Project window
        Selection.activeObject = controller;
    }
} 