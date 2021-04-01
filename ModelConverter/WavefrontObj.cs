using System;
using System.Collections.Generic;
using System.IO;

class WavefrontObj : IModel {
    public struct Surface {
        public List<int> VertexIndex;
        public List<int> UvIndex;
        public string MaterialName;
    }

    public struct Object {
        public string Name;
        public List<Surface> SurfaceList;
    }

    public struct Material {
        public string Name;
        public vec3 Diffuse;
        public vec3 Ambient;
        public vec3 Specular;
        public float SpecularPower;
        public float Alpha;
        public Texture TexDiffuse;
        public Texture TexAmbient;
        public Texture TexSpecular;
        public Texture TexSpecularPower;
        public Texture TexAlapha;
        public Texture TexBumpMap;
    }

    public class Texture {
        public string FileName;
        public vec3 Offset = new vec3();
        public vec3 Scale = new vec3(1, 1, 1);
        public float BaseValue = 0;
        public float GainValue = 1;
        public float BumpScale = 0;
        public bool ChannelR = false;
        public bool ChannelG = false;
        public bool ChannelB = false;
        public bool ChannelM = false;
        public bool ChannelL = false;
        public bool ChannelZ = false;

        public Texture() { }

        public Texture(string[] cols) {
            FileName = cols[cols.Length - 1];

            int colIdx = 1;
            for (; colIdx < cols.Length; colIdx++) {
                switch (cols[colIdx]) {
                case "-blendu":
                case "-blendv":
                case "-boost":
                case "-texres":
                case "-clamp":
                    colIdx++;
                    break;

                case "-mm":
                    BaseValue = float.Parse(cols[++colIdx]);
                    GainValue = float.Parse(cols[++colIdx]);
                    break;
                case "-bm":
                    BumpScale = float.Parse(cols[++colIdx]);
                    break;
                case "-o": {
                    float x = float.Parse(cols[++colIdx]);
                    float y;
                    float z;
                    if (float.TryParse(cols[colIdx + 1], out y)) {
                        colIdx++;
                        if (float.TryParse(cols[colIdx + 1], out z)) {
                            colIdx++;
                        } else {
                            z = 0;
                        }
                    } else {
                        y = 0;
                        z = 0;
                    }
                    Offset = new vec3(x, y, z);
                    break;
                }
                case "-s": {
                    float x = float.Parse(cols[++colIdx]);
                    float y;
                    float z;
                    if (float.TryParse(cols[colIdx + 1], out y)) {
                        colIdx++;
                        if (float.TryParse(cols[colIdx + 1], out z)) {
                            colIdx++;
                        } else {
                            z = 1;
                        }
                    } else {
                        y = 1;
                        z = 1;
                    }
                    Scale = new vec3(x, y, z);
                    break;
                }
                case "-t": {
                    colIdx++;
                    float y;
                    float z;
                    if (float.TryParse(cols[colIdx + 1], out y)) {
                        colIdx++;
                        if (float.TryParse(cols[colIdx + 1], out z)) {
                            colIdx++;
                        }
                    }
                    break;
                }
                case "-imfchan": {
                    do {
                        switch (cols[++colIdx]) {
                        case "r":
                            ChannelR = true;
                            break;
                        case "g":
                            ChannelG = true;
                            break;
                        case "b":
                            ChannelB = true;
                            break;
                        case "m":
                            ChannelM = true;
                            break;
                        case "l":
                            ChannelL = true;
                            break;
                        case "z":
                            ChannelZ = true;
                            break;
                        default:
                            break;
                        }
                    } while ("|" == cols[++colIdx]);
                    break;
                }
                default:
                    break;
                }
            }
        }

        public void Write(StreamWriter fs) {
            if (0 < Offset.Abs) {
                fs.Write(" -o {0} {1} {2}", Offset.x, Offset.y, Offset.z);
            }
            if (Scale.x != 1 || Scale.y != 1 || Scale.z != 1) {
                fs.Write(" -s {0} {1} {2}", Scale.x, Scale.y, Scale.z);
            }
            if (BaseValue != 0 || GainValue != 1) {
                fs.Write(" -mm {0} {1}", BaseValue, GainValue);
            }
            if (BumpScale != 0) {
                fs.Write(" -bm {0}", BumpScale);
            }

            fs.Write(" -imfchan");
            var imfchan = "";
            if (ChannelR) {
                imfchan += " r";
            }
            if (ChannelG) {
                imfchan += 0 < imfchan.Length ? " | g" : " g";
            }
            if (ChannelB) {
                imfchan += 0 < imfchan.Length ? " | b" : " b";
            }
            if (ChannelM) {
                imfchan += 0 < imfchan.Length ? " | m" : " m";
            }
            if (ChannelL) {
                imfchan += 0 < imfchan.Length ? " | l" : " l";
            }
            if (ChannelZ) {
                imfchan += 0 < imfchan.Length ? " | z" : " z";
            }
            fs.Write(imfchan);

            fs.WriteLine(" " + FileName);
        }
    }

