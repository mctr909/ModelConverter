using System;
using System.Collections.Generic;
using System.IO;

class StlText : BaseModel {
    public struct Surface {
        public vec3 Norm;
        public vec3 V1;
        public vec3 V2;
        public vec3 V3;
    }

    public struct Object {
        public string Name;
        public List<Surface> SurfaceList;
    }

    public List<Object> ObjectList = new List<Object>();

    public StlText() { }

    public StlText(string path) {
        var curSurface = new Surface();
        var curVertex = new List<vec3>();
        var curObject = new Object();
        curObject.SurfaceList = new List<Surface>();

        using (var fs = new StreamReader(path)) {
            while (!fs.EndOfStream) {
                var line = fs.ReadLine().Replace("\t", " ").TrimStart().Replace("  ", " ");
                var cols = line.Split(" ");
                if (cols.Length < 1 || string.IsNullOrEmpty(cols[0])) {
                    continue;
                }

                switch (cols[0].ToLower()) {
                case "solid":
                    curObject = new Object();
                    curObject.SurfaceList = new List<Surface>();
                    curObject.Name = 2 <= cols.Length ? cols[1] : "";
                    break;
                case "facet":
                    curSurface = new Surface();
                    if (cols.Length < 2) {
                        break;
                    }
                    if (cols[1].ToLower() == "normal") {
                        curSurface.Norm = new vec3(float.Parse(cols[2]), float.Parse(cols[3]), float.Parse(cols[4]));
                    }
                    break;
                case "outer":
                    curVertex = new List<vec3>();
                    break;
                case "vertex": {
                    var v = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
                    curVertex.Add(v);
                    break;
                }
                case "endloop":
                    if (curVertex.Count != 3) {
                        return;
                    }
                    curSurface.V1 = curVertex[0];
                    curSurface.V2 = curVertex[1];
                    curSurface.V3 = curVertex[2];
                    break;
                case "endfacet":
                    curObject.SurfaceList.Add(curSurface);
                    break;
                case "endsolid":
                    ObjectList.Add(curObject);
                    break;

                default:
                    return;
                }
            }
        }
    }

    public override void Save(string path) {
        var fs = new StreamWriter(path);
        foreach (var obj in ObjectList) {
            fs.WriteLine("solid {0}", obj.Name);
            foreach (var s in obj.SurfaceList) {
                fs.WriteLine("\tfacet normal {0} {1} {2}", s.Norm.x, s.Norm.y, s.Norm.z);
                fs.WriteLine("\t\touter loop");
                fs.WriteLine("\t\t\tvertex {0} {1} {2}", s.V1.x, s.V1.y, s.V1.z);
                fs.WriteLine("\t\t\tvertex {0} {1} {2}", s.V2.x, s.V2.y, s.V2.z);
                fs.WriteLine("\t\t\tvertex {0} {1} {2}", s.V3.x, s.V3.y, s.V3.z);
                fs.WriteLine("\t\tendloop");
                fs.WriteLine("\tendfacet");
            }
            fs.WriteLine("endsolid");
        }
        fs.Close();
    }

    public override void Normalize(float scale = 1) {
        var ofs = new vec3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (var obj in ObjectList) {
            foreach (var s in obj.SurfaceList) {
                ofs.x = Math.Min(ofs.x, s.V1.x);
                ofs.y = Math.Min(ofs.y, s.V1.y);
                ofs.z = Math.Min(ofs.z, s.V1.z);
                ofs.x = Math.Min(ofs.x, s.V2.x);
                ofs.y = Math.Min(ofs.y, s.V2.y);
                ofs.z = Math.Min(ofs.z, s.V2.z);
                ofs.x = Math.Min(ofs.x, s.V3.x);
                ofs.y = Math.Min(ofs.y, s.V3.y);
                ofs.z = Math.Min(ofs.z, s.V3.z);
            }
        }
        var max = new vec3(float.MinValue, float.MinValue, float.MinValue);
        foreach (var obj in ObjectList) {
            foreach (var s in obj.SurfaceList) {
                var sv = s.V1 - ofs;
                max.x = Math.Max(max.x, sv.x);
                max.y = Math.Max(max.y, sv.y);
                max.z = Math.Max(max.z, sv.z);
                sv = s.V2 - ofs;
                max.x = Math.Max(max.x, sv.x);
                max.y = Math.Max(max.y, sv.y);
                max.z = Math.Max(max.z, sv.z);
                sv = s.V3 - ofs;
                max.x = Math.Max(max.x, sv.x);
                max.y = Math.Max(max.y, sv.y);
                max.z = Math.Max(max.z, sv.z);
            }
        }
        var size = Math.Max(max.x, Math.Max(max.y, max.z));
        foreach (var obj in ObjectList) {
            for (int i = 0; i < obj.SurfaceList.Count; i++) {
                var s = obj.SurfaceList[i];
                s.V1 *= scale / size;
                s.V2 *= scale / size;
                s.V3 *= scale / size;
                obj.SurfaceList[i] = s;
            }
        }
    }
}
