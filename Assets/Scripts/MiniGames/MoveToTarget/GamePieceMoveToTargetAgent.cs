using UnityEngine;
using System.Collections;

public class GamePieceMoveToTargetAgent : GamePiece {

	public override Mesh BuildMesh() {
		MeshBuilder meshBuilder = new MeshBuilder();
		int m_HeightSegmentCount = 8;
		int m_RadialSegmentCount = 8;
		float m_Radius = 1f;
		float m_VerticalScale = 1f;
		Quaternion rotation = Quaternion.identity;
		Vector3 offset = new Vector3(0f, -1f, 0f);
		//the angle increment per height segment:
		float angleInc = Mathf.PI / m_HeightSegmentCount;
		
		//the vertical (scaled) radius of the sphere:
		float verticalRadius = m_Radius * m_VerticalScale;
		
		//build the rings:
		for (int i = 0; i <= m_HeightSegmentCount; i++)
		{
			Vector3 centrePos = Vector3.zero;
			
			//calculate a height offset and radius based on a vertical circle calculation:
			centrePos.y = -Mathf.Cos(angleInc * i);
			float radius = Mathf.Sin(angleInc * i);
			
			//calculate the slope of the shpere at this ring based on the height and radius:
			Vector2 slope = new Vector3(-centrePos.y / m_VerticalScale, radius);
			slope.Normalize();
			
			//multiply the unit height by the vertical radius, and then add the radius to the height to make this sphere originate from its base rather than its centre:
			centrePos.y = centrePos.y * verticalRadius + verticalRadius;
			
			//scale the radius by the one stored in the partData:
			radius *= m_Radius;
			
			//calculate the final position of the ring centre:
			Vector3 finalRingCentre = rotation * centrePos + offset;
			
			//V coordinate:
			float v = (float)i / m_HeightSegmentCount;
			
			//build the ring:
			BuildRing(meshBuilder, m_RadialSegmentCount, finalRingCentre, radius, v, i > 0, rotation, slope);
		}

		return meshBuilder.CreateMesh ();
	}
}
