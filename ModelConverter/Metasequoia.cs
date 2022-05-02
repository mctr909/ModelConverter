using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

class Metasequoia : BaseModel {
    /*
     * Object
     *   Name : string
     *   Vertex : Array<vec3>
     *   Surface : Array<{
     *     VertexIndex : Array<int>
     *     UV : Array<float[]>
     *     Material : int
     *   }>
     */

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

    string mCurrentChunk;
    List<string> mSkipChunks = new List<string> {
        "BackImage",
        "Blob",
        "IncludeXml",
        "Scene",
        "Thumbnail",
        "TrialNoise"
    };

    public Metasequoia() { }

    public Metasequoia(string path) {
        var zipextFile = "";
        if (".mqoz" == Path.GetExtension(path)) {
            foreach (var e in ZipFile.OpenRead(path).Entries) {
                if (".mqo" == Path.GetExtension(e.Name)) {
                    zipextFile = AppContext.BaseDirectory + e.Name;
                    e.ExtractToFile(zipextFile);
                    path = zipextFile;
                    break;
                }
            }
            if (string.IsNullOrEmpty(zipextFile)) {
                return;
            }
        }

        using (var fs = new StreamReader(path)) {
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
                case "CodePage":
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

        if (!string.IsNullOrEmpty(zipextFile)) {
            File.Delete(zipextFile);
        }
    }

    public override void Save(string path) {
        var fileName = Path.GetFileNameWithoutExtension(path);
        var textFilePath = AppContext.BaseDirectory + fileName;
        using (var fs = new StreamWriter(textFilePath)) {
            fs.WriteLine("Metasequoia Document");
            fs.WriteLine("Format Text Ver 1.1");
            fs.WriteLine();
            saveMaterial(fs);
            foreach (var obj in mObjectList) {
                fs.WriteLine("Object \"" + obj.Name + "\" {");
                fs.WriteLine("\tdepth 0");
                fs.WriteLine("\tfolding 0");
                fs.WriteLine("\tscale 1 1 1");
                fs.WriteLine("\trotation 0 0 0");
                fs.WriteLine("\ttranslation 0 0 0");
                fs.WriteLine("\tvisible 15");
                fs.WriteLine("\tlocking 0");
                fs.WriteLine("\tshading 1");
                fs.WriteLine("\tfacet 45");
                fs.WriteLine("\tnormal_weight 1");
                fs.WriteLine("\tcolor 0.5 0.5 0.5");
                fs.WriteLine("\tcolor_type 0");
                // Vertex
                var convIdxList = new Dictionary<int, int>();
                {
                    var count = 0;
                    foreach (var s in obj.Surfaces) {
                        foreach (var idx in s.Indices) {
                            if (!convIdxList.ContainsKey(idx.Vert)) {
                                count++;
                                convIdxList.Add(idx.Vert, convIdxList.Count);
                            }
                        }
                    }
                    convIdxList.Clear();
                    fs.WriteLine("\tvertex " + count + " {");
                    foreach (var s in obj.Surfaces) {
                        foreach (var idx in s.Indices) {
                            if (!convIdxList.ContainsKey(idx.Vert)) {
                                var v = mVertList[idx.Vert];
                                fs.WriteLine("\t\t{0} {1} {2}", v.x, v.y, v.z);
                                convIdxList.Add(idx.Vert, convIdxList.Count);
                            }
                        }
                    }
                    fs.WriteLine("\t}");
                }
                // Face
                {
                    fs.WriteLine("\tface " + obj.Surfaces.Count + " {");
                    foreach (var s in obj.Surfaces) {
                        // V
                        fs.Write("\t\t {0} V(", s.Indices.Count);
                        fs.Write("{0}", convIdxList[s.Indices[0].Vert]);
                        for (int vi = 1; vi < s.Indices.Count; vi++) {
                            fs.Write(" {0}", convIdxList[s.Indices[vi].Vert]);
                        }
                        // Todo: M
                        fs.Write(")");
                        //fs.Write(") M({0})", f.Material);
                        // Todo: UV
                        //if (f.UvList != null && 0 < f.UvList.Count) {
                        //    fs.Write(" UV(");
                        //    fs.Write("{0} {1}", f.UvList[0][0], f.UvList[0][1]);
                        //    for (int iuv = 1; iuv < f.UvList.Count; iuv++) {
                        //        fs.Write(" {0} {1}", f.UvList[iuv][0], f.UvList[iuv][1]);
                        //    }
                        //    fs.Write(")");
                        //}
                        fs.WriteLine();
                    }
                    fs.WriteLine("\t}");
                }
                fs.WriteLine("}");
            }
        }

        using (var z = ZipFile.Open(path, ZipArchiveMode.Create)) {
            z.CreateEntryFromFile(textFilePath, fileName + ".mqo");
        }

        File.Delete(textFilePath);
    }

    void loadMaterial(StreamReader fs) {
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

    void saveMaterial(StreamWriter fs) {
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

    void loadObject(StreamReader fs, string name) {
        var idxOfs = 0;
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
                idxOfs = mVertList.Count;
                loadVertex(fs);
                break;
            case "\tBVertex":
                idxOfs = mVertList.Count;
                loadBVertex(fs);
                break;
            case "\tface":
                obj.Surfaces = loadFace(fs, idxOfs);
                break;
            case "}":
                mObjectList.Add(obj);
                return;
            default:
                return;
            }
        }
        return;
    }

    void loadVertex(StreamReader fs) {
        float x, y, z;
        while (!fs.EndOfStream) {
            var line = fs.ReadLine();
            var cols = line.Split(" ");
            if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) {
                continue;
            }
            if (cols[0] == "\t}") {
                return;
            }

            if (float.TryParse(cols[0].Replace("\t", ""), out x)) {
                float.TryParse(cols[1], out y);
                float.TryParse(cols[2], out z);
                mVertList.Add(new vec3(x, y, z));
            }
        }
        return;
    }

    void loadBVertex(StreamReader fs) {
    }

    List<Surface> loadFace(StreamReader fs, int idxOfs) {
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

            // Vertex count
            var vertexCountEnd = line.IndexOf(" ");

            // Attribute
            var cols = line.Substring(vertexCountEnd).Replace(")", "(").Split("(");
            for (int colIdx = 0; colIdx < cols.Length; colIdx += 2) {
                var type = cols[colIdx].Trim();
                switch (type) {
                case "V": {
                    var indexes = cols[colIdx + 1].Split(" ");
                    foreach (var str in indexes) {
                        surface.Indices.Add(new Index(idxOfs + int.Parse(str)));
                    }
                    break;
                }
                //Todo:M
                //case "M":
                //    surface.Material = int.Parse(cols[colIdx + 1]);
                //    break;
                //Todo:UV
                //case "UV": {
                //    var uv = cols[colIdx + 1].Split(" ");
                //    for (int i = 0; i < uv.Length; i += 2) {
                //        surface.UvList.Add(new float[] { float.Parse(uv[i]), float.Parse(uv[i + 1]) });
                //    }
                //    break;
                //}
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
