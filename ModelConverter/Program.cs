using System;
using System.Collections.Generic;
using System.IO;

namespace ModelConverter {
    class Program {
        const string STL_BIN = "0 : STL(バイナリ)";
        const string STL_TEXT = "1 : STL(テキスト)";
        const string WAVEFRONT_OBJ = "2 : Wavefront OBJ";
        const string METASEQUOIA = "3 : Metasequoia";

        static readonly List<string> TYPE_LIST = new List<string> {
            STL_BIN,
            STL_TEXT,
            WAVEFRONT_OBJ,
            METASEQUOIA
        };

        static void Main(string[] args) {
            Console.WriteLine("変換する形式を番号で選んでください");
            foreach (var t in TYPE_LIST) {
                Console.WriteLine(t);
            }
            var keyLine = Console.ReadLine();
            int typeNo;
            if (!int.TryParse(keyLine, out typeNo)) {
                Console.WriteLine("数字で選択してください");
                Console.ReadKey();
                return;
            }
            if (typeNo < 0 || TYPE_LIST.Count <= typeNo) {
                Console.WriteLine("0～{0}の範囲で選択してください", TYPE_LIST.Count - 1);
                Console.ReadKey();
                return;
            }
            string convertTo = TYPE_LIST[typeNo];

            for (int argc = 0; argc < args.Length; argc++) {
                var filePath = args[argc];
                if (!File.Exists(filePath)) {
                    Console.WriteLine("ファイルが存在しません {0}", filePath);
                    continue;
                }
                Console.WriteLine(filePath);

                var fileDir = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var ext = Path.GetExtension(filePath);

                // Load model
                StlText stlText = null;
                StlBin stlBin = null;
                WavefrontObj wavefrontObj = null;
                Metasequoia metasequoia = null;
                try {
                    switch (ext.ToLower()) {
                    case ".stl":
                        stlText = new StlText(filePath);
                        if (0 == stlText.ObjectList.Count) {
                            stlText = null;
                            stlBin = new StlBin(filePath);
                        }
                        break;
                    case ".obj":
                        wavefrontObj = new WavefrontObj(filePath);
                        break;
                    case ".mqo":
                        metasequoia = new Metasequoia(filePath);
                        break;
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }

                var convertFilePath = fileDir + "\\" + fileName;

                try {
                    // Convert from STL(text) model
                    if (null != stlText) {
                        switch (convertTo) {
                        case STL_BIN:
                            if (File.Exists(convertFilePath + ".stl")) {
                                convertFilePath += "(STLtext to STLbin)";
                            }
                            stlTextToStlBin(stlText, convertFilePath + ".stl");
                            break;
                        case WAVEFRONT_OBJ:
                            if (File.Exists(convertFilePath + ".obj")) {
                                convertFilePath += "(STLtext to obj)";
                            }
                            stlTextToWavefrontObj(stlText, convertFilePath + ".obj");
                            break;
                        case METASEQUOIA:
                            if (File.Exists(convertFilePath + ".mqo")) {
                                convertFilePath += "(STLtext to mqo)";
                            }
                            stlTextToMetasequoia(stlText, convertFilePath + ".mqo");
                            break;
                        }
                    }
                    // Convert from STL(bin) model
                    if (null != stlBin) {
                        switch (convertTo) {
                        case STL_TEXT:
                            if (File.Exists(convertFilePath + ".stl")) {
                                convertFilePath += "(STLbin to STLtext)";
                            }
                            stlBinToStlText(stlBin, convertFilePath + ".stl");
                            break;
                        case WAVEFRONT_OBJ:
                            if (File.Exists(convertFilePath + ".obj")) {
                                convertFilePath += "(STLbin to obj)";
                            }
                            stlBinToWavefrontObj(stlBin, convertFilePath + ".obj");
                            break;
                        case METASEQUOIA:
                            if (File.Exists(convertFilePath + ".mqo")) {
                                convertFilePath += "(STLbin to mqo)";
                            }
                            stlBinToMetasequoia(stlBin, convertFilePath + ".mqo");
                            break;
                        }
                    }
                    // Convert from WavefrontObj model
                    if (null != wavefrontObj) {
                        switch (convertTo) {
                        case STL_BIN:
                            if (File.Exists(convertFilePath + ".stl")) {
                                convertFilePath += "(obj to STLbin)";
                            }
                            wavefrontObjToStlBin(wavefrontObj, convertFilePath + ".stl");
                            break;
                        case STL_TEXT:
                            if (File.Exists(convertFilePath + ".stl")) {
                                convertFilePath += "(obj to STLtext)";
                            }
                            wavefrontObjToStlText(wavefrontObj, convertFilePath + ".stl");
                            break;
                        case METASEQUOIA:
                            if (File.Exists(convertFilePath + ".mqo")) {
                                convertFilePath += "(obj to mqo)";
                            }
                            wavefrontObjToMetasequoia(wavefrontObj, convertFilePath + ".mqo");
                            break;
                        }
                    }
                    // Convert from Metasequoia model
                    if (null != metasequoia) {
                        switch (convertTo) {
                        case STL_BIN:
                            if (File.Exists(convertFilePath + ".stl")) {
                                convertFilePath += "(mqo to STLbin)";
                            }
                            metasequoiaToStlBin(metasequoia, convertFilePath + ".stl");
                            break;
                        case STL_TEXT:
                            if (File.Exists(convertFilePath + ".stl")) {
                                convertFilePath += "(mqo to STLtext)";
                            }
                            metasequoiaToStlText(metasequoia, convertFilePath + ".stl");
                            break;
                        case WAVEFRONT_OBJ:
                            if (File.Exists(convertFilePath + ".obj")) {
                                convertFilePath += "(mqo to obj)";
                            }
                            metasequoiaToWavefrontObj(metasequoia, convertFilePath + ".obj");
                            break;
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            }
        }

        #region Convert from STL(text) model
        static void stlTextToStlBin(StlText stl, string filePath) {
            var output = new StlBin();
            foreach (var obj in stl.ObjectList) {
                var curObject = new StlBin.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<StlBin.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var curSurface = new StlBin.Surface();
                    curSurface.Norm = new vec3(s.Norm);
                    curSurface.V1 = new vec3(s.V1);
                    curSurface.V2 = new vec3(s.V2);
                    curSurface.V3 = new vec3(s.V3);
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void stlTextToWavefrontObj(StlText stl, string filePath) {
            var output = new WavefrontObj();
            foreach (var obj in stl.ObjectList) {
                var curObject = new WavefrontObj.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<WavefrontObj.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var surface = new WavefrontObj.Surface();
                    surface.VertexIndex = new List<int>{
                        output.VertexList.Count + 1,
                        output.VertexList.Count + 2,
                        output.VertexList.Count + 3
                    };
                    curObject.SurfaceList.Add(surface);
                    output.VertexList.Add(new vec3(s.V1.y, s.V1.z, s.V1.x));
                    output.VertexList.Add(new vec3(s.V2.y, s.V2.z, s.V2.x));
                    output.VertexList.Add(new vec3(s.V3.y, s.V3.z, s.V3.x));
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void stlTextToMetasequoia(StlText stl, string filePath) {
            var output = new Metasequoia();
            foreach (var obj in stl.ObjectList) {
                var curObject = new Metasequoia.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<Metasequoia.Surface>();
                curObject.VertexList = new List<vec3>();
                foreach (var s in obj.SurfaceList) {
                    var curSurface = new Metasequoia.Surface();
                    curSurface.MaterialIndex = -1;
                    curSurface.VertexIndex = new List<int> {
                        curObject.VertexList.Count,
                        curObject.VertexList.Count + 1,
                        curObject.VertexList.Count + 2
                    };
                    curObject.VertexList.Add(new vec3(s.V3.y, s.V3.z, s.V3.x));
                    curObject.VertexList.Add(new vec3(s.V2.y, s.V2.z, s.V2.x));
                    curObject.VertexList.Add(new vec3(s.V1.y, s.V1.z, s.V1.x));
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }
        #endregion

        #region Convert from STL(bin) model
        static void stlBinToStlText(StlBin stl, string filePath) {
            var output = new StlText();
            foreach (var obj in stl.ObjectList) {
                var curObject = new StlText.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<StlText.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var curSurface = new StlText.Surface();
                    curSurface.Norm = new vec3(s.Norm);
                    curSurface.V1 = new vec3(s.V1);
                    curSurface.V2 = new vec3(s.V2);
                    curSurface.V3 = new vec3(s.V3);
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void stlBinToWavefrontObj(StlBin stl, string filePath) {
            var output = new WavefrontObj();
            foreach (var obj in stl.ObjectList) {
                var curObject = new WavefrontObj.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<WavefrontObj.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var surface = new WavefrontObj.Surface();
                    surface.VertexIndex = new List<int> {
                        output.VertexList.Count + 1,
                        output.VertexList.Count + 2,
                        output.VertexList.Count + 3
                    };
                    curObject.SurfaceList.Add(surface);
                    output.VertexList.Add(new vec3(s.V1.y, s.V1.z, s.V1.x));
                    output.VertexList.Add(new vec3(s.V2.y, s.V2.z, s.V2.x));
                    output.VertexList.Add(new vec3(s.V3.y, s.V3.z, s.V3.x));
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void stlBinToMetasequoia(StlBin stl, string filePath) {
            var output = new Metasequoia();
            foreach (var obj in stl.ObjectList) {
                var curObject = new Metasequoia.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<Metasequoia.Surface>();
                curObject.VertexList = new List<vec3>();
                foreach (var s in obj.SurfaceList) {
                    var curSurface = new Metasequoia.Surface();
                    curSurface.MaterialIndex = -1;
                    curSurface.VertexIndex = new List<int> {
                        curObject.VertexList.Count,
                        curObject.VertexList.Count + 1,
                        curObject.VertexList.Count + 2
                    };
                    curObject.VertexList.Add(new vec3(s.V3.y, s.V3.z, s.V3.x));
                    curObject.VertexList.Add(new vec3(s.V2.y, s.V2.z, s.V2.x));
                    curObject.VertexList.Add(new vec3(s.V1.y, s.V1.z, s.V1.x));
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }
        #endregion

        #region Convert from WavefrontObj model
        static void wavefrontObjToStlBin(WavefrontObj obj, string filePath) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報が消えます");
            obj.ToTriangle();
            var output = new StlBin();
            foreach (var o in obj.ObjectList) {
                var curObject = new StlBin.Object();
                curObject.Name = o.Name;
                curObject.SurfaceList = new List<StlBin.Surface>();
                foreach (var s in o.SurfaceList) {
                    var curSaface = new StlBin.Surface();
                    var va = obj.VertexList[s.VertexIndex[0] - 1];
                    var vo = obj.VertexList[s.VertexIndex[1] - 1];
                    var vb = obj.VertexList[s.VertexIndex[2] - 1];
                    curSaface.Norm = ((va - vo) * (vb - vo)).Norm;
                    curSaface.V1 = new vec3(va.z, va.x, va.y);
                    curSaface.V2 = new vec3(vo.z, vo.x, vo.y);
                    curSaface.V3 = new vec3(vb.z, vb.x, vb.y);
                    curObject.SurfaceList.Add(curSaface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void wavefrontObjToStlText(WavefrontObj obj, string filePath) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報が消えます");
            obj.ToTriangle();
            var output = new StlText();
            foreach (var o in obj.ObjectList) {
                var curObject = new StlText.Object();
                curObject.Name = o.Name;
                curObject.SurfaceList = new List<StlText.Surface>();
                foreach (var s in o.SurfaceList) {
                    var curSaface = new StlText.Surface();
                    var va = obj.VertexList[s.VertexIndex[0] - 1];
                    var vo = obj.VertexList[s.VertexIndex[1] - 1];
                    var vb = obj.VertexList[s.VertexIndex[2] - 1];
                    curSaface.Norm = ((va - vo) * (vb - vo)).Norm;
                    curSaface.V1 = new vec3(va.z, va.x, va.y);
                    curSaface.V2 = new vec3(vo.z, vo.x, vo.y);
                    curSaface.V3 = new vec3(vb.z, vb.x, vb.y);
                    curObject.SurfaceList.Add(curSaface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void wavefrontObjToMetasequoia(WavefrontObj obj, string filePath) {
            var output = new Metasequoia();
            if (obj.VertexList.Count == 0) {
                return;
            }
            foreach (var m in obj.MaterialList) {
                var color = m.Ambient.Norm;
                var mat = new Metasequoia.Material();
                mat.Name = m.Name;
                mat.R = color.x;
                mat.G = color.y;
                mat.B = color.z;
                mat.A = m.Alpha;
                mat.Ambient = (float)m.Ambient.Abs;
                mat.Diffuse = (float)m.Diffuse.Abs;
                mat.Specular = (float)m.Specular.Abs;
                mat.SpecularPower = m.SpecularPower;
                if (null != m.TexAmbient) {
                    mat.TexturePath = m.TexAmbient.FileName;
                }
                if (null != m.TexAlapha) {
                    mat.AlaphaPlanePath = m.TexAlapha.FileName;
                }
                if (null != m.TexBumpMap) {
                    mat.BumpMapPath = m.TexBumpMap.FileName;
                }
                output.MaterialList.Add(mat);
            }
            foreach (var o in obj.ObjectList) {
                var curObject = new Metasequoia.Object();
                curObject.Name = o.Name;
                curObject.SurfaceList = new List<Metasequoia.Surface>();
                curObject.VertexList = new List<vec3>();
                var surfaceVertexIdx = new List<int>();
                foreach (var s in o.SurfaceList) {
                    var curSurface = new Metasequoia.Surface();
                    curSurface.MaterialIndex = obj.GetMaterialIndex(s.MaterialName);
                    curSurface.VertexIndex = new List<int>();
                    curSurface.UvList = new List<float[]>();
                    for (int iv = s.VertexIndex.Count - 1; 0 <= iv; iv--) {
                        var idx = s.VertexIndex[iv] - 1;
                        if (!surfaceVertexIdx.Contains(idx)) {
                            if (obj.VertexList.Count <= idx || idx < 0) {
                                Console.WriteLine("VertexList out of range (Max:{0}, Value:{1}", obj.VertexList.Count - 1, idx);
                                Console.ReadKey();
                                return;
                            }
                            curObject.VertexList.Add(obj.VertexList[idx]);
                            surfaceVertexIdx.Add(idx);
                        };
                        curSurface.VertexIndex.Add(surfaceVertexIdx.IndexOf(idx));
                    }
                    for (int it = s.UvIndex.Count - 1; 0 <= it; it--) {
                        var idx = s.UvIndex[it] - 1;
                        if (obj.UvList.Count <= idx || idx < 0) {
                            Console.WriteLine("UV List out of range (Max:{0}, Value:{1}", obj.UvList.Count - 1, idx);
                            Console.ReadKey();
                            return;
                        }
                        var uv = obj.UvList[idx];
                        curSurface.UvList.Add(new float[] { uv[0], uv[1] });
                    }
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }
        #endregion

        #region Convert from Metasequoia model
        static void metasequoiaToStlBin(Metasequoia mqo, string filePath) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報が消えます");
            mqo.ToTriangle();
            var output = new StlBin();
            foreach (var obj in mqo.ObjectList) {
                var curObject = new StlBin.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<StlBin.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var curSurface = new StlBin.Surface();
                    var va = obj.VertexList[s.VertexIndex[0]];
                    var vo = obj.VertexList[s.VertexIndex[1]];
                    var vb = obj.VertexList[s.VertexIndex[2]];
                    curSurface.Norm = ((va - vo) * (vb - vo)).Norm;
                    curSurface.V1 = new vec3(vb.z, vb.x, vb.y);
                    curSurface.V2 = new vec3(vo.z, vo.x, vo.y);
                    curSurface.V3 = new vec3(va.z, va.x, va.y);
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void metasequoiaToStlText(Metasequoia mqo, string filePath) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報が消えます");
            mqo.ToTriangle();
            var output = new StlText();
            foreach (var obj in mqo.ObjectList) {
                var curObject = new StlText.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<StlText.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var curSurface = new StlText.Surface();
                    var va = obj.VertexList[s.VertexIndex[0]];
                    var vo = obj.VertexList[s.VertexIndex[1]];
                    var vb = obj.VertexList[s.VertexIndex[2]];
                    curSurface.Norm = ((va - vo) * (vb - vo)).Norm;
                    curSurface.V1 = new vec3(vb.z, vb.x, vb.y);
                    curSurface.V2 = new vec3(vo.z, vo.x, vo.y);
                    curSurface.V3 = new vec3(va.z, va.x, va.y);
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }

        static void metasequoiaToWavefrontObj(Metasequoia mqo, string filePath) {
            var output = new WavefrontObj();
            foreach (var m in mqo.MaterialList) {
                var color = new vec3(m.R, m.G, m.B);
                var mat = new WavefrontObj.Material();
                mat.Name = m.Name;
                mat.Ambient = color * m.Ambient;
                mat.Diffuse = color * m.Diffuse;
                mat.Specular = new vec3(1, 1, 1) * m.Specular;
                mat.SpecularPower = m.SpecularPower;
                mat.Alpha = m.A;
                if (!string.IsNullOrEmpty(m.TexturePath)) {
                    mat.TexAmbient = new WavefrontObj.Texture();
                    mat.TexAmbient.FileName = m.TexturePath;
                    mat.TexAmbient.ChannelR = true;
                    mat.TexAmbient.ChannelG = true;
                    mat.TexAmbient.ChannelB = true;
                }
                if (!string.IsNullOrEmpty(m.TexturePath)) {
                    mat.TexDiffuse = new WavefrontObj.Texture();
                    mat.TexDiffuse.FileName = m.TexturePath;
                    mat.TexDiffuse.ChannelR = true;
                    mat.TexDiffuse.ChannelG = true;
                    mat.TexDiffuse.ChannelB = true;
                }
                if (!string.IsNullOrEmpty(m.BumpMapPath)) {
                    mat.TexBumpMap = new WavefrontObj.Texture();
                    mat.TexBumpMap.FileName = m.BumpMapPath;
                    mat.TexBumpMap.ChannelL = true;
                }
                if (!string.IsNullOrEmpty(m.AlaphaPlanePath)) {
                    mat.TexAlapha = new WavefrontObj.Texture();
                    mat.TexAlapha.FileName = m.AlaphaPlanePath;
                    mat.TexAlapha.ChannelM = true;
                }
                output.MaterialList.Add(mat);
            }
            foreach (var obj in mqo.ObjectList) {
                var curObject = new WavefrontObj.Object();
                curObject.Name = obj.Name;
                curObject.SurfaceList = new List<WavefrontObj.Surface>();
                foreach (var s in obj.SurfaceList) {
                    var surface = new WavefrontObj.Surface();
                    if (0 <= s.MaterialIndex) {
                        surface.MaterialName = mqo.MaterialList[s.MaterialIndex].Name;
                    }
                    surface.VertexIndex = new List<int>();
                    surface.UvIndex = new List<int>();
                    for (int i = s.VertexIndex.Count - 1; 0 <= i; i--) {
                        surface.VertexIndex.Add(output.VertexList.Count + s.VertexIndex[i] + 1);
                    }
                    for (int idx = s.UvList.Count - 1; 0 <= idx; idx--) {
                        surface.UvIndex.Add(output.UvList.Count + idx + 1);
                    }
                    curObject.SurfaceList.Add(surface);
                    foreach (var uv in s.UvList) {
                        output.UvList.Add(new float[] { uv[0], uv[1] });
                    }
                }
                foreach (var v in obj.VertexList) {
                    output.VertexList.Add(v);
                }
                output.ObjectList.Add(curObject);
            }
            output.Save(filePath);
        }
        #endregion
    }
}
