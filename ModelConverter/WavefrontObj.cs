using System;
using System.IO;

class WavefrontObj : BaseModel {
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
            //fs.Write(" -imfchan");
            //var imfchan = "";
            //if (ChannelR) {
            //    imfchan += " r";
            //}
            //if (ChannelG) {
            //    imfchan += 0 < imfchan.Length ? " | g" : " g";
            //}
            //if (ChannelB) {
            //    imfchan += 0 < imfchan.Length ? " | b" : " b";
            //}
            //if (ChannelM) {
            //    imfchan += 0 < imfchan.Length ? " | m" : " m";
            //}
            //if (ChannelL) {
            //    imfchan += 0 < imfchan.Length ? " | l" : " l";
            //}
            //if (ChannelZ) {
            //    imfchan += 0 < imfchan.Length ? " | z" : " z";
            //}
            //fs.Write(imfchan);

            fs.WriteLine(" " + FileName.Replace("\"", ""));
        }
    }

    public WavefrontObj() { }

    public WavefrontObj(string path) {
        int vertexIdx, uvIdx;
        var curMaterial = "";
        var curObject = new Object();
        var curSurface = new Surface();

        using (var fs = new StreamReader(path)) {
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
                case "v":
                    if (cols.Length != 4) {
                        Console.WriteLine("頂点エラー：値の個数が3未満 {0}行目\n\"{1}\"", row, line);
                    } else {
                        var v = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
                        mVertList.Add(v);
                    }
                    break;
                case "vt":
                    if (cols.Length < 3) {
                        Console.WriteLine("UVエラー：値の個数が2未満 {0}行目\n\"{1}\"", row, line);
                    } else {
                        var uv = new float[] { float.Parse(cols[1]), float.Parse(cols[2]) };
                        mUvList.Add(uv);
                    }
                    break;
                case "vn":
                    break;
                case "vp":
                    break;
                }
            }
        }

        using (var fs = new StreamReader(path)) {
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
                    loadMaterial(Path.GetDirectoryName(path), line.Substring(7));
                    break;
                case "usemtl":
                    curMaterial = line.Substring(7).Replace("\"", "");
                    break;
                case "s":
                    break;
                case "g":
                    if (0 < curObject.Surfaces.Count) {
                        mObjectList.Add(curObject);
                    }
                    curObject = new Object();
                    if (2 <= cols.Length) {
                        curObject.Name = line.Substring(2).Replace("\"", "");
                    }
                    break;
                case "l":
                    curSurface = new Surface();
                    curSurface.MaterialName = curMaterial;
                    for (int icol = 1; icol < cols.Length; icol++) {
                        if (!int.TryParse(cols[icol], out vertexIdx) || vertexIdx < 1) {
                            Console.WriteLine("インデックスエラー：{0}行目 点:\"{1}\"", row, cols[icol]);
                        } else {
                            curSurface.Line.Add(vertexIdx - 1);
                        }
                    }
                    curObject.Surfaces.Add(curSurface);
                    break;
                case "f":
                    if (cols.Length < 4) {
                        return;
                    }
                    curSurface = new Surface();
                    curSurface.MaterialName = curMaterial;
                    for (int icol = 1; icol < cols.Length; icol++) {
                        var fcols = cols[icol].Split("/");
                        switch (fcols.Length) {
                        case 1:
                            if (!int.TryParse(fcols[0], out vertexIdx) || vertexIdx < 1) {
                                Console.WriteLine("インデックスエラー：{0}行目 頂点:\"{1}\"", row, fcols[0]);
                            } else {
                                curSurface.Indices.Add(new Index(vertexIdx - 1));
                            }
                            break;
                        case 2:
                            if (!int.TryParse(fcols[0], out vertexIdx) || vertexIdx < 1 ||
                                !int.TryParse(fcols[1], out uvIdx) || uvIdx < 1) {
                                Console.WriteLine("インデックスエラー：{0}行目 頂点:\"{1}\" UV:\"{2}\"", row, fcols[0], fcols[1]);
                            } else {
                                curSurface.Indices.Add(new Index(vertexIdx - 1, uvIdx - 1));
                            }
                            break;
                        case 3:
                            if ("" == fcols[1]) {
                                if (!int.TryParse(fcols[0], out vertexIdx) || vertexIdx < 1) {
                                    Console.WriteLine("インデックスエラー：{0}行目 頂点:\"{1}\"", row, fcols[0]);
                                } else {
                                    curSurface.Indices.Add(new Index(vertexIdx - 1));
                                }
                            } else {
                                if (!int.TryParse(fcols[0], out vertexIdx) || vertexIdx < 1 ||
                                    !int.TryParse(fcols[1], out uvIdx) || uvIdx < 1) {
                                    Console.WriteLine("インデックスエラー：{0}行目 頂点:\"{1}\" UV:\"{2}\"", row, fcols[0], fcols[1]);
                                } else {
                                    curSurface.Indices.Add(new Index(vertexIdx - 1, uvIdx - 1));
                                }
                            }
                            break;
                        }
                    }
                    curObject.Surfaces.Add(curSurface);
                    break;
                }
            }
            if (0 < curObject.Surfaces.Count) {
                mObjectList.Add(curObject);
            }
        }

        Reverse();
        TransformUV(EInvertUV.ForwordU_ReverseV);
    }

    public override void Save(string path) {
        Reverse();
        TransformUV(EInvertUV.ForwordU_ReverseV);
        ToTriangle();

        // Material
        var mtlName = Path.GetFileNameWithoutExtension(path) + ".mtl";
        var mtlPath = Path.GetDirectoryName(path) + "\\" + mtlName;
        saveMaterial(mtlPath);
        var fs = new StreamWriter(path);
        fs.WriteLine("mtllib {0}", mtlName);
        // Vertex
        foreach (var v in mVertList) {
            fs.WriteLine("v {0} {1} {2}", v.x, v.y, v.z);
        }
        // UV
        foreach (var uv in mUvList) {
            fs.WriteLine("vt {0} {1}", uv[0], uv[1]);
        }
        // Surface
        foreach (var obj in mObjectList) {
            var curMaterial = "";
            fs.WriteLine("g {0}", obj.Name.Replace("\"", ""));
            foreach (var s in obj.Surfaces) {
                if (curMaterial != s.MaterialName) {
                    if (!string.IsNullOrEmpty(s.MaterialName)) {
                        fs.WriteLine("usemtl {0}", s.MaterialName);
                    }
                    curMaterial = s.MaterialName;
                }
                if (0 < s.Indices.Count) {
                    fs.Write("f");
                    if (0 <= s.Indices[0].Uv) {
                        for (int i = 0; i < s.Indices.Count; i++) {
                            fs.Write(" {0}/{1}", s.Indices[i].Vert + 1, s.Indices[i].Uv + 1);
                        }
                    } else {
                        for (int i = 0; i < s.Indices.Count; i++) {
                            fs.Write(" {0}", s.Indices[i].Vert + 1);
                        }
                    }
                    fs.WriteLine();
                }
                if (0 < s.Line.Count) {
                    fs.Write("l");
                    for (int i = 0; i < s.Line.Count; i++) {
                        fs.Write(" {0}", s.Line[i] + 1);
                    }
                    fs.WriteLine();
                }
            }
        }
        fs.Close();
    }

    void loadMaterial(string path, string fileName) {
        if (!File.Exists(path + "\\" + fileName)) {
            return;
        }

        using (var fs = new StreamReader(path + "\\" + fileName)) {
            Texture tex;
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
                        mMaterialList.Add(material.Name, material);
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
                    break;
                case "map_Kd":
                    tex = new Texture(cols);
                    material.TexDiffuse = tex.FileName;
                    break;
                case "map_Ks":
                    break;
                case "map_Ns":
                    break;
                case "map_d":
                    tex = new Texture(cols);
                    material.TexAlapha = tex.FileName;
                    break;
                case "map_bump":
                case "bump":
                    tex = new Texture(cols);
                    material.TexBumpMap = tex.FileName;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(material.Name)) {
                mMaterialList.Add(material.Name, material);
            }
        }
    }

    void saveMaterial(string path) {
        if (mMaterialList.Count == 0) {
            return;
        }

        using (var fs = new StreamWriter(path)) {
            foreach (var m in mMaterialList) {
                var val = m.Value;
                fs.WriteLine("newmtl {0}", val.Name.Replace("\"", ""));
                fs.WriteLine("\td {0}", val.Alpha);
                fs.WriteLine("\tKd {0} {1} {2}", val.Diffuse.x, val.Diffuse.y, val.Diffuse.z);
                if (0 < val.Ambient.Abs) {
                    fs.WriteLine("\tKa {0} {1} {2}", val.Ambient.x, val.Ambient.y, val.Ambient.z);
                }
                if (0 < val.Specular.Abs) {
                    fs.WriteLine("\tKs {0} {1} {2}", val.Specular.x, val.Specular.y, val.Specular.z);
                    fs.WriteLine("\tNs {0}", val.SpecularPower);
                }
                if (!string.IsNullOrEmpty(val.TexDiffuse)) {
                    fs.WriteLine("\tmap_Kd {0}", val.TexDiffuse);
                }
                if (!string.IsNullOrEmpty(val.TexAlapha)) {
                    fs.WriteLine("\tmap_d {0}", val.TexAlapha);
                }
                if (!string.IsNullOrEmpty(val.TexBumpMap)) {
                    fs.WriteLine("\tbump {0}", val.TexBumpMap);
                }
            }
        }
    }
}
