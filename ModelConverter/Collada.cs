using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

class Collada : BaseModel {
    static class TAG {
        public const string INPUT = "input";
        public const string VCOUNT = "vcount";

        public const string SOURCE = "source";
        public const string FLOAT_ARRAY = "float_array";
        public const string NAME_ARRAY = "Name_array";
        public const string ACCESSOR = "accessor";
        public const string PARAM = "param";

        public const string IMAGE = "image";
        public const string INIT_FROM = "init_from";
        public const string REF = "ref";

        public const string EFFECT = "effect";
        public const string PROFILE_COMMON = "profile_COMMON";
        public const string NEW_PARAM = "newparam";
        public const string SAMPLER2D = "sampler2D";
        public const string INSTANCE_IMAGE = "instance_image";
        public const string TECHNIQUE = "technique";
        public const string BLINN = "blinn";
        public const string AMBIENT = "ambient";
        public const string DIFFUSE = "diffuse";
        public const string SPECULAR = "specular";
        public const string COLOR = "color";
        public const string TEXTURE = "texture";

        public const string MATERIAL = "material";
        public const string INSTANCE_EFFECT = "instance_effect";

        public const string GEOMETRY = "geometry";
        public const string MESH = "mesh";
        public const string VERTICES = "vertices";
        public const string POLYLIST = "polylist";
        public const string P = "p";

        public const string CONTROLLER = "controller";
        public const string SKIN = "skin";
        public const string BIND_SHAPE_MATRIX = "bind_shape_matrix";
        public const string JOINTS = "joints";
        public const string VERTEX_WEIGHTS = "vertex_weights";
        public const string V = "v";

        public const string VISUAL_SCENE = "visual_scene";
        public const string NODE = "node";
        public const string ROTATE = "rotate";
        public const string SCALE = "scale";
        public const string TRANSLATE = "translate";
        public const string INSTANCE_CONTROLLER = "instance_controller";
        public const string INSTANCE_GEOMETRY = "instance_geometry";
        public const string INSTANCE_MATERIAL = "instance_material";
        public const string BIND_VERTEX_INPUT = "bind_vertex_input";
        public const string BIND_MATERIAL = "bind_material";
        public const string TECHNIQUE_COMMON = "technique_common";

    }

    static class ATTR {
        public const string COUNT = "count";
        public const string DIGITS = "digits";
        public const string ID = "id";
        public const string INPUT_SEMANTIC = "input_semantic";
        public const string INPUT_SET = "input_set";
        public const string MATERIAL = "material";
        public const string NAME = "name";
        public const string OFFSET = "offset";
        public const string SEMANTIC = "semantic";
        public const string SOURCE = "source";
        public const string SID = "sid";
        public const string STRIDE = "stride";
        public const string SYMBOL = "symbol";
        public const string TARGET = "target";
        public const string TEXCOORD = "texcoord";
        public const string TEXTURE = "texture";
        public const string TYPE = "type";
        public const string URL = "url";
    }

    class Input {
        public string Semantic;
        public string Source;
        public int Offset = -1;

        public Input() { }

