using System;
using System.Collections.Generic;
using System.IO;

namespace ModelConverter {
    class Program {
        const string STL_BIN = "0 : STL(バイナリ)";
        const string STL_TEXT = "1 : STL(テキスト)";
        const string WAVEFRONT_OBJ = "2 : Wavefront OBJ";
        const string METASEQUOIA = "3 : Metasequoia";

        static bool mNomalizeFlg = false;
        static float mScale = 1.0f;

        static readonly List<string> TYPE_LIST = new List<string> {
            STL_BIN,
            STL_TEXT,
            WAVEFRONT_OBJ,
            METASEQUOIA
        };

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("このexeファイルのアイコンに変換するファイルをドラッグ&ドロップしてください");
                Console.WriteLine("一度に複数のファイルを変換できます");
                Console.ReadKey();
                return;
            }

            foreach (var t in TYPE_LIST) {
                Console.WriteLine(t);
            }
            Console.Write("変換する形式を番号で選んでください：");
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

            Console.Write("スケールを変更しますか?[y/n]：");
            keyLine = Console.ReadLine();
            if ("y" == keyLine.ToLower()) {
                mNomalizeFlg = true;
                Console.Write("スケールを入力してください(単位:mm)：");
                keyLine = Console.ReadLine();
                if (!float.TryParse(keyLine, out mScale)) {
                    Console.WriteLine("数字を入力してください");
                    Console.ReadKey();
                    return;
                }
            }

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
                case ".mqoz":
                    metasequoia = new Metasequoia(filePath);
                    break;
                }

                // savefile path
                var saveExt = "";
                switch (convertTo) {
                case STL_BIN:
                case STL_TEXT:
                    saveExt = ".stl";
                    break;
                case WAVEFRONT_OBJ:
                    saveExt = ".obj";
                    break;
                case METASEQUOIA:
                    saveExt = ".mqoz";
                    break;
                }
                var saveFilePath = fileDir + "\\" + fileName;
                var tmpFilePath = saveFilePath + saveExt;
                for (int i = 1; File.Exists(tmpFilePath); i++) {
                    tmpFilePath = saveFilePath + "_" + i + saveExt;
                }
                saveFilePath = tmpFilePath;

                IModel convertedModel = null;
                // Convert from STL(text) model
                if (null != stlText) {
                    switch (convertTo) {
                    case STL_BIN:
                        convertedModel = stlTextToStlBin(stlText);
                        break;
                    case STL_TEXT:
                        convertedModel = stlText;
                        break;
                    case WAVEFRONT_OBJ:
                        convertedModel = stlTextToWavefrontObj(stlText);
                        break;
                    case METASEQUOIA:
                        convertedModel = stlTextToMetasequoia(stlText);
                        break;
                    }
                }
                // Convert from STL(bin) model
                if (null != stlBin) {
                    switch (convertTo) {
                    case STL_BIN:
                        convertedModel = stlBin;
                        break;
                    case STL_TEXT:
                        convertedModel = stlBinToStlText(stlBin);
                        break;
                    case WAVEFRONT_OBJ:
                        convertedModel = stlBinToWavefrontObj(stlBin);
                        break;
                    case METASEQUOIA:
                        convertedModel = stlBinToMetasequoia(stlBin);
                        break;
                    }
                }
                // Convert from WavefrontObj model
                if (null != wavefrontObj) {
                    switch (convertTo) {
                    case STL_BIN:
                        convertedModel = wavefrontObjToStlBin(wavefrontObj);
                        break;
                    case STL_TEXT:
                        convertedModel = wavefrontObjToStlText(wavefrontObj);
                        break;
                    case WAVEFRONT_OBJ:
                        convertedModel = wavefrontObj;
                        break;
                    case METASEQUOIA:
                        convertedModel = wavefrontObjToMetasequoia(wavefrontObj);
                        break;
                    }
                }
                // Convert from Metasequoia model
                if (null != metasequoia) {
                    switch (convertTo) {
                    case STL_BIN:
                        convertedModel = metasequoiaToStlBin(metasequoia);
                        break;
                    case STL_TEXT:
                        convertedModel = metasequoiaToStlText(metasequoia);
                        break;
                    case WAVEFRONT_OBJ:
                        convertedModel = metasequoiaToWavefrontObj(metasequoia);
                        break;
                    case METASEQUOIA:
                        convertedModel = metasequoia;
                        break;
                    }
                }

                // Save & Normalize
                if (null != convertedModel) {
                    if (mNomalizeFlg) {
                        convertedModel.Normalize(mScale);
                    }
                    convertedModel.Save(saveFilePath);
                }
            }
        }

        #region Convert from STL(text) model
        static StlBin stlTextToStlBin(StlText stl) {
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
            return output;
        }

        static WavefrontObj stlTextToWavefrontObj(StlText stl) {
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
            return output;
        }

        static Metasequoia stlTextToMetasequoia(StlText stl) {
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
            return output;
        }
        #endregion

        #region Convert from STL(bin) model
        static StlText stlBinToStlText(StlBin stl) {
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
            return output;
        }

        static WavefrontObj stlBinToWavefrontObj(StlBin stl) {
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
            return output;
        }

        static Metasequoia stlBinToMetasequoia(StlBin stl) {
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
            return output;
        }
        #endregion

        #region Convert from WavefrontObj model
        static StlBin wavefrontObjToStlBin(WavefrontObj obj) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報は消えます");
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
            return output;
        }

        static StlText wavefrontObjToStlText(WavefrontObj obj) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報は消えます");
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
            return output;
        }

        static Metasequoia wavefrontObjToMetasequoia(WavefrontObj obj) {
            var output = new Metasequoia();
            if (obj.VertexList.Count == 0) {
                return null;
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
                                return null;
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
                            return null;
                        }
                        var uv = obj.UvList[idx];
                        curSurface.UvList.Add(new float[] { uv[0], uv[1] });
                    }
                    curObject.SurfaceList.Add(curSurface);
                }
                output.ObjectList.Add(curObject);
            }
            return output;
        }
        #endregion

        #region Convert from Metasequoia model
        static StlBin metasequoiaToStlBin(Metasequoia mqo) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報は消えます");
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
            return output;
        }

        static StlText metasequoiaToStlText(Metasequoia mqo) {
            Console.WriteLine("STLはマテリアル情報を持てないため、マテリアル情報は消えます");
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
            return output;
        }

        static WavefrontObj metasequoiaToWavefrontObj(Metasequoia mqo) {
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
            return output;
        }
        #endregion
    }
}
