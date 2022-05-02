using System;
using System.Collections.Generic;
using System.Text;
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
        var materialNameDic = new Dictionary<string, int>();
        foreach (var n in mMaterialList) {
            materialNameDic.Add(n.Key, materialNameDic.Count);
        }

        var fileName = Path.GetFileNameWithoutExtension(path);
        var textFilePath = AppContext.BaseDirectory + fileName;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using (var fs = new StreamWriter(textFilePath, false, Encoding.GetEncoding("Shift_JIS"))) {
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
                        foreach (var idx in s.Line) {
                            if (!convIdxList.ContainsKey(idx)) {
                                count++;
                                convIdxList.Add(idx, convIdxList.Count);
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
                        foreach (var idx in s.Line) {
                            if (!convIdxList.ContainsKey(idx)) {
                                var v = mVertList[idx];
                                fs.WriteLine("\t\t{0} {1} {2}", v.x, v.y, v.z);
                                convIdxList.Add(idx, convIdxList.Count);
                            }
                        }
                    }
                    fs.WriteLine("\t}");
                }
                // Face
                {
                    fs.WriteLine("\tface " + obj.Surfaces.Count + " {");
                    foreach (var s in obj.Surfaces) {
                        // V(surface)
                        if (0 < s.Indices.Count) {
                            fs.Write("\t\t {0} V(", s.Indices.Count);
                            fs.Write("{0}", convIdxList[s.Indices[0].Vert]);
                            for (int vi = 1; vi < s.Indices.Count; vi++) {
                                fs.Write(" {0}", convIdxList[s.Indices[vi].Vert]);
                            }
                            fs.Write(")");
                        }
                        // V(line)
                        if (0 < s.Line.Count) {
                            fs.Write("\t\t {0} V(", s.Line.Count);
                            fs.Write("{0}", convIdxList[s.Line[0]]);
                            for (int vi = 1; vi < s.Line.Count; vi++) {
                                fs.Write(" {0}", convIdxList[s.Line[vi]]);
                            }
                            fs.Write(")");
                        }
                        // M
                        if (materialNameDic.ContainsKey(s.MaterialName)) {
                            fs.Write(" M({0})", materialNameDic[s.MaterialName]);
                        }
                        // UV
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

            mMaterialList.Add(mat.Name, mat);
        }
    }

    void saveMaterial(StreamWriter fs) {
        if (mMaterialList.Count == 0) {
            return;
        }
        fs.WriteLine("Material " + mMaterialList.Count + " {");
        foreach (var m in mMaterialList) {
            var val = m.Value;
            var col = val.Diffuse.Norm;
            fs.Write("\t\"{0}\"", val.Name);
            fs.Write(" shader(3)");
            fs.Write(" col({0} {1} {2} {3})", col.x, col.y, col.z, val.Alpha);
            fs.Write(" dif({0})", val.Diffuse.Abs);
            fs.Write(" amb({0})", val.Ambient.Abs);
            fs.Write(" emi(0)");
            fs.Write(" spe({0})", val.Specular.Abs);
            fs.Write(" power({0})", val.SpecularPower);
            if (!string.IsNullOrEmpty(val.TexDiffuse)) {
                fs.Write(" tex({0})", val.TexDiffuse);
            }
            if (!string.IsNullOrEmpty(val.TexAlapha)) {
                fs.Write(" aplane({0})", val.TexAlapha);
            }
            if (!string.IsNullOrEmpty(val.TexBumpMap)) {
                fs.Write(" bump({0})", val.TexBumpMap);
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
                loadFace(fs, idxOfs, obj);
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

    void loadFace(StreamReader fs, int idxOfs, Object obj) {
        var matNames = new List<string>();
        foreach (var n in mMaterialList) {
            matNames.Add(n.Key);
        }

        while (!fs.EndOfStream) {
            var line = fs.ReadLine();
            if (string.IsNullOrEmpty(line)) {
                continue;
            }
            if (line == "\t}") {
                return;
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
                    if (3 <= indexes.Length) {
                        foreach (var str in indexes) {
                            vertIdx.Add(idxOfs + int.Parse(str));
                        }
                    } else {
                        foreach (var str in indexes) {
                            surface.Line.Add(idxOfs + int.Parse(str));
                        }
                    }
                    break;
                }
                case "M":
                    surface.MaterialName = matNames[int.Parse(cols[colIdx + 1])];
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

            obj.Surfaces.Add(surface);
        }
    }
}
