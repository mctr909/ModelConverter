using System;
using System.Collections.Generic;

abstract class BaseModel {
    public enum ESwapAxiz {
        XYZ,
        XZY,
        YXZ,
        YZX,
        ZXY,
        ZYX
    }

    public enum EInvertAxiz {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    public enum EInvertUV {
        None = 0,
        U = 1,
        V = 2
    }

    public struct Surface {
        public string MaterialName;
        public List<int> VertIdx;
        public List<int> NormIdx;
        public List<int> UvIdx;
        public Surface() {
            MaterialName = "";
            VertIdx = new List<int>();
            NormIdx = new List<int>();
            UvIdx = new List<int>();
        }
    }

    public struct Object {
        public string Name;
        public List<Surface> Surfaces;
        public Object() {
            Name = "";
            Surfaces = new List<Surface>();
        }
    }

    protected List<Object> mObjectList = new List<Object>();
    protected List<vec3> mVertList = new List<vec3>();
    protected List<vec3> mNormList = new List<vec3>();
    protected List<float[]> mUvList = new List<float[]>();

    public bool SwapUV = false;
    public EInvertAxiz InvertAxiz = EInvertAxiz.None;
    public EInvertUV InvertUV = EInvertUV.None;

    public int ObjectCount { get { return mObjectList.Count; } }

    public int SignX { get { return 0 < (InvertAxiz & EInvertAxiz.X) ? -1 : 1; } }
    public int SignY { get { return 0 < (InvertAxiz & EInvertAxiz.Y) ? -1 : 1; } }
    public int SignZ { get { return 0 < (InvertAxiz & EInvertAxiz.Z) ? -1 : 1; } }

    public bool InvertU { get { return 0 < (InvertUV & EInvertUV.U); } }
    public bool InvertV { get { return 0 < (InvertUV & EInvertUV.V); } }

    public abstract void Save(string path);

    public void Load(BaseModel srcModel) {
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

    public void SwapAxiz(ESwapAxiz type) {
        for (int vi = 0; vi < mVertList.Count; vi++) {
            var v = mVertList[vi];
            switch (type) {
            case ESwapAxiz.XYZ:
                mVertList[vi] = new vec3(v.x, v.y, v.z);
                break;
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
            var surfaceList = new List<Surface>();
            foreach (var s in obj.Surfaces) {
                if (s.VertIdx.Count % 2 == 0) {
                    evenPoligon(surfaceList, s);
                } else {
                    oddPoligon(surfaceList, s);
                }
            }
            obj.Surfaces.Clear();
            foreach (var s in surfaceList) {
                obj.Surfaces.Add(s);
            }
        }
    }

    void evenPoligon(List<Surface> surfaceList, Surface surface) {
        for (int i = 0; i < surface.VertIdx.Count / 2 - 1; i++) {
            var surfA = new Surface();
            var surfB = new Surface();
            var a0 = i;
            var a1 = i + 1;
            var a2 = surface.VertIdx.Count - i - 2;
            var b0 = a2;
            var b1 = surface.VertIdx.Count - i - 1;
            var b2 = a0;

            surfA.VertIdx = new List<int> {
                surface.VertIdx[a0],
                surface.VertIdx[a1],
                surface.VertIdx[a2]
            };
            surfB.VertIdx = new List<int> {
                surface.VertIdx[b0],
                surface.VertIdx[b1],
                surface.VertIdx[b2]
            };
            if (surface.NormIdx.Count == surface.VertIdx.Count) {
                surfA.NormIdx = new List<int> {
                    surface.NormIdx[a0],
                    surface.NormIdx[a1],
                    surface.NormIdx[a2]
                };
                surfB.NormIdx = new List<int> {
                    surface.NormIdx[b0],
                    surface.NormIdx[b1],
                    surface.NormIdx[b2]
                };
            }
            if (surface.UvIdx.Count == surface.VertIdx.Count) {
                surfA.UvIdx = new List<int> {
                    surface.UvIdx[a0],
                    surface.UvIdx[a1],
                    surface.UvIdx[a2]
                };
                surfB.UvIdx = new List<int> {
                    surface.UvIdx[b0],
                    surface.UvIdx[b1],
                    surface.UvIdx[b2]
                };
            }

            surfaceList.Add(surfA);
            surfaceList.Add(surfB);
        }
    }

    void oddPoligon(List<Surface> surfaceList, Surface surface) {
        {
            var a0 = surface.VertIdx.Count - 2;
            var a1 = surface.VertIdx.Count - 1;
            var a2 = 0;
            var surf = new Surface();
            surf.VertIdx = new List<int> {
                surface.VertIdx[a0],
                surface.VertIdx[a1],
                surface.VertIdx[a2]
            };
            if (surface.NormIdx.Count == surface.VertIdx.Count) {
                surf.NormIdx = new List<int> {
                    surface.NormIdx[a0],
                    surface.NormIdx[a1],
                    surface.NormIdx[a2]
                };
            }
            if (surface.UvIdx.Count == surface.VertIdx.Count) {
                surf.UvIdx = new List<int> {
                    surface.UvIdx[a0],
                    surface.UvIdx[a1],
                    surface.UvIdx[a2]
                };
            }
            surfaceList.Add(surf);
        }
        for (int i = 0; i < surface.VertIdx.Count / 2 - 1; i++) {
            var surfA = new Surface();
            var surfB = new Surface();
            var a0 = i;
            var a1 = i + 1;
            var a2 = surface.VertIdx.Count - i - 3;
            var b0 = a2;
            var b1 = surface.VertIdx.Count - i - 2;
            var b2 = a0;

            surfA.VertIdx = new List<int> {
                surface.VertIdx[a0],
                surface.VertIdx[a1],
                surface.VertIdx[a2]
            };
            surfB.VertIdx = new List<int> {
                surface.VertIdx[b0],
                surface.VertIdx[b1],
                surface.VertIdx[b2]
            };
            if (surface.NormIdx.Count == surface.VertIdx.Count) {
                surfA.NormIdx = new List<int> {
                    surface.NormIdx[a0],
                    surface.NormIdx[a1],
                    surface.NormIdx[a2]
                };
                surfB.NormIdx = new List<int> {
                    surface.NormIdx[b0],
                    surface.NormIdx[b1],
                    surface.NormIdx[b2]
                };
            }
            if (surface.UvIdx.Count == surface.VertIdx.Count) {
                surfA.UvIdx = new List<int> {
                    surface.UvIdx[a0],
                    surface.UvIdx[a1],
                    surface.UvIdx[a2]
                };
                surfB.UvIdx = new List<int> {
                    surface.UvIdx[b0],
                    surface.UvIdx[b1],
                    surface.UvIdx[b2]
                };
            }

            surfaceList.Add(surfA);
            surfaceList.Add(surfB);
        }
    }
}
