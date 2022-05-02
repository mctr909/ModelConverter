using System;
using System.Collections.Generic;
using System.IO;

namespace ModelConverter {
    class Program {
        const string STL_BIN = "0 : STLバイナリ(.stl)";
        const string STL_TEXT = "1 : STLテキスト(.stl)";
        const string WAVEFRONT_OBJ = "2 : Wavefront OBJ(.obj)";
        const string METASEQUOIA = "3 : Metasequoia(.mqoz)";
        const string COLLADA = "4 : Collada(.dae)";
        const string MMD_PMX = "5 : MMD(.pmx)";

        static readonly List<string> TYPE_LIST = new List<string> {
            STL_BIN,
            STL_TEXT,
            WAVEFRONT_OBJ,
            METASEQUOIA,
            COLLADA,
            MMD_PMX
        };

        static bool mNomalizeFlg = false;
        static float mScale = 1.0f;

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

            Console.Write("UVの上下を入れ替えますか?[y/n]：");
            var invertUV = BaseModel.EInvertUV.None;
            if ("y" == Console.ReadLine()) {
                invertUV |= BaseModel.EInvertUV.V;
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
                case MMD_PMX:
                    saveExt = ".pmx";
                    break;
                case COLLADA:
                    saveExt = ".dae";
                    break;
                }
                var saveFilePath = fileDir + "\\" + fileName;
                var tmpFilePath = saveFilePath + saveExt;
                for (int i = 1; File.Exists(tmpFilePath); i++) {
                    tmpFilePath = saveFilePath + "_" + i + saveExt;
                }
                saveFilePath = tmpFilePath;

                // Load model
                BaseModel srcModel = null;
                try {
                    switch (ext.ToLower()) {
                    case ".stl":
                        srcModel = new StlText(filePath);
                        if (0 == srcModel.ObjectCount) {
                            srcModel = new StlBin(filePath);
                        }
                        break;
                    case ".obj":
                        srcModel = new WavefrontObj(filePath);
                        break;
                    case ".mqo":
                    case ".mqoz":
                        srcModel = new Metasequoia(filePath);
                        break;
                    case ".dae":
                        srcModel = new Collada(filePath);
                        break;
                    case ".pmx":
                        srcModel = new MmdPmx(filePath);
                        break;
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }

                BaseModel convertedModel = null;
                switch (convertTo) {
                case STL_BIN:
                    convertedModel = new StlBin();
                    break;
                case STL_TEXT:
                    convertedModel = new StlText();
                    break;
                case WAVEFRONT_OBJ:
                    convertedModel = new WavefrontObj();
                    break;
                case METASEQUOIA:
                    convertedModel = new Metasequoia();
                    break;
                case MMD_PMX:
                    convertedModel = new MmdPmx();
                    break;
                case COLLADA:
                    convertedModel = new Collada();
                    break;
                }

                // Save & Normalize
                if (null == convertedModel) {
                    continue;
                }
                convertedModel.Load(srcModel);
                if (mNomalizeFlg) {
                    convertedModel.Normalize(mScale);
                }
                if (srcModel is WavefrontObj || srcModel is Metasequoia) {
                    if (convertedModel is StlBin || convertedModel is StlText) {
                        convertedModel.SwapAxiz(BaseModel.ESwapAxiz.ZXY);
                    }
                }
                if (srcModel is StlBin || srcModel is StlText) {
                    if (convertedModel is WavefrontObj || convertedModel is Metasequoia) {
                        convertedModel.SwapAxiz(BaseModel.ESwapAxiz.YZX);
                    }
                }
                convertedModel.InvertUV = invertUV;
                convertedModel.Save(saveFilePath);
            }
        }

