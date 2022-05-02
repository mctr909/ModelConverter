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
                        fs.Write(") M({0})", s.MaterialName);
                        if (0 < s.Indices.Count && 0 <= s.Indices[0].Uv) {
                            fs.Write(" UV(");
                            var uv = mUvList[s.Indices[0].Uv];
                            fs.Write("{0} {1}", uv[0], uv[1]);
                            for (int i = 1; i < s.Indices.Count; i++) {
                                uv = mUvList[s.Indices[i].Uv];
                                fs.Write(" {0} {1}", uv[0], uv[1]);
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
                    mat.Diffuse = new vec3(
                        float.Parse(color[0]),
                        float.Parse(color[1]),
                        float.Parse(color[2])
                    );
                    mat.Alpha = float.Parse(color[3]);
                    break;
                }
                case "dif":
                    break;
                case "amb":
                    mat.Ambient = mat.Diffuse * float.Parse(cols[colIdx + 1]);
                    break;
                case "spc":
                    mat.Specular = mat.Diffuse * float.Parse(cols[colIdx + 1]);
                    break;
                case "power":
                    mat.SpecularPower = float.Parse(cols[colIdx + 1]);
                    break;
                case "tex":
                    mat.TexDiffuse = cols[colIdx + 1];
                    break;
                case "aplane":
                    mat.TexAlapha = cols[colIdx + 1];
                    break;
                case "bump":
                    mat.TexBumpMap = cols[colIdx + 1];
                    break;
                default:
                    break;
                }
            }

            mMaterialList.Add(mat);
        }
    }

    void saveMaterial(StreamWriter fs) {
        if (mMaterialList.Count == 0) {
            return;
        }
        fs.WriteLine("Material " + mMaterialList.Count + " {");
        foreach (var m in mMaterialList) {
            var col = m.Diffuse.Norm;
            fs.Write("\t\"{0}\"", m.Name);
            fs.Write(" shader(3)");
            fs.Write(" col({0} {1} {2} {3})", col.x, col.y, col.z, m.Alpha);
            fs.Write(" dif({0})", m.Diffuse.Abs);
            fs.Write(" amb({0})", m.Ambient.Abs);
            fs.Write(" emi(0)");
            fs.Write(" spe({0})", m.Specular.Abs);
            fs.Write(" power({0})", m.SpecularPower);
            if (!string.IsNullOrEmpty(m.TexDiffuse)) {
                fs.Write(" tex({0})", m.TexDiffuse);
            }
            if (!string.IsNullOrEmpty(m.TexAlapha)) {
                fs.Write(" aplane({0})", m.TexAlapha);
            }
            if (!string.IsNullOrEmpty(m.TexBumpMap)) {
                fs.Write(" bump({0})", m.TexBumpMap);
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
            var vertIdx = new List<int>();
            var uvIdx = new List<int>();
            var cols = line.Substring(vertexCountEnd).Replace(")", "(").Split("(");
            for (int colIdx = 0; colIdx < cols.Length; colIdx += 2) {
                var type = cols[colIdx].Trim();
                switch (type) {
                case "V": {
                    var indexes = cols[colIdx + 1].Split(" ");
                    foreach (var str in indexes) {
                        vertIdx.Add(idxOfs + int.Parse(str));
                    }
                    break;
                }
                case "M":
                    surface.MaterialName = mMaterialList[int.Parse(cols[colIdx + 1])].Name;
                    break;
                case "UV": {
                    var uv = cols[colIdx + 1].Split(" ");
                    for (int i = 0; i < uv.Length; i += 2) {
                        uvIdx.Add(mUvList.Count);
                        mUvList.Add(new float[] { float.Parse(uv[i]), float.Parse(uv[i + 1]) });
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

            if (uvIdx.Count == vertIdx.Count) {
                for (int i = 0; i < vertIdx.Count; i++) {
                    surface.Indices.Add(new Index(vertIdx[i], uvIdx[i]));
                }
            } else {
                for (int i = 0; i < vertIdx.Count; i++) {
                    surface.Indices.Add(new Index(vertIdx[i]));
                }
            }

            surfaceList.Add(surface);
        }
        return null;
    }
}