    public List<Material> MaterialList = new List<Material>();
    public List<vec3> VertexList = new List<vec3>();
    public List<float[]> UvList = new List<float[]>();
    public List<Object> ObjectList = new List<Object>();

    public WavefrontObj() { }

    public WavefrontObj(string filePath) {
        var curMaterial = "";
        var curObject = new Object();
        curObject.SurfaceList = new List<Surface>();

        using (var fs = new StreamReader(filePath)) {
            int row = 0;
            while (!fs.EndOfStream) {
                var line = fs.ReadLine().Replace("\t", "").Replace("  ", " ").Trim();
                var cols = line.Split(" ");
                row++;
                if (cols.Length < 1 || string.IsNullOrEmpty(cols[0])) {
                    continue;
                }
                if (0 == cols[0].TrimStart().IndexOf("#")) {
                    continue;
                }

                switch (cols[0].ToLower()) {
                case "mtllib":
                    loadMaterial(Path.GetDirectoryName(filePath), cols[1]);
                    break;
                case "usemtl":
                    curMaterial = line.Substring(7).Replace("\"", "");
                    break;
                case "l":
                    break;
                case "s":
                    break;
                case "v":
                    if (cols.Length != 4) {
                        Console.WriteLine("頂点の値の個数が4でない {0}\n\"{1}\"", cols.Length, line);
                        Console.ReadKey();
                        return;
                    } else {
                        var v = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
                        VertexList.Add(v);
                    }
                    break;
                case "vt":
                    if (cols.Length < 3) {
                        Console.WriteLine("UVの値の個数が3未満 {0}\n\"{1}\"", cols.Length, line);
                        Console.ReadKey();
                        return;
                    }
                    UvList.Add(new float[] { float.Parse(cols[1]), float.Parse(cols[2]) });
                    break;
                case "vn":
                    break;
                case "vp":
                    break;

                case "g":
                    if (0 < curObject.SurfaceList.Count) {
                        ObjectList.Add(curObject);
                    }
                    curObject = new Object();
                    curObject.SurfaceList = new List<Surface>();
                    if (2 <= cols.Length) {
                        curObject.Name = line.Substring(2).Replace("\"", "");
                    }
                    break;
                case "f":
                    if (cols.Length < 4) {
                        return;
                    }
                    var s = new Surface();
                    s.MaterialName = curMaterial;
                    s.VertexIndex = new List<int>();
                    s.UvIndex = new List<int>();
                    int vertexIdx, uvIdx;
                    for (int i = 1; i < cols.Length; i++) {
                        var fcols = cols[i].Split("/");
                        switch (fcols.Length) {
                        case 1:
                            if (!int.TryParse(fcols[0], out vertexIdx)) {
                                Console.WriteLine("頂点インデックスエラー velue\"{0}\"\n\"{1}\"", fcols[0], line);
                                Console.ReadKey();
                                return;
                            }
                            s.VertexIndex.Add(vertexIdx);
                            break;
                        case 2:
                            if (!int.TryParse(fcols[0], out vertexIdx)) {
                                Console.WriteLine("頂点インデックスエラー velue\"{0}\"\n\"{1}\"", fcols[0], line);
                                Console.ReadKey();
                                return;
                            }
                            if (!int.TryParse(fcols[1], out uvIdx)) {
                                Console.WriteLine("UVインデックスエラー velue\"{0}\"\n\"{1}\"", fcols[1], line);
                                Console.ReadKey();
                                return;
                            }
                            s.VertexIndex.Add(vertexIdx);
                            s.UvIndex.Add(uvIdx);
                            break;
                        case 3:
                            if ("" == fcols[1]) {
                                if (!int.TryParse(fcols[0], out vertexIdx)) {
                                    Console.WriteLine("頂点インデックスエラー velue\"{0}\"\n\"{1}\"", fcols[0], line);
                                    Console.ReadKey();
                                    return;
                                }
                                s.VertexIndex.Add(vertexIdx);
                            } else {
                                if (!int.TryParse(fcols[0], out vertexIdx)) {
                                    Console.WriteLine("頂点インデックスエラー velue\"{0}\"\n\"{1}\"", fcols[0], line);
                                    Console.ReadKey();
                                    return;
                                }
                                if (!int.TryParse(fcols[1], out uvIdx)) {
                                    Console.WriteLine("UVインデックスエラー velue\"{0}\"\n\"{1}\"", fcols[1], line);
                                    Console.ReadKey();
                                    return;
                                }
                                s.VertexIndex.Add(vertexIdx);
                                s.UvIndex.Add(uvIdx);
                            }
                            break;
                        }
                    }
                    curObject.SurfaceList.Add(s);
                    break;
                default:
                    return;
                }
            }
            if (0 < curObject.SurfaceList.Count) {
                ObjectList.Add(curObject);
            }
        }
    }

