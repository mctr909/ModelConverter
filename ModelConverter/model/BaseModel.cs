using System;
using System.Collections.Generic;

abstract class BaseModel {
	public enum EInvertUV {
		None,
		ForwordU_ReverseV,
		ReverseU_ForwordV,
		ReverseU_ReverseV
	}

	protected struct Index {
		public int Vert;
		public int Norm;
		public int Uv;
		public Index(int vert = -1, int uv = -1, int norm = -1) {
			Vert = vert;
			Norm = norm;
			Uv = uv;
		}
	}

	protected class Surface {
		public string MaterialName = "";
		public List<Index> Indices = new List<Index>();
		public List<int> Line = new List<int>();
	}

	protected class Object {
		public string Name = "";
		public List<Surface> Surfaces = new List<Surface>();
	}

	protected class Material {
		public string Name = "";
		public vec3 Diffuse = new vec3(0.0f, 0.75f, 0.0f);
		public vec3 Ambient = new vec3();
		public vec3 Specular = new vec3();
		public float SpecularPower = 1;
		public float Alpha = 0;
		public string TexDiffuse = "";
		public string TexAlapha = "";
		public string TexBumpMap = "";
	}

	protected List<Object> mObjectList = new List<Object>();
	protected List<vec3> mVertList = new List<vec3>();
	protected List<vec3> mNormList = new List<vec3>();
	protected List<float[]> mUvList = new List<float[]>();
	protected Dictionary<string, Material> mMaterialList = new Dictionary<string, Material>();

	public abstract EAxisOrder AxisOrder { get; }
	public abstract EAxisDir AxisDir { get; }

	public EAxisOrder ChangeAxisOrder { get; set; }
	public EAxisDir ChangeAxisDir { get; set; }

	public int ObjectCount { get { return mObjectList.Count; } }

	public abstract void Save(string path);

	public virtual void Load(BaseModel srcModel) {
		foreach (var m in srcModel.mMaterialList) {
			mMaterialList.Add(m.Key, m.Value);
		}
		foreach (var o in srcModel.mObjectList) {
			mObjectList.Add(o);
		}
		foreach (var v in srcModel.mVertList) {
			mVertList.Add(v);
		}
		foreach (var n in srcModel.mNormList) {
			mVertList.Add(n);
		}
		foreach (var u in srcModel.mUvList) {
			mUvList.Add(u);
		}
		SwapAxis(srcModel.ChangeAxisOrder);
		ReverseAxis(srcModel.ChangeAxisDir);
	}

	public void Reverse() {
		for (int iobj = 0; iobj < mObjectList.Count; iobj++) {
			var obj = mObjectList[iobj];
			for (int isurf = 0; isurf < obj.Surfaces.Count; isurf++) {
				var surf = obj.Surfaces[isurf];
				var tmpIdxList = new List<Index>();
				for (int i = surf.Indices.Count - 1; 0 <= i; i--) {
					tmpIdxList.Add(surf.Indices[i]);
				}
				surf.Indices.Clear();
				surf.Indices.AddRange(tmpIdxList);
			}
		}
	}

	public void SwapAxis(EAxisOrder type) {
		switch (type) {
		case EAxisOrder.XZY:
			for (int vi = 0; vi < mVertList.Count; vi++) {
				var v = mVertList[vi];
				mVertList[vi] = new vec3(v.x, v.z, v.y);
			}
			break;
		case EAxisOrder.YXZ:
			for (int vi = 0; vi < mVertList.Count; vi++) {
				var v = mVertList[vi];
				mVertList[vi] = new vec3(v.y, v.x, v.z);
			}
			break;
		case EAxisOrder.YZX:
			for (int vi = 0; vi < mVertList.Count; vi++) {
				var v = mVertList[vi];
				mVertList[vi] = new vec3(v.y, v.z, v.x);
			}
			break;
		case EAxisOrder.ZXY:
			for (int vi = 0; vi < mVertList.Count; vi++) {
				var v = mVertList[vi];
				mVertList[vi] = new vec3(v.z, v.x, v.y);
			}
			break;
		case EAxisOrder.ZYX:
			for (int vi = 0; vi < mVertList.Count; vi++) {
				var v = mVertList[vi];
				mVertList[vi] = new vec3(v.z, v.y, v.x);
			}
			break;
		}
	}

	public void ReverseAxis(EAxisDir type) {
		int sx = 1, sy = 1, sz = 1;
		switch (type) {
		case EAxisDir.XpYpZn:
			sz = -1;
			break;
		case EAxisDir.XpYnZp:
			sy = -1;
			break;
		case EAxisDir.XpYnZn:
			sy = -1;
			sz = -1;
			break;
		case EAxisDir.XnYpZp:
			sx = -1;
			break;
		case EAxisDir.XnYpZn:
			sx = -1;
			sz = -1;
			break;
		case EAxisDir.XnYnZp:
			sx = -1;
			sy = -1;
			break;
		case EAxisDir.XnYnZn:
			sx = -1;
			sy = -1;
			sz = -1;
			break;
		}
		for (int i = 0; i < mVertList.Count; i++) {
			mVertList[i].x *= sx;
			mVertList[i].y *= sy;
			mVertList[i].z *= sz;
		}
	}

