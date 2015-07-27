using UnityEngine;
using System.Collections;

public class GamePieceMoveToTargetWalls : GamePiece {

	public override Mesh BuildMesh() {
		MeshBuilder meshBuilder = new MeshBuilder();

		BuildQuad (meshBuilder, new Vector3(-1f, -1f, 1f), Vector3.right*2f, Vector3.up*2f); // BACK
		BuildQuad (meshBuilder, new Vector3(1f, -1f, 1f), Vector3.back*2f, Vector3.up*2f); // RIGHT
		BuildQuad (meshBuilder, new Vector3(-1f, -1f, 1f), Vector3.back*2f, Vector3.right*2f); // BOTTOM
		BuildQuad (meshBuilder, new Vector3(1f, -1f, -1f), Vector3.left*2f, Vector3.up*2f); // FRONT
		BuildQuad (meshBuilder, new Vector3(-1f, -1f, -1f), Vector3.forward*2f, Vector3.up*2f); // LEFT
		BuildQuad (meshBuilder, new Vector3(-1f, 1f, 1f), Vector3.right*2f, Vector3.back*2f); // TOP
		return meshBuilder.CreateMesh ();
	}
}