    public void Save(string filePath) {
        // Material
        var mtlName = Path.GetFileNameWithoutExtension(filePath) + ".mtl";
        var mtlPath = Path.GetDirectoryName(filePath) + "\\" + mtlName;
        saveMaterial(mtlPath);
        var fs = new StreamWriter(filePath);
        fs.WriteLine("mtllib {0}", mtlName);
        // Vertex
        foreach (var v in VertexList) {
            fs.WriteLine("v {0} {1} {2}", v.x, v.y, v.z);
        }
        // UV
        foreach (var v in UvList) {
            fs.WriteLine("vt {0} {1}", v[0], v[1]);
        }
        // Surface
        foreach (var obj in ObjectList) {
            var curMaterial = "";
            fs.WriteLine("g {0}", obj.Name);
            foreach (var s in obj.SurfaceList) {
                if (curMaterial != s.MaterialName) {
                    fs.WriteLine("usemtl {0}", s.MaterialName);
                    curMaterial = s.MaterialName;
                }
                fs.Write("f");
                if (s.UvIndex != null && 0 < s.UvIndex.Count) {
                    for (int i = 0; i < s.VertexIndex.Count; i++) {
                        fs.Write(" {0}/{1}", s.VertexIndex[i], s.UvIndex[i]);
                    }
                } else {
                    foreach (var v in s.VertexIndex) {
                        fs.Write(" {0}", v);
                    }
                }
                fs.WriteLine();
            }
        }
        fs.Close();
    }

    public void Normalize(float scale = 1) {
        var ofs = new vec3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (var v in VertexList) {
            ofs.x = Math.Min(ofs.x, v.x);
            ofs.y = Math.Min(ofs.y, v.y);
            ofs.z = Math.Min(ofs.z, v.z);
        }
        var max = new vec3(float.MinValue, float.MinValue, float.MinValue);
        foreach (var v in VertexList) {
            var sv = v - ofs;
            max.x = Math.Max(max.x, sv.x);
            max.y = Math.Max(max.y, sv.y);
            max.z = Math.Max(max.z, sv.z);
        }
        var size = Math.Max(max.x, Math.Max(max.y, max.z));
        for (int i = 0; i < VertexList.Count; i++) {
            VertexList[i] *= scale / size;
        }
    }

    public void ToTriangle() {
        for (int j = 0; j < ObjectList.Count; j++) {
            var obj = ObjectList[j];
            var surfaceList = new List<Surface>();
            foreach (var s in obj.SurfaceList) {
                if (s.VertexIndex.Count % 2 == 0) {
                    evenPoligon(surfaceList, s.VertexIndex);
                } else {
                    oddPoligon(surfaceList, s.VertexIndex);
                }
            }
            obj.SurfaceList.Clear();
            foreach(var s in surfaceList) {
                obj.SurfaceList.Add(s);
            }
        }
    }

    public int GetMaterialIndex(string name) {
        for (int i = 0; i < MaterialList.Count; i++) {
            if (MaterialList[i].Name == name) {
                return i;
            }
        }
        return -1;
    }

    void oddPoligon(List<Surface> surfaceList, List<int> vertexIndex) {
        var surface = new Surface();
        surface.VertexIndex = new List<int> {
            vertexIndex[vertexIndex.Count - 2],
            vertexIndex[vertexIndex.Count - 1],
            vertexIndex[0]
        };
        surfaceList.Add(surface);
        for (int i = 0; i < vertexIndex.Count / 2 - 1; i++) {
            surface = new Surface();
            surface.VertexIndex = new List<int> {
                vertexIndex[i],
                vertexIndex[i + 1],
                vertexIndex[vertexIndex.Count - i - 3]
            };
            surfaceList.Add(surface);
            surface = new Surface();
            surface.VertexIndex = new List<int> {
                vertexIndex[vertexIndex.Count - i - 3],
                vertexIndex[vertexIndex.Count - i - 2],
                vertexIndex[i]
            };
            surfaceList.Add(surface);
        }
    }