        #region Convert from WavefrontObj model
        static Metasequoia wavefrontObjToMetasequoia(WavefrontObj obj) {
            var output = new Metasequoia();
            //if (obj.VertList.Count == 0) {
            //    return null;
            //}
            //foreach (var m in obj.MaterialList) {
            //    var color = (m.Ambient.Abs < m.Diffuse.Abs) ? m.Diffuse.Norm : m.Ambient.Norm;
            //    var mat = new Metasequoia.Material();
            //    mat.Name = m.Name;
            //    mat.R = color.x;
            //    mat.G = color.y;
            //    mat.B = color.z;
            //    mat.A = m.Alpha;
            //    mat.Diffuse = (float)m.Diffuse.Abs;
            //    mat.Ambient = (float)m.Ambient.Abs;
            //    mat.Specular = (float)m.Specular.Abs;
            //    mat.SpecularPower = m.SpecularPower;
            //    if (null != m.TexDiffuse) {
            //        mat.TexturePath = m.TexDiffuse.FileName;
            //    }
            //    if (null != m.TexAlapha) {
            //        mat.AlaphaPlanePath = m.TexAlapha.FileName;
            //    }
            //    if (null != m.TexBumpMap) {
            //        mat.BumpMapPath = m.TexBumpMap.FileName;
            //    }
            //    output.MaterialList.Add(mat);
            //}
            //foreach (var o in obj.ObjectList) {
            //    var curObj = new Metasequoia.Object();
            //    curObj.Name = o.Name;
            //    curObj.Surfaces = new List<Metasequoia.Surface>();
            //    curObj.VertList = new List<vec3>();
            //    var curObjVertexIdx = new List<int>();
            //    foreach (var s in o.Surfaces) {
            //        var curSurface = new Metasequoia.Surface();
            //        curSurface.Material = obj.GetMaterialIndex(s.MaterialName);
            //        // Vertex
            //        curSurface.VertIdx = new List<int>();
            //        for (int i = s.VertIdx.Count - 1; 0 <= i; i--) {
            //            var idx = s.VertIdx[i] - 1;
            //            if (!curObjVertexIdx.Contains(idx)) {
            //                curObjVertexIdx.Add(idx);
            //                curObj.VertList.Add(obj.VertList[idx]);
            //            };
            //            curSurface.VertIdx.Add(curObjVertexIdx.IndexOf(idx));
            //        }
            //        // UV
            //        curSurface.UvList = new List<float[]>();
            //        for (int i = 0; i < s.UvIdx.Count; i++) {
            //            var uv = obj.UvList[s.UvIdx[i] - 1];
            //            curSurface.UvList.Add(uv);
            //        }
            //        curObj.Surfaces.Add(curSurface);
            //    }
            //    output.ObjectList.Add(curObj);
            //}
            return output;
        }

        static Collada wavefrontObjToCollada(WavefrontObj obj) {
            var output = new Collada();
            //foreach (var m in obj.MaterialList) {
            //    var mat = new Collada.MATERIAL();
            //    mat.Name = m.Name;

            //    mat.DiffuseTexture = m.TexDiffuse.FileName;
            //    mat.AmbientTexture = m.TexAmbient.FileName;
            //    mat.SpecularTexture = m.TexSpecular.FileName;

            //    if (0.0 < m.Diffuse.Abs) {
            //        mat.DiffuseColor = new double[] {
            //            m.Diffuse.x,
            //            m.Diffuse.y,
            //            m.Diffuse.z,
            //            1
            //        };
            //    }
            //    if (0.0 < m.Ambient.Abs) {
            //        mat.AmbientColor = new double[] {
            //            m.Ambient.x,
            //            m.Ambient.y,
            //            m.Ambient.z,
            //            1
            //        };
            //    }
            //    if (0.0 < m.Specular.Abs) {
            //        mat.SpecularColor = new double[] {
            //            m.Specular.x,
            //            m.Specular.y,
            //            m.Specular.z,
            //            1
            //        };
            //    }

            //    output.AddMaterial(mat);
            //}
            //foreach (var o in obj.mObjectList) {
            //    var cobj = new Collada.OBJECT();
            //    foreach (var s in o.Surfaces) {
            //        cobj.Name = o.Name;
            //        cobj.Material = s.MaterialName;
            //        cobj.Vert = new List<vec3>();
            //        cobj.Norm = new List<vec3>();
            //        cobj.UV = new List<double[]>();
            //        cobj.Face = new List<int[]>();
            //        for (int i = 0; i < s.VertIdx.Count; i++) {
            //        }
            //        output.AddObject(cobj);
            //    }
            //}
            return output;
        }
        #endregion