	public void TransformUV(EInvertUV invertType, bool swapUV = false) {
		switch (invertType) {
		case EInvertUV.ForwordU_ReverseV:
			for (int ui = 0; ui < mUvList.Count; ui++) {
				var uv = new float[] { mUvList[ui][0], 1.0f - mUvList[ui][1] };
				mUvList[ui] = uv;
			}
			break;
		case EInvertUV.ReverseU_ForwordV:
			for (int ui = 0; ui < mUvList.Count; ui++) {
				var uv = new float[] { 1.0f - mUvList[ui][0], mUvList[ui][1] };
				mUvList[ui] = uv;
			}
			break;
		case EInvertUV.ReverseU_ReverseV:
			for (int ui = 0; ui < mUvList.Count; ui++) {
				var uv = new float[] { 1.0f - mUvList[ui][0], 1.0f - mUvList[ui][1] };
				mUvList[ui] = uv;
			}
			break;
		}
		if (swapUV) {
			for (int ui = 0; ui < mUvList.Count; ui++) {
				var uv = new float[] { mUvList[ui][1], mUvList[ui][0] };
				mUvList[ui] = uv;
			}
		}
	}

	public vec3 GetSize() {
		var ofs = new vec3(float.MaxValue, float.MaxValue, float.MaxValue);
		foreach (var v in mVertList) {
			ofs.x = Math.Min(ofs.x, v.x);
			ofs.y = Math.Min(ofs.y, v.y);
			ofs.z = Math.Min(ofs.z, v.z);
		}
		var size = new vec3(float.MinValue, float.MinValue, float.MinValue);
		foreach (var v in mVertList) {
			var sv = v - ofs;
			size.x = Math.Max(size.x, sv.x);
			size.y = Math.Max(size.y, sv.y);
			size.z = Math.Max(size.z, sv.z);
		}
		return size;
	}

	public void Normalize(float scale = 1) {
		var max = GetSize();
		var size = Math.Max(max.x, Math.Max(max.y, max.z));
		for (int i = 0; i < mVertList.Count; i++) {
			mVertList[i] *= scale / size;
		}
	}

	protected void ToTriangle() {
		for (int j = 0; j < mObjectList.Count; j++) {
			var obj = mObjectList[j];
			var tmpSurfList = new List<Surface>();
			foreach (var s in obj.Surfaces) {
				if (s.Indices.Count < 4) {
					tmpSurfList.Add(s);
					continue;
				}
				if (s.Indices.Count % 2 == 0) {
					evenPoligon(tmpSurfList, s);
				} else {
					oddPoligon(tmpSurfList, s);
				}
			}
			obj.Surfaces.Clear();
			foreach (var s in tmpSurfList) {
				obj.Surfaces.Add(s);
			}
		}
	}

	void evenPoligon(List<Surface> surfaceList, Surface surface) {
		for (int i = 0; i < surface.Indices.Count / 2 - 1; i++) {
			var a0 = surface.Indices[i];
			var a1 = surface.Indices[i + 1];
			var a2 = surface.Indices[surface.Indices.Count - i - 2];
			var b0 = a2;
			var b1 = surface.Indices[surface.Indices.Count - i - 1];
			var b2 = a0;

			var surfA = new Surface();
			surfA.MaterialName = surface.MaterialName;
			surfA.Indices.Add(a0);
			surfA.Indices.Add(a1);
			surfA.Indices.Add(a2);
			surfaceList.Add(surfA);

			var surfB = new Surface();
			surfB.MaterialName = surface.MaterialName;
			surfB.Indices.Add(b0);
			surfB.Indices.Add(b1);
			surfB.Indices.Add(b2);
			surfaceList.Add(surfB);
		}
	}

	void oddPoligon(List<Surface> surfaceList, Surface surface) {
		{
			var a0 = surface.Indices[surface.Indices.Count - 2];
			var a1 = surface.Indices[surface.Indices.Count - 1];
			var a2 = surface.Indices[0];
			var surf = new Surface();
			surf.MaterialName = surface.MaterialName;
			surf.Indices.Add(a0);
			surf.Indices.Add(a1);
			surf.Indices.Add(a2);
			surfaceList.Add(surf);
		}
		for (int i = 0; i < surface.Indices.Count / 2 - 1; i++) {
			var a0 = surface.Indices[i];
			var a1 = surface.Indices[i + 1];
			var a2 = surface.Indices[surface.Indices.Count - i - 3];
			var b0 = a2;
			var b1 = surface.Indices[surface.Indices.Count - i - 2];
			var b2 = a0;

			var surfA = new Surface();
			surfA.MaterialName = surface.MaterialName;
			surfA.Indices.Add(a0);
			surfA.Indices.Add(a1);
			surfA.Indices.Add(a2);
			surfaceList.Add(surfA);

			var surfB = new Surface();
			surfB.MaterialName = surface.MaterialName;
			surfB.Indices.Add(b0);
			surfB.Indices.Add(b1);
			surfB.Indices.Add(b2);
			surfaceList.Add(surfB);
		}
	}
}