    void evenPoligon(List<Surface> surfaceList, List<int> vertexIndex) {
        Surface surface;
        for (int i = 0; i < vertexIndex.Count / 2 - 1; i++) {
            surface = new Surface();
            surface.VertexIndex = new List<int> {
                vertexIndex[i],
                vertexIndex[i + 1],
                vertexIndex[vertexIndex.Count - i - 2]
            };
            surfaceList.Add(surface);
            surface = new Surface();
            surface.VertexIndex = new List<int> {
                vertexIndex[vertexIndex.Count - i - 2],
                vertexIndex[vertexIndex.Count - i - 1],
                vertexIndex[i]
            };
            surfaceList.Add(surface);
        }
    }

    void loadMaterial(string path, string fileName) {
        if (!File.Exists(path + "\\" + fileName)) {
            return;
        }

        using (var fs = new StreamReader(path + "\\" + fileName)) {
            var material = new Material();
            while (!fs.EndOfStream) {
                var line = fs.ReadLine().Replace("\t", " ").TrimStart();
                var cols = line.Split(" ");
                if (cols.Length < 1 || string.IsNullOrEmpty(cols[0])) {
                    continue;
                }

                switch (cols[0]) {
                case "newmtl":
                    if (!string.IsNullOrEmpty(material.Name)) {
                        MaterialList.Add(material);
                    }
                    material = new Material();
                    material.Name = cols[1].Replace("\"", "");
                    material.Ambient = new vec3();
                    material.Diffuse = new vec3();
                    material.Specular = new vec3(1, 1, 1);
                    material.SpecularPower = 10;
                    material.Alpha = 1;
                    break;
                case "Ka":
                    material.Ambient = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
                    break;
                case "Kd":
                    material.Diffuse = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
                    break;
                case "Ks":
                    material.Specular = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
                    break;
                case "Ns":
                    material.SpecularPower = float.Parse(cols[1]);
                    break;
                case "d":
                    material.Alpha = float.Parse(cols[1]);
                    break;
                case "Tr":
                    material.Alpha = 1 - float.Parse(cols[1]);
                    break;

                case "map_Ka":
                    material.TexAmbient = new Texture(cols);
                    break;
                case "map_Kd":
                    material.TexDiffuse = new Texture(cols);
                    break;
                case "map_Ks":
                    material.TexSpecular = new Texture(cols);
                    break;
                case "map_Ns":
                    material.TexSpecularPower = new Texture(cols);
                    break;
                case "map_d":
                    material.TexAlapha = new Texture(cols);
                    break;
                case "map_bump":
                case "bump":
                    material.TexBumpMap = new Texture(cols);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(material.Name)) {
                MaterialList.Add(material);
            }
        }
    }

    void saveMaterial(string path) {
        if (MaterialList.Count == 0) {
            return;
        }

        using(var fs = new StreamWriter(path)) {
            foreach(var m in MaterialList) {
                fs.WriteLine("newmtl {0}", m.Name);
                fs.WriteLine("\tKa {0} {1} {2}", m.Ambient.x, m.Ambient.y, m.Ambient.z);
                fs.WriteLine("\tKd {0} {1} {2}", m.Diffuse.x, m.Diffuse.y, m.Diffuse.z);
                fs.WriteLine("\tKs {0} {1} {2}", m.Specular.x, m.Specular.y, m.Specular.z);
                fs.WriteLine("\tNs {0}", m.SpecularPower);
                fs.WriteLine("\td {0}", m.Alpha);
                if (null != m.TexAmbient) {
                    fs.Write("\tmap_Ka");
                    m.TexAmbient.Write(fs);
                }
                if (null != m.TexDiffuse) {
                    fs.Write("\tmap_Kd");
                    m.TexDiffuse.Write(fs);
                }
                if (null != m.TexSpecular) {
                    fs.Write("\tmap_Ks");
                    m.TexSpecular.Write(fs);
                }
                if (null != m.TexSpecularPower) {
                    fs.Write("\tmap_Ns");
                    m.TexSpecularPower.Write(fs);
                }
                if (null != m.TexAlapha) {
                    fs.Write("\tmap_d");
                    m.TexAlapha.Write(fs);
                }
                if (null != m.TexBumpMap) {
                    fs.Write("\tbump");
                    m.TexBumpMap.Write(fs);
                }
            }
        }
    }
}
