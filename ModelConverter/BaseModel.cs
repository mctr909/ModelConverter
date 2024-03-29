﻿using System;
using System.Collections.Generic;

abstract class BaseModel {
    public enum ESwapAxiz {
        XZY,

        YXZ,
        YZX,
        
        ZXY,
        ZYX
    }

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

    public void SwapAxiz(ESwapAxiz type) {
        for (int vi = 0; vi < mVertList.Count; vi++) {
            var v = mVertList[vi];
            switch (type) {
            case ESwapAxiz.XZY:
                mVertList[vi] = new vec3(v.x, v.z, v.y);
                break;

            case ESwapAxiz.YXZ:
                mVertList[vi] = new vec3(v.y, v.x, v.z);
                break;
            case ESwapAxiz.YZX:
                mVertList[vi] = new vec3(v.y, v.z, v.x);
                break;

            case ESwapAxiz.ZXY:
                mVertList[vi] = new vec3(v.z, v.x, v.y);
                break;
            case ESwapAxiz.ZYX:
                mVertList[vi] = new vec3(v.z, v.y, v.x);
                break;
            }
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

    public void Normalize(float scale = 1) {
        var ofs = new vec3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (var v in mVertList) {
            ofs.x = Math.Min(ofs.x, v.x);
            ofs.y = Math.Min(ofs.y, v.y);
            ofs.z = Math.Min(ofs.z, v.z);
        }
        var max = new vec3(float.MinValue, float.MinValue, float.MinValue);
        foreach (var v in mVertList) {
            var sv = v - ofs;
            max.x = Math.Max(max.x, sv.x);
            max.y = Math.Max(max.y, sv.y);
            max.z = Math.Max(max.z, sv.z);
        }
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
