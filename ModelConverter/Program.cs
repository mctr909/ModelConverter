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
                convertedModel.Save(saveFilePath);
            }
        }
    }
}