        #region Convert from Metasequoia model
        static WavefrontObj metasequoiaToWavefrontObj(Metasequoia mqo) {
            var output = new WavefrontObj();
            //foreach (var m in mqo.MaterialList) {
            //    var color = new vec3(m.R, m.G, m.B);
            //    var mat = new WavefrontObj.Material();
            //    mat.Name = m.Name;
            //    mat.Diffuse = color * m.Diffuse;
            //    mat.Ambient = color * m.Ambient;
            //    mat.Specular = new vec3(1, 1, 1) * m.Specular;
            //    mat.SpecularPower = m.SpecularPower;
            //    mat.Alpha = m.A;
            //    if (!string.IsNullOrEmpty(m.TexturePath)) {
            //        mat.TexDiffuse = new WavefrontObj.Texture();
            //        mat.TexDiffuse.FileName = m.TexturePath;
            //        mat.TexDiffuse.ChannelR = true;
            //        mat.TexDiffuse.ChannelG = true;
            //        mat.TexDiffuse.ChannelB = true;
            //    }
            //    if (!string.IsNullOrEmpty(m.BumpMapPath)) {
            //        mat.TexBumpMap = new WavefrontObj.Texture();
            //        mat.TexBumpMap.FileName = m.BumpMapPath;
            //        mat.TexBumpMap.ChannelL = true;
            //    }
            //    if (!string.IsNullOrEmpty(m.AlaphaPlanePath)) {
            //        mat.TexAlapha = new WavefrontObj.Texture();
            //        mat.TexAlapha.FileName = m.AlaphaPlanePath;
            //        mat.TexAlapha.ChannelM = true;
            //    }
            //    output.MaterialList.Add(mat);
            //}
            //foreach (var obj in mqo.ObjectList) {
            //    var curObject = new WavefrontObj.Object();
            //    curObject.Name = obj.Name;
            //    curObject.Surfaces = new List<WavefrontObj.Surface>();
            //    foreach (var s in obj.Surfaces) {
            //        var surface = new WavefrontObj.Surface();
            //        if (0 <= s.Material) {
            //            surface.MaterialName = mqo.MaterialList[s.Material].Name;
            //        }
            //        surface.VertIdx = new List<int>();
            //        surface.UvIdx = new List<int>();
            //        for (int i = s.VertIdx.Count - 1; 0 <= i; i--) {
            //            surface.VertIdx.Add(output.mVertList.Count + s.VertIdx[i] + 1);
            //        }
            //        for (int idx = s.UvList.Count - 1; 0 <= idx; idx--) {
            //            surface.UvIdx.Add(output.mUvList.Count + idx + 1);
            //        }
            //        curObject.Surfaces.Add(surface);
            //        foreach (var uv in s.UvList) {
            //            output.mUvList.Add(new float[] { uv[0], uv[1] });
            //        }
            //    }
            //    foreach (var v in obj.VertList) {
            //        output.mVertList.Add(v);
            //    }
            //    output.ObjectList.Add(curObject);
            //}
            return output;
        }

        static Collada metasequoiaToCollada(Metasequoia mqo) {
            var output = new Collada();
            //foreach (var m in mqo.MaterialList) {
            //    var mat = new Collada.MATERIAL();
            //    mat.Name = m.Name;

            //    mat.DiffuseTexture = m.TexturePath;

            //    if (0.0 < m.Diffuse) {
            //        mat.DiffuseColor = new double[] {
            //            m.R * m.Diffuse,
            //            m.G * m.Diffuse,
            //            m.B * m.Diffuse,
            //            1
            //        };
            //    }
            //    if (0.0 < m.Ambient) {
            //        mat.AmbientColor = new double[] {
            //            m.R * m.Ambient,
            //            m.G * m.Ambient,
            //            m.B * m.Ambient,
            //            1
            //        };
            //    }
            //    if (0.0 < m.Specular) {
            //        mat.SpecularColor = new double[] {
            //            m.R * m.Specular,
            //            m.G * m.Specular,
            //            m.B * m.Specular,
            //            1
            //        };
            //    }

            //    output.AddMaterial(mat);
            //}
            //foreach (var o in mqo.ObjectList) {
            //    var surface = new Collada.OBJECT();
            //    foreach (var s in o.Surfaces) {
            //        var mat = mqo.MaterialList[s.Material];
            //        surface.Material = mat.Name;
            //        surface.Vert = new List<vec3>();
            //        surface.Norm = new List<vec3>();
            //        surface.UV = new List<double[]>();
            //        surface.Face = new List<int[]>();
            //    }
            //}
            return output;
        }
        #endregion
    }
}
