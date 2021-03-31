using System.Collections.Generic;
using System.IO;

class Metasequoia {
    public struct Object {
        public string Name;
        public List<vec3> VertexList;
        public List<Surface> SurfaceList;
    }

    public struct Surface {
        public List<int> VertexIndex;
        public List<float[]> UvList;
        public int MaterialIndex;
    }

    public struct Material {
        public string Name;
        public float R;
        public float G;
        public float B;
        public float A;
        public float Diffuse;
        public float Ambient;
        public float Specular;
        public float SpecularPower;
        public string TexturePath;
        public string AlaphaPlanePath;
        public string BumpMapPath;
    }

    public List<Material> MaterialList = new List<Material>();
    public List<Object> ObjectList = new List<Object>();

    private string mCurrentChunk;
    private List<string> mSkipChunks = new List<string> {
        "BackImage",
        "Blob",
        "CodePage",
        "IncludeXml",
        "Scene",
        "Thumbnail",
        "TrialNoise"
    };

    public Metasequoia() { }

    public Metasequoia(string filePath) {
        using (var fs = new StreamReader(filePath)) {
            while (!fs.EndOfStream) {
                var line = fs.ReadLine();
                var cols = line.Split(" ");
                if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) {
                    continue;
                }

                switch (cols[0]) {
                case "Metasequoia":
                case "Format":
                case "Ver":
                case "Eof":
                    mCurrentChunk = "";
                    break;
                case "Material":
                    loadMaterial(fs);
                    break;
                case "Object":
                    loadObject(fs, cols[1]);
                    break;
                case "}":
                    mCurrentChunk = "";
                    break;
                default:
                    if (mSkipChunks.Contains(mCurrentChunk)) {
                    } else if (mSkipChunks.Contains(cols[0])) {
                        mCurrentChunk = cols[0];
                    } else {
                        return;
                    }
                    break;
                }
            }
        }
    }

    public void Save(string filePath) {
        using(var fs = new StreamWriter(filePath)) {
            fs.WriteLine("Metasequoia Document");
            fs.WriteLine("Format Text Ver 1.1");
            fs.WriteLine();
            saveMaterial(fs);
            foreach(var obj in ObjectList) {
                fs.WriteLine("Object \"" + obj.Name + "\" {");
                fs.WriteLine("\tdepth 0");
                fs.WriteLine("\tfolding 0");
                fs.WriteLine("\tscale 1 1 1");
                fs.WriteLine("\trotation 0 0 0");
                fs.WriteLine("\ttranslation 0 0 0");
                fs.WriteLine("\tvisible 15");
                fs.WriteLine("\tlocking 0");
                fs.WriteLine("\tshading 1");
                fs.WriteLine("\tfacet 30");
                fs.WriteLine("\tnormal_weight 1");
                fs.WriteLine("\tcolor 0.5 0.5 0.5");
                fs.WriteLine("\tcolor_type 0");
                // Vertex
                {
                    fs.WriteLine("\tvertex " + obj.VertexList.Count + " {");
                    foreach (var v in obj.VertexList) {
                        fs.WriteLine("\t\t{0} {1} {2}", v.x, v.y, v.z);
                    }
                    fs.WriteLine("\t}");
                }
                // Face
                {
                    fs.WriteLine("\tface " + obj.SurfaceList.Count + " {");
                    foreach (var f in obj.SurfaceList) {
                        // V
                        fs.Write("\t\t {0} V(", f.VertexIndex.Count);
                        fs.Write("{0}", f.VertexIndex[0]);
                        for (int ivert = 1; ivert < f.VertexIndex.Count; ivert++) {
                            fs.Write(" {0}", f.VertexIndex[ivert]);
                        }
                        // M
                        fs.Write(") M({0})", f.MaterialIndex);
                        // UV
                        if (f.UvList != null && 0 < f.UvList.Count) {
                            fs.Write(" UV(");
                            fs.Write("{0} {1}", f.UvList[0][0], f.UvList[0][1]);
                            for (int iuv = 1; iuv < f.UvList.Count; iuv++) {
                                fs.Write(" {0} {1}", f.UvList[iuv][0], f.UvList[iuv][1]);
                            }
                            fs.Write(")");
                        }
                        fs.WriteLine();
                    }
                    fs.WriteLine("\t}");
                }
                fs.WriteLine("}");
            }
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
            foreach (var s in surfaceList) {
                obj.SurfaceList.Add(s);
            }
        }
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

    private void loadMaterial(StreamReader fs) {
        while (!fs.EndOfStream) {
            var line = fs.ReadLine().Replace("\t", "").TrimStart();
            if (string.IsNullOrEmpty(line)) {
                continue;
            }
            if (line == "}") {
                return;
            }

            var mat = new Material();

            // Name
            var nameBegin = line.IndexOf("\"") + 1;
            var nameEnd = line.IndexOf("\"", nameBegin);
            mat.Name = line.Substring(nameBegin, nameEnd - nameBegin);

            // Attribute
            var cols = line.Substring(nameEnd + 1).Replace(")", "(").Split("(");
            for (int colIdx = 0; colIdx < cols.Length; colIdx += 2) {
                var type = cols[colIdx].Trim();
                switch (type) {
                case "shader":
                case "vcol":
                case "dbls":
                case "emi":
                case "reflect":
                case "refract":
                case "proj_type":
                case "proj_pos":
                case "proj_scale":
                case "proj_angle":
                    break;
                case "col": {
                    var color = cols[colIdx + 1].Split(" ");
                    mat.R = float.Parse(color[0]);
                    mat.G = float.Parse(color[1]);
                    mat.B = float.Parse(color[2]);
                    mat.A = float.Parse(color[3]);
                    break;
                }
                case "dif":
                    mat.Diffuse = float.Parse(cols[colIdx + 1]);
                    break;
                case "amb":
                    mat.Ambient = float.Parse(cols[colIdx + 1]);
                    break;
                case "spc":
                    mat.Specular = float.Parse(cols[colIdx + 1]);
                    break;
                case "power":
                    mat.SpecularPower = float.Parse(cols[colIdx + 1]);
                    break;
                case "tex":
                    mat.TexturePath = cols[colIdx + 1];
                    break;
                case "aplane":
                    mat.AlaphaPlanePath = cols[colIdx + 1];
                    break;
                case "bump":
                    mat.BumpMapPath = cols[colIdx + 1];
                    break;
                default:
                    break;
                }
            }

            MaterialList.Add(mat);
        }
    }

    private void saveMaterial(StreamWriter fs) {
        if (MaterialList == null || MaterialList.Count == 0) {
            return;
        }
        fs.WriteLine("Material " + MaterialList.Count + " {");
        foreach (var m in MaterialList) {
            fs.Write("\t\"{0}\"", m.Name);
            fs.Write(" shader(3)");
            fs.Write(" col({0} {1} {2} {3})", m.R, m.G, m.B, m.A);
            fs.Write(" dif({0})", m.Diffuse);
            fs.Write(" amb({0})", m.Ambient);
            fs.Write(" emi(0)");
            fs.Write(" spe({0})", m.Specular);
            fs.Write(" power({0})", m.SpecularPower);
            if (!string.IsNullOrEmpty(m.TexturePath)) {
                fs.Write(" tex({0})", m.TexturePath);
            }
            if (!string.IsNullOrEmpty(m.AlaphaPlanePath)) {
                fs.Write(" aplane({0})", m.AlaphaPlanePath);
            }
            if (!string.IsNullOrEmpty(m.BumpMapPath)) {
                fs.Write(" bump({0})", m.BumpMapPath);
            }
            fs.WriteLine();
        }
        fs.WriteLine("}");
    }

    private void loadObject(StreamReader fs, string name) {
        var obj = new Object();
        obj.Name = name;
        while (!fs.EndOfStream) {
            var line = fs.ReadLine();
            var cols = line.Split(" ");
            if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) {
                continue;
            }

            switch (cols[0]) {
            case "\tuid":
            case "\tdepth":
            case "\tfolding":
            case "\tscale":
            case "\trotation":
            case "\ttranslation":
            case "\tpatch":
            case "\tpatchtri":
            case "\tsegment":
            case "\tvisible":
            case "\tlocking":
            case "\tshading":
            case "\tfacet":
            case "\tcolor":
            case "\tcolor_type":
            case "\tmirror":
            case "\tmirror_axis":
            case "\tmirror_dis":
            case "\tnormal_weight":
                break;
            case "\tvertex":
                obj.VertexList = loadVertex(fs);
                break;
            case "\tBVertex":
                obj.VertexList = loadBVertex(fs);
                break;
            case "\tface":
                obj.SurfaceList = loadFace(fs);
                break;
            case "}":
                ObjectList.Add(obj);
                return;
            default:
                return;
            }
        }
        return;
    }

    private List<vec3> loadVertex(StreamReader fs) {
        float x, y, z;
        var vertex = new List<vec3>();
        while (!fs.EndOfStream) {
            var line = fs.ReadLine();
            var cols = line.Split(" ");
            if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) {
                continue;
            }
            if (cols[0] == "\t}") {
                return vertex;
            }

            if (float.TryParse(cols[0].Replace("\t", ""), out x)) {
                float.TryParse(cols[1], out y);
                float.TryParse(cols[2], out z);
                vertex.Add(new vec3(x, y, z));
            }
        }
        return null;
    }

    private List<vec3> loadBVertex(StreamReader fs) {
        var vertex = new List<vec3>();
        return vertex;
    }

    private List<Surface> loadFace(StreamReader fs) {
        var surfaceList = new List<Surface>();
        while (!fs.EndOfStream) {
            var line = fs.ReadLine();
            if (string.IsNullOrEmpty(line)) {
                continue;
            }
            if (line == "\t}") {
                return surfaceList;
            }

            line = line.TrimStart();

            var surface = new Surface();
            surface.VertexIndex = new List<int>();
            surface.UvList = new List<float[]>();
            surface.MaterialIndex = -1;

            // Vertex count
            var vertexCountEnd = line.IndexOf(" ");

            // Attribute
            var cols = line.Substring(vertexCountEnd).Replace(")", "(").Split("(");
            for (int colIdx = 0; colIdx < cols.Length; colIdx += 2) {
                var type = cols[colIdx].Trim();
                switch (type) {
                case "V": {
                    var indexes = cols[colIdx + 1].Split(" ");
                    foreach (var idx in indexes) {
                        surface.VertexIndex.Add(int.Parse(idx));
                    }
                    break;
                }
                case "M":
                    surface.MaterialIndex = int.Parse(cols[colIdx + 1]);
                    break;
                case "UV": {
                    var uv = cols[colIdx + 1].Split(" ");
                    for (int i = 0; i < uv.Length; i += 2) {
                        surface.UvList.Add(new float[] { float.Parse(uv[i]), float.Parse(uv[i + 1]) });
                    }
                    break;
                }
                case "COL":
                case "CRS":
                    break;
                default:
                    break;
                }
            }

            surfaceList.Add(surface);
        }
        return null;
    }
}
