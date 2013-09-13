using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Useful Orthello static helper functions
/// </summary>
public class OTHelper {

	/// <summary>
	/// Get children transforms of a GameObject, ordered by name (as displayed in the scene hierarchy)
	/// </summary>
	public static Transform[] ChildrenOrderedByName(Transform parent)
	{
		List<Transform> res = new List<Transform>();
		if (parent!=null && parent.childCount>0)
			foreach(Transform child in parent.transform)
				res.Add(child);
		
		if (res.Count>0)
			res.Sort(delegate(Transform a , Transform b)
			{
				string sa = a.name.Replace("-","_").ToLower();
				string sb = b.name.Replace("-","_").ToLower();
				return string.Compare(sa, sb);
			});
				
		return res.ToArray();		
	}
	
	/// <summary>
	/// Sets the layer of the childrens of the provided parent
	/// </summary>
	public static void ChildrenSetLayer(GameObject parent, int layer, List<GameObject> exclude)
	{
		foreach (Transform child in parent.transform)
			if (exclude == null || !exclude.Contains(child.gameObject))
			{
				child.gameObject.layer = layer;
				ChildrenSetLayer(child.gameObject,layer,exclude);
			}
	}
	public static void ChildrenSetLayer(GameObject parent, int layer)
	{
		ChildrenSetLayer(parent,layer,null);
	}

	/// <summary>
	/// Converts world coordinate based Rectangle to Bounds, using a specifc depth size 
	/// </summary>
	public static Bounds RectToBounds(Rect r, int depthSize)
	{
		bool td = OT.world == OT.World.WorldTopDown2D;
		Vector3 center = new Vector3(r.center.x, td?0:r.center.y, td?r.center.y:0);
		Vector3 size = new Vector3(Mathf.Abs(r.width), td?depthSize:Mathf.Abs(r.height), td?Mathf.Abs(r.height):depthSize);			
		return new Bounds(center, size);		
	}

	/// <summary>
	/// Converts world coordinate based Rectangle to Bounds
	/// </summary>
	public static Bounds RectToBounds(Rect r)
	{
		return RectToBounds(r,3000);
	}

	/// <summary>
	/// Converts world coordinate based Rectangle to Bounds
	/// </summary>
	public static Vector3 WorldPoint(GameObject g, Vector3 point)
	{
		if (g.transform.parent == null)
			return point;
		else
			return g.transform.parent.localToWorldMatrix.MultiplyPoint3x4(point);
	}
	
	/// loads a texture from resources
	public static Texture2D ResourceTexture(string filename)
	{
		Texture2D tex = Resources.Load(filename, typeof(Texture2D)) as Texture2D;
		return  tex;
	}
	

	
}