        public Input(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SEMANTIC:
                    Semantic = attr.Value;
                    break;
                case ATTR.SOURCE:
                    Source = attr.Value;
                    break;
                case ATTR.OFFSET:
                    Offset = int.Parse(attr.Value);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.INPUT);
            xml.WriteAttributeString(ATTR.SEMANTIC, "", Semantic);
            xml.WriteAttributeString(ATTR.SOURCE, "", Source);
            if (0 <= Offset) {
                xml.WriteAttributeString(ATTR.OFFSET, "", Offset.ToString());
            }
            xml.WriteEndElement();
        }
    }

    class Image {
        public string Id;
        public string File;

        public Image() { }

        public Image(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INIT_FROM:
                    loadInitFrom(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadInitFrom(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.REF:
                    File = elm1.Value;
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.IMAGE);
            xml.WriteAttributeString(ATTR.ID, "", Id);

            xml.WriteStartElement(TAG.INIT_FROM);
            xml.WriteStartElement(TAG.REF);
            xml.WriteString(File);
            xml.WriteEndElement();
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }

    new class Material {
        public string Id;
        public string Name;
        public string URL;

        public Material() { }

        public Material(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                case ATTR.NAME:
                    Name = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INSTANCE_EFFECT:
                    loadInstanceEffect(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadInstanceEffect(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.URL:
                    URL = attr.Value;
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.MATERIAL);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            xml.WriteAttributeString(ATTR.NAME, "", Name);

            xml.WriteStartElement(TAG.INSTANCE_EFFECT);
            xml.WriteAttributeString(ATTR.URL, "", URL);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }

    #region Effect
    class Effect {
        public string Id;
        public List<NewParam> NewParams;
        public Technique Technique;

        public Effect() { }

        public Effect(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.PROFILE_COMMON:
                    loadProfileCommon(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadProfileCommon(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.NEW_PARAM:
                    if (null == NewParams) {
                        NewParams = new List<NewParam>();
                    }
                    NewParams.Add(new NewParam(elm1));
                    break;
                case TAG.TECHNIQUE:
                    Technique = new Technique(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.EFFECT);
            xml.WriteAttributeString(ATTR.ID, "", Id);

            xml.WriteStartElement(TAG.PROFILE_COMMON);
            if (null != NewParams) {
                foreach (var p in NewParams) {
                    p.Save(xml);
                }
            }
            Technique.Save(xml);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }

    class NewParam {
        public string SId;
        public string URL;

        public NewParam() { }

        public NewParam(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SID:
                    SId = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.SAMPLER2D:
                    loadSampler2D(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadSampler2D(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INSTANCE_IMAGE:
                    loadInstanceImage(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadInstanceImage(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.URL:
                    URL = attr.Value;
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.NEW_PARAM);
            xml.WriteAttributeString(ATTR.SID, "", SId);

            xml.WriteStartElement(TAG.SAMPLER2D);
            xml.WriteStartElement(TAG.INSTANCE_IMAGE);
            xml.WriteAttributeString(ATTR.URL, "", URL);
            xml.WriteEndElement();
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }

    class Technique {
        public string SId;
        public Texture DiffuseTexture;
        public double[] DiffuseColor;
        public Texture AmbientTexture;
        public double[] AmbientColor;
        public Texture SpecularTexture;
        public double[] SpecularColor;

        public Technique() { }

        public Technique(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SID:
                    SId = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.BLINN:
                    loadBlinn(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadBlinn(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.DIFFUSE:
                    load(elm1, DiffuseTexture, DiffuseColor);
                    break;
                case TAG.AMBIENT:
                    load(elm1, AmbientTexture, AmbientColor);
                    break;
                case TAG.SPECULAR:
                    load(elm1, SpecularTexture, SpecularColor);
                    break;
                default:
                    break;
                }
            }
        }

        void load(XElement elm, Texture texture, double[] color) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.TEXTURE:
                    texture = new Texture(elm1);
                    break;
                case TAG.COLOR:
                    color = loadColor(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        double[] loadColor(XElement elm) {
            var arr = elm.Value.Split(' ');
            var color = new double[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                color[i] = double.Parse(arr[i]);
            }
            return color;
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.TECHNIQUE);
            xml.WriteAttributeString(ATTR.SID, "", SId);
            xml.WriteStartElement(TAG.BLINN);

            if (null != DiffuseTexture || null != DiffuseColor) {
                xml.WriteStartElement(TAG.DIFFUSE);
                if (null != DiffuseTexture) {
                    DiffuseTexture.Save(xml);
                }
                if (null != DiffuseColor) {
                    xml.WriteStartElement(TAG.COLOR);
                    xml.WriteString(string.Join(" ", DiffuseColor));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }

            if (null != AmbientTexture || null != AmbientColor) {
                xml.WriteStartElement(TAG.AMBIENT);
                if (null != AmbientTexture) {
                    AmbientTexture.Save(xml);
                }
                if (null != AmbientColor) {
                    xml.WriteStartElement(TAG.COLOR);
                    xml.WriteString(string.Join(" ", AmbientColor));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }

            if (null != SpecularTexture || null != SpecularColor) {
                xml.WriteStartElement(TAG.SPECULAR);
                if (null != SpecularTexture) {
                    SpecularTexture.Save(xml);
                }
                if (null != SpecularColor) {
                    xml.WriteStartElement(TAG.COLOR);
                    xml.WriteString(string.Join(" ", SpecularColor));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndElement();
        }
    }

    class Texture {
        public string Id;
        public string TexCoord;

        public Texture() { }

        public Texture(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.TEXTURE:
                    Id = attr.Value;
                    break;
                case ATTR.TEXCOORD:
                    TexCoord = attr.Value;
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.TEXTURE);
            xml.WriteAttributeString(ATTR.TEXTURE, "", Id);
            xml.WriteAttributeString(ATTR.TEXCOORD, "", TexCoord);
            xml.WriteEndElement();
        }
    }
    #endregion

    #region Source
    class Source {
        public string Id;
        public FloatArray FloatArray;
        public NameArray NameArray;
        public Accessor Accessor;

        public Source() { }

        public Source(XElement elm) {
            Accessor = new Accessor();
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.FLOAT_ARRAY:
                    FloatArray = new FloatArray(elm1);
                    break;
                case TAG.NAME_ARRAY:
                    NameArray = new NameArray(elm1);
                    break;
                case TAG.TECHNIQUE_COMMON:
                    Accessor = loadTechniqueCommon(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        Accessor loadTechniqueCommon(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.ACCESSOR:
                    return new Accessor(elm1);
                default:
                    return null;
                }
            }
            return null;
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.SOURCE);
            xml.WriteAttributeString(ATTR.ID, "", Id);

            if (null != FloatArray) {
                FloatArray.Save(xml);
            }
            if (null != NameArray) {
                NameArray.Save(xml);
            }
            if (null != Accessor) {
                xml.WriteStartElement(TAG.TECHNIQUE_COMMON);
                Accessor.Save(xml);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
        }
    }

    class FloatArray {
        public string Id;
        public int Count;
        public int Digits;
        public double[] Value;

        public FloatArray() { }

        public FloatArray(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                case ATTR.COUNT:
                    Count = int.Parse(attr.Value);
                    break;
                case ATTR.DIGITS:
                    Digits = int.Parse(attr.Value);
                    break;
                default:
                    break;
                }
            }
            var arr = elm.Value.Split(' ');
            Value = new double[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                Value[i] = double.Parse(arr[i]);
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.FLOAT_ARRAY);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            xml.WriteAttributeString(ATTR.COUNT, "", Count.ToString());
            xml.WriteAttributeString(ATTR.DIGITS, "", Digits.ToString());
            xml.WriteString(string.Join(" ", Value));
            xml.WriteEndElement();
        }
    }

    class NameArray {
        public string Id;
        public int Count;
        public string[] Value;

        public NameArray() { }

        public NameArray(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                case ATTR.COUNT:
                    Count = int.Parse(attr.Value);
                    break;
                default:
                    break;
                }
            }
            Value = elm.Value.Split(' ');
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.NAME_ARRAY);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            xml.WriteAttributeString(ATTR.COUNT, "", Count.ToString());
            xml.WriteString(string.Join(" ", Value));
            xml.WriteEndElement();
        }
    }

    class Accessor {
        public string Source;
        public int Stride;
        public int Count;
        public List<Param> Params;

        public Accessor() { }

        public Accessor(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SOURCE:
                    Source = attr.Value;
                    break;
                case ATTR.COUNT:
                    Count = int.Parse(attr.Value);
                    break;
                case ATTR.STRIDE:
                    Stride = int.Parse(attr.Value);
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.PARAM:
                    if (null == Params) {
                        Params = new List<Param>();
                    }
                    Params.Add(new Param(elm1));
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.ACCESSOR);

            xml.WriteAttributeString(ATTR.SOURCE, "", Source);
            xml.WriteAttributeString(ATTR.COUNT, "", Count.ToString());
            if (1 < Stride) {
                xml.WriteAttributeString(ATTR.STRIDE, "", Stride.ToString());
            }

            if (null != Params) {
                foreach (var param in Params) {
                    param.Save(xml);
                }
            }

            xml.WriteEndElement();
        }
    }

    class Param {
        public string Name;
        public string Type;

        public Param() { }

        public Param(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.NAME:
                    Name = attr.Value;
                    break;
                case ATTR.TYPE:
                    Type = attr.Value;
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.PARAM);
            xml.WriteAttributeString(ATTR.NAME, "", Name);
            xml.WriteAttributeString(ATTR.TYPE, "", Type);
            xml.WriteEndElement();
        }
    }
    #endregion

    #region Geometry
    class Geometry {
        public string Id;
        public Mesh Mesh;

        public Geometry() { }

        public Geometry(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.MESH:
                    Mesh = new Mesh(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.GEOMETRY);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            if (null != Mesh) {
                Mesh.Save(xml);
            }
            xml.WriteEndElement();
        }
    }

    class Mesh {
        public List<Source> Sources;
        public Vertices Vertices;
        public Polylist Polylist;

        public Mesh() { }

        public Mesh(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.SOURCE:
                    if (null == Sources) {
                        Sources = new List<Source>();
                    }
                    Sources.Add(new Source(elm1));
                    break;
                case TAG.VERTICES:
                    Vertices = new Vertices(elm1);
                    break;
                case TAG.POLYLIST:
                    Polylist = new Polylist(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.MESH);
            if (null != Sources) {
                foreach (var source in Sources) {
                    source.Save(xml);
                }
            }
            if (null != Vertices) {
                Vertices.Save(xml);
            }
            if (null != Polylist) {
                Polylist.Save(xml);
            }
            xml.WriteEndElement();
        }
    }

    class Vertices {
        public string Id;
        public Input Input;

        public Vertices() { }

        public Vertices(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INPUT:
                    Input = new Input(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.VERTICES);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            if (null != Input) {
                Input.Save(xml);
            }
            xml.WriteEndElement();
        }
    }

    class Polylist {
        public string Material;
        public int Count;
        public List<Input> Inputs;
        public int[] VCount;
        public int[] Index;

        public Polylist() { }

        public Polylist(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.MATERIAL:
                    Material = attr.Value;
                    break;
                case ATTR.COUNT:
                    Count = int.Parse(attr.Value);
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INPUT:
                    if (null == Inputs) {
                        Inputs = new List<Input>();
                    }
                    Inputs.Add(new Input(elm1));
                    break;
                case TAG.VCOUNT:
                    VCount = loadVCount(elm1);
                    break;
                case TAG.P:
                    Index = loadP(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        int[] loadVCount(XElement elm) {
            var arr = elm.Value.Split(' ');
            var v = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                v[i] = int.Parse(arr[i]);
            }
            return v;
        }

        int[] loadP(XElement elm) {
            var arr = elm.Value.Split(' ');
            var p = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                p[i] = int.Parse(arr[i]);
            }
            return p;
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.POLYLIST);
            xml.WriteAttributeString(ATTR.MATERIAL, "", Material);
            xml.WriteAttributeString(ATTR.COUNT, "", Count.ToString());

            if (null != Inputs) {
                foreach (var input in Inputs) {
                    input.Save(xml);
                }
            }

            xml.WriteStartElement(TAG.VCOUNT);
            xml.WriteString(string.Join(" ", VCount));
            xml.WriteEndElement();

            xml.WriteStartElement(TAG.P);
            xml.WriteString(string.Join(" ", Index));
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }
    #endregion

    #region Controller
    class Controller {
        public string Id;
        public Skin Skin;

        public Controller() { }

        public Controller(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.SKIN:
                    Skin = new Skin(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.CONTROLLER);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            if (null != Skin) {
                Skin.Save(xml);
            }
            xml.WriteEndElement();
        }
    }

    class Skin {
        public string Source;
        public List<Source> Sources;
        public BindShapeMatrix BindShapeMatrix;
        public Joints Joints;
        public VertexWeights VertexWeights;

        public Skin() { }

        public Skin(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SOURCE:
                    Source = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.SOURCE:
                    if (null == Sources) {
                        Sources = new List<Source>();
                    }
                    Sources.Add(new Source(elm1));
                    break;
                case TAG.BIND_SHAPE_MATRIX:
                    BindShapeMatrix = new BindShapeMatrix(elm1);
                    break;
                case TAG.JOINTS:
                    Joints = new Joints(elm1);
                    break;
                case TAG.VERTEX_WEIGHTS:
                    VertexWeights = new VertexWeights(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.SKIN);
            xml.WriteAttributeString(ATTR.SOURCE, "", Source);

            if (null != Sources) {
                foreach (var source in Sources) {
                    source.Save(xml);
                }
            }
            if (null != BindShapeMatrix) {
                BindShapeMatrix.Save(xml);
            }
            if (null != Joints) {
                Joints.Save(xml);
            }
            if (null != VertexWeights) {
                VertexWeights.Save(xml);
            }

            xml.WriteEndElement();
        }
    }

    class BindShapeMatrix {
        public double[,] Value;

        public BindShapeMatrix() {
            Value = new double[,] {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                };
        }

        public BindShapeMatrix(XElement elm) {
            var arr = elm.Value.Split(' ');
            Value = new double[4, 4];
            for (int i = 0; i < arr.Length; i++) {
                Value[i / 4, i % 4] = double.Parse(arr[i]);
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.BIND_SHAPE_MATRIX);
            var str = "";
            for (int i = 0; i < 16; i++) {
                str = string.Join(" ", str, Value[i / 4, i % 4]);
            }
            xml.WriteString(str);
            xml.WriteEndElement();
        }
    }

    class Joints {
        public List<Input> Inputs;

        public Joints() { }

        public Joints(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INPUT:
                    if (null == Inputs) {
                        Inputs = new List<Input>();
                    }
                    Inputs.Add(new Input(elm1));
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.JOINTS);
            if (null != Inputs) {
                foreach (var input in Inputs) {
                    input.Save(xml);
                }
            }
            xml.WriteEndElement();
        }
    }

    class VertexWeights {
        public int Count;
        public List<Input> Inputs;
        public int[] VCount;
        public int[] Index;

        public VertexWeights() { }

        public VertexWeights(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.COUNT:
                    Count = int.Parse(attr.Value);
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INPUT:
                    if (null == Inputs) {
                        Inputs = new List<Input>();
                    }
                    Inputs.Add(new Input(elm1));
                    break;
                case TAG.VCOUNT:
                    VCount = loadVCount(elm1);
                    break;
                case TAG.V:
                    Index = loadV(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        int[] loadVCount(XElement elm) {
            var arr = elm.Value.Split(' ');
            var v = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                v[i] = int.Parse(arr[i]);
            }
            return v;
        }

        int[] loadV(XElement elm) {
            var arr = elm.Value.Split(' ');
            var v = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                v[i] = int.Parse(arr[i]);
            }
            return v;
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.VERTEX_WEIGHTS);
            xml.WriteAttributeString(ATTR.COUNT, "", Count.ToString());

            if (null != Inputs) {
                foreach (var input in Inputs) {
                    input.Save(xml);
                }
            }

            xml.WriteStartElement(TAG.VCOUNT);
            xml.WriteString(string.Join(" ", VCount));
            xml.WriteEndElement();

            xml.WriteStartElement(TAG.V);
            xml.WriteString(string.Join(" ", Index));
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }
    #endregion

    #region VisualScene
    class VisualScene {
        public string Id;
        public List<Node> Nodes;

        public VisualScene(string id) {
            Id = id;
        }

        public VisualScene(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm2 in elm.Elements()) {
                switch (elm2.Name.LocalName) {
                case TAG.NODE:
                    if (null == Nodes) {
                        Nodes = new List<Node>();
                    }
                    Nodes.Add(new Node(elm2));
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.VISUAL_SCENE);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            if (null != Nodes) {
                foreach (var node in Nodes) {
                    node.Save(xml);
                }
            }
            xml.WriteEndElement();
        }
    }

    class Node {
        public string Id;
        public string Name;
        public string Type;
        public string SId;
        public Translate Translate;
        public Scale Scale;
        public List<Rotate> Rotate;
        public List<Node> Nodes;
        public InstanceGeometry InstanceGeometry;
        public InstanceController InstanceController;

        public Node(string id, string name, string type = null, string sid = null) {
            Id = id;
            Name = name;
            Type = type;
            SId = sid;
            Translate = new Translate("translate");
            Scale = new Scale("scale");
            Rotate = new List<Rotate>();
            Rotate.Add(new Rotate("rotateY", 0, 1, 0));
            Rotate.Add(new Rotate("rotateX", 1, 0, 0));
            Rotate.Add(new Rotate("rotateZ", 0, 0, 1));
        }

        public Node(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.ID:
                    Id = attr.Value;
                    break;
                case ATTR.NAME:
                    Name = attr.Value;
                    break;
                case ATTR.SID:
                    SId = attr.Value;
                    break;
                case ATTR.TYPE:
                    Type = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.NODE:
                    if (null == Nodes) {
                        Nodes = new List<Node>();
                    }
                    Nodes.Add(new Node(elm1));
                    break;
                case TAG.TRANSLATE:
                    Translate = new Translate(elm1);
                    break;
                case TAG.SCALE:
                    Scale = new Scale(elm1);
                    break;
                case TAG.ROTATE:
                    if (null == Rotate) {
                        Rotate = new List<Rotate>();
                    }
                    Rotate.Add(new Rotate(elm1));
                    break;
                case TAG.INSTANCE_GEOMETRY:
                    InstanceGeometry = new InstanceGeometry(elm1);
                    break;
                case TAG.INSTANCE_CONTROLLER:
                    InstanceController = new InstanceController(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.NODE);
            xml.WriteAttributeString(ATTR.ID, "", Id);
            xml.WriteAttributeString(ATTR.NAME, "", Name);
            if (!string.IsNullOrEmpty(Type)) {
                xml.WriteAttributeString(ATTR.TYPE, "", Type);
            }
            if (!string.IsNullOrEmpty(SId)) {
                xml.WriteAttributeString(ATTR.SID, "", SId);
            }

            if (null != Translate) {
                Translate.Save(xml);
            }
            if (null != Scale) {
                Scale.Save(xml);
            }
            if (null != Rotate) {
                foreach (var rot in Rotate) {
                    rot.Save(xml);
                }
            }
            if (null != InstanceGeometry) {
                InstanceGeometry.Save(xml);
            }
            if (null != InstanceController) {
                InstanceController.Save(xml);
            }
            if (null != Nodes) {
                foreach (var node in Nodes) {
                    node.Save(xml);
                }
            }

            xml.WriteEndElement();
        }
    }

    class Translate {
        public string SId;
        public double X;
        public double Y;
        public double Z;

        public Translate(string sid, double x = 0, double y = 0, double z = 0) {
            SId = sid;
            X = x;
            Y = y;
            Z = z;
        }

        public Translate(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SID:
                    SId = attr.Value;
                    break;
                default:
                    break;
                }
            }
            var arr = elm.Value.Split(' ');
            X = double.Parse(arr[0]);
            Y = double.Parse(arr[1]);
            Z = double.Parse(arr[2]);
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.TRANSLATE);
            xml.WriteAttributeString(ATTR.SID, "", SId);
            xml.WriteString(string.Join(" ", X, Y, Z));
            xml.WriteEndElement();
        }
    }

    class Scale {
        public string SId;
        public double X;
        public double Y;
        public double Z;

        public Scale(string sid, double x = 1, double y = 1, double z = 1) {
            SId = sid;
            X = x;
            Y = y;
            Z = z;
        }

        public Scale(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SID:
                    SId = attr.Value;
                    break;
                default:
                    break;
                }
            }
            var arr = elm.Value.Split(' ');
            X = double.Parse(arr[0]);
            Y = double.Parse(arr[1]);
            Z = double.Parse(arr[2]);
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.SCALE);
            xml.WriteAttributeString(ATTR.SID, "", SId);
            xml.WriteString(string.Join(" ", X, Y, Z));
            xml.WriteEndElement();
        }
    }

    class Rotate {
        public string SId;
        public double X;
        public double Y;
        public double Z;
        public double Angle;

        public Rotate(string sid, double x, double y, double z, double angle = 0) {
            SId = sid;
            X = x;
            Y = y;
            Z = z;
            Angle = angle;
        }

        public Rotate(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SID:
                    SId = attr.Value;
                    break;
                default:
                    break;
                }
            }
            var arr = elm.Value.Split(' ');
            X = double.Parse(arr[0]);
            Y = double.Parse(arr[1]);
            Z = double.Parse(arr[2]);
            Angle = double.Parse(arr[3]);
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.ROTATE);
            xml.WriteAttributeString(ATTR.SID, "", SId);
            xml.WriteString(string.Join(" ", X, Y, Z, Angle));
            xml.WriteEndElement();
        }
    }

    class InstanceGeometry {
        public string URL;
        public InstanceMaterial InstanceMaterial;

        public InstanceGeometry() { }

        public InstanceGeometry(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.URL:
                    URL = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.BIND_MATERIAL:
                    loadBindMaterial(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadBindMaterial(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.TECHNIQUE_COMMON:
                    loadTechniqueCommon(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadTechniqueCommon(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INSTANCE_MATERIAL:
                    InstanceMaterial = new InstanceMaterial(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.INSTANCE_GEOMETRY);
            xml.WriteAttributeString(ATTR.URL, "", URL);
            if (null != InstanceMaterial) {
                xml.WriteStartElement(TAG.BIND_MATERIAL);
                xml.WriteStartElement(TAG.TECHNIQUE_COMMON);
                InstanceMaterial.Save(xml);
                xml.WriteEndElement();
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
    }

    class InstanceController {
        public string URL;
        public InstanceMaterial InstanceMaterial;

        public InstanceController() { }

        public InstanceController(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.URL:
                    URL = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.BIND_MATERIAL:
                    loadBindMaterial(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadBindMaterial(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.TECHNIQUE_COMMON:
                    loadTechniqueCommon(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        void loadTechniqueCommon(XElement elm) {
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.INSTANCE_MATERIAL:
                    InstanceMaterial = new InstanceMaterial(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.INSTANCE_CONTROLLER);
            xml.WriteAttributeString(ATTR.URL, "", URL);
            if (null != InstanceMaterial) {
                xml.WriteStartElement(TAG.BIND_MATERIAL);
                xml.WriteStartElement(TAG.TECHNIQUE_COMMON);
                InstanceMaterial.Save(xml);
                xml.WriteEndElement();
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
    }

    class InstanceMaterial {
        public string Symbol;
        public string Target;
        public BindVertexInput BindVertexInput;

        public InstanceMaterial() { }

        public InstanceMaterial(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SYMBOL:
                    Symbol = attr.Value;
                    break;
                case ATTR.TARGET:
                    Target = attr.Value;
                    break;
                default:
                    break;
                }
            }
            foreach (var elm1 in elm.Elements()) {
                switch (elm1.Name.LocalName) {
                case TAG.BIND_VERTEX_INPUT:
                    BindVertexInput = new BindVertexInput(elm1);
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.INSTANCE_MATERIAL);
            xml.WriteAttributeString(ATTR.SYMBOL, "", Symbol);
            xml.WriteAttributeString(ATTR.TARGET, "", Target);
            if (null != BindVertexInput) {
                BindVertexInput.Save(xml);
            }
            xml.WriteEndElement();
        }
    }

    class BindVertexInput {
        public string Semantic;
        public string InputSemantic;
        public string InputSet;

        public BindVertexInput() { }

        public BindVertexInput(XElement elm) {
            foreach (var attr in elm.Attributes()) {
                switch (attr.Name.LocalName) {
                case ATTR.SEMANTIC:
                    Semantic = attr.Value;
                    break;
                case ATTR.INPUT_SEMANTIC:
                    InputSemantic = attr.Value;
                    break;
                case ATTR.INPUT_SET:
                    InputSet = attr.Value;
                    break;
                default:
                    break;
                }
            }
        }

        public void Save(XmlWriter xml) {
            xml.WriteStartElement(TAG.BIND_VERTEX_INPUT);
            xml.WriteAttributeString(ATTR.SEMANTIC, "", Semantic);
            xml.WriteAttributeString(ATTR.INPUT_SEMANTIC, "", InputSemantic);
            xml.WriteAttributeString(ATTR.INPUT_SET, "", InputSet);
            xml.WriteEndElement();
        }
    }
    #endregion

    public class MATERIAL {
        public string Name = "";
        public string DiffuseTexture = "";
        public double[] DiffuseColor = null;
        public string AmbientTexture = "";
        public double[] AmbientColor = null;
        public string SpecularTexture = "";
        public double[] SpecularColor = null;
    }

    public class OBJECT {
        public string Name = "";
        public string Material = "";
        public List<vec3> Vert = new List<vec3>();
        public List<vec3> Norm = new List<vec3>();
        public List<double[]> UV = new List<double[]>();
        public List<int[]> Face = new List<int[]>();
    }

    List<Image> Images = new List<Image>();
    List<Effect> Effects = new List<Effect>();
    List<Material> Materials = new List<Material>();
    List<Geometry> Geometries = new List<Geometry>();
    List<Controller> Controllers = new List<Controller>();
    List<VisualScene> VisualScenes = new List<VisualScene>();

    int unique_id = 1;

    public Collada() { }

    public Collada(string path) {
        var xml = XElement.Load(path);
        if ("COLLADA" != xml.Name.LocalName) {
            return;
        }
        foreach (var elm in xml.Elements()) {
            switch (elm.Name.LocalName) {
            case "library_images":
                foreach (var elm1 in elm.Elements()) {
                    if (TAG.IMAGE != elm1.Name.LocalName) {
                        continue;
                    }
                    Images.Add(new Image(elm1));
                }
                break;
            case "library_effects":
                foreach (var elm1 in elm.Elements()) {
                    if (TAG.EFFECT != elm1.Name.LocalName) {
                        continue;
                    }
                    Effects.Add(new Effect(elm1));
                }
                break;
            case "library_materials":
                foreach (var elm1 in elm.Elements()) {
                    if (TAG.MATERIAL != elm1.Name.LocalName) {
                        continue;
                    }
                    Materials.Add(new Material(elm1));
                }
                break;
            case "library_geometries":
                foreach (var elm1 in elm.Elements()) {
                    if (TAG.GEOMETRY != elm1.Name.LocalName) {
                        continue;
                    }
                    Geometries.Add(new Geometry(elm1));
                }
                break;
            case "library_controllers":
                foreach (var elm1 in elm.Elements()) {
                    if (TAG.CONTROLLER != elm1.Name.LocalName) {
                        continue;
                    }
                    Controllers.Add(new Controller(elm1));
                }
                break;
            case "library_visual_scenes":
                foreach (var elm1 in elm.Elements()) {
                    if (TAG.VISUAL_SCENE != elm1.Name.LocalName) {
                        continue;
                    }
                    VisualScenes.Add(new VisualScene(elm1));
                }
                break;
            default:
                break;
            }
        }
    }

    public override void Save(string path) {
        XmlWriterSettings xmls = new XmlWriterSettings();
        xmls.Indent = true;
        xmls.IndentChars = "\t";
        var xml = XmlWriter.Create(path, xmls);

        xml.WriteStartElement("COLLADA");
        xml.WriteAttributeString("version", "", "1.5.0");

        if (0 < Images.Count) {
            xml.WriteStartElement("library_images");
            foreach (var i in Images) {
                i.Save(xml);
            }
            xml.WriteEndElement();
        }

        if (0 < Effects.Count) {
            xml.WriteStartElement("library_effects");
            foreach (var e in Effects) {
                e.Save(xml);
            }
            xml.WriteEndElement();
        }

        if (0 < Materials.Count) {
            xml.WriteStartElement("library_materials");
            foreach (var m in Materials) {
                m.Save(xml);
            }
            xml.WriteEndElement();
        }

        if (0 < Controllers.Count) {
            xml.WriteStartElement("library_controllers");
            foreach (var c in Controllers) {
                c.Save(xml);
            }
            xml.WriteEndElement();
        }

        if (0 < Geometries.Count) {
            xml.WriteStartElement("library_geometries");
            foreach (var g in Geometries) {
                g.Save(xml);
            }
            xml.WriteEndElement();
        }

        if (0 < VisualScenes.Count) {
            xml.WriteStartElement("library_visual_scenes");
            foreach (var s in VisualScenes) {
                s.Save(xml);
            }
            xml.WriteEndElement();

            xml.WriteStartElement("scene");
            xml.WriteStartElement("instance_visual_scene");
            xml.WriteAttributeString("url", "", "#" + VisualScenes[0].Id);
            xml.WriteEndElement();
            xml.WriteEndElement();
        }

        xml.WriteEndElement();
        xml.Close();
    }

    public void AddMaterial(MATERIAL material) {
        var mat = new Material();
        mat.Id = string.Format("ID{0}", unique_id++);
        mat.Name = material.Name;

        var eff = new Effect();
        eff.Id = string.Format("ID{0}", unique_id++);
        eff.NewParams = new List<NewParam>();
        eff.Technique = new Technique();
        eff.Technique.SId = "COMMON";
        eff.Technique.DiffuseColor = material.DiffuseColor;
        eff.Technique.AmbientColor = material.AmbientColor;
        eff.Technique.SpecularColor = material.SpecularColor;

        if (!string.IsNullOrEmpty(material.DiffuseTexture)) {
            var img = AddImage(material.DiffuseTexture);
            var para = new NewParam();
            para.SId = string.Format("ID{0}", unique_id++);
            para.URL = "#" + img.Id;
            eff.NewParams.Add(para);
            eff.Technique.DiffuseTexture = new Texture();
            eff.Technique.DiffuseTexture.Id = para.SId;
            eff.Technique.DiffuseTexture.TexCoord = "UVSET0";
        }
        if (!string.IsNullOrEmpty(material.AmbientTexture)) {
            var img = AddImage(material.AmbientTexture);
            var para = new NewParam();
            para.SId = string.Format("ID{0}", unique_id++);
            para.URL = "#" + img.Id;
            eff.NewParams.Add(para);
            eff.Technique.AmbientTexture = new Texture();
            eff.Technique.AmbientTexture.Id = para.SId;
            eff.Technique.AmbientTexture.TexCoord = "UVSET0";
        }
        if (!string.IsNullOrEmpty(material.SpecularTexture)) {
            var img = AddImage(material.SpecularTexture);
            var para = new NewParam();
            para.SId = string.Format("ID{0}", unique_id++);
            para.URL = "#" + img.Id;
            eff.NewParams.Add(para);
            eff.Technique.SpecularTexture = new Texture();
            eff.Technique.SpecularTexture.Id = para.SId;
            eff.Technique.SpecularTexture.TexCoord = "UVSET0";
        }

        Effects.Add(eff);

        mat.URL = "#" + eff.Id;
        Materials.Add(mat);
    }

    public void AddObject(OBJECT obj) {

    }

    Image AddImage(string path) {
        Image image = null;
        foreach (var img in Images) {
            if (img.File == path) {
                image = img;
                break;
            }
        }
        if (null == image) {
            image = new Image();
            image.File = path;
            image.Id = string.Format("ID{0}", unique_id++);
            Images.Add(image);
        }
        return image;
    }
}

