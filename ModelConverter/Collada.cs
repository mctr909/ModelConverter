using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace ModelConverter {
    class Collada : BaseModel {
        class Input {
            public string Semantic;
            public string Source;
            public int Offset = -1;

            public Input() { }

            public Input(XElement elm) {
                foreach (var attr in elm.Attributes()) {
                    switch (attr.Name.LocalName) {
                    case "semantic":
                        Semantic = attr.Value;
                        break;
                    case "source":
                        Source = attr.Value;
                        break;
                    case "offset":
                        Offset = int.Parse(attr.Value);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("input");
                xml.WriteAttributeString("semantic", "", Semantic);
                xml.WriteAttributeString("source", "", Source);
                if (0 <= Offset) {
                    xml.WriteAttributeString("offset", "", Offset.ToString());
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
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "init_from":
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
                    case "ref":
                        File = elm1.Value;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("image");
                xml.WriteAttributeString("id", "", Id);

                xml.WriteStartElement("init_from");
                xml.WriteStartElement("ref");
                xml.WriteString(File);
                xml.WriteEndElement();
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

        class Material {
            public string Id;
            public string Name;
            public string URL;

            public Material() { }

            public Material(XElement elm) {
                foreach (var attr in elm.Attributes()) {
                    switch (attr.Name.LocalName) {
                    case "id":
                        Id = attr.Value;
                        break;
                    case "name":
                        Name = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "instance_effect":
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
                    case "url":
                        URL = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("material");
                xml.WriteAttributeString("id", "", Id);
                xml.WriteAttributeString("name", "", Name);

                xml.WriteStartElement("instance_effect");
                xml.WriteAttributeString("url", "", URL);
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

        #region Effect
        class Effect {
            public string Id;
            public NewParam NewParam;
            public Technique Technique;

            public Effect() { }

            public Effect(XElement elm) {
                foreach (var attr in elm.Attributes()) {
                    switch (attr.Name.LocalName) {
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "profile_COMMON":
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
                    case "newparam":
                        NewParam = new NewParam(elm1);
                        break;
                    case "technique":
                        Technique = new Technique(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("effect");
                xml.WriteAttributeString("id", "", Id);

                xml.WriteStartElement("profile_COMMON");
                if (null != NewParam) {
                    NewParam.Save(xml);
                }
                if (null != Technique) {
                    Technique.Save(xml);
                }
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
                    case "sid":
                        SId = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "sampler2D":
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
                    case "instance_image":
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
                    case "url":
                        URL = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("newparam");
                xml.WriteAttributeString("sid", "", SId);

                xml.WriteStartElement("sampler2D");
                xml.WriteStartElement("instance_image");
                xml.WriteAttributeString("url", "", URL);
                xml.WriteEndElement();
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

        class Technique {
            public string SId;
            public Texture DiffuseTexture;

            public Technique() { }

            public Technique(XElement elm) {
                foreach (var attr in elm.Attributes()) {
                    switch (attr.Name.LocalName) {
                    case "sid":
                        SId = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "blinn":
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
                    case "diffuse":
                        DiffuseTexture = loadTexture(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            Texture loadTexture(XElement elm) {
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "texture":
                        return new Texture(elm1);
                    default:
                        return null;
                    }
                }
                return null;
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("technique");
                xml.WriteAttributeString("sid", "", SId);

                if (null != DiffuseTexture) {
                    xml.WriteStartElement("blinn");
                    xml.WriteStartElement("diffuse");
                    DiffuseTexture.Save(xml);
                    xml.WriteEndElement();
                    xml.WriteEndElement();
                }

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
                    case "texture":
                        Id = attr.Value;
                        break;
                    case "texcoord":
                        TexCoord = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("texture");
                xml.WriteAttributeString("texture", "", Id);
                xml.WriteAttributeString("texcoord", "", TexCoord);
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
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "float_array":
                        FloatArray = new FloatArray(elm1);
                        break;
                    case "Name_array":
                        NameArray = new NameArray(elm1);
                        break;
                    case "technique_common":
                        Accessor = new TechniqueCommon(elm1).Accessor;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("source");

                xml.WriteAttributeString("id", "", Id);

                if (null != FloatArray) {
                    FloatArray.Save(xml);
                }
                if (null != NameArray) {
                    NameArray.Save(xml);
                }
                if (null != Accessor) {
                    xml.WriteStartElement("technique_common");
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
                    case "id":
                        Id = attr.Value;
                        break;
                    case "count":
                        Count = int.Parse(attr.Value);
                        break;
                    case "digits":
                        Digits = int.Parse(attr.Value);
                        break;
                    default:
                        break;
                    }
                }
                var arr = elm.Value.Split(" ");
                Value = new double[arr.Length];
                for (int i = 0; i < arr.Length; i++) {
                    Value[i] = double.Parse(arr[i]);
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("float_array");

                xml.WriteAttributeString("id", "", Id);
                xml.WriteAttributeString("count", "", Count.ToString());
                xml.WriteAttributeString("digits", "", Digits.ToString());

                xml.WriteString(string.Join(' ', Value));

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
                    case "id":
                        Id = attr.Value;
                        break;
                    case "count":
                        Count = int.Parse(attr.Value);
                        break;
                    default:
                        break;
                    }
                }
                Value = elm.Value.Split(" ");
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("name_array");

                xml.WriteAttributeString("id", "", Id);
                xml.WriteAttributeString("count", "", Count.ToString());

                xml.WriteString(string.Join(' ', Value));

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
                    case "source":
                        Source = attr.Value;
                        break;
                    case "count":
                        Count = int.Parse(attr.Value);
                        break;
                    case "stride":
                        Stride = int.Parse(attr.Value);
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "param":
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
                xml.WriteStartElement("accessor");

                xml.WriteAttributeString("source", "", Source);
                xml.WriteAttributeString("count", "", Count.ToString());
                if (1 < Stride) {
                    xml.WriteAttributeString("stride", "", Stride.ToString());
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
                    case "name":
                        Name = attr.Value;
                        break;
                    case "type":
                        Type = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("param");
                xml.WriteAttributeString("name", "", Name);
                xml.WriteAttributeString("type", "", Type);
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
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "mesh":
                        Mesh = new Mesh(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("geometry");
                xml.WriteAttributeString("id", "", Id);
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
                    case "source":
                        if (null == Sources) {
                            Sources = new List<Source>();
                        }
                        Sources.Add(new Source(elm1));
                        break;
                    case "vertices":
                        Vertices = new Vertices(elm1);
                        break;
                    case "polylist":
                        Polylist = new Polylist(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("mesh");
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
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "input":
                        Input = new Input(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("vertices");
                xml.WriteAttributeString("id", "", Id);
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
                    case "material":
                        Material = attr.Value;
                        break;
                    case "count":
                        Count = int.Parse(attr.Value);
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "input":
                        if (null == Inputs) {
                            Inputs = new List<Input>();
                        }
                        Inputs.Add(new Input(elm1));
                        break;
                    case "vcount":
                        VCount = loadVCount(elm1);
                        break;
                    case "p":
                        Index = loadP(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            int[] loadVCount(XElement elm) {
                var arr = elm.Value.Split(" ");
                var v = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++) {
                    v[i] = int.Parse(arr[i]);
                }
                return v;
            }

            int[] loadP(XElement elm) {
                var arr = elm.Value.Split(" ");
                var p = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++) {
                    p[i] = int.Parse(arr[i]);
                }
                return p;
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("polylist");

                xml.WriteAttributeString("material", "", Material);
                xml.WriteAttributeString("count", "", Count.ToString());

                if (null != Inputs) {
                    foreach(var input in Inputs) {
                        input.Save(xml);
                    }
                }

                xml.WriteStartElement("vcount");
                xml.WriteString(string.Join(' ', VCount));
                xml.WriteEndElement();

                xml.WriteStartElement("p");
                xml.WriteString(string.Join(' ', Index));
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
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "skin":
                        Skin = new Skin(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("controller");
                xml.WriteAttributeString("id", "", Id);
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
                    case "source":
                        Source = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "source":
                        if (null == Sources) {
                            Sources = new List<Source>();
                        }
                        Sources.Add(new Source(elm1));
                        break;
                    case "bind_shape_matrix":
                        BindShapeMatrix = new BindShapeMatrix(elm1);
                        break;
                    case "joints":
                        Joints = new Joints(elm1);
                        break;
                    case "vertex_weights":
                        VertexWeights = new VertexWeights(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("skin");

                xml.WriteAttributeString("source", "", Source);

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
                var arr = elm.Value.Split(" ");
                Value = new double[4, 4];
                for (int i = 0; i < arr.Length; i++) {
                    Value[i / 4, i % 4] = double.Parse(arr[i]);
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("bind_shape_matrix");
                var str = "";
                for (int i = 0; i < 16; i++) {
                    str = string.Join(' ', str, Value[i / 4, i % 4]);
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
                    case "input":
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
                xml.WriteStartElement("joints");
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
                    case "count":
                        Count = int.Parse(attr.Value);
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "input":
                        if (null == Inputs) {
                            Inputs = new List<Input>();
                        }
                        Inputs.Add(new Input(elm1));
                        break;
                    case "vcount":
                        VCount = loadVCount(elm1);
                        break;
                    case "v":
                        Index = loadV(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            int[] loadVCount(XElement elm) {
                var arr = elm.Value.Split(" ");
                var v = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++) {
                    v[i] = int.Parse(arr[i]);
                }
                return v;
            }

            int[] loadV(XElement elm) {
                var arr = elm.Value.Split(" ");
                var v = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++) {
                    v[i] = int.Parse(arr[i]);
                }
                return v;
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("vertex_weights");

                xml.WriteAttributeString("count", "", Count.ToString());

                if (null != Inputs) {
                    foreach (var input in Inputs) {
                        input.Save(xml);
                    }
                }

                xml.WriteStartElement("vcount");
                xml.WriteString(string.Join(' ', VCount));
                xml.WriteEndElement();

                xml.WriteStartElement("v");
                xml.WriteString(string.Join(' ', Index));
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
                    case "id":
                        Id = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm2 in elm.Elements()) {
                    switch (elm2.Name.LocalName) {
                    case "node":
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
                xml.WriteStartElement("visual_scene");
                xml.WriteAttributeString("id", "", Id);
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
                    case "id":
                        Id = attr.Value;
                        break;
                    case "name":
                        Name = attr.Value;
                        break;
                    case "sid":
                        SId = attr.Value;
                        break;
                    case "type":
                        Type = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "node":
                        if (null == Nodes) {
                            Nodes = new List<Node>();
                        }
                        Nodes.Add(new Node(elm1));
                        break;
                    case "translate":
                        Translate = new Translate(elm1);
                        break;
                    case "scale":
                        Scale = new Scale(elm1);
                        break;
                    case "rotate":
                        if (null == Rotate) {
                            Rotate = new List<Rotate>();
                        }
                        Rotate.Add(new Rotate(elm1));
                        break;
                    case "instance_geometry":
                        InstanceGeometry = new InstanceGeometry(elm1);
                        break;
                    case "instance_controller":
                        InstanceController = new InstanceController(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("node");

                xml.WriteAttributeString("id", "", Id);
                xml.WriteAttributeString("name", "", Name);
                if (!string.IsNullOrEmpty(Type)) {
                    xml.WriteAttributeString("type", "", Type);
                }
                if (!string.IsNullOrEmpty(SId)) {
                    xml.WriteAttributeString("sid", "", SId);
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
                    case "sid":
                        SId = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                var arr = elm.Value.Split(" ");
                X = double.Parse(arr[0]);
                Y = double.Parse(arr[1]);
                Z = double.Parse(arr[2]);
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("translate");
                xml.WriteAttributeString("sid", "", SId);
                xml.WriteString(string.Join(' ', X, Y, Z));
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
                    case "sid":
                        SId = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                var arr = elm.Value.Split(" ");
                X = double.Parse(arr[0]);
                Y = double.Parse(arr[1]);
                Z = double.Parse(arr[2]);
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("scale");
                xml.WriteAttributeString("sid", "", SId);
                xml.WriteString(string.Join(' ', X, Y, Z));
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
                    case "sid":
                        SId = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                var arr = elm.Value.Split(" ");
                X = double.Parse(arr[0]);
                Y = double.Parse(arr[1]);
                Z = double.Parse(arr[2]);
                Angle = double.Parse(arr[3]);
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("rotate");
                xml.WriteAttributeString("sid", "", SId);
                xml.WriteString(string.Join(' ', X, Y, Z, Angle));
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
                    case "url":
                        URL = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "bind_material":
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
                    case "technique_common":
                        InstanceMaterial = new TechniqueCommon(elm1).InstanceMaterial;
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("instance_geometry");
                xml.WriteAttributeString("url", "", URL);
                if (null != InstanceMaterial) {
                    xml.WriteStartElement("bind_material");
                    xml.WriteStartElement("technique_common");
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
                    case "url":
                        URL = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "bind_material":
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
                    case "technique_common":
                        InstanceMaterial = new TechniqueCommon(elm1).InstanceMaterial;
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("instance_controller");
                xml.WriteAttributeString("url", "", URL);
                if (null != InstanceMaterial) {
                    xml.WriteStartElement("bind_material");
                    xml.WriteStartElement("technique_common");
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
                    case "symbol":
                        Symbol = attr.Value;
                        break;
                    case "target":
                        Target = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "bind_vertex_input":
                        BindVertexInput = new BindVertexInput(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("instance_material");
                xml.WriteAttributeString("symbol", "", Symbol);
                xml.WriteAttributeString("target", "", Target);
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
                    case "semantic":
                        Semantic = attr.Value;
                        break;
                    case "input_semantic":
                        InputSemantic = attr.Value;
                        break;
                    case "input_set":
                        InputSet = attr.Value;
                        break;
                    default:
                        break;
                    }
                }
            }

            public void Save(XmlWriter xml) {
                xml.WriteStartElement("bind_vertex_input");
                xml.WriteAttributeString("semantic", "", Semantic);
                xml.WriteAttributeString("input_semantic", "", InputSemantic);
                xml.WriteAttributeString("input_set", "", InputSet);
                xml.WriteEndElement();
            }
        }
        #endregion

        class TechniqueCommon {
            public Accessor Accessor;
            public InstanceMaterial InstanceMaterial;

            public TechniqueCommon() { }

            public TechniqueCommon(XElement elm) {
                foreach (var elm1 in elm.Elements()) {
                    switch (elm1.Name.LocalName) {
                    case "accessor":
                        Accessor = new Accessor(elm1);
                        break;
                    case "instance_material":
                        InstanceMaterial = new InstanceMaterial(elm1);
                        break;
                    default:
                        break;
                    }
                }
            }
        }

        List<Image> Images = new List<Image>();
        List<Effect> Effects = new List<Effect>();
        List<Material> Materials = new List<Material>();
        List<Geometry> Geometries = new List<Geometry>();
        List<Controller> Controllers = new List<Controller>();
        List<VisualScene> VisualScenes = new List<VisualScene>();

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
                        if ("image" != elm1.Name.LocalName) {
                            continue;
                        }
                        Images.Add(new Image(elm1));
                    }
                    break;
                case "library_effects":
                    foreach (var elm1 in elm.Elements()) {
                        if ("effect" != elm1.Name.LocalName) {
                            continue;
                        }
                        Effects.Add(new Effect(elm1));
                    }
                    break;
                case "library_materials":
                    foreach (var elm1 in elm.Elements()) {
                        if ("material" != elm1.Name.LocalName) {
                            continue;
                        }
                        Materials.Add(new Material(elm1));
                    }
                    break;
                case "library_geometries":
                    foreach (var elm1 in elm.Elements()) {
                        if ("geometry" != elm1.Name.LocalName) {
                            continue;
                        }
                        Geometries.Add(new Geometry(elm1));
                    }
                    break;
                case "library_controllers":
                    foreach (var elm1 in elm.Elements()) {
                        if ("controller" != elm1.Name.LocalName) {
                            continue;
                        }
                        Controllers.Add(new Controller(elm1));
                    }
                    break;
                case "library_visual_scenes":
                    foreach (var elm1 in elm.Elements()) {
                        if ("visual_scene" != elm1.Name.LocalName) {
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

        public override void Normalize(float scale = 1) { }

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
    }
}
