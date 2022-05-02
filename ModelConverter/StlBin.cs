using System;
using System.IO;
using System.Text;

class StlBin : BaseModel {
    public StlBin() { }

    public StlBin(string path) {
        var arrName = new byte[80];
        var arrCount = new byte[4];
        var arrFloat = new byte[4];
        var arrReserved = new byte[2];
        using (var fs = new FileStream(path, FileMode.Open)) {
            while (fs.Position < fs.Length) {
                var curObject = new Object();
                fs.Read(arrName);
                fs.Read(arrCount);
                curObject.Name = Encoding.UTF8.GetString(arrName).Replace("\0", "");
                var count = BitConverter.ToInt32(arrCount);
                for (int i = 0; i < count; i++) {
                    var curSurface = new Surface();
                    // Normal
                    var curNorm = mNormList.Count;
                    fs.Read(arrFloat);
                    var x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    var y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    var z = BitConverter.ToSingle(arrFloat);
                    mNormList.Add(new vec3(x, y, z));

                    // Vartex 1
                    var idx = new Index();
                    idx.Vert = mVertList.Count;
                    idx.Norm = curNorm;
                    fs.Read(arrFloat);
                    x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    z = BitConverter.ToSingle(arrFloat);
                    mVertList.Add(new vec3(x, y, z));
                    curSurface.Indices.Add(idx);

                    // Vartex 2
                    idx = new Index();
                    idx.Vert = mVertList.Count;
                    idx.Norm = curNorm;
                    fs.Read(arrFloat);
                    x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    z = BitConverter.ToSingle(arrFloat);
                    mVertList.Add(new vec3(x, y, z));
                    curSurface.Indices.Add(idx);

                    // Vartex 3
                    idx = new Index();
                    idx.Vert = mVertList.Count;
                    idx.Norm = curNorm;
                    fs.Read(arrFloat);
                    x = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    y = BitConverter.ToSingle(arrFloat);
                    fs.Read(arrFloat);
                    z = BitConverter.ToSingle(arrFloat);
                    mVertList.Add(new vec3(x, y, z));
                    curSurface.Indices.Add(idx);

                    // Reserved
                    fs.Read(arrReserved);
                    //
                    curObject.Surfaces.Add(curSurface);
                }
                mObjectList.Add(curObject);
            }
        }
    }

    public override void Save(string path) {
        ToTriangle();
        var arrReserved = new byte[2];
        var fs = new FileStream(path, FileMode.Create);
        foreach (var obj in mObjectList) {
            byte[] tName = new byte[] { 0 };
            if (!string.IsNullOrEmpty(obj.Name)) {
                tName = Encoding.UTF8.GetBytes(obj.Name);
            }
            var arrName = new byte[80];
            Array.Copy(tName, arrName, Math.Min(arrName.Length, tName.Length));
            fs.Write(arrName);
            fs.Write(BitConverter.GetBytes(obj.Surfaces.Count));
            foreach (var s in obj.Surfaces) {
                // Normal
                var nn = new vec3();
                foreach (var idx in s.Indices) {
                    if (0 <= idx.Norm) {
                        var n = mNormList[idx.Norm];
                        nn += n;
                    }
                }
                nn.Normalize();
                fs.Write(BitConverter.GetBytes(nn.x));
                fs.Write(BitConverter.GetBytes(nn.y));
                fs.Write(BitConverter.GetBytes(nn.z));
                // Vertex
                foreach (var idx in s.Indices) {
                    var v = mVertList[idx.Vert];
                    fs.Write(BitConverter.GetBytes(v.x));
                    fs.Write(BitConverter.GetBytes(v.y));
                    fs.Write(BitConverter.GetBytes(v.z));
                }
                // Reserved
                fs.Write(arrReserved);
            }
        }
        fs.Close();
    }
}
