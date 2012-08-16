using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/***
* MeshCreatorInspector
*	modifies the inspector to show controls for the Mesh Creator.
*	this script needs to be in the Editor folder of your project along
*	with the SimpleSurfaceEdge.cs and the Triangulator.cs script.
*
*	version 0.5 - updated 1/1/2012
***/
[CustomEditor(typeof(MeshCreatorData))]
public class MeshCreatorInspector :  Editor {
	
	private MeshCreatorData mcd;
	private const float versionNumber = 0.61f;
	private bool showColliderInfo = false;
	private bool showMeshInfo = false;
	private bool showMaterialInfo = false;
	private bool showExperimentalInfo = false;
	
	/***
	* OnEnable
	* 	set the MeshCreator when component is added to the object
	***/
	private void OnEnable()
    {
		mcd = target as MeshCreatorData;
		if (mcd == null) {
			Debug.LogError("MeshCreatorInspector::OnEnable(): couldn't find a MeshCreatorData component");
		}
    }
	 
	/***
	* OnInspectorGUI
	*	this does the main display of information in the inspector.
	***/
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		
		// TODO: inspector layout should be redesigned so that it's easier to 
		//	 see the texture and material information
		if (mcd != null) {
			GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
			boldStyle.fontStyle = FontStyle.Bold;
			boldStyle.font.material.color = Color.grey;
			GUILayout.Label("UCLA Game Lab Mesh Creator", boldStyle );
			EditorGUILayout.Space();
			//GUILayout.Label(" Mesh Creation Outline", boldStyle);
			EditorGUIUtility.LookLikeControls();
			mcd.outlineTexture = 
				EditorGUILayout.ObjectField("Mesh Outline Texture", mcd.outlineTexture, typeof(Texture2D), true) as Texture2D;
			EditorGUIUtility.LookLikeInspector();
			EditorGUILayout.Space();
			mcd.uvWrapMesh = EditorGUILayout.Toggle("Create Full Mesh?", mcd.uvWrapMesh);
			if (mcd.uvWrapMesh	) GUILayout.Label("  A 3d mesh will be created.");
			else 
			{
				if (mcd.createEdges && mcd.createBacksidePlane) GUILayout.Label("  Flat front and back planes will be created, with a mesh side edge.");
				else if (mcd.createEdges) GUILayout.Label("  Flat front plane will be created, with a mesh side edge.");
				else if (mcd.createBacksidePlane) GUILayout.TextArea("  Flat front and back planes will be created.");
				else GUILayout.Label("  A flat front plane will be created.");
			}
			EditorGUILayout.Space();
			mcd.generateCollider = EditorGUILayout.Toggle("Generate collider?", mcd.generateCollider);
			
			EditorGUILayout.Space();
			GUILayout.Label("  Mesh Size", boldStyle);
			mcd.meshHeight = EditorGUILayout.FloatField("Mesh height", mcd.meshHeight);
			mcd.meshWidth = EditorGUILayout.FloatField("Mesh width", mcd.meshWidth);
			mcd.meshDepth = EditorGUILayout.FloatField("Mesh depth", mcd.meshDepth);
			
			EditorGUILayout.Space();
			showMeshInfo = EditorGUILayout.Foldout(showMeshInfo, "Mesh Options");
			if (showMeshInfo)
			{
				EditorGUILayout.LabelField("  Mesh id number", mcd.idNumber );
				if (!mcd.uvWrapMesh) {
					mcd.createEdges = EditorGUILayout.Toggle("  Create full mesh for edge?", mcd.createEdges);
					mcd.createBacksidePlane = EditorGUILayout.Toggle("  Create backside plane?", mcd.createBacksidePlane);
					EditorGUILayout.Space();
				}
				if (mcd.uvWrapMesh || mcd.createEdges)
				{
					mcd.mergeClosePoints = EditorGUILayout.Toggle( "  Merge Close Points", mcd.mergeClosePoints);
					if (mcd.mergeClosePoints) mcd.mergeDistance = EditorGUILayout.FloatField( "  Merge Distance (px)", mcd.mergeDistance);
					EditorGUILayout.Space();
				}
			}
			
			EditorGUILayout.Space();
			showMaterialInfo = EditorGUILayout.Foldout(showMaterialInfo, "Material Options");
			if (showMaterialInfo)
			{
				mcd.useAutoGeneratedMaterial = EditorGUILayout.Toggle("  Auto Generate Material?", mcd.useAutoGeneratedMaterial);
				if (!mcd.useAutoGeneratedMaterial) mcd.frontMaterial = 
					EditorGUILayout.ObjectField("    Use Other Material", mcd.frontMaterial, typeof(Material), true ) as Material;
			
				
			}
			
			EditorGUILayout.Space();
			showColliderInfo = EditorGUILayout.Foldout(showColliderInfo, "Collider Options");
			if (showColliderInfo)
			{
				if (mcd.generateCollider) mcd.usePrimitiveCollider = EditorGUILayout.Toggle("  Create Primitive Collider?", mcd.usePrimitiveCollider);
				if (mcd.generateCollider && mcd.usePrimitiveCollider) mcd.smallestBoxArea = EditorGUILayout.FloatField("    Smallest Box Area", mcd.smallestBoxArea);
				//if (mcd.generateCollider && mcd.usePrimitiveCollider) mcd.useBoxCollider = EditorGUILayout.Toggle("Use Box Collider", mcd.useBoxCollider);
				if (mcd.generateCollider) {
					mcd.usePhysicMaterial = EditorGUILayout.Toggle("  Use Physics Material?", mcd.usePhysicMaterial);
					if (mcd.usePhysicMaterial) mcd.physicMaterial = 
						EditorGUILayout.ObjectField("    Physical Material", mcd.physicMaterial, typeof(PhysicMaterial), true) as PhysicMaterial;
					mcd.addRigidBody = EditorGUILayout.Toggle("  Add Rigidbody?", mcd.addRigidBody);
				}
			}
			
			EditorGUILayout.Space();
			showExperimentalInfo = EditorGUILayout.Foldout(showExperimentalInfo, "Experimental Options");
			if (showExperimentalInfo)
			{
				
				EditorGUILayout.LabelField("  Pivot Position", "");
				mcd.pivotHeightOffset = EditorGUILayout.FloatField("    Pivot Height Offset", mcd.pivotHeightOffset);
				mcd.pivotWidthOffset = EditorGUILayout.FloatField("    Pivot Width Offset", mcd.pivotWidthOffset);
				mcd.pivotDepthOffset = EditorGUILayout.FloatField("    Pivot Depth Offset", mcd.pivotDepthOffset);
			}
			
			EditorGUILayout.Space();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Update Mesh")){//, GUILayout.MinWidth(100))) {
				// do some simple parameter checking here so we don't get into trouble
				if (mcd.smallestBoxArea < 2) {
					Debug.LogWarning("Mesh Creator: smallest box area should be larger than 1.");
				}
				else {
					MeshCreator.UpdateMesh(mcd.gameObject);
					//Editor.Repaint(); // error when deleting colliders
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Mesh Creator Data", "version " + MeshCreatorData.versionNumber.ToString() );
			EditorGUILayout.LabelField("Creator Editor", "version " + versionNumber.ToString() );
		}
		else {
			Debug.LogError("MeshCreatorInspector::OnInspectorGUI(): couldn't find a MeshCreatorData component");
		}
		
	}
}

