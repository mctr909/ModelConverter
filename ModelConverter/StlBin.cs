using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class StlBin : BaseModel {
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

    public StlBin() { }

    public StlBin(string path) {
        var arrName = new byte[80];
        var arrCount = new byte[4];
        var arrFloat = new byte[4];
        var arrReserved = new byte[2];
        using (var fs = new FileStream(path, FileMode.Open)) {
            while (fs.Position < fs.Length) {
                var curObject = new Object();
                curObject.SurfaceList = new List<Surface>();
                fs.Read(arrName);
                fs.Read(arrCount);
                curObject.Name = Encoding.UTF8.GetString(arrName).Replace("\0", "");
                var count = BitConverter.ToInt32(arrCount);
                for (int i = 0; i < count; i++) {
                    var curSurface = new Surface();
                    // Normal
                    fs.Read(arrFloat);
                    var x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    var y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    var z = BitConverter.ToSingle(arrFloat);
                    curSurface.Norm = new vec3(x, y, z);
                    // Vartex 1
                    fs.Read(arrFloat);
                    x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    z = BitConverter.ToSingle(arrFloat);
                    curSurface.V1 = new vec3(x, y, z);
                    // Vartex 2
                    fs.Read(arrFloat);
                    x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    z = BitConverter.ToSingle(arrFloat);
                    curSurface.V2 = new vec3(x, y, z);
                    // Vartex 3
                    fs.Read(arrFloat);
                    x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    z = BitConverter.ToSingle(arrFloat);
                    // Reserved
                    fs.Read(arrReserved);
                    //
                    curSurface.V3 = new vec3(x, y, z);
                    curObject.SurfaceList.Add(curSurface);
                }
                ObjectList.Add(curObject);
            }
        }
    }

    public override void Save(string path) {
        var arrReserved = new byte[2];
        var fs = new FileStream(path, FileMode.Create);
        foreach (var obj in ObjectList) {
            byte[] tName = new byte[] { 0 };
            if (!string.IsNullOrEmpty(obj.Name)) {
                tName = Encoding.UTF8.GetBytes(obj.Name);
            }
            var arrName = new byte[80];
            Array.Copy(tName, arrName, Math.Min(arrName.Length, tName.Length));
            fs.Write(arrName);
            fs.Write(BitConverter.GetBytes(obj.SurfaceList.Count));
            foreach (var s in obj.SurfaceList) {
                // Normal
                fs.Write(BitConverter.GetBytes(s.Norm.x));
                fs.Write(BitConverter.GetBytes(s.Norm.y));
                fs.Write(BitConverter.GetBytes(s.Norm.z));
                // Vertex 1
                fs.Write(BitConverter.GetBytes(s.V1.x));
                fs.Write(BitConverter.GetBytes(s.V1.y));
                fs.Write(BitConverter.GetBytes(s.V1.z));
                // Vertex 2
                fs.Write(BitConverter.GetBytes(s.V2.x));
                fs.Write(BitConverter.GetBytes(s.V2.y));
                fs.Write(BitConverter.GetBytes(s.V2.z));
                // Vertex 3
                fs.Write(BitConverter.GetBytes(s.V3.x));
                fs.Write(BitConverter.GetBytes(s.V3.y));
                fs.Write(BitConverter.GetBytes(s.V3.z));
                // Reserved
                fs.Write(arrReserved);
            }
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
